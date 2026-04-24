using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SeguimientoColegio.Web.Models;
using SeguimientoColegio.Web.Services;

namespace SeguimientoColegio.Web.Controllers;

[Authorize]
public sealed class AcuerdosController(
    IRegistroEscolarService registroService,
    ISelectorNinoService selectorNinoService) : Controller
{
    public async Task<IActionResult> Index([FromQuery] RegistroFiltrosViewModel filtros, CancellationToken cancellationToken)
    {
        var ninoId = await selectorNinoService.ObtenerNinoSeleccionadoAsync(cancellationToken);
        var model = await registroService.ObtenerListadoAsync(filtros, ninoId, true, cancellationToken);
        return View("~/Views/Registros/Index.cshtml", model);
    }

    [Authorize(Policy = "PuedeEditar")]
    public IActionResult Create()
        => RedirectToAction("Create", "Registros", new { modoAcuerdos = true });

    public IActionResult Details(int id)
        => RedirectToAction("Details", "Registros", new { id });

    [Authorize(Policy = "PuedeEditar")]
    public IActionResult Edit(int id)
        => RedirectToAction("Edit", "Registros", new { id, modoAcuerdos = true });
}
