using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SeguimientoColegio.Web.Configuration;
using SeguimientoColegio.Web.Constants;
using SeguimientoColegio.Web.Data;
using SeguimientoColegio.Web.Data.Entities;

namespace SeguimientoColegio.Web.Services;

public interface IDbInitializer
{
    Task InitializeAsync(CancellationToken cancellationToken);
}

public sealed class DbInitializer(
    SeguimientoDbContext dbContext,
    IOptions<DatabaseStartupSettings> databaseOptions,
    IOptions<AutoAccessSettings> autoAccessOptions,
    IOptions<BootstrapAdminSettings> bootstrapOptions,
    ILogger<DbInitializer> logger) : IDbInitializer
{
    private readonly PasswordHasher<Usuario> _passwordHasher = new();

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        if (databaseOptions.Value.AplicarMigracionesAlInicio)
        {
            await dbContext.Database.MigrateAsync(cancellationToken);
        }

        await EnsureAutoAccessUserAsync(cancellationToken);
        await EnsureBootstrapAdminAsync(cancellationToken);
    }

    private async Task EnsureAutoAccessUserAsync(CancellationToken cancellationToken)
    {
        var settings = autoAccessOptions.Value;
        var correo = NormalizarCorreo(settings.Correo);
        if (!settings.Habilitado || correo is null)
        {
            return;
        }

        var usuario = await dbContext.Usuarios.SingleOrDefaultAsync(x => x.Correo == correo, cancellationToken);
        var nombres = ResolveNombres(settings, correo);
        var apellidos = ResolveApellidos(settings);
        var rolId = ResolveRolId(settings.Rol);
        var now = DateTime.UtcNow;

        if (usuario is null)
        {
            logger.LogInformation("Creando usuario de acceso automatico para {Correo}.", correo);

            usuario = new Usuario
            {
                Nombres = nombres,
                Apellidos = apellidos,
                Correo = correo,
                RolId = rolId,
                Activo = true,
                FechaCreacion = now,
                FechaActualizacion = now
            };

            usuario.PasswordHash = _passwordHasher.HashPassword(usuario, $"auto-access::{correo}");

            dbContext.Usuarios.Add(usuario);
            await dbContext.SaveChangesAsync(cancellationToken);
            return;
        }

        var requiresSave = false;

        if (!usuario.Activo)
        {
            usuario.Activo = true;
            requiresSave = true;
        }

        if (usuario.RolId != rolId)
        {
            usuario.RolId = rolId;
            requiresSave = true;
        }

        if (string.IsNullOrWhiteSpace(usuario.Nombres))
        {
            usuario.Nombres = nombres;
            requiresSave = true;
        }

        if (string.IsNullOrWhiteSpace(usuario.Apellidos) && !string.IsNullOrWhiteSpace(apellidos))
        {
            usuario.Apellidos = apellidos;
            requiresSave = true;
        }

        if (requiresSave)
        {
            logger.LogInformation("Actualizando usuario de acceso automatico para {Correo}.", correo);
            usuario.FechaActualizacion = now;
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task EnsureBootstrapAdminAsync(CancellationToken cancellationToken)
    {
        var settings = bootstrapOptions.Value;
        if (!settings.Habilitado ||
            string.IsNullOrWhiteSpace(settings.Correo) ||
            string.IsNullOrWhiteSpace(settings.Password))
        {
            return;
        }

        var correo = settings.Correo.Trim().ToLowerInvariant();
        var alreadyExists = await dbContext.Usuarios.AnyAsync(x => x.Correo == correo, cancellationToken);
        if (alreadyExists)
        {
            return;
        }

        logger.LogInformation("Creando usuario administrador bootstrap para {Correo}.", correo);

        var now = DateTime.UtcNow;
        var usuario = new Usuario
        {
            Nombres = settings.Nombres.Trim(),
            Apellidos = string.IsNullOrWhiteSpace(settings.Apellidos) ? null : settings.Apellidos.Trim(),
            Correo = correo,
            RolId = SeedDataIds.Roles.Administrador,
            Activo = true,
            FechaCreacion = now,
            FechaActualizacion = now
        };

        usuario.PasswordHash = _passwordHasher.HashPassword(usuario, settings.Password);

        dbContext.Usuarios.Add(usuario);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static string? NormalizarCorreo(string? correo)
    {
        var trimmed = correo?.Trim();
        return string.IsNullOrWhiteSpace(trimmed)
            ? null
            : trimmed.ToLowerInvariant();
    }

    private static string ResolveNombres(AutoAccessSettings settings, string correo)
    {
        if (!string.IsNullOrWhiteSpace(settings.Nombres))
        {
            return settings.Nombres.Trim();
        }

        var localPart = correo.Split('@', 2)[0].Trim();
        return string.IsNullOrWhiteSpace(localPart) ? "Acceso automatico" : localPart;
    }

    private static string? ResolveApellidos(AutoAccessSettings settings)
        => string.IsNullOrWhiteSpace(settings.Apellidos) ? null : settings.Apellidos.Trim();

    private static int ResolveRolId(string? rol)
    {
        if (string.Equals(rol, AppRoles.Editor, StringComparison.OrdinalIgnoreCase))
        {
            return SeedDataIds.Roles.Editor;
        }

        if (string.Equals(rol, AppRoles.Consulta, StringComparison.OrdinalIgnoreCase))
        {
            return SeedDataIds.Roles.Consulta;
        }

        return SeedDataIds.Roles.Administrador;
    }
}
