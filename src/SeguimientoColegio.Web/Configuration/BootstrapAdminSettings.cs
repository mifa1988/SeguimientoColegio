namespace SeguimientoColegio.Web.Configuration;

public sealed class BootstrapAdminSettings
{
    public const string SectionName = "BootstrapAdmin";

    public bool Habilitado { get; set; }
    public string? Correo { get; set; }
    public string? Password { get; set; }
    public string Nombres { get; set; } = "Administrador";
    public string Apellidos { get; set; } = "Sistema";
}
