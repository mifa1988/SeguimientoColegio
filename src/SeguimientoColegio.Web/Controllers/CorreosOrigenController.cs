using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeguimientoColegio.Web.Constants;
using SeguimientoColegio.Web.Data;
using SeguimientoColegio.Web.Dtos;
using SeguimientoColegio.Web.Models;

namespace SeguimientoColegio.Web.Controllers;

[Authorize]
public sealed class CorreosOrigenController(SeguimientoDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index(string? search, CancellationToken cancellationToken)
    {
        var query = dbContext.CorreosOrigen.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var texto = search.Trim();
            query = query.Where(x =>
                x.Asunto.Contains(texto) ||
                x.Remitente.Contains(texto) ||
                (x.Resumen != null && x.Resumen.Contains(texto)));
        }

        var items = await query
            .OrderByDescending(x => x.FechaCorreo)
            .Select(x => new CorreoOrigenItemViewModel
            {
                Id = x.Id,
                Asunto = x.Asunto,
                Remitente = x.Remitente,
                FechaCorreo = x.FechaCorreo,
                Resumen = x.Resumen,
                RegistrosRelacionados = x.Registros.Count,
                FechaActualizacion = x.FechaActualizacion
            })
            .ToListAsync(cancellationToken);

        return View(new CorreosOrigenIndexViewModel
        {
            Search = search,
            Items = items
        });
    }

    [Authorize(Policy = "PuedeEditar")]
    public IActionResult Create()
        => View(new CorreoOrigenFormPageViewModel
        {
            Form = new CorreoOrigenInputModel
            {
                FechaCorreo = DateTime.Now
            }
        });

    [Authorize(Policy = "PuedeEditar")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CorreoOrigenFormPageViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var now = DateTime.UtcNow;
        dbContext.CorreosOrigen.Add(new Data.Entities.CorreoOrigen
        {
            Asunto = model.Form.Asunto.Trim(),
            Remitente = model.Form.Remitente.Trim().ToLowerInvariant(),
            FechaCorreo = model.Form.FechaCorreo,
            Resumen = Normalize(model.Form.Resumen),
            ReferenciaUrl = Normalize(model.Form.ReferenciaUrl),
            MessageId = Normalize(model.Form.MessageId),
            Observaciones = Normalize(model.Form.Observaciones),
            FechaCreacion = now,
            FechaActualizacion = now
        });

        await dbContext.SaveChangesAsync(cancellationToken);
        TempData["StatusMessage"] = "Correo origen registrado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var correo = await dbContext.CorreosOrigen
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new
            {
                Item = new CorreoOrigenItemViewModel
                {
                    Id = x.Id,
                    Asunto = x.Asunto,
                    Remitente = x.Remitente,
                    FechaCorreo = x.FechaCorreo,
                    Resumen = x.Resumen,
                    RegistrosRelacionados = x.Registros.Count,
                    FechaActualizacion = x.FechaActualizacion
                },
                x.ReferenciaUrl,
                x.MessageId,
                x.Observaciones
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (correo is null)
        {
            return NotFound();
        }

        var registros = await dbContext.RegistrosEscolares
            .AsNoTracking()
            .Where(x => x.CorreoOrigenId == id && x.Activo)
            .OrderByDescending(x => x.FechaActualizacion)
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
                              x.FechaVencimiento.Value < DateOnly.FromDateTime(DateTime.Today) &&
                              x.EstadoSeguimientoId != SeedDataIds.Estados.Concluido &&
                              x.EstadoSeguimientoId != SeedDataIds.Estados.Descartado
            })
            .ToListAsync(cancellationToken);

        return View(new CorreoOrigenDetailViewModel
        {
            Correo = correo.Item,
            ReferenciaUrl = correo.ReferenciaUrl,
            MessageId = correo.MessageId,
            Observaciones = correo.Observaciones,
            RegistrosRelacionados = registros
        });
    }

    [Authorize(Policy = "PuedeEditar")]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var entity = await dbContext.CorreosOrigen.FindAsync([id], cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        return View(new CorreoOrigenFormPageViewModel
        {
            IsEdit = true,
            Form = new CorreoOrigenInputModel
            {
                Asunto = entity.Asunto,
                Remitente = entity.Remitente,
                FechaCorreo = entity.FechaCorreo,
                Resumen = entity.Resumen,
                ReferenciaUrl = entity.ReferenciaUrl,
                MessageId = entity.MessageId,
                Observaciones = entity.Observaciones
            }
        });
    }

    [Authorize(Policy = "PuedeEditar")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CorreoOrigenFormPageViewModel model, CancellationToken cancellationToken)
    {
        var entity = await dbContext.CorreosOrigen.FindAsync([id], cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            model = new CorreoOrigenFormPageViewModel { IsEdit = true, Form = model.Form };
            return View(model);
        }

        entity.Asunto = model.Form.Asunto.Trim();
        entity.Remitente = model.Form.Remitente.Trim().ToLowerInvariant();
        entity.FechaCorreo = model.Form.FechaCorreo;
        entity.Resumen = Normalize(model.Form.Resumen);
        entity.ReferenciaUrl = Normalize(model.Form.ReferenciaUrl);
        entity.MessageId = Normalize(model.Form.MessageId);
        entity.Observaciones = Normalize(model.Form.Observaciones);
        entity.FechaActualizacion = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        TempData["StatusMessage"] = "Correo origen actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    private static string? Normalize(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
