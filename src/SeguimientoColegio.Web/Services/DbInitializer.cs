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

        await EnsureBootstrapAdminAsync(cancellationToken);
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
}
