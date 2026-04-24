namespace SeguimientoColegio.Web.Configuration;

public sealed class AlertSettings
{
    public const string SectionName = "Alertas";

    public int DiasPendienteAntiguo { get; set; } = 7;
    public int DiasSinMovimiento { get; set; } = 5;
    public int DiasPrioridadAlta { get; set; } = 3;
    public int MaxAlertasDashboard { get; set; } = 8;
}
