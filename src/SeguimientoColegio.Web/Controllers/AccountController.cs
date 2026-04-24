using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SeguimientoColegio.Web.Models;
using SeguimientoColegio.Web.Services;

namespace SeguimientoColegio.Web.Controllers;

public sealed class AccountController(IAuthService authService) : Controller
{
    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Dashboard");
        }

        return View(new LoginViewModel
        {
            Form = new LoginInputModel
            {
                ReturnUrl = returnUrl
            }
        });
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await authService.IniciarSesionAsync(HttpContext, model.Form, cancellationToken);
        if (!result.Success)
        {
            model.ErrorMessage = result.ErrorMessage ?? "No se pudo iniciar sesion.";
            return View(model);
        }

        if (!string.IsNullOrWhiteSpace(model.Form.ReturnUrl) && Url.IsLocalUrl(model.Form.ReturnUrl))
        {
            return Redirect(model.Form.ReturnUrl);
        }

        return RedirectToAction("Index", "Dashboard");
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await authService.CerrarSesionAsync(HttpContext);
        return RedirectToAction(nameof(Login));
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult AccessDenied()
        => View();
}
