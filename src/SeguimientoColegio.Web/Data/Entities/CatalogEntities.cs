namespace SeguimientoColegio.Web.Data.Entities;

public sealed class Rol : EntityBase
{
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;

    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}

public sealed class Usuario : ActivableEntity
{
    public string Nombres { get; set; } = string.Empty;
    public string? Apellidos { get; set; }
    public string Correo { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public int RolId { get; set; }
    public DateTime? UltimoLogin { get; set; }

    public Rol? Rol { get; set; }
    public ICollection<RegistroEscolar> RegistrosCreados { get; set; } = new List<RegistroEscolar>();
    public ICollection<RegistroEscolar> RegistrosActualizados { get; set; } = new List<RegistroEscolar>();
    public ICollection<EvidenciaRegistro> EvidenciasSubidas { get; set; } = new List<EvidenciaRegistro>();
    public ICollection<RegistroAuditoria> Auditorias { get; set; } = new List<RegistroAuditoria>();

    public string NombreCompleto => string.Join(
        " ",
        new[] { Nombres, Apellidos }.Where(static value => !string.IsNullOrWhiteSpace(value)));
}

public sealed class Nino : ActivableEntity
{
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string? Alias { get; set; }
    public DateOnly? FechaNacimiento { get; set; }

    public ICollection<RegistroEscolar> Registros { get; set; } = new List<RegistroEscolar>();

    public string NombreCompleto => string.Join(
        " ",
        new[] { Nombres, Apellidos }.Where(static value => !string.IsNullOrWhiteSpace(value)));
}

public sealed class Disciplina : CatalogoBaseEntity
{
    public ICollection<RegistroEscolar> Registros { get; set; } = new List<RegistroEscolar>();
}

public sealed class Plataforma : CatalogoBaseEntity
{
    public ICollection<RegistroEscolar> Registros { get; set; } = new List<RegistroEscolar>();
}

public sealed class TipoRegistro : CatalogoBaseEntity
{
    public ICollection<RegistroEscolar> Registros { get; set; } = new List<RegistroEscolar>();
}

public sealed class EstadoSeguimiento : CatalogoConOrdenEntity
{
    public ICollection<RegistroEscolar> Registros { get; set; } = new List<RegistroEscolar>();
}

public sealed class Prioridad : CatalogoConOrdenEntity
{
    public ICollection<RegistroEscolar> Registros { get; set; } = new List<RegistroEscolar>();
}

public sealed class CorreoOrigen : FechasAuditoriaEntity
{
    public string Asunto { get; set; } = string.Empty;
    public string Remitente { get; set; } = string.Empty;
    public DateTime FechaCorreo { get; set; }
    public string? Resumen { get; set; }
    public string? ReferenciaUrl { get; set; }
    public string? MessageId { get; set; }
    public string? Observaciones { get; set; }

    public ICollection<RegistroEscolar> Registros { get; set; } = new List<RegistroEscolar>();
}
