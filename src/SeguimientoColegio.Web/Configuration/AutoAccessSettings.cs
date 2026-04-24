using SeguimientoColegio.Web.Constants;

namespace SeguimientoColegio.Web.Configuration;

public sealed class AutoAccessSettings
{
    public const string SectionName = "AccesoAutomatico";

    public bool Habilitado { get; set; }
    public string? Correo { get; set; }
    public string? Nombres { get; set; }
    public string? Apellidos { get; set; }
    public string Rol { get; set; } = AppRoles.Administrador;
}
