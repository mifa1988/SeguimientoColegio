using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MySqlConnector;
using SeguimientoColegio.Web.Configuration;
using SeguimientoColegio.Web.Constants;
using SeguimientoColegio.Web.Data;
using SeguimientoColegio.Web.Services;

namespace SeguimientoColegio.Web;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);
        builder.Configuration.AddEnvironmentVariables();

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "No se encontro la cadena de conexion 'DefaultConnection'. Configurala en appsettings.Local.json o variables de entorno.");

        builder.Services.Configure<AlertSettings>(builder.Configuration.GetSection(AlertSettings.SectionName));
        builder.Services.Configure<FileStorageSettings>(builder.Configuration.GetSection(FileStorageSettings.SectionName));
        builder.Services.Configure<BootstrapAdminSettings>(builder.Configuration.GetSection(BootstrapAdminSettings.SectionName));
        builder.Services.Configure<DatabaseStartupSettings>(builder.Configuration.GetSection(DatabaseStartupSettings.SectionName));
        builder.Services.Configure<AutoAccessSettings>(builder.Configuration.GetSection(AutoAccessSettings.SectionName));

        builder.Services.AddDbContext<SeguimientoDbContext>(options =>
        {
            options.UseMySql(
                connectionString,
                CrearVersionServidor(builder.Configuration),
                mysqlOptions => mysqlOptions.EnableRetryOnFailure(3));
        });

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddSession(options =>
        {
            options.Cookie.Name = "SeguimientoColegio.Session";
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
            options.Cookie.SameSite = SameSiteMode.Lax;
            options.IdleTimeout = TimeSpan.FromHours(8);
        });

        builder.Services.Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = 25 * 1024 * 1024;
        });

        builder.Services.AddControllersWithViews(options =>
        {
            options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(_ => "El valor es obligatorio.");
        });

        builder.Services.AddScoped<IDbInitializer, DbInitializer>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IUsuarioActualService, UsuarioActualService>();
        builder.Services.AddScoped<ISelectorNinoService, SelectorNinoService>();
        builder.Services.AddScoped<IArchivoService, ArchivoService>();
        builder.Services.AddScoped<IAlertaService, AlertaService>();
        builder.Services.AddScoped<IDashboardService, DashboardService>();
        builder.Services.AddScoped<IRegistroEscolarService, RegistroEscolarService>();

        builder.Services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.Cookie.Name = "SeguimientoColegio.Auth";
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromHours(8);
                options.Events = new CookieAuthenticationEvents
                {
                    OnValidatePrincipal = async context =>
                    {
                        var authService = context.HttpContext.RequestServices.GetRequiredService<IAuthService>();
                        var validation = await authService.ValidarSesionAsync(context.Principal, context.HttpContext.RequestAborted);

                        if (!validation.EsValida)
                        {
                            context.RejectPrincipal();
                            await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                            return;
                        }

                        if (validation.PrincipalActualizado is not null)
                        {
                            context.ReplacePrincipal(validation.PrincipalActualizado);
                            context.ShouldRenew = true;
                        }
                    }
                };
            });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy(
                "PuedeEditar",
                policy => policy.RequireRole(AppRoles.Administrador, AppRoles.Editor));
            options.AddPolicy(
                "SoloAdministrador",
                policy => policy.RequireRole(AppRoles.Administrador));
        });

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseStaticFiles();
        app.UseRouting();
        app.UseSession();
        app.UseAuthentication();
        app.Use(async (context, next) =>
        {
            var autoAccess = context.RequestServices.GetRequiredService<IOptions<AutoAccessSettings>>().Value;
            if (autoAccess.Habilitado)
            {
                var authService = context.RequestServices.GetRequiredService<IAuthService>();
                var principal = await authService.ObtenerPrincipalPorCorreoAsync(autoAccess.Correo, context.RequestAborted);

                if (principal is not null)
                {
                    context.User = principal;
                }
            }

            await next();
        });
        app.UseAuthorization();

        using (var scope = app.Services.CreateScope())
        {
            var initializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
            try
            {
                await initializer.InitializeAsync(CancellationToken.None);
            }
            catch (MySqlException ex)
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                logger.LogCritical(
                    ex,
                    "No se pudo conectar a MySQL/MariaDB. Verifica host, puerto, usuario, password y permisos del usuario desde la IP cliente.");

                throw new InvalidOperationException(
                    "No se pudo inicializar la base de datos. Verifica la cadena de conexion y que MySQL/MariaDB permita el acceso del usuario configurado desde tu IP publica.",
                    ex);
            }
        }

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Dashboard}/{action=Index}/{id?}");

        await app.RunAsync();
    }

    private static ServerVersion CrearVersionServidor(IConfiguration configuration)
    {
        var versionText = configuration["BaseDeDatos:VersionServidor"];
        var version = Version.TryParse(versionText, out var parsedVersion)
            ? parsedVersion
            : new Version(10, 6, 0);

        var tipoServidor = configuration["BaseDeDatos:TipoServidor"] ?? "MariaDb";

        return string.Equals(tipoServidor, "MySql", StringComparison.OrdinalIgnoreCase)
            ? new MySqlServerVersion(version)
            : new MariaDbServerVersion(version);
    }
}
