using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
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

        builder.Services.AddDbContext<SeguimientoDbContext>(options =>
        {
            options.UseMySql(
                connectionString,
                ServerVersion.AutoDetect(connectionString),
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
        app.UseAuthorization();

        using (var scope = app.Services.CreateScope())
        {
            var initializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
            await initializer.InitializeAsync(CancellationToken.None);
        }

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Dashboard}/{action=Index}/{id?}");

        await app.RunAsync();
    }
}
