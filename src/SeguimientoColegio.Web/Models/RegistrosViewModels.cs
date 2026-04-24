using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using SeguimientoColegio.Web.Constants;
using SeguimientoColegio.Web.Dtos;

namespace SeguimientoColegio.Web.Models;

public sealed class RegistroUrlInputModel
{
    public int? Id { get; set; }

    [Url(ErrorMessage = "Ingresa una URL valida.")]
    [StringLength(1000)]
    public string? Url { get; set; }

    [StringLength(250)]
    public string? Descripcion { get; set; }

    [Range(0, 999, ErrorMessage = "Ingresa un orden valido.")]
    public int Orden { get; set; }
}

public sealed class RegistroFiltrosViewModel
{
    public int? NinoId { get; set; }
    public int? DisciplinaId { get; set; }
    public int? TipoRegistroId { get; set; }
    public int? PlataformaId { get; set; }
    public int? EstadoSeguimientoId { get; set; }
    public int? PrioridadId { get; set; }
    public DateOnly? FechaRecibidoDesde { get; set; }
    public DateOnly? FechaRecibidoHasta { get; set; }
    public DateOnly? FechaRealizadoDesde { get; set; }
    public DateOnly? FechaRealizadoHasta { get; set; }
    public DateOnly? FechaVencimientoDesde { get; set; }
    public DateOnly? FechaVencimientoHasta { get; set; }
    public string? Texto { get; set; }
}

public sealed class RegistroEscolarInputModel : IValidatableObject
{
    public int? NinoId { get; set; }
    public int? DisciplinaId { get; set; }
    public int? TipoRegistroId { get; set; }
    public int? PlataformaId { get; set; }
    public int? CorreoOrigenId { get; set; }
    public int? EstadoSeguimientoId { get; set; }
    public int? PrioridadId { get; set; }

    [Required(ErrorMessage = "Ingresa el titulo.")]
    [StringLength(220)]
    public string Titulo { get; set; } = string.Empty;

    [StringLength(4000)]
    public string? Descripcion { get; set; }

    [StringLength(180)]
    public string? ResponsablePrincipal { get; set; }

    [DataType(DataType.Date)]
    public DateOnly FechaRecibido { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    [DataType(DataType.Date)]
    public DateOnly? FechaVencimiento { get; set; }

    [DataType(DataType.Date)]
    public DateOnly? FechaRealizado { get; set; }

    [Range(1, 365, ErrorMessage = "Ingresa un numero de dias valido.")]
    public int? DiasAlerta { get; set; }

    public bool RequiereSeguimiento { get; set; } = true;

    [StringLength(4000)]
    public string? ComentarioGeneral { get; set; }

    public bool Activo { get; set; } = true;

    public List<RegistroUrlInputModel> Urls { get; set; } = new()
    {
        new RegistroUrlInputModel()
    };

    public List<IFormFile> NuevosAdjuntos { get; set; } = new();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!NinoId.HasValue)
        {
            yield return new ValidationResult("Selecciona el nino.", new[] { nameof(NinoId) });
        }

        if (!TipoRegistroId.HasValue)
        {
            yield return new ValidationResult("Selecciona el tipo de registro.", new[] { nameof(TipoRegistroId) });
        }

        if (!EstadoSeguimientoId.HasValue)
        {
            yield return new ValidationResult("Selecciona el estado.", new[] { nameof(EstadoSeguimientoId) });
        }

        if (TipoRegistroId != SeedDataIds.TiposRegistro.AcuerdoProfesora && !DisciplinaId.HasValue)
        {
            yield return new ValidationResult("Selecciona la disciplina.", new[] { nameof(DisciplinaId) });
        }

        if (FechaVencimiento.HasValue && FechaVencimiento.Value < FechaRecibido)
        {
            yield return new ValidationResult(
                "La fecha de vencimiento no puede ser menor que la fecha recibido.",
                new[] { nameof(FechaVencimiento) });
        }

        if (FechaRealizado.HasValue && FechaRealizado.Value < FechaRecibido)
        {
            yield return new ValidationResult(
                "La fecha realizado no puede ser menor que la fecha recibido.",
                new[] { nameof(FechaRealizado) });
        }
    }
}

public sealed class RegistrosIndexViewModel
{
    public string Titulo { get; init; } = "Registros escolares";
    public bool ModoAcuerdos { get; init; }
    public RegistroFiltrosViewModel Filtros { get; init; } = new();
    public IReadOnlyList<RegistroListaDto> Registros { get; init; } = Array.Empty<RegistroListaDto>();
    public IEnumerable<SelectListItem> Ninos { get; init; } = Array.Empty<SelectListItem>();
    public IEnumerable<SelectListItem> Disciplinas { get; init; } = Array.Empty<SelectListItem>();
    public IEnumerable<SelectListItem> TiposRegistro { get; init; } = Array.Empty<SelectListItem>();
    public IEnumerable<SelectListItem> Plataformas { get; init; } = Array.Empty<SelectListItem>();
    public IEnumerable<SelectListItem> Estados { get; init; } = Array.Empty<SelectListItem>();
    public IEnumerable<SelectListItem> Prioridades { get; init; } = Array.Empty<SelectListItem>();
}

public sealed class RegistroEditorViewModel
{
    public string Titulo { get; init; } = string.Empty;
    public bool IsEdit { get; init; }
    public bool ModoAcuerdos { get; init; }
    public RegistroEscolarInputModel Form { get; init; } = new();
    public IEnumerable<SelectListItem> Ninos { get; init; } = Array.Empty<SelectListItem>();
    public IEnumerable<SelectListItem> Disciplinas { get; init; } = Array.Empty<SelectListItem>();
    public IEnumerable<SelectListItem> TiposRegistro { get; init; } = Array.Empty<SelectListItem>();
    public IEnumerable<SelectListItem> Plataformas { get; init; } = Array.Empty<SelectListItem>();
    public IEnumerable<SelectListItem> CorreosOrigen { get; init; } = Array.Empty<SelectListItem>();
    public IEnumerable<SelectListItem> Estados { get; init; } = Array.Empty<SelectListItem>();
    public IEnumerable<SelectListItem> Prioridades { get; init; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<EvidenciaDto> EvidenciasExistentes { get; init; } = Array.Empty<EvidenciaDto>();
}

public sealed class RegistroDetalleViewModel
{
    public RegistroDetalleDto Registro { get; init; } = new();
    public IEnumerable<SelectListItem> Estados { get; init; } = Array.Empty<SelectListItem>();
    public bool PuedeEditar { get; init; }
}

public sealed class AlertasFiltrosViewModel
{
    public int? NinoId { get; set; }
    public int? DisciplinaId { get; set; }
    public string? Severidad { get; set; }
    public string? Texto { get; set; }
}

public sealed class AlertasIndexViewModel
{
    public AlertasFiltrosViewModel Filtros { get; init; } = new();
    public IReadOnlyList<AlertaRegistroDto> Alertas { get; init; } = Array.Empty<AlertaRegistroDto>();
    public IEnumerable<SelectListItem> Ninos { get; init; } = Array.Empty<SelectListItem>();
    public IEnumerable<SelectListItem> Disciplinas { get; init; } = Array.Empty<SelectListItem>();
}
