using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using SeguimientoColegio.Web.Constants;
using SeguimientoColegio.Web.Data.Entities;

namespace SeguimientoColegio.Web.Services;

public static class AppPrincipalFactory
{
    public static ClaimsPrincipal Create(Usuario usuario)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(AppClaimTypes.UsuarioId, usuario.Id.ToString()),
            new(ClaimTypes.Name, usuario.NombreCompleto),
            new(ClaimTypes.Email, usuario.Correo),
            new(ClaimTypes.Role, usuario.Rol?.Nombre ?? AppRoles.Consulta)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        return new ClaimsPrincipal(identity);
    }
}
