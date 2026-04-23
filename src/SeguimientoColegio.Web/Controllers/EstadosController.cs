using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeguimientoColegio.Web.Data;
using SeguimientoColegio.Web.Models;

namespace SeguimientoColegio.Web.Controllers;

[Authorize(Policy = "SoloAdministrador")]
public sealed class EstadosController(SeguimientoDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var items = await dbContext.EstadosSeguimiento
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
        dbContext.EstadosSeguimiento.Add(new Data.Entities.EstadoSeguimiento
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
        TempData["StatusMessage"] = "Estado creado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var entity = await dbContext.EstadosSeguimiento.FindAsync([id], cancellationToken);
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
        var entity = await dbContext.EstadosSeguimiento.FindAsync([id], cancellationToken);
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
        TempData["StatusMessage"] = "Estado actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    private static CatalogoConOrdenPageViewModel BuildPage(IReadOnlyList<CatalogoConOrdenItemViewModel> items)
        => new()
        {
            Titulo = "Estados de seguimiento",
            DescripcionPagina = "Mantiene el catalogo de estados visibles en el seguimiento y control.",
            ControllerName = "Estados",
            SingularName = "estado",
            Items = items
        };

    private static CatalogoConOrdenFormPageViewModel BuildFormPage(CatalogoConOrdenInputModel form, bool isEdit)
        => new()
        {
            Titulo = isEdit ? "Editar estado" : "Nuevo estado",
            DescripcionPagina = "Configura orden y color visual del estado.",
            ControllerName = "Estados",
            SingularName = "estado",
            IsEdit = isEdit,
            Form = form
        };

    private static string? Normalize(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
