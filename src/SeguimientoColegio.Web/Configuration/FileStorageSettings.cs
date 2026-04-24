namespace SeguimientoColegio.Web.Configuration;

public sealed class FileStorageSettings
{
    public const string SectionName = "Almacenamiento";

    public string RutaBase { get; set; } = "App_Data/Uploads";
}
