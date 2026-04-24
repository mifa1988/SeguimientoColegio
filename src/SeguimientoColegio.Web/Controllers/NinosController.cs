using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeguimientoColegio.Web.Data;
using SeguimientoColegio.Web.Data.Entities;
using SeguimientoColegio.Web.Models;

namespace SeguimientoColegio.Web.Controllers;

[Authorize(Policy = "SoloAdministrador")]
public sealed class NinosController(SeguimientoDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var items = await dbContext.Ninos
            .AsNoTracking()
            .OrderByDescending(x => x.Activo)
            .ThenBy(x => x.Nombres)
            .ThenBy(x => x.Apellidos)
            .Select(x => new NinoItemViewModel
            {
                Id = x.Id,
                Nombres = x.Nombres,
                Apellidos = x.Apellidos,
                Alias = x.Alias,
                FechaNacimiento = x.FechaNacimiento,
                Activo = x.Activo,
                FechaActualizacion = x.FechaActualizacion
            })
            .ToListAsync(cancellationToken);

        return View(new NinosIndexViewModel { Items = items });
    }

    public IActionResult Create()
        => View(new NinoFormPageViewModel { Form = new NinoInputModel() });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(NinoFormPageViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var now = DateTime.UtcNow;
        dbContext.Ninos.Add(new Nino
        {
            Nombres = model.Form.Nombres.Trim(),
            Apellidos = model.Form.Apellidos.Trim(),
            Alias = Normalize(model.Form.Alias),
            FechaNacimiento = model.Form.FechaNacimiento,
            Activo = model.Form.Activo,
            FechaCreacion = now,
            FechaActualizacion = now
        });

        await dbContext.SaveChangesAsync(cancellationToken);
        TempData["StatusMessage"] = "Nino creado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Ninos.FindAsync([id], cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        return View(new NinoFormPageViewModel
        {
            IsEdit = true,
            Form = new NinoInputModel
            {
                Nombres = entity.Nombres,
                Apellidos = entity.Apellidos,
                Alias = entity.Alias,
                FechaNacimiento = entity.FechaNacimiento,
                Activo = entity.Activo
            }
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, NinoFormPageViewModel model, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Ninos.FindAsync([id], cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            model = new NinoFormPageViewModel { IsEdit = true, Form = model.Form };
            return View(model);
        }

        entity.Nombres = model.Form.Nombres.Trim();
        entity.Apellidos = model.Form.Apellidos.Trim();
        entity.Alias = Normalize(model.Form.Alias);
        entity.FechaNacimiento = model.Form.FechaNacimiento;
        entity.Activo = model.Form.Activo;
        entity.FechaActualizacion = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        TempData["StatusMessage"] = "Nino actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    private static string? Normalize(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
