using System.ComponentModel.DataAnnotations;

namespace SeguimientoColegio.Web.Models;

public class CatalogoBasicoInputModel
{
    [Required(ErrorMessage = "Ingresa el nombre.")]
    [StringLength(120)]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Descripcion { get; set; }

    public bool Activo { get; set; } = true;
}

public sealed class CatalogoConOrdenInputModel : CatalogoBasicoInputModel
{
    [Range(1, 999, ErrorMessage = "Ingresa un orden valido.")]
    public int Orden { get; set; } = 1;

    [StringLength(30)]
    public string? ColorCss { get; set; }
}

public class CatalogoBasicoItemViewModel
{
    public int Id { get; init; }
    public string Nombre { get; init; } = string.Empty;
    public string? Descripcion { get; init; }
    public bool Activo { get; init; }
    public DateTime FechaActualizacion { get; init; }
}

public sealed class CatalogoConOrdenItemViewModel : CatalogoBasicoItemViewModel
{
    public int Orden { get; init; }
    public string? ColorCss { get; init; }
}

public sealed class CatalogoBasicoPageViewModel
{
    public string Titulo { get; init; } = string.Empty;
    public string DescripcionPagina { get; init; } = string.Empty;
    public string ControllerName { get; init; } = string.Empty;
    public string SingularName { get; init; } = string.Empty;
    public IReadOnlyList<CatalogoBasicoItemViewModel> Items { get; init; } = Array.Empty<CatalogoBasicoItemViewModel>();
}

public sealed class CatalogoBasicoFormPageViewModel
{
    public string Titulo { get; init; } = string.Empty;
    public string DescripcionPagina { get; init; } = string.Empty;
    public string ControllerName { get; init; } = string.Empty;
    public string SingularName { get; init; } = string.Empty;
    public bool IsEdit { get; init; }
    public CatalogoBasicoInputModel Form { get; init; } = new();
}

public sealed class CatalogoConOrdenPageViewModel
{
    public string Titulo { get; init; } = string.Empty;
    public string DescripcionPagina { get; init; } = string.Empty;
    public string ControllerName { get; init; } = string.Empty;
    public string SingularName { get; init; } = string.Empty;
    public IReadOnlyList<CatalogoConOrdenItemViewModel> Items { get; init; } = Array.Empty<CatalogoConOrdenItemViewModel>();
}

public sealed class CatalogoConOrdenFormPageViewModel
{
    public string Titulo { get; init; } = string.Empty;
    public string DescripcionPagina { get; init; } = string.Empty;
    public string ControllerName { get; init; } = string.Empty;
    public string SingularName { get; init; } = string.Empty;
    public bool IsEdit { get; init; }
    public CatalogoConOrdenInputModel Form { get; init; } = new();
}
