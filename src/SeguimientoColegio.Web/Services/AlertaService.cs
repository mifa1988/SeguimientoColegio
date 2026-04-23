using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SeguimientoColegio.Web.Configuration;
using SeguimientoColegio.Web.Constants;
using SeguimientoColegio.Web.Data;
using SeguimientoColegio.Web.Dtos;

namespace SeguimientoColegio.Web.Services;

public sealed class ConsultaAlertas
{
    public int? NinoId { get; init; }
    public int? DisciplinaId { get; init; }
    public string? Severidad { get; init; }
    public string? Texto { get; init; }
}

public interface IAlertaService
{
    Task<IReadOnlyList<AlertaRegistroDto>> ObtenerAlertasAsync(
        ConsultaAlertas consulta,
        int? ninoSeleccionadoId,
        CancellationToken cancellationToken);

    IReadOnlyDictionary<int, IReadOnlyList<AlertaBadgeDto>> EvaluarBadges(IEnumerable<SnapshotAlertaRegistroDto> snapshots);
}

public sealed class AlertaService(
    SeguimientoDbContext dbContext,
    IOptions<AlertSettings> settings) : IAlertaService
{
    public async Task<IReadOnlyList<AlertaRegistroDto>> ObtenerAlertasAsync(
        ConsultaAlertas consulta,
        int? ninoSeleccionadoId,
        CancellationToken cancellationToken)
    {
        var ninoId = consulta.NinoId ?? ninoSeleccionadoId;

        var query = dbContext.RegistrosEscolares
            .AsNoTracking()
            .Where(x => x.Activo);

        if (ninoId.HasValue)
        {
            query = query.Where(x => x.NinoId == ninoId.Value);
        }

        if (consulta.DisciplinaId.HasValue)
        {
            query = query.Where(x => x.DisciplinaId == consulta.DisciplinaId.Value);
        }

        if (!string.IsNullOrWhiteSpace(consulta.Texto))
        {
            var texto = consulta.Texto.Trim();
            query = query.Where(x =>
                x.Titulo.Contains(texto) ||
                (x.Descripcion != null && x.Descripcion.Contains(texto)) ||
                (x.ComentarioGeneral != null && x.ComentarioGeneral.Contains(texto)) ||
                x.Nino!.Nombres.Contains(texto) ||
                x.Nino.Apellidos.Contains(texto) ||
                (x.Disciplina != null && x.Disciplina.Nombre.Contains(texto)));
        }

        var snapshots = await query
            .OrderByDescending(x => x.FechaActualizacion)
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

        var badges = EvaluarBadges(snapshots);
        var severidadFiltro = NormalizarSeveridad(consulta.Severidad);

        return snapshots
            .Where(snapshot => badges.TryGetValue(snapshot.RegistroId, out var snapshotBadges) && snapshotBadges.Count > 0)
            .Select(snapshot =>
            {
                var snapshotBadges = badges[snapshot.RegistroId];
                return new AlertaRegistroDto
                {
                    RegistroId = snapshot.RegistroId,
                    Titulo = snapshot.Titulo,
                    Nino = snapshot.Nino,
                    Disciplina = snapshot.Disciplina,
                    TipoRegistro = snapshot.TipoRegistro,
                    Estado = snapshot.EstadoSeguimiento,
                    Prioridad = snapshot.Prioridad,
                    FechaRecibido = snapshot.FechaRecibido,
                    FechaVencimiento = snapshot.FechaVencimiento,
                    FechaActualizacion = snapshot.FechaActualizacion,
                    AntiguedadDias = CalcularAntiguedadDias(snapshot.FechaRecibido),
                    SeveridadMaxima = ObtenerSeveridadMaxima(snapshotBadges),
                    Badges = snapshotBadges
                };
            })
            .Where(alerta => severidadFiltro is null || string.Equals(alerta.SeveridadMaxima, severidadFiltro, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(alerta => RankSeveridad(alerta.SeveridadMaxima))
            .ThenByDescending(alerta => alerta.AntiguedadDias)
            .ThenBy(alerta => alerta.FechaVencimiento ?? DateOnly.MaxValue)
            .ToList();
    }

    public IReadOnlyDictionary<int, IReadOnlyList<AlertaBadgeDto>> EvaluarBadges(IEnumerable<SnapshotAlertaRegistroDto> snapshots)
    {
        var result = new Dictionary<int, IReadOnlyList<AlertaBadgeDto>>();

        foreach (var snapshot in snapshots)
        {
            var badges = ConstruirBadges(snapshot, settings.Value);
            if (badges.Count > 0)
            {
                result[snapshot.RegistroId] = badges;
            }
        }

        return result;
    }

    public static List<AlertaBadgeDto> ConstruirBadges(SnapshotAlertaRegistroDto snapshot, AlertSettings settings)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var badges = new List<AlertaBadgeDto>();
        var estaCerrado = snapshot.EstadoSeguimientoId is SeedDataIds.Estados.Concluido or SeedDataIds.Estados.Descartado;
        var antiguedadDias = CalcularAntiguedadDias(snapshot.FechaRecibido);
        var diasSinMovimiento = Math.Max(0, (DateTime.Today - snapshot.FechaActualizacion.ToLocalTime().Date).Days);
        var diasAlerta = snapshot.DiasAlerta ?? settings.DiasPendienteAntiguo;

        if (!snapshot.Activo)
        {
            return badges;
        }

        if (!estaCerrado && snapshot.FechaVencimiento.HasValue && snapshot.FechaVencimiento.Value < today)
        {
            badges.Add(new AlertaBadgeDto
            {
                Codigo = "vencido",
                Titulo = "Vencido",
                Severidad = "critica",
                Mensaje = "La fecha de vencimiento ya paso y el registro sigue abierto."
            });
        }

        if (!estaCerrado && snapshot.RequiereSeguimiento && antiguedadDias >= diasAlerta)
        {
            badges.Add(new AlertaBadgeDto
            {
                Codigo = "pendiente-antiguo",
                Titulo = "Pendiente antiguo",
                Severidad = "advertencia",
                Mensaje = $"Lleva {antiguedadDias} dias abierto sin cerrarse."
            });
        }

        if (!estaCerrado && snapshot.PrioridadId == SeedDataIds.Prioridades.Critica)
        {
            badges.Add(new AlertaBadgeDto
            {
                Codigo = "prioridad-critica",
                Titulo = "Prioridad critica",
                Severidad = "critica",
                Mensaje = "El registro esta marcado con prioridad critica."
            });
        }
        else if (!estaCerrado && snapshot.PrioridadId == SeedDataIds.Prioridades.Alta && antiguedadDias >= settings.DiasPrioridadAlta)
        {
            badges.Add(new AlertaBadgeDto
            {
                Codigo = "prioridad-alta",
                Titulo = "Prioridad alta",
                Severidad = "advertencia",
                Mensaje = "El registro de prioridad alta sigue abierto."
            });
        }

        if (!estaCerrado && snapshot.EstadoSeguimientoId == SeedDataIds.Estados.EnProceso && diasSinMovimiento >= settings.DiasSinMovimiento)
        {
            badges.Add(new AlertaBadgeDto
            {
                Codigo = "sin-movimiento-proceso",
                Titulo = "En proceso sin movimiento",
                Severidad = "info",
                Mensaje = $"No registra movimiento hace {diasSinMovimiento} dias."
            });
        }
        else if (!estaCerrado && diasSinMovimiento >= settings.DiasSinMovimiento)
        {
            badges.Add(new AlertaBadgeDto
            {
                Codigo = "sin-movimiento",
                Titulo = "Sin movimiento",
                Severidad = "advertencia",
                Mensaje = $"No registra actualizacion hace {diasSinMovimiento} dias."
            });
        }

        if (!estaCerrado && snapshot.TipoRegistroId == SeedDataIds.TiposRegistro.AcuerdoProfesora)
        {
            badges.Add(new AlertaBadgeDto
            {
                Codigo = "acuerdo-pendiente",
                Titulo = "Acuerdo pendiente",
                Severidad = "advertencia",
                Mensaje = "Hay un acuerdo con profesora pendiente de seguimiento."
            });
        }

        if (!estaCerrado &&
            snapshot.TipoRegistroId is SeedDataIds.TiposRegistro.MaterialRefuerzo
                or SeedDataIds.TiposRegistro.Lectura
                or SeedDataIds.TiposRegistro.Tarea
                or SeedDataIds.TiposRegistro.Actividad
                or SeedDataIds.TiposRegistro.Evaluacion &&
            antiguedadDias >= 2)
        {
            badges.Add(new AlertaBadgeDto
            {
                Codigo = "actividad-abierta",
                Titulo = "Actividad abierta",
                Severidad = "info",
                Mensaje = "Sigue sin marcarse como concluida."
            });
        }

        return badges;
    }

    private static int CalcularAntiguedadDias(DateOnly fechaRecibido)
        => Math.Max(0, DateOnly.FromDateTime(DateTime.Today).DayNumber - fechaRecibido.DayNumber);

    public static string ObtenerSeveridadMaxima(IReadOnlyList<AlertaBadgeDto> badges)
        => badges.OrderByDescending(x => RankSeveridad(x.Severidad)).Select(x => x.Severidad).FirstOrDefault() ?? "info";

    private static string? NormalizarSeveridad(string? severidad)
    {
        if (string.IsNullOrWhiteSpace(severidad))
        {
            return null;
        }

        return severidad.Trim().ToLowerInvariant() switch
        {
            "critica" => "critica",
            "advertencia" => "advertencia",
            "info" => "info",
            _ => null
        };
    }

    private static int RankSeveridad(string? severidad)
        => severidad?.ToLowerInvariant() switch
        {
            "critica" => 3,
            "advertencia" => 2,
            "info" => 1,
            _ => 0
        };
}
