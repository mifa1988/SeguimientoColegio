using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SeguimientoColegio.Web.Models;
using SeguimientoColegio.Web.Services;

namespace SeguimientoColegio.Web.Controllers;

[Authorize]
public sealed class RegistrosController(
    IRegistroEscolarService registroService,
    ISelectorNinoService selectorNinoService,
    IUsuarioActualService usuarioActualService) : Controller
{
    public async Task<IActionResult> Index([FromQuery] RegistroFiltrosViewModel filtros, CancellationToken cancellationToken)
    {
        var ninoId = await selectorNinoService.ObtenerNinoSeleccionadoAsync(cancellationToken);
        var model = await registroService.ObtenerListadoAsync(filtros, ninoId, false, cancellationToken);
        return View(model);
    }

    [Authorize(Policy = "PuedeEditar")]
    public async Task<IActionResult> Create(int? tipoRegistroId, bool modoAcuerdos = false, CancellationToken cancellationToken = default)
    {
        var ninoId = await selectorNinoService.ObtenerNinoSeleccionadoAsync(cancellationToken);
        var model = await registroService.ObtenerEditorAsync(null, tipoRegistroId, ninoId, modoAcuerdos, cancellationToken);
        return View(model);
    }

    [Authorize(Policy = "PuedeEditar")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RegistroEditorViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            var ninoId = await selectorNinoService.ObtenerNinoSeleccionadoAsync(cancellationToken);
            var hydrated = await registroService.ObtenerEditorAsync(null, model.Form.TipoRegistroId, ninoId, model.ModoAcuerdos, cancellationToken);
            return View(new RegistroEditorViewModel
            {
                Titulo = hydrated.Titulo,
                IsEdit = hydrated.IsEdit,
                ModoAcuerdos = hydrated.ModoAcuerdos,
                Form = model.Form,
                Ninos = hydrated.Ninos,
                Disciplinas = hydrated.Disciplinas,
                TiposRegistro = hydrated.TiposRegistro,
                Plataformas = hydrated.Plataformas,
                CorreosOrigen = hydrated.CorreosOrigen,
                Estados = hydrated.Estados,
                Prioridades = hydrated.Prioridades,
                EvidenciasExistentes = hydrated.EvidenciasExistentes
            });
        }

        var id = await registroService.CrearAsync(model.Form, usuarioActualService.GetUsuarioIdOrThrow(), cancellationToken);
        TempData["StatusMessage"] = "Registro guardado correctamente.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [Authorize(Policy = "PuedeEditar")]
    public async Task<IActionResult> Edit(int id, bool modoAcuerdos = false, CancellationToken cancellationToken = default)
    {
        var model = await registroService.ObtenerEditorAsync(id, null, null, modoAcuerdos, cancellationToken);
        return View(model);
    }

    [Authorize(Policy = "PuedeEditar")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, RegistroEditorViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            var hydrated = await registroService.ObtenerEditorAsync(id, model.Form.TipoRegistroId, null, model.ModoAcuerdos, cancellationToken);
            return View(new RegistroEditorViewModel
            {
                Titulo = hydrated.Titulo,
                IsEdit = hydrated.IsEdit,
                ModoAcuerdos = hydrated.ModoAcuerdos,
                Form = model.Form,
                Ninos = hydrated.Ninos,
                Disciplinas = hydrated.Disciplinas,
                TiposRegistro = hydrated.TiposRegistro,
                Plataformas = hydrated.Plataformas,
                CorreosOrigen = hydrated.CorreosOrigen,
                Estados = hydrated.Estados,
                Prioridades = hydrated.Prioridades,
                EvidenciasExistentes = hydrated.EvidenciasExistentes
            });
        }

        var ok = await registroService.ActualizarAsync(id, model.Form, usuarioActualService.GetUsuarioIdOrThrow(), cancellationToken);
        if (!ok)
        {
            return NotFound();
        }

        TempData["StatusMessage"] = "Registro actualizado correctamente.";
        return RedirectToAction(nameof(Details), new { id });
    }

    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var model = await registroService.ObtenerDetalleAsync(id, cancellationToken);
        return model is null ? NotFound() : View(model);
    }

    [Authorize(Policy = "PuedeEditar")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> QuickStatus(int id, int estadoSeguimientoId, string? comentario, CancellationToken cancellationToken)
    {
        var ok = await registroService.CambiarEstadoRapidoAsync(id, estadoSeguimientoId, usuarioActualService.GetUsuarioIdOrThrow(), comentario, cancellationToken);
        if (!ok)
        {
            return NotFound();
        }

        TempData["StatusMessage"] = "Estado actualizado correctamente.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [Authorize(Policy = "PuedeEditar")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Deactivate(int id, CancellationToken cancellationToken)
    {
        var ok = await registroService.DesactivarAsync(id, usuarioActualService.GetUsuarioIdOrThrow(), cancellationToken);
        if (!ok)
        {
            return NotFound();
        }

        TempData["StatusMessage"] = "Registro marcado como inactivo.";
        return RedirectToAction(nameof(Index));
    }
}
