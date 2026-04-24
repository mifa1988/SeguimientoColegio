using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SeguimientoColegio.Web.Constants;
using SeguimientoColegio.Web.Data;
using SeguimientoColegio.Web.Data.Entities;
using SeguimientoColegio.Web.Models;

namespace SeguimientoColegio.Web.Services;

public interface IAuthService
{
    Task<ResultadoLogin> IniciarSesionAsync(HttpContext httpContext, LoginInputModel input, CancellationToken cancellationToken);
    Task CerrarSesionAsync(HttpContext httpContext);
    Task<ResultadoValidacionSesion> ValidarSesionAsync(ClaimsPrincipal? principal, CancellationToken cancellationToken);
    Task<ClaimsPrincipal?> ObtenerPrincipalPorCorreoAsync(string? correo, CancellationToken cancellationToken);
}

public sealed record ResultadoLogin(bool Success, string? ErrorMessage = null);

public sealed record ResultadoValidacionSesion(bool EsValida, ClaimsPrincipal? PrincipalActualizado = null);

public sealed class AuthService(
    SeguimientoDbContext dbContext,
    ILogger<AuthService> logger) : IAuthService
{
    private readonly PasswordHasher<Usuario> _passwordHasher = new();

    public async Task<ResultadoLogin> IniciarSesionAsync(
        HttpContext httpContext,
        LoginInputModel input,
        CancellationToken cancellationToken)
    {
        var correo = NormalizarCorreo(input.Correo);
        if (correo is null)
        {
            return new ResultadoLogin(false, "Ingresa un correo valido.");
        }

        var usuario = await dbContext.Usuarios
            .Include(x => x.Rol)
            .SingleOrDefaultAsync(x => x.Correo == correo, cancellationToken);

        if (usuario is null || !usuario.Activo)
        {
            logger.LogWarning("Intento de login rechazado para {Correo}. Usuario inexistente o inactivo.", correo);
            return new ResultadoLogin(false, "Credenciales invalidas o usuario inactivo.");
        }

        var verification = _passwordHasher.VerifyHashedPassword(usuario, usuario.PasswordHash, input.Password);
        if (verification == PasswordVerificationResult.Failed)
        {
            logger.LogWarning("Password invalido para {Correo}.", correo);
            return new ResultadoLogin(false, "Credenciales invalidas.");
        }

        if (verification == PasswordVerificationResult.SuccessRehashNeeded)
        {
            usuario.PasswordHash = _passwordHasher.HashPassword(usuario, input.Password);
        }

        usuario.UltimoLogin = DateTime.UtcNow;
        usuario.FechaActualizacion = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        await httpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            AppPrincipalFactory.Create(usuario),
            new AuthenticationProperties
            {
                IsPersistent = input.Recordarme,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(input.Recordarme ? 72 : 8)
            });

        logger.LogInformation("Login correcto para {Correo} con rol {Rol}.", correo, usuario.Rol?.Nombre);
        return new ResultadoLogin(true);
    }

    public Task CerrarSesionAsync(HttpContext httpContext)
        => httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

    public async Task<ClaimsPrincipal?> ObtenerPrincipalPorCorreoAsync(string? correo, CancellationToken cancellationToken)
    {
        var correoNormalizado = NormalizarCorreo(correo);
        if (correoNormalizado is null)
        {
            return null;
        }

        var usuario = await dbContext.Usuarios
            .AsNoTracking()
            .Include(x => x.Rol)
            .SingleOrDefaultAsync(x => x.Correo == correoNormalizado, cancellationToken);

        if (usuario is null || !usuario.Activo || usuario.Rol is null)
        {
            return null;
        }

        return AppPrincipalFactory.Create(usuario);
    }

    public async Task<ResultadoValidacionSesion> ValidarSesionAsync(ClaimsPrincipal? principal, CancellationToken cancellationToken)
    {
        var usuarioIdClaim = principal?.FindFirst(AppClaimTypes.UsuarioId)?.Value;
        if (!int.TryParse(usuarioIdClaim, out var usuarioId))
        {
            return new ResultadoValidacionSesion(false);
        }

        var usuario = await dbContext.Usuarios
            .AsNoTracking()
            .Include(x => x.Rol)
            .SingleOrDefaultAsync(x => x.Id == usuarioId, cancellationToken);

        if (usuario is null || !usuario.Activo || usuario.Rol is null)
        {
            return new ResultadoValidacionSesion(false);
        }

        var nombreActual = principal?.Identity?.Name;
        var correoActual = principal?.FindFirst(ClaimTypes.Email)?.Value;
        var rolActual = principal?.FindFirst(ClaimTypes.Role)?.Value;

        var hasChanges =
            !string.Equals(nombreActual, usuario.NombreCompleto, StringComparison.Ordinal) ||
            !string.Equals(correoActual, usuario.Correo, StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(rolActual, usuario.Rol.Nombre, StringComparison.Ordinal);

        return hasChanges
            ? new ResultadoValidacionSesion(true, AppPrincipalFactory.Create(usuario))
            : new ResultadoValidacionSesion(true);
    }

    private static string? NormalizarCorreo(string? correo)
    {
        var trimmed = correo?.Trim();
        return string.IsNullOrWhiteSpace(trimmed)
            ? null
            : trimmed.ToLowerInvariant();
    }
}
