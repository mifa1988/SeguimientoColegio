namespace SeguimientoColegio.Web.Data.Entities;

public abstract class EntityBase
{
    public int Id { get; set; }
}

public abstract class FechasAuditoriaEntity : EntityBase
{
    public DateTime FechaCreacion { get; set; }
    public DateTime FechaActualizacion { get; set; }
}

public abstract class ActivableEntity : FechasAuditoriaEntity
{
    public bool Activo { get; set; } = true;
}

public abstract class CatalogoBaseEntity : ActivableEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
}

public abstract class CatalogoConOrdenEntity : CatalogoBaseEntity
{
    public int Orden { get; set; }
    public string? ColorCss { get; set; }
}
