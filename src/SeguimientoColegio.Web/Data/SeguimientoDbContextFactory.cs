using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SeguimientoColegio.Web.Data;

public sealed class SeguimientoDbContextFactory : IDesignTimeDbContextFactory<SeguimientoDbContext>
{
    public SeguimientoDbContext CreateDbContext(string[] args)
    {
        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
            .AddJsonFile("appsettings.Local.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Server=localhost;Port=3306;Database=PRACTICAR_CE;User=root;Password=;SslMode=None;";

        var optionsBuilder = new DbContextOptionsBuilder<SeguimientoDbContext>();
        optionsBuilder.UseMySql(
            connectionString,
            new MariaDbServerVersion(new Version(10, 6, 0)));

        return new SeguimientoDbContext(optionsBuilder.Options);
    }
}
