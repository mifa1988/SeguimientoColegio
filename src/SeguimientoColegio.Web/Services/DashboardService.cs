using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SeguimientoColegio.Web.Configuration;
using SeguimientoColegio.Web.Constants;
using SeguimientoColegio.Web.Data;
using SeguimientoColegio.Web.Dtos;

namespace SeguimientoColegio.Web.Services;

public interface IDashboardService
{
    Task<DashboardResumenDto> ObtenerResumenAsync(int? ninoSeleccionadoId, CancellationToken cancellationToken);
}

public sealed class DashboardService(
    SeguimientoDbContext dbContext,
    IAlertaService alertaService,
    IOptions<AlertSettings> settings) : IDashboardService
{
    public async Task<DashboardResumenDto> ObtenerResumenAsync(int? ninoSeleccionadoId, CancellationToken cancellationToken)
    {
        var query = dbContext.RegistrosEscolares
            .AsNoTracking()
            .Where(x => x.Activo);

        if (ninoSeleccionadoId.HasValue)
        {
            query = query.Where(x => x.NinoId == ninoSeleccionadoId.Value);
        }

        var snapshots = await query
            .Select(x => new SnapshotAlertaRegistroDto
            {
                RegistroId = x.Id,
                NinoId = x.NinoId,
                Nino = x.Nino!.Nombres + " " + x.Nino.Apellidos,
                DisciplinaId = x.DisciplinaId,
                Disciplina = x.Disciplina != null ? x.Disciplina.Nombre : null,
                TipoRegistroId = x.TipoRegistroId,
                TipoRegistro = x.TipoRegistro!.Nombre,
                EstadoSeguimientoId = x.EstadoSeguimientoId,
                EstadoSeguimiento = x.EstadoSeguimiento!.Nombre,
                PrioridadId = x.PrioridadId,
                Prioridad = x.Prioridad != null ? x.Prioridad.Nombre : null,
                Titulo = x.Titulo,
                FechaRecibido = x.FechaRecibido,
                FechaVencimiento = x.FechaVencimiento,
                FechaRealizado = x.FechaRealizado,
                FechaActualizacion = x.FechaActualizacion,
                RequiereSeguimiento = x.RequiereSeguimiento,
                DiasAlerta = x.DiasAlerta,
                Activo = x.Activo
            })
            .ToListAsync(cancellationToken);

        var badgeMap = alertaService.EvaluarBadges(snapshots);
        var today = DateOnly.FromDateTime(DateTime.Today);
        var abiertos = snapshots.Where(x => x.EstadoSeguimientoId is not SeedDataIds.Estados.Concluido and not SeedDataIds.Estados.Descartado).ToList();

        var totalPendientes = snapshots.Count(x => x.EstadoSeguimientoId == SeedDataIds.Estados.Pendiente);
        var totalEnProceso = snapshots.Count(x => x.EstadoSeguimientoId == SeedDataIds.Estados.EnProceso);
        var totalConcluidos = snapshots.Count(x => x.EstadoSeguimientoId == SeedDataIds.Estados.Concluido);
        var totalVencidos = snapshots.Count(x =>
            x.EstadoSeguimientoId == SeedDataIds.Estados.Vencido ||
            (x.FechaVencimiento.HasValue &&
             x.FechaVencimiento.Value < today &&
             x.EstadoSeguimientoId is not SeedDataIds.Estados.Concluido and not SeedDataIds.Estados.Descartado));

        var pendientesAntiguos = badgeMap.Values.Count(badges => badges.Any(x => x.Codigo == "pendiente-antiguo"));
        var acuerdosPendientes = abiertos.Count(x => x.TipoRegistroId == SeedDataIds.TiposRegistro.AcuerdoProfesora);

        var pendientesPorNino = abiertos
            .GroupBy(x => x.Nino)
            .Select(group => new ConteoAgrupadoDto
            {
                Etiqueta = group.Key,
                Total = group.Count()
            })
            .OrderByDescending(x => x.Total)
            .ThenBy(x => x.Etiqueta)
            .ToList();

        var pendientesPorDisciplina = abiertos
            .GroupBy(x => x.Disciplina ?? "Sin disciplina")
            .Select(group => new ConteoAgrupadoDto
            {
                Etiqueta = group.Key,
                Total = group.Count()
            })
            .OrderByDescending(x => x.Total)
            .ThenBy(x => x.Etiqueta)
            .ToList();

        var ultimosCreados = await query
            .OrderByDescending(x => x.FechaCreacion)
            .Take(8)
            .Select(x => new RegistroBreveDto
            {
                Id = x.Id,
                Titulo = x.Titulo,
                Nino = x.Nino!.Nombres + " " + x.Nino.Apellidos,
                Disciplina = x.Disciplina != null ? x.Disciplina.Nombre : null,
                TipoRegistro = x.TipoRegistro!.Nombre,
                Estado = x.EstadoSeguimiento!.Nombre,
                EstadoColorCss = x.EstadoSeguimiento.ColorCss ?? "#7a828f",
                Prioridad = x.Prioridad != null ? x.Prioridad.Nombre : null,
                PrioridadColorCss = x.Prioridad != null ? x.Prioridad.ColorCss : null,
                FechaRecibido = x.FechaRecibido,
                FechaActualizacion = x.FechaActualizacion,
                AntiguedadDias = 0,
                EstaVencido = x.FechaVencimiento.HasValue &&
                              x.FechaVencimiento.Value < today &&
                              x.EstadoSeguimientoId != SeedDataIds.Estados.Concluido &&
                              x.EstadoSeguimientoId != SeedDataIds.Estados.Descartado
            })
            .ToListAsync(cancellationToken);

        var ultimosActualizados = await query
            .OrderByDescending(x => x.FechaActualizacion)
            .Take(8)
            .Select(x => new RegistroBreveDto
            {
                Id = x.Id,
                Titulo = x.Titulo,
                Nino = x.Nino!.Nombres + " " + x.Nino.Apellidos,
                Disciplina = x.Disciplina != null ? x.Disciplina.Nombre : null,
                TipoRegistro = x.TipoRegistro!.Nombre,
                Estado = x.EstadoSeguimiento!.Nombre,
                EstadoColorCss = x.EstadoSeguimiento.ColorCss ?? "#7a828f",
                Prioridad = x.Prioridad != null ? x.Prioridad.Nombre : null,
                PrioridadColorCss = x.Prioridad != null ? x.Prioridad.ColorCss : null,
                FechaRecibido = x.FechaRecibido,
                FechaActualizacion = x.FechaActualizacion,
                AntiguedadDias = 0,
                EstaVencido = x.FechaVencimiento.HasValue &&
                              x.FechaVencimiento.Value < today &&
                              x.EstadoSeguimientoId != SeedDataIds.Estados.Concluido &&
                              x.EstadoSeguimientoId != SeedDataIds.Estados.Descartado
            })
            .ToListAsync(cancellationToken);

        var alertasCriticas = (await alertaService.ObtenerAlertasAsync(
                new ConsultaAlertas { NinoId = ninoSeleccionadoId, Severidad = "critica" },
                ninoSeleccionadoId,
                cancellationToken))
            .Take(settings.Value.MaxAlertasDashboard)
            .ToList();

        return new DashboardResumenDto
        {
            TotalPendientes = totalPendientes,
            TotalEnProceso = totalEnProceso,
            TotalConcluidos = totalConcluidos,
            TotalVencidos = totalVencidos,
            PendientesAntiguos = pendientesAntiguos,
            AcuerdosPendientes = acuerdosPendientes,
            PendientesPorNino = pendientesPorNino,
            PendientesPorDisciplina = pendientesPorDisciplina,
            UltimosRegistrosCreados = EnriquecerAntiguedad(ultimosCreados),
            UltimosRegistrosActualizados = EnriquecerAntiguedad(ultimosActualizados),
            AlertasCriticas = alertasCriticas
        };
    }

    private static IReadOnlyList<RegistroBreveDto> EnriquecerAntiguedad(IEnumerable<RegistroBreveDto> items)
        => items
            .Select(x => new RegistroBreveDto
            {
                Id = x.Id,
                Titulo = x.Titulo,
                Nino = x.Nino,
                Disciplina = x.Disciplina,
                TipoRegistro = x.TipoRegistro,
                Estado = x.Estado,
                EstadoColorCss = x.EstadoColorCss,
                Prioridad = x.Prioridad,
                PrioridadColorCss = x.PrioridadColorCss,
                FechaRecibido = x.FechaRecibido,
                FechaActualizacion = x.FechaActualizacion,
                AntiguedadDias = Math.Max(0, DateOnly.FromDateTime(DateTime.Today).DayNumber - x.FechaRecibido.DayNumber),
                EstaVencido = x.EstaVencido
            })
            .ToList();
}
