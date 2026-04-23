using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeguimientoColegio.Web.Data;
using SeguimientoColegio.Web.Models;

namespace SeguimientoColegio.Web.Controllers;

[Authorize(Policy = "SoloAdministrador")]
public sealed class PrioridadesController(SeguimientoDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var items = await dbContext.Prioridades
            .AsNoTracking()
            .OrderBy(x => x.Orden)
            .Select(x => new CatalogoConOrdenItemViewModel
            {
                Id = x.Id,
                Nombre = x.Nombre,
                Descripcion = x.Descripcion,
                Activo = x.Activo,
                FechaActualizacion = x.FechaActualizacion,
                Orden = x.Orden,
                ColorCss = x.ColorCss
            })
            .ToListAsync(cancellationToken);

        return View("~/Views/Shared/CatalogoConOrdenIndex.cshtml", BuildPage(items));
    }

    public IActionResult Create()
        => View("~/Views/Shared/CatalogoConOrdenForm.cshtml", BuildFormPage(new CatalogoConOrdenInputModel(), false));

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CatalogoConOrdenInputModel form, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View("~/Views/Shared/CatalogoConOrdenForm.cshtml", BuildFormPage(form, false));
        }

        var now = DateTime.UtcNow;
        dbContext.Prioridades.Add(new Data.Entities.Prioridad
        {
            Nombre = form.Nombre.Trim(),
            Descripcion = Normalize(form.Descripcion),
            Activo = form.Activo,
            Orden = form.Orden,
            ColorCss = Normalize(form.ColorCss),
            FechaCreacion = now,
            FechaActualizacion = now
        });

        await dbContext.SaveChangesAsync(cancellationToken);
        TempData["StatusMessage"] = "Prioridad creada correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Prioridades.FindAsync([id], cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        return View("~/Views/Shared/CatalogoConOrdenForm.cshtml", BuildFormPage(new CatalogoConOrdenInputModel
        {
            Nombre = entity.Nombre,
            Descripcion = entity.Descripcion,
            Activo = entity.Activo,
            Orden = entity.Orden,
            ColorCss = entity.ColorCss
        }, true));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CatalogoConOrdenInputModel form, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Prioridades.FindAsync([id], cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View("~/Views/Shared/CatalogoConOrdenForm.cshtml", BuildFormPage(form, true));
        }

        entity.Nombre = form.Nombre.Trim();
        entity.Descripcion = Normalize(form.Descripcion);
        entity.Activo = form.Activo;
        entity.Orden = form.Orden;
        entity.ColorCss = Normalize(form.ColorCss);
        entity.FechaActualizacion = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        TempData["StatusMessage"] = "Prioridad actualizada correctamente.";
        return RedirectToAction(nameof(Index));
    }

    private static CatalogoConOrdenPageViewModel BuildPage(IReadOnlyList<CatalogoConOrdenItemViewModel> items)
        => new()
        {
            Titulo = "Prioridades",
            DescripcionPagina = "Configura el semaforo de prioridades para el control escolar.",
            ControllerName = "Prioridades",
            SingularName = "prioridad",
            Items = items
        };

    private static CatalogoConOrdenFormPageViewModel BuildFormPage(CatalogoConOrdenInputModel form, bool isEdit)
        => new()
        {
            Titulo = isEdit ? "Editar prioridad" : "Nueva prioridad",
            DescripcionPagina = "Define orden, color y visibilidad de las prioridades.",
            ControllerName = "Prioridades",
            SingularName = "prioridad",
            IsEdit = isEdit,
            Form = form
        };

    private static string? Normalize(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
