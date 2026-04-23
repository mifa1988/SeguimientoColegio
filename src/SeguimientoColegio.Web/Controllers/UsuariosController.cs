using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SeguimientoColegio.Web.Data;
using SeguimientoColegio.Web.Data.Entities;
using SeguimientoColegio.Web.Models;

namespace SeguimientoColegio.Web.Controllers;

[Authorize(Policy = "SoloAdministrador")]
public sealed class UsuariosController(SeguimientoDbContext dbContext) : Controller
{
    private readonly PasswordHasher<Usuario> _passwordHasher = new();

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var items = await dbContext.Usuarios
            .AsNoTracking()
            .Include(x => x.Rol)
            .OrderByDescending(x => x.Activo)
            .ThenBy(x => x.Nombres)
            .Select(x => new UsuarioItemViewModel
            {
                Id = x.Id,
                Nombres = x.Nombres,
                Apellidos = x.Apellidos,
                Correo = x.Correo,
                Rol = x.Rol!.Nombre,
                Activo = x.Activo,
                UltimoLogin = x.UltimoLogin,
                FechaActualizacion = x.FechaActualizacion
            })
            .ToListAsync(cancellationToken);

        return View(new UsuariosIndexViewModel { Items = items });
    }

    public async Task<IActionResult> Create(CancellationToken cancellationToken)
        => View(await BuildFormPageAsync(new UsuarioInputModel(), false, cancellationToken));

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UsuarioFormPageViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(await BuildFormPageAsync(model.Form, false, cancellationToken));
        }

        var now = DateTime.UtcNow;
        var usuario = new Usuario
        {
            Nombres = model.Form.Nombres.Trim(),
            Apellidos = Normalize(model.Form.Apellidos),
            Correo = model.Form.Correo.Trim().ToLowerInvariant(),
            RolId = model.Form.RolId!.Value,
            Activo = model.Form.Activo,
            FechaCreacion = now,
            FechaActualizacion = now
        };
        usuario.PasswordHash = _passwordHasher.HashPassword(usuario, model.Form.Password!.Trim());

        dbContext.Usuarios.Add(usuario);
        await dbContext.SaveChangesAsync(cancellationToken);
        TempData["StatusMessage"] = "Usuario creado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Usuarios.FindAsync([id], cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        return View(await BuildFormPageAsync(new UsuarioInputModel
        {
            Nombres = entity.Nombres,
            Apellidos = entity.Apellidos,
            Correo = entity.Correo,
            RolId = entity.RolId,
            Activo = entity.Activo,
            RequierePassword = false
        }, true, cancellationToken));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UsuarioFormPageViewModel model, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Usuarios.FindAsync([id], cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        model.Form.RequierePassword = false;
        if (!ModelState.IsValid)
        {
            return View(await BuildFormPageAsync(model.Form, true, cancellationToken));
        }

        entity.Nombres = model.Form.Nombres.Trim();
        entity.Apellidos = Normalize(model.Form.Apellidos);
        entity.Correo = model.Form.Correo.Trim().ToLowerInvariant();
        entity.RolId = model.Form.RolId!.Value;
        entity.Activo = model.Form.Activo;
        entity.FechaActualizacion = DateTime.UtcNow;

        if (!string.IsNullOrWhiteSpace(model.Form.Password))
        {
            entity.PasswordHash = _passwordHasher.HashPassword(entity, model.Form.Password.Trim());
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        TempData["StatusMessage"] = "Usuario actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<UsuarioFormPageViewModel> BuildFormPageAsync(UsuarioInputModel form, bool isEdit, CancellationToken cancellationToken)
        => new()
        {
            IsEdit = isEdit,
            Form = form,
            Roles = await dbContext.Roles
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem(x.Nombre, x.Id.ToString(), x.Id == form.RolId))
                .ToListAsync(cancellationToken)
        };

    private static string? Normalize(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
