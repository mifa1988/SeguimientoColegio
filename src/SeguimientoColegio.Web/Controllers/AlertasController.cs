using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SeguimientoColegio.Web.Data;
using SeguimientoColegio.Web.Models;
using SeguimientoColegio.Web.Services;

namespace SeguimientoColegio.Web.Controllers;

[Authorize]
public sealed class AlertasController(
    IAlertaService alertaService,
    ISelectorNinoService selectorNinoService,
    SeguimientoDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index([FromQuery] AlertasFiltrosViewModel filtros, CancellationToken cancellationToken)
    {
        var ninoId = await selectorNinoService.ObtenerNinoSeleccionadoAsync(cancellationToken);
        var alertas = await alertaService.ObtenerAlertasAsync(new ConsultaAlertas
        {
            NinoId = filtros.NinoId,
            DisciplinaId = filtros.DisciplinaId,
            Severidad = filtros.Severidad,
            Texto = filtros.Texto
        }, ninoId, cancellationToken);

        return View(new AlertasIndexViewModel
        {
            Filtros = new AlertasFiltrosViewModel
            {
                NinoId = filtros.NinoId ?? ninoId,
                DisciplinaId = filtros.DisciplinaId,
                Severidad = filtros.Severidad,
                Texto = filtros.Texto
            },
            Alertas = alertas,
            Ninos = await dbContext.Ninos
                .AsNoTracking()
                .Where(x => x.Activo)
                .OrderBy(x => x.Nombres)
                .Select(x => new SelectListItem(x.NombreCompleto, x.Id.ToString(), x.Id == (filtros.NinoId ?? ninoId)))
                .Prepend(new SelectListItem("Todos", string.Empty, !(filtros.NinoId ?? ninoId).HasValue))
                .ToListAsync(cancellationToken),
            Disciplinas = await dbContext.Disciplinas
                .AsNoTracking()
                .Where(x => x.Activo)
                .OrderBy(x => x.Nombre)
                .Select(x => new SelectListItem(x.Nombre, x.Id.ToString(), x.Id == filtros.DisciplinaId))
                .Prepend(new SelectListItem("Todas", string.Empty, !filtros.DisciplinaId.HasValue))
                .ToListAsync(cancellationToken)
        });
    }
}
