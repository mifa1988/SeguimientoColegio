using System.ComponentModel.DataAnnotations;

namespace SeguimientoColegio.Web.Models;

public sealed class LoginInputModel
{
    [Required(ErrorMessage = "Ingresa el correo.")]
    [EmailAddress(ErrorMessage = "Ingresa un correo valido.")]
    public string Correo { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ingresa la contrasena.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public bool Recordarme { get; set; }
    public string? ReturnUrl { get; set; }
}

public sealed class LoginViewModel
{
    public LoginInputModel Form { get; set; } = new();
    public string? ErrorMessage { get; set; }
}
