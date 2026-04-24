namespace SeguimientoColegio.Web.Constants;

public static class AppRoles
{
    public const string Administrador = "Administrador";
    public const string Editor = "Editor";
    public const string Consulta = "Consulta";

    public static bool PuedeEditar(string? rol)
        => string.Equals(rol, Administrador, StringComparison.OrdinalIgnoreCase)
            || string.Equals(rol, Editor, StringComparison.OrdinalIgnoreCase);
}
