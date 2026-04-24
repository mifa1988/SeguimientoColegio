using SeguimientoColegio.Web.Dtos;

namespace SeguimientoColegio.Web.Models;

public sealed class DashboardIndexViewModel
{
    public string Titulo { get; init; } = "Dashboard";
    public string? AlcanceNino { get; init; }
    public DashboardResumenDto Resumen { get; init; } = new();
}
