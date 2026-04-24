using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SeguimientoColegio.Web.Models;

public sealed class UsuarioInputModel : IValidatableObject
{
    [Required(ErrorMessage = "Ingresa los nombres.")]
    [StringLength(120)]
    public string Nombres { get; set; } = string.Empty;

    [StringLength(150)]
    public string? Apellidos { get; set; }

    [Required(ErrorMessage = "Ingresa el correo.")]
    [EmailAddress(ErrorMessage = "Ingresa un correo valido.")]
    [StringLength(180)]
    public string Correo { get; set; } = string.Empty;

    [Required(ErrorMessage = "Selecciona el rol.")]
    public int? RolId { get; set; }

    [DataType(DataType.Password)]
    public string? Password { get; set; }

    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Las contrasenas no coinciden.")]
    public string? ConfirmPassword { get; set; }

    public bool Activo { get; set; } = true;
    public bool RequierePassword { get; set; } = true;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (RequierePassword && string.IsNullOrWhiteSpace(Password))
        {
            yield return new ValidationResult("Ingresa una contrasena.", new[] { nameof(Password) });
        }

        if (!string.IsNullOrWhiteSpace(Password) && Password.Trim().Length < 8)
        {
            yield return new ValidationResult("La contrasena debe tener al menos 8 caracteres.", new[] { nameof(Password) });
        }
    }
}

public sealed class UsuarioItemViewModel
{
    public int Id { get; init; }
    public string Nombres { get; init; } = string.Empty;
    public string? Apellidos { get; init; }
    public string Correo { get; init; } = string.Empty;
    public string Rol { get; init; } = string.Empty;
    public bool Activo { get; init; }
    public DateTime? UltimoLogin { get; init; }
    public DateTime FechaActualizacion { get; init; }
}

public sealed class UsuariosIndexViewModel
{
    public IReadOnlyList<UsuarioItemViewModel> Items { get; init; } = Array.Empty<UsuarioItemViewModel>();
}

public sealed class UsuarioFormPageViewModel
{
    public bool IsEdit { get; init; }
    public UsuarioInputModel Form { get; init; } = new();
    public IEnumerable<SelectListItem> Roles { get; init; } = Array.Empty<SelectListItem>();
}
