using System.ComponentModel.DataAnnotations;
using SeguimientoColegio.Web.Dtos;

namespace SeguimientoColegio.Web.Models;

public sealed class CorreoOrigenInputModel
{
    [Required(ErrorMessage = "Ingresa el asunto.")]
    [StringLength(250)]
    public string Asunto { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ingresa el remitente.")]
    [EmailAddress(ErrorMessage = "Ingresa un correo valido.")]
    [StringLength(180)]
    public string Remitente { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ingresa la fecha del correo.")]
    public DateTime FechaCorreo { get; set; } = DateTime.Now;

    [StringLength(1000)]
    public string? Resumen { get; set; }

    [Url(ErrorMessage = "Ingresa una URL valida.")]
    [StringLength(500)]
    public string? ReferenciaUrl { get; set; }

    [StringLength(255)]
    public string? MessageId { get; set; }

    [StringLength(1000)]
    public string? Observaciones { get; set; }
}

public sealed class CorreoOrigenItemViewModel
{
    public int Id { get; init; }
    public string Asunto { get; init; } = string.Empty;
    public string Remitente { get; init; } = string.Empty;
    public DateTime FechaCorreo { get; init; }
    public string? Resumen { get; init; }
    public int RegistrosRelacionados { get; init; }
    public DateTime FechaActualizacion { get; init; }
}

public sealed class CorreosOrigenIndexViewModel
{
    public string? Search { get; set; }
    public IReadOnlyList<CorreoOrigenItemViewModel> Items { get; init; } = Array.Empty<CorreoOrigenItemViewModel>();
}

public sealed class CorreoOrigenFormPageViewModel
{
    public bool IsEdit { get; init; }
    public CorreoOrigenInputModel Form { get; init; } = new();
}

public sealed class CorreoOrigenDetailViewModel
{
    public CorreoOrigenItemViewModel Correo { get; init; } = new();
    public string? ReferenciaUrl { get; init; }
    public string? MessageId { get; init; }
    public string? Observaciones { get; init; }
    public IReadOnlyList<RegistroBreveDto> RegistrosRelacionados { get; init; } = Array.Empty<RegistroBreveDto>();
}
