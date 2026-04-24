using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SeguimientoColegio.Web.Data;

namespace SeguimientoColegio.Web.Services;

public interface ISelectorNinoService
{
    Task<int?> ObtenerNinoSeleccionadoAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<SelectListItem>> ObtenerOpcionesAsync(int? selectedId, CancellationToken cancellationToken);
    Task EstablecerNinoSeleccionadoAsync(int? ninoId, CancellationToken cancellationToken);
}

public sealed class SelectorNinoService(
    IHttpContextAccessor httpContextAccessor,
    SeguimientoDbContext dbContext) : ISelectorNinoService
{
    private const string SessionKey = "SeguimientoColegio.CurrentNinoId";

    public async Task<int?> ObtenerNinoSeleccionadoAsync(CancellationToken cancellationToken)
    {
        var session = httpContextAccessor.HttpContext?.Session;
        var ninoId = session?.GetInt32(SessionKey);
        if (ninoId.HasValue)
        {
            var exists = await dbContext.Ninos.AnyAsync(x => x.Id == ninoId.Value && x.Activo, cancellationToken);
            if (exists)
            {
                return ninoId;
            }

            session?.Remove(SessionKey);
        }

        var activos = await dbContext.Ninos
            .AsNoTracking()
            .Where(x => x.Activo)
            .OrderBy(x => x.Nombres)
            .ThenBy(x => x.Apellidos)
            .Select(x => x.Id)
            .Take(2)
            .ToListAsync(cancellationToken);

        if (activos.Count == 1)
        {
            session?.SetInt32(SessionKey, activos[0]);
            return activos[0];
        }

        return null;
    }

    public async Task<IReadOnlyList<SelectListItem>> ObtenerOpcionesAsync(int? selectedId, CancellationToken cancellationToken)
    {
        var ninos = await dbContext.Ninos
            .AsNoTracking()
            .Where(x => x.Activo)
            .OrderBy(x => x.Nombres)
            .ThenBy(x => x.Apellidos)
            .Select(x => new
            {
                x.Id,
                Nombre = string.Join(" ", new[] { x.Nombres, x.Apellidos })
            })
            .ToListAsync(cancellationToken);

        return ninos
            .Select(x => new SelectListItem(x.Nombre, x.Id.ToString(), x.Id == selectedId))
            .ToList();
    }

    public async Task EstablecerNinoSeleccionadoAsync(int? ninoId, CancellationToken cancellationToken)
    {
        var session = httpContextAccessor.HttpContext?.Session;
        if (session is null)
        {
            return;
        }

        if (!ninoId.HasValue)
        {
            session.Remove(SessionKey);
            return;
        }

        var exists = await dbContext.Ninos.AnyAsync(x => x.Id == ninoId.Value && x.Activo, cancellationToken);
        if (!exists)
        {
            session.Remove(SessionKey);
            return;
        }

        session.SetInt32(SessionKey, ninoId.Value);
    }
}
