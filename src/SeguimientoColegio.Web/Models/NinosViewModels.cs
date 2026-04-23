using System.ComponentModel.DataAnnotations;

namespace SeguimientoColegio.Web.Models;

public sealed class NinoInputModel
{
    [Required(ErrorMessage = "Ingresa los nombres.")]
    [StringLength(120)]
    public string Nombres { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ingresa los apellidos.")]
    [StringLength(150)]
    public string Apellidos { get; set; } = string.Empty;

    [StringLength(120)]
    public string? Alias { get; set; }

    [DataType(DataType.Date)]
    public DateOnly? FechaNacimiento { get; set; }

    public bool Activo { get; set; } = true;
}

public sealed class NinoItemViewModel
{
    public int Id { get; init; }
    public string Nombres { get; init; } = string.Empty;
    public string Apellidos { get; init; } = string.Empty;
    public string? Alias { get; init; }
    public DateOnly? FechaNacimiento { get; init; }
    public bool Activo { get; init; }
    public DateTime FechaActualizacion { get; init; }
}

public sealed class NinosIndexViewModel
{
    public IReadOnlyList<NinoItemViewModel> Items { get; init; } = Array.Empty<NinoItemViewModel>();
}

public sealed class NinoFormPageViewModel
{
    public bool IsEdit { get; init; }
    public NinoInputModel Form { get; init; } = new();
}
