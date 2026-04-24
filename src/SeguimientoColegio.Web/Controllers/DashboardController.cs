using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeguimientoColegio.Web.Models;
using SeguimientoColegio.Web.Services;

namespace SeguimientoColegio.Web.Controllers;

[Authorize]
public sealed class DashboardController(
    IDashboardService dashboardService,
    ISelectorNinoService selectorNinoService,
    SeguimientoColegio.Web.Data.SeguimientoDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var ninoId = await selectorNinoService.ObtenerNinoSeleccionadoAsync(cancellationToken);
        var resumen = await dashboardService.ObtenerResumenAsync(ninoId, cancellationToken);
        var alcanceNino = ninoId.HasValue
            ? await dbContext.Ninos
                .AsNoTracking()
                .Where(x => x.Id == ninoId.Value)
                .Select(x => x.Nombres + " " + x.Apellidos)
                .SingleOrDefaultAsync(cancellationToken)
            : null;

        return View(new DashboardIndexViewModel
        {
            AlcanceNino = alcanceNino,
            Resumen = resumen
        });
    }
}
