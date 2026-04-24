using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeguimientoColegio.Web.Data;
using SeguimientoColegio.Web.Data.Entities;
using SeguimientoColegio.Web.Models;

namespace SeguimientoColegio.Web.Controllers;

[Authorize(Policy = "SoloAdministrador")]
public sealed class TiposRegistroController(SeguimientoDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var items = await dbContext.TiposRegistro
            .AsNoTracking()
            .OrderByDescending(x => x.Activo)
            .ThenBy(x => x.Nombre)
            .Select(x => new CatalogoBasicoItemViewModel
            {
                Id = x.Id,
                Nombre = x.Nombre,
                Descripcion = x.Descripcion,
                Activo = x.Activo,
                FechaActualizacion = x.FechaActualizacion
            })
            .ToListAsync(cancellationToken);

        return View("~/Views/Shared/CatalogoBasicoIndex.cshtml", BuildPage(items));
    }

    public IActionResult Create()
        => View("~/Views/Shared/CatalogoBasicoForm.cshtml", BuildFormPage(new CatalogoBasicoInputModel(), false));

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CatalogoBasicoInputModel form, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View("~/Views/Shared/CatalogoBasicoForm.cshtml", BuildFormPage(form, false));
        }

        var now = DateTime.UtcNow;
        dbContext.TiposRegistro.Add(new TipoRegistro
        {
            Nombre = form.Nombre.Trim(),
            Descripcion = Normalize(form.Descripcion),
            Activo = form.Activo,
            FechaCreacion = now,
            FechaActualizacion = now
        });

        await dbContext.SaveChangesAsync(cancellationToken);
        TempData["StatusMessage"] = "Tipo de registro creado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var entity = await dbContext.TiposRegistro.FindAsync([id], cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        return View("~/Views/Shared/CatalogoBasicoForm.cshtml", BuildFormPage(new CatalogoBasicoInputModel
        {
            Nombre = entity.Nombre,
            Descripcion = entity.Descripcion,
            Activo = entity.Activo
        }, true));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CatalogoBasicoInputModel form, CancellationToken cancellationToken)
    {
        var entity = await dbContext.TiposRegistro.FindAsync([id], cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View("~/Views/Shared/CatalogoBasicoForm.cshtml", BuildFormPage(form, true));
        }

        entity.Nombre = form.Nombre.Trim();
        entity.Descripcion = Normalize(form.Descripcion);
        entity.Activo = form.Activo;
        entity.FechaActualizacion = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        TempData["StatusMessage"] = "Tipo de registro actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    private static CatalogoBasicoPageViewModel BuildPage(IReadOnlyList<CatalogoBasicoItemViewModel> items)
        => new()
        {
            Titulo = "Tipos de registro",
            DescripcionPagina = "Clasifica el origen y naturaleza de cada seguimiento escolar.",
            ControllerName = "TiposRegistro",
            SingularName = "tipo de registro",
            Items = items
        };

    private static CatalogoBasicoFormPageViewModel BuildFormPage(CatalogoBasicoInputModel form, bool isEdit)
        => new()
        {
            Titulo = isEdit ? "Editar tipo de registro" : "Nuevo tipo de registro",
            DescripcionPagina = "Define categorias como tarea, lectura, requerimiento o acuerdo con profesora.",
            ControllerName = "TiposRegistro",
            SingularName = "tipo de registro",
            IsEdit = isEdit,
            Form = form
        };

    private static string? Normalize(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
