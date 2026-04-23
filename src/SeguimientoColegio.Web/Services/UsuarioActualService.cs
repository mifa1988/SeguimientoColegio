using System.Security.Claims;
using SeguimientoColegio.Web.Constants;

namespace SeguimientoColegio.Web.Services;

public interface IUsuarioActualService
{
    bool EstaAutenticado { get; }
    int? UsuarioId { get; }
    string? Correo { get; }
    string? Nombre { get; }
    string? Rol { get; }
    bool PuedeEditar { get; }
    int GetUsuarioIdOrThrow();
}

public sealed class UsuarioActualService(IHttpContextAccessor httpContextAccessor) : IUsuarioActualService
{
    private ClaimsPrincipal? Usuario => httpContextAccessor.HttpContext?.User;

    public bool EstaAutenticado => Usuario?.Identity?.IsAuthenticated ?? false;

    public int? UsuarioId
        => int.TryParse(Usuario?.FindFirstValue(AppClaimTypes.UsuarioId), out var id) ? id : null;

    public string? Correo => Usuario?.FindFirstValue(ClaimTypes.Email);

    public string? Nombre => Usuario?.Identity?.Name;

    public string? Rol => Usuario?.FindFirstValue(ClaimTypes.Role);

    public bool PuedeEditar => AppRoles.PuedeEditar(Rol);

    public int GetUsuarioIdOrThrow()
        => UsuarioId ?? throw new InvalidOperationException("No existe un usuario autenticado.");
}
