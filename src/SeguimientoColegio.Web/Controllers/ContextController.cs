using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SeguimientoColegio.Web.Services;

namespace SeguimientoColegio.Web.Controllers;

[Authorize]
public sealed class ContextController(ISelectorNinoService selectorNinoService) : Controller
{
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SeleccionarNino(int? ninoId, string? returnUrl, CancellationToken cancellationToken)
    {
        await selectorNinoService.EstablecerNinoSeleccionadoAsync(ninoId, cancellationToken);
        return !string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl)
            ? Redirect(returnUrl)
            : RedirectToAction("Index", "Dashboard");
    }
}
