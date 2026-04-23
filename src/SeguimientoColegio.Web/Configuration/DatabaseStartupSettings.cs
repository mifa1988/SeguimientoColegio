namespace SeguimientoColegio.Web.Configuration;

public sealed class DatabaseStartupSettings
{
    public const string SectionName = "BaseDeDatos";

    public bool AplicarMigracionesAlInicio { get; set; }
}
