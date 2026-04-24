using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SeguimientoColegio.Web.Constants;
using SeguimientoColegio.Web.Data;
using SeguimientoColegio.Web.Data.Entities;
using SeguimientoColegio.Web.Dtos;
using SeguimientoColegio.Web.Models;

namespace SeguimientoColegio.Web.Services;

public interface IRegistroEscolarService
{
    Task<RegistrosIndexViewModel> ObtenerListadoAsync(
        RegistroFiltrosViewModel filtros,
        int? ninoSeleccionadoId,
        bool modoAcuerdos,
        CancellationToken cancellationToken);

    Task<RegistroEditorViewModel> ObtenerEditorAsync(
        int? id,
        int? tipoRegistroPreset,
        int? ninoSeleccionadoId,
        bool modoAcuerdos,
        CancellationToken cancellationToken);

    Task<RegistroDetalleViewModel?> ObtenerDetalleAsync(int id, CancellationToken cancellationToken);
    Task<int> CrearAsync(RegistroEscolarInputModel form, int actorId, CancellationToken cancellationToken);
    Task<bool> ActualizarAsync(int id, RegistroEscolarInputModel form, int actorId, CancellationToken cancellationToken);
    Task<bool> CambiarEstadoRapidoAsync(int id, int estadoId, int actorId, string? comentario, CancellationToken cancellationToken);
    Task<bool> DesactivarAsync(int id, int actorId, CancellationToken cancellationToken);
}

public sealed class RegistroEscolarService(
    SeguimientoDbContext dbContext,
    IArchivoService archivoService,
    IAlertaService alertaService) : IRegistroEscolarService
{
    public async Task<RegistrosIndexViewModel> ObtenerListadoAsync(
        RegistroFiltrosViewModel filtros,
        int? ninoSeleccionadoId,
        bool modoAcuerdos,
        CancellationToken cancellationToken)
    {
        var effectiveFilters = new RegistroFiltrosViewModel
        {
            NinoId = filtros.NinoId ?? ninoSeleccionadoId,
            DisciplinaId = filtros.DisciplinaId,
            TipoRegistroId = modoAcuerdos ? SeedDataIds.TiposRegistro.AcuerdoProfesora : filtros.TipoRegistroId,
            PlataformaId = filtros.PlataformaId,
            EstadoSeguimientoId = filtros.EstadoSeguimientoId,
            PrioridadId = filtros.PrioridadId,
            FechaRecibidoDesde = filtros.FechaRecibidoDesde,
            FechaRecibidoHasta = filtros.FechaRecibidoHasta,
            FechaRealizadoDesde = filtros.FechaRealizadoDesde,
            FechaRealizadoHasta = filtros.FechaRealizadoHasta,
            FechaVencimientoDesde = filtros.FechaVencimientoDesde,
            FechaVencimientoHasta = filtros.FechaVencimientoHasta,
            Texto = filtros.Texto
        };

        var query = dbContext.RegistrosEscolares
            .AsNoTracking()
            .Where(x => x.Activo);

        if (effectiveFilters.NinoId.HasValue)
        {
            query = query.Where(x => x.NinoId == effectiveFilters.NinoId.Value);
        }

        if (effectiveFilters.DisciplinaId.HasValue)
        {
            query = query.Where(x => x.DisciplinaId == effectiveFilters.DisciplinaId.Value);
        }

        if (effectiveFilters.TipoRegistroId.HasValue)
        {
            query = query.Where(x => x.TipoRegistroId == effectiveFilters.TipoRegistroId.Value);
        }

        if (effectiveFilters.PlataformaId.HasValue)
        {
            query = query.Where(x => x.PlataformaId == effectiveFilters.PlataformaId.Value);
        }

        if (effectiveFilters.EstadoSeguimientoId.HasValue)
        {
            query = query.Where(x => x.EstadoSeguimientoId == effectiveFilters.EstadoSeguimientoId.Value);
        }

        if (effectiveFilters.PrioridadId.HasValue)
        {
            query = query.Where(x => x.PrioridadId == effectiveFilters.PrioridadId.Value);
        }

        if (effectiveFilters.FechaRecibidoDesde.HasValue)
        {
            query = query.Where(x => x.FechaRecibido >= effectiveFilters.FechaRecibidoDesde.Value);
        }

        if (effectiveFilters.FechaRecibidoHasta.HasValue)
        {
            query = query.Where(x => x.FechaRecibido <= effectiveFilters.FechaRecibidoHasta.Value);
        }

        if (effectiveFilters.FechaRealizadoDesde.HasValue)
        {
            query = query.Where(x => x.FechaRealizado >= effectiveFilters.FechaRealizadoDesde.Value);
        }

        if (effectiveFilters.FechaRealizadoHasta.HasValue)
        {
            query = query.Where(x => x.FechaRealizado <= effectiveFilters.FechaRealizadoHasta.Value);
        }

        if (effectiveFilters.FechaVencimientoDesde.HasValue)
        {
            query = query.Where(x => x.FechaVencimiento >= effectiveFilters.FechaVencimientoDesde.Value);
        }

        if (effectiveFilters.FechaVencimientoHasta.HasValue)
        {
            query = query.Where(x => x.FechaVencimiento <= effectiveFilters.FechaVencimientoHasta.Value);
        }

        if (!string.IsNullOrWhiteSpace(effectiveFilters.Texto))
        {
            var texto = effectiveFilters.Texto.Trim();
            query = query.Where(x =>
                x.Titulo.Contains(texto) ||
                (x.Descripcion != null && x.Descripcion.Contains(texto)) ||
                (x.ComentarioGeneral != null && x.ComentarioGeneral.Contains(texto)) ||
                (x.CorreoOrigen != null && x.CorreoOrigen.Asunto.Contains(texto)) ||
                (x.ResponsablePrincipal != null && x.ResponsablePrincipal.Contains(texto)));
        }

        var rows = await query
            .OrderByDescending(x => x.FechaActualizacion)
            .Select(x => new RegistroListaRow
            {
                Snapshot = new SnapshotAlertaRegistroDto
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
                },
                EstadoColorCss = x.EstadoSeguimiento!.ColorCss ?? "#7a828f",
                PrioridadColorCss = x.Prioridad != null ? x.Prioridad.ColorCss : null,
                Plataforma = x.Plataforma != null ? x.Plataforma.Nombre : null,
                CorreoOrigenAsunto = x.CorreoOrigen != null ? x.CorreoOrigen.Asunto : null,
                ResponsablePrincipal = x.ResponsablePrincipal,
                UrlsCount = x.Urls.Count,
                EvidenciasCount = x.Evidencias.Count
            })
            .ToListAsync(cancellationToken);

        var badges = alertaService.EvaluarBadges(rows.Select(x => x.Snapshot));
        var today = DateOnly.FromDateTime(DateTime.Today);

        var registros = rows
            .Select(x =>
            {
                badges.TryGetValue(x.Snapshot.RegistroId, out var rowBadges);

                return new RegistroListaDto
                {
                    Id = x.Snapshot.RegistroId,
                    Titulo = x.Snapshot.Titulo,
                    Nino = x.Snapshot.Nino,
                    Disciplina = x.Snapshot.Disciplina,
                    TipoRegistro = x.Snapshot.TipoRegistro,
                    Plataforma = x.Plataforma,
                    Estado = x.Snapshot.EstadoSeguimiento,
                    EstadoColorCss = x.EstadoColorCss,
                    EstadoSeguimientoId = x.Snapshot.EstadoSeguimientoId,
                    Prioridad = x.Snapshot.Prioridad,
                    PrioridadColorCss = x.PrioridadColorCss,
                    PrioridadId = x.Snapshot.PrioridadId,
                    FechaRecibido = x.Snapshot.FechaRecibido,
                    FechaVencimiento = x.Snapshot.FechaVencimiento,
                    FechaRealizado = x.Snapshot.FechaRealizado,
                    FechaActualizacion = x.Snapshot.FechaActualizacion,
                    ResponsablePrincipal = x.ResponsablePrincipal,
                    CorreoOrigenAsunto = x.CorreoOrigenAsunto,
                    UrlsCount = x.UrlsCount,
                    EvidenciasCount = x.EvidenciasCount,
                    AntiguedadDias = Math.Max(0, today.DayNumber - x.Snapshot.FechaRecibido.DayNumber),
                    EstaVencido = x.Snapshot.FechaVencimiento.HasValue &&
                                  x.Snapshot.FechaVencimiento.Value < today &&
                                  x.Snapshot.EstadoSeguimientoId is not SeedDataIds.Estados.Concluido and not SeedDataIds.Estados.Descartado,
                    Alertas = rowBadges ?? Array.Empty<AlertaBadgeDto>()
                };
            })
            .ToList();

        return new RegistrosIndexViewModel
        {
            Titulo = modoAcuerdos ? "Acuerdos con profesoras" : "Registros escolares",
            ModoAcuerdos = modoAcuerdos,
            Filtros = effectiveFilters,
            Registros = registros,
            Ninos = await BuildSelectListAsync(dbContext.Ninos.Where(x => x.Activo), x => x.NombreCompleto, effectiveFilters.NinoId, true, cancellationToken),
            Disciplinas = await BuildSelectListAsync(dbContext.Disciplinas.Where(x => x.Activo), x => x.Nombre, effectiveFilters.DisciplinaId, true, cancellationToken),
            TiposRegistro = await BuildSelectListAsync(
                modoAcuerdos
                    ? dbContext.TiposRegistro.Where(x => x.Id == SeedDataIds.TiposRegistro.AcuerdoProfesora)
                    : dbContext.TiposRegistro.Where(x => x.Activo),
                x => x.Nombre,
                effectiveFilters.TipoRegistroId,
                !modoAcuerdos,
                cancellationToken),
            Plataformas = await BuildSelectListAsync(dbContext.Plataformas.Where(x => x.Activo), x => x.Nombre, effectiveFilters.PlataformaId, true, cancellationToken),
            Estados = await BuildSelectListAsync(dbContext.EstadosSeguimiento.Where(x => x.Activo).OrderBy(x => x.Orden), x => x.Nombre, effectiveFilters.EstadoSeguimientoId, true, cancellationToken),
            Prioridades = await BuildSelectListAsync(dbContext.Prioridades.Where(x => x.Activo).OrderBy(x => x.Orden), x => x.Nombre, effectiveFilters.PrioridadId, true, cancellationToken)
        };
    }

    public async Task<RegistroEditorViewModel> ObtenerEditorAsync(
        int? id,
        int? tipoRegistroPreset,
        int? ninoSeleccionadoId,
        bool modoAcuerdos,
        CancellationToken cancellationToken)
    {
        RegistroEscolarInputModel form;
        IReadOnlyList<EvidenciaDto> evidencias = Array.Empty<EvidenciaDto>();

        if (id.HasValue)
        {
            var entity = await dbContext.RegistrosEscolares
                .AsNoTracking()
                .Include(x => x.Urls)
                .Include(x => x.Evidencias)
                    .ThenInclude(x => x.SubidoPorUsuario)
                .SingleAsync(x => x.Id == id.Value, cancellationToken);

            form = new RegistroEscolarInputModel
            {
                NinoId = entity.NinoId,
                DisciplinaId = entity.DisciplinaId,
                TipoRegistroId = entity.TipoRegistroId,
                PlataformaId = entity.PlataformaId,
                CorreoOrigenId = entity.CorreoOrigenId,
                EstadoSeguimientoId = entity.EstadoSeguimientoId,
                PrioridadId = entity.PrioridadId,
                Titulo = entity.Titulo,
                Descripcion = entity.Descripcion,
                ResponsablePrincipal = entity.ResponsablePrincipal,
                FechaRecibido = entity.FechaRecibido,
                FechaVencimiento = entity.FechaVencimiento,
                FechaRealizado = entity.FechaRealizado,
                DiasAlerta = entity.DiasAlerta,
                RequiereSeguimiento = entity.RequiereSeguimiento,
                ComentarioGeneral = entity.ComentarioGeneral,
                Activo = entity.Activo,
                Urls = entity.Urls
                    .OrderBy(x => x.Orden)
                    .Select(x => new RegistroUrlInputModel
                    {
                        Id = x.Id,
                        Url = x.Url,
                        Descripcion = x.Descripcion,
                        Orden = x.Orden
                    })
                    .ToList()
            };

            if (form.Urls.Count == 0)
            {
                form.Urls.Add(new RegistroUrlInputModel());
            }

            evidencias = entity.Evidencias
                .OrderByDescending(x => x.FechaCreacion)
                .Select(x => new EvidenciaDto
                {
                    Id = x.Id,
                    NombreArchivo = x.NombreArchivo,
                    TipoMime = x.TipoMime,
                    TamanoBytes = x.TamanoBytes,
                    Descripcion = x.Descripcion,
                    SubidoPor = x.SubidoPorUsuario != null ? x.SubidoPorUsuario.NombreCompleto : "Usuario",
                    FechaCreacion = x.FechaCreacion
                })
                .ToList();
        }
        else
        {
            form = new RegistroEscolarInputModel
            {
                NinoId = ninoSeleccionadoId,
                TipoRegistroId = modoAcuerdos ? SeedDataIds.TiposRegistro.AcuerdoProfesora : tipoRegistroPreset,
                EstadoSeguimientoId = SeedDataIds.Estados.Pendiente,
                RequiereSeguimiento = true,
                Urls = new List<RegistroUrlInputModel> { new() }
            };
        }

        var tipoRegistroActual = form.TipoRegistroId ?? tipoRegistroPreset;
        var esAcuerdo = modoAcuerdos || tipoRegistroActual == SeedDataIds.TiposRegistro.AcuerdoProfesora;

        return new RegistroEditorViewModel
        {
            Titulo = id.HasValue
                ? (esAcuerdo ? "Editar acuerdo" : "Editar registro")
                : (esAcuerdo ? "Nuevo acuerdo" : "Nuevo registro"),
            IsEdit = id.HasValue,
            ModoAcuerdos = esAcuerdo,
            Form = form,
            EvidenciasExistentes = evidencias,
            Ninos = await BuildSelectListAsync(dbContext.Ninos.Where(x => x.Activo), x => x.NombreCompleto, form.NinoId, false, cancellationToken),
            Disciplinas = await BuildSelectListAsync(dbContext.Disciplinas.Where(x => x.Activo), x => x.Nombre, form.DisciplinaId, true, cancellationToken),
            TiposRegistro = await BuildSelectListAsync(
                esAcuerdo
                    ? dbContext.TiposRegistro.Where(x => x.Id == SeedDataIds.TiposRegistro.AcuerdoProfesora)
                    : dbContext.TiposRegistro.Where(x => x.Activo),
                x => x.Nombre,
                form.TipoRegistroId,
                !esAcuerdo,
                cancellationToken),
            Plataformas = await BuildSelectListAsync(dbContext.Plataformas.Where(x => x.Activo), x => x.Nombre, form.PlataformaId, true, cancellationToken),
            CorreosOrigen = await dbContext.CorreosOrigen
                .AsNoTracking()
                .OrderByDescending(x => x.FechaCorreo)
                .Take(100)
                .Select(x => new SelectListItem($"{x.FechaCorreo:dd/MM/yyyy} - {x.Asunto}", x.Id.ToString(), x.Id == form.CorreoOrigenId))
                .Prepend(new SelectListItem("Sin correo origen", string.Empty, !form.CorreoOrigenId.HasValue))
                .ToListAsync(cancellationToken),
            Estados = await BuildSelectListAsync(dbContext.EstadosSeguimiento.Where(x => x.Activo).OrderBy(x => x.Orden), x => x.Nombre, form.EstadoSeguimientoId, false, cancellationToken),
            Prioridades = await BuildSelectListAsync(dbContext.Prioridades.Where(x => x.Activo).OrderBy(x => x.Orden), x => x.Nombre, form.PrioridadId, true, cancellationToken)
        };
    }

    public async Task<RegistroDetalleViewModel?> ObtenerDetalleAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await dbContext.RegistrosEscolares
            .AsNoTracking()
            .Include(x => x.Nino)
            .Include(x => x.Disciplina)
            .Include(x => x.TipoRegistro)
            .Include(x => x.Plataforma)
            .Include(x => x.CorreoOrigen)
            .Include(x => x.EstadoSeguimiento)
            .Include(x => x.Prioridad)
            .Include(x => x.CreadoPorUsuario)
            .Include(x => x.ActualizadoPorUsuario)
            .Include(x => x.Urls)
            .Include(x => x.Evidencias)
                .ThenInclude(x => x.SubidoPorUsuario)
            .Include(x => x.Auditoria)
                .ThenInclude(x => x.Usuario)
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (entity is null)
        {
            return null;
        }

        var snapshot = BuildSnapshot(entity);
        var badgesPorRegistro = alertaService.EvaluarBadges(new[] { snapshot });
        badgesPorRegistro.TryGetValue(entity.Id, out var badges);

        return new RegistroDetalleViewModel
        {
            PuedeEditar = true,
            Estados = await BuildSelectListAsync(dbContext.EstadosSeguimiento.Where(x => x.Activo).OrderBy(x => x.Orden), x => x.Nombre, entity.EstadoSeguimientoId, false, cancellationToken),
            Registro = new RegistroDetalleDto
            {
                Id = entity.Id,
                Titulo = entity.Titulo,
                Descripcion = entity.Descripcion,
                Nino = entity.Nino?.NombreCompleto ?? string.Empty,
                NinoId = entity.NinoId,
                Disciplina = entity.Disciplina?.Nombre,
                DisciplinaId = entity.DisciplinaId,
                TipoRegistro = entity.TipoRegistro?.Nombre ?? string.Empty,
                TipoRegistroId = entity.TipoRegistroId,
                Plataforma = entity.Plataforma?.Nombre,
                PlataformaId = entity.PlataformaId,
                Estado = entity.EstadoSeguimiento?.Nombre ?? string.Empty,
                EstadoSeguimientoId = entity.EstadoSeguimientoId,
                EstadoColorCss = entity.EstadoSeguimiento?.ColorCss ?? "#7a828f",
                Prioridad = entity.Prioridad?.Nombre,
                PrioridadId = entity.PrioridadId,
                PrioridadColorCss = entity.Prioridad?.ColorCss,
                CorreoOrigenAsunto = entity.CorreoOrigen?.Asunto,
                CorreoOrigenId = entity.CorreoOrigenId,
                ResponsablePrincipal = entity.ResponsablePrincipal,
                FechaRecibido = entity.FechaRecibido,
                FechaVencimiento = entity.FechaVencimiento,
                FechaRealizado = entity.FechaRealizado,
                DiasAlerta = entity.DiasAlerta,
                RequiereSeguimiento = entity.RequiereSeguimiento,
                ComentarioGeneral = entity.ComentarioGeneral,
                CreadoPor = entity.CreadoPorUsuario?.NombreCompleto ?? string.Empty,
                ActualizadoPor = entity.ActualizadoPorUsuario?.NombreCompleto ?? string.Empty,
                FechaCreacion = entity.FechaCreacion,
                FechaActualizacion = entity.FechaActualizacion,
                Activo = entity.Activo,
                Urls = entity.Urls
                    .OrderBy(x => x.Orden)
                    .Select(x => new RegistroUrlDto
                    {
                        Id = x.Id,
                        Url = x.Url,
                        Descripcion = x.Descripcion,
                        Orden = x.Orden
                    })
                    .ToList(),
                Evidencias = entity.Evidencias
                    .OrderByDescending(x => x.FechaCreacion)
                    .Select(x => new EvidenciaDto
                    {
                        Id = x.Id,
                        NombreArchivo = x.NombreArchivo,
                        TipoMime = x.TipoMime,
                        TamanoBytes = x.TamanoBytes,
                        Descripcion = x.Descripcion,
                        SubidoPor = x.SubidoPorUsuario != null ? x.SubidoPorUsuario.NombreCompleto : "Usuario",
                        FechaCreacion = x.FechaCreacion
                    })
                    .ToList(),
                Historial = entity.Auditoria
                    .OrderByDescending(x => x.FechaCreacion)
                    .Select(x => new RegistroAuditoriaDto
                    {
                        Id = x.Id,
                        Accion = x.Accion,
                        CampoModificado = x.CampoModificado,
                        ValorAnterior = x.ValorAnterior,
                        ValorNuevo = x.ValorNuevo,
                        Comentario = x.Comentario,
                        Usuario = x.Usuario != null ? x.Usuario.NombreCompleto : "Usuario",
                        FechaCreacion = x.FechaCreacion
                    })
                    .ToList(),
                Alertas = badges ?? Array.Empty<AlertaBadgeDto>()
            }
        };
    }

    public async Task<int> CrearAsync(RegistroEscolarInputModel form, int actorId, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        var entity = new RegistroEscolar
        {
            NinoId = form.NinoId!.Value,
            DisciplinaId = form.DisciplinaId,
            TipoRegistroId = form.TipoRegistroId!.Value,
            PlataformaId = form.PlataformaId,
            CorreoOrigenId = form.CorreoOrigenId,
            EstadoSeguimientoId = form.EstadoSeguimientoId ?? SeedDataIds.Estados.Pendiente,
            PrioridadId = form.PrioridadId,
            Titulo = form.Titulo.Trim(),
            Descripcion = NormalizeNullable(form.Descripcion),
            ResponsablePrincipal = NormalizeNullable(form.ResponsablePrincipal),
            FechaRecibido = form.FechaRecibido,
            FechaVencimiento = form.FechaVencimiento,
            FechaRealizado = form.FechaRealizado ?? AutoAssignCompletionDate(form.EstadoSeguimientoId),
            DiasAlerta = form.DiasAlerta,
            RequiereSeguimiento = form.RequiereSeguimiento,
            ComentarioGeneral = NormalizeNullable(form.ComentarioGeneral),
            CreadoPorUsuarioId = actorId,
            ActualizadoPorUsuarioId = actorId,
            FechaCreacion = now,
            FechaActualizacion = now,
            Activo = form.Activo
        };

        dbContext.RegistrosEscolares.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        var audits = new List<RegistroAuditoria>
        {
            CrearAuditoria(entity.Id, actorId, now, "Creado", comentario: "Registro creado.")
        };

        await SyncUrlsAsync(entity, NormalizeUrls(form.Urls), actorId, audits, now, cancellationToken);
        await GuardarAdjuntosAsync(entity, form.NuevosAdjuntos, actorId, audits, now, cancellationToken);

        dbContext.RegistroAuditoria.AddRange(audits);
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return entity.Id;
    }

    public async Task<bool> ActualizarAsync(int id, RegistroEscolarInputModel form, int actorId, CancellationToken cancellationToken)
    {
        var entity = await dbContext.RegistrosEscolares
            .Include(x => x.Nino)
            .Include(x => x.Disciplina)
            .Include(x => x.TipoRegistro)
            .Include(x => x.Plataforma)
            .Include(x => x.CorreoOrigen)
            .Include(x => x.EstadoSeguimiento)
            .Include(x => x.Prioridad)
            .Include(x => x.Urls)
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (entity is null)
        {
            return false;
        }

        var now = DateTime.UtcNow;
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        var newLabels = await ResolveReferenceLabelsAsync(form, cancellationToken);
        var audits = new List<RegistroAuditoria>();

        TrackChange(audits, entity.Id, actorId, now, "Nino", entity.Nino?.NombreCompleto, newLabels.Nino, entity.NinoId != form.NinoId);
        TrackChange(audits, entity.Id, actorId, now, "Disciplina", entity.Disciplina?.Nombre, newLabels.Disciplina, entity.DisciplinaId != form.DisciplinaId);
        TrackChange(audits, entity.Id, actorId, now, "Tipo", entity.TipoRegistro?.Nombre, newLabels.TipoRegistro, entity.TipoRegistroId != form.TipoRegistroId);
        TrackChange(audits, entity.Id, actorId, now, "Plataforma", entity.Plataforma?.Nombre, newLabels.Plataforma, entity.PlataformaId != form.PlataformaId);
        TrackChange(audits, entity.Id, actorId, now, "Correo origen", entity.CorreoOrigen?.Asunto, newLabels.CorreoOrigen, entity.CorreoOrigenId != form.CorreoOrigenId);
        TrackChange(audits, entity.Id, actorId, now, "Estado", entity.EstadoSeguimiento?.Nombre, newLabels.Estado, entity.EstadoSeguimientoId != form.EstadoSeguimientoId);
        TrackChange(audits, entity.Id, actorId, now, "Prioridad", entity.Prioridad?.Nombre, newLabels.Prioridad, entity.PrioridadId != form.PrioridadId);
        TrackChange(audits, entity.Id, actorId, now, "Titulo", entity.Titulo, form.Titulo.Trim(), !string.Equals(entity.Titulo, form.Titulo.Trim(), StringComparison.Ordinal));
        TrackChange(audits, entity.Id, actorId, now, "Descripcion", entity.Descripcion, NormalizeNullable(form.Descripcion), !string.Equals(entity.Descripcion, NormalizeNullable(form.Descripcion), StringComparison.Ordinal));
        TrackChange(audits, entity.Id, actorId, now, "Responsable principal", entity.ResponsablePrincipal, NormalizeNullable(form.ResponsablePrincipal), !string.Equals(entity.ResponsablePrincipal, NormalizeNullable(form.ResponsablePrincipal), StringComparison.Ordinal));
        TrackChange(audits, entity.Id, actorId, now, "Fecha recibido", entity.FechaRecibido.ToString("dd/MM/yyyy"), form.FechaRecibido.ToString("dd/MM/yyyy"), entity.FechaRecibido != form.FechaRecibido);
        TrackChange(audits, entity.Id, actorId, now, "Fecha vencimiento", FormatDate(entity.FechaVencimiento), FormatDate(form.FechaVencimiento), entity.FechaVencimiento != form.FechaVencimiento);
        TrackChange(audits, entity.Id, actorId, now, "Fecha realizado", FormatDate(entity.FechaRealizado), FormatDate(form.FechaRealizado ?? AutoAssignCompletionDate(form.EstadoSeguimientoId)), entity.FechaRealizado != (form.FechaRealizado ?? AutoAssignCompletionDate(form.EstadoSeguimientoId)));
        TrackChange(audits, entity.Id, actorId, now, "Dias alerta", entity.DiasAlerta?.ToString(), form.DiasAlerta?.ToString(), entity.DiasAlerta != form.DiasAlerta);
        TrackChange(audits, entity.Id, actorId, now, "Requiere seguimiento", entity.RequiereSeguimiento ? "Si" : "No", form.RequiereSeguimiento ? "Si" : "No", entity.RequiereSeguimiento != form.RequiereSeguimiento);
        TrackChange(audits, entity.Id, actorId, now, "Comentario general", entity.ComentarioGeneral, NormalizeNullable(form.ComentarioGeneral), !string.Equals(entity.ComentarioGeneral, NormalizeNullable(form.ComentarioGeneral), StringComparison.Ordinal));
        TrackChange(audits, entity.Id, actorId, now, "Activo", entity.Activo ? "Si" : "No", form.Activo ? "Si" : "No", entity.Activo != form.Activo);

        entity.NinoId = form.NinoId!.Value;
        entity.DisciplinaId = form.DisciplinaId;
        entity.TipoRegistroId = form.TipoRegistroId!.Value;
        entity.PlataformaId = form.PlataformaId;
        entity.CorreoOrigenId = form.CorreoOrigenId;
        entity.EstadoSeguimientoId = form.EstadoSeguimientoId ?? entity.EstadoSeguimientoId;
        entity.PrioridadId = form.PrioridadId;
        entity.Titulo = form.Titulo.Trim();
        entity.Descripcion = NormalizeNullable(form.Descripcion);
        entity.ResponsablePrincipal = NormalizeNullable(form.ResponsablePrincipal);
        entity.FechaRecibido = form.FechaRecibido;
        entity.FechaVencimiento = form.FechaVencimiento;
        entity.FechaRealizado = form.FechaRealizado ?? AutoAssignCompletionDate(form.EstadoSeguimientoId);
        entity.DiasAlerta = form.DiasAlerta;
        entity.RequiereSeguimiento = form.RequiereSeguimiento;
        entity.ComentarioGeneral = NormalizeNullable(form.ComentarioGeneral);
        entity.Activo = form.Activo;
        entity.ActualizadoPorUsuarioId = actorId;
        entity.FechaActualizacion = now;

        await SyncUrlsAsync(entity, NormalizeUrls(form.Urls), actorId, audits, now, cancellationToken);
        await GuardarAdjuntosAsync(entity, form.NuevosAdjuntos, actorId, audits, now, cancellationToken);

        dbContext.RegistroAuditoria.AddRange(audits);
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return true;
    }

    public async Task<bool> CambiarEstadoRapidoAsync(int id, int estadoId, int actorId, string? comentario, CancellationToken cancellationToken)
    {
        var entity = await dbContext.RegistrosEscolares
            .Include(x => x.EstadoSeguimiento)
            .SingleOrDefaultAsync(x => x.Id == id && x.Activo, cancellationToken);

        if (entity is null)
        {
            return false;
        }

        var nuevoEstado = await dbContext.EstadosSeguimiento
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == estadoId, cancellationToken);

        if (nuevoEstado is null)
        {
            return false;
        }

        var now = DateTime.UtcNow;
        var audit = CrearAuditoria(
            entity.Id,
            actorId,
            now,
            "Cambio de estado",
            "Estado",
            entity.EstadoSeguimiento?.Nombre,
            nuevoEstado.Nombre,
            NormalizeNullable(comentario));

        entity.EstadoSeguimientoId = estadoId;
        entity.FechaRealizado = estadoId == SeedDataIds.Estados.Concluido
            ? entity.FechaRealizado ?? DateOnly.FromDateTime(DateTime.Today)
            : entity.FechaRealizado;
        entity.ActualizadoPorUsuarioId = actorId;
        entity.FechaActualizacion = now;

        dbContext.RegistroAuditoria.Add(audit);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DesactivarAsync(int id, int actorId, CancellationToken cancellationToken)
    {
        var entity = await dbContext.RegistrosEscolares.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        entity.Activo = false;
        entity.ActualizadoPorUsuarioId = actorId;
        entity.FechaActualizacion = DateTime.UtcNow;

        dbContext.RegistroAuditoria.Add(CrearAuditoria(entity.Id, actorId, DateTime.UtcNow, "Desactivado", comentario: "Registro marcado como inactivo."));
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task<ReferenceLabels> ResolveReferenceLabelsAsync(RegistroEscolarInputModel form, CancellationToken cancellationToken)
    {
        return new ReferenceLabels
        {
            Nino = form.NinoId.HasValue
                ? await dbContext.Ninos.Where(x => x.Id == form.NinoId.Value).Select(x => x.NombreCompleto).SingleAsync(cancellationToken)
                : null,
            Disciplina = form.DisciplinaId.HasValue
                ? await dbContext.Disciplinas.Where(x => x.Id == form.DisciplinaId.Value).Select(x => x.Nombre).SingleOrDefaultAsync(cancellationToken)
                : null,
            TipoRegistro = form.TipoRegistroId.HasValue
                ? await dbContext.TiposRegistro.Where(x => x.Id == form.TipoRegistroId.Value).Select(x => x.Nombre).SingleAsync(cancellationToken)
                : null,
            Plataforma = form.PlataformaId.HasValue
                ? await dbContext.Plataformas.Where(x => x.Id == form.PlataformaId.Value).Select(x => x.Nombre).SingleOrDefaultAsync(cancellationToken)
                : null,
            CorreoOrigen = form.CorreoOrigenId.HasValue
                ? await dbContext.CorreosOrigen.Where(x => x.Id == form.CorreoOrigenId.Value).Select(x => x.Asunto).SingleOrDefaultAsync(cancellationToken)
                : null,
            Estado = form.EstadoSeguimientoId.HasValue
                ? await dbContext.EstadosSeguimiento.Where(x => x.Id == form.EstadoSeguimientoId.Value).Select(x => x.Nombre).SingleAsync(cancellationToken)
                : null,
            Prioridad = form.PrioridadId.HasValue
                ? await dbContext.Prioridades.Where(x => x.Id == form.PrioridadId.Value).Select(x => x.Nombre).SingleOrDefaultAsync(cancellationToken)
                : null
        };
    }

    private async Task SyncUrlsAsync(
        RegistroEscolar entity,
        List<RegistroUrlInputModel> urls,
        int actorId,
        List<RegistroAuditoria> audits,
        DateTime now,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var currentUrls = entity.Urls.ToDictionary(x => x.Id);
        var incomingIds = urls.Where(x => x.Id.HasValue).Select(x => x.Id!.Value).ToHashSet();

        foreach (var existing in entity.Urls.Where(x => !incomingIds.Contains(x.Id)).ToList())
        {
            dbContext.RegistroUrls.Remove(existing);
            audits.Add(CrearAuditoria(entity.Id, actorId, now, "URL eliminada", "URL", existing.Url, null, existing.Descripcion));
        }

        foreach (var input in urls)
        {
            if (input.Id.HasValue && currentUrls.TryGetValue(input.Id.Value, out var existing))
            {
                TrackChange(audits, entity.Id, actorId, now, "URL", existing.Url, input.Url, !string.Equals(existing.Url, input.Url, StringComparison.Ordinal));
                TrackChange(audits, entity.Id, actorId, now, "Descripcion URL", existing.Descripcion, NormalizeNullable(input.Descripcion), !string.Equals(existing.Descripcion, NormalizeNullable(input.Descripcion), StringComparison.Ordinal));

                existing.Url = input.Url!.Trim();
                existing.Descripcion = NormalizeNullable(input.Descripcion);
                existing.Orden = input.Orden;
            }
            else
            {
                var url = new RegistroUrl
                {
                    Url = input.Url!.Trim(),
                    Descripcion = NormalizeNullable(input.Descripcion),
                    Orden = input.Orden,
                    FechaCreacion = now
                };

                entity.Urls.Add(url);
                audits.Add(CrearAuditoria(entity.Id, actorId, now, "URL agregada", "URL", null, url.Url, url.Descripcion));
            }
        }
    }

    private async Task GuardarAdjuntosAsync(
        RegistroEscolar entity,
        IEnumerable<IFormFile> archivos,
        int actorId,
        List<RegistroAuditoria> audits,
        DateTime now,
        CancellationToken cancellationToken)
    {
        foreach (var archivo in archivos.Where(x => x.Length > 0))
        {
            var saved = await archivoService.GuardarAdjuntoAsync(archivo, entity.Id, cancellationToken);
            entity.Evidencias.Add(new EvidenciaRegistro
            {
                NombreArchivo = saved.NombreOriginal,
                StorageKey = saved.StorageKey,
                TipoMime = saved.TipoMime,
                TamanoBytes = saved.TamanoBytes,
                SubidoPorUsuarioId = actorId,
                FechaCreacion = now
            });

            audits.Add(CrearAuditoria(entity.Id, actorId, now, "Evidencia agregada", "Adjunto", null, saved.NombreOriginal, archivo.ContentType));
        }
    }

    private static List<RegistroUrlInputModel> NormalizeUrls(IEnumerable<RegistroUrlInputModel> urls)
        => urls
            .Where(x => !string.IsNullOrWhiteSpace(x.Url))
            .Select((x, index) => new RegistroUrlInputModel
            {
                Id = x.Id,
                Url = x.Url?.Trim(),
                Descripcion = NormalizeNullable(x.Descripcion),
                Orden = x.Orden > 0 ? x.Orden : index + 1
            })
            .ToList();

    private static void TrackChange(
        ICollection<RegistroAuditoria> audits,
        int registroId,
        int actorId,
        DateTime now,
        string campo,
        string? valorAnterior,
        string? valorNuevo,
        bool changed)
    {
        if (!changed)
        {
            return;
        }

        audits.Add(CrearAuditoria(registroId, actorId, now, "Actualizado", campo, valorAnterior, valorNuevo));
    }

    private static RegistroAuditoria CrearAuditoria(
        int registroId,
        int actorId,
        DateTime now,
        string accion,
        string? campo = null,
        string? valorAnterior = null,
        string? valorNuevo = null,
        string? comentario = null)
        => new()
        {
            RegistroEscolarId = registroId,
            Accion = accion,
            CampoModificado = campo,
            ValorAnterior = valorAnterior,
            ValorNuevo = valorNuevo,
            Comentario = comentario,
            UsuarioId = actorId,
            FechaCreacion = now
        };

    private static string? NormalizeNullable(string? value)
    {
        var trimmed = value?.Trim();
        return string.IsNullOrWhiteSpace(trimmed) ? null : trimmed;
    }

    private static DateOnly? AutoAssignCompletionDate(int? estadoId)
        => estadoId == SeedDataIds.Estados.Concluido
            ? DateOnly.FromDateTime(DateTime.Today)
            : null;

    private static string? FormatDate(DateOnly? date)
        => date?.ToString("dd/MM/yyyy");

    private static SnapshotAlertaRegistroDto BuildSnapshot(RegistroEscolar entity)
        => new()
        {
            RegistroId = entity.Id,
            NinoId = entity.NinoId,
            Nino = entity.Nino?.NombreCompleto ?? string.Empty,
            DisciplinaId = entity.DisciplinaId,
            Disciplina = entity.Disciplina?.Nombre,
            TipoRegistroId = entity.TipoRegistroId,
            TipoRegistro = entity.TipoRegistro?.Nombre ?? string.Empty,
            EstadoSeguimientoId = entity.EstadoSeguimientoId,
            EstadoSeguimiento = entity.EstadoSeguimiento?.Nombre ?? string.Empty,
            PrioridadId = entity.PrioridadId,
            Prioridad = entity.Prioridad?.Nombre,
            Titulo = entity.Titulo,
            FechaRecibido = entity.FechaRecibido,
            FechaVencimiento = entity.FechaVencimiento,
            FechaRealizado = entity.FechaRealizado,
            FechaActualizacion = entity.FechaActualizacion,
            RequiereSeguimiento = entity.RequiereSeguimiento,
            DiasAlerta = entity.DiasAlerta,
            Activo = entity.Activo
        };

    private static async Task<List<SelectListItem>> BuildSelectListAsync<TEntity>(
        IQueryable<TEntity> query,
        Func<TEntity, string> textSelector,
        int? selectedValue,
        bool includeEmptyOption,
        CancellationToken cancellationToken)
        where TEntity : EntityBase
    {
        var items = await query.ToListAsync(cancellationToken);
        var options = items
            .Select(item => new SelectListItem(textSelector(item), item.Id.ToString(), item.Id == selectedValue))
            .ToList();

        if (includeEmptyOption)
        {
            options.Insert(0, new SelectListItem("Seleccione", string.Empty, !selectedValue.HasValue));
        }

        return options;
    }

    private sealed class RegistroListaRow
    {
        public SnapshotAlertaRegistroDto Snapshot { get; init; } = new();
        public string EstadoColorCss { get; init; } = "#7a828f";
        public string? PrioridadColorCss { get; init; }
        public string? Plataforma { get; init; }
        public string? CorreoOrigenAsunto { get; init; }
        public string? ResponsablePrincipal { get; init; }
        public int UrlsCount { get; init; }
        public int EvidenciasCount { get; init; }
    }

    private sealed class ReferenceLabels
    {
        public string? Nino { get; init; }
        public string? Disciplina { get; init; }
        public string? TipoRegistro { get; init; }
        public string? Plataforma { get; init; }
        public string? CorreoOrigen { get; init; }
        public string? Estado { get; init; }
        public string? Prioridad { get; init; }
    }
}
