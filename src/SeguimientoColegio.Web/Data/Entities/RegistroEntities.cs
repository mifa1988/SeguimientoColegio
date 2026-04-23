namespace SeguimientoColegio.Web.Data.Entities;

public sealed class RegistroEscolar : ActivableEntity
{
    public int NinoId { get; set; }
    public int? DisciplinaId { get; set; }
    public int TipoRegistroId { get; set; }
    public int? PlataformaId { get; set; }
    public int? CorreoOrigenId { get; set; }
    public int EstadoSeguimientoId { get; set; }
    public int? PrioridadId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string? ResponsablePrincipal { get; set; }
    public DateOnly FechaRecibido { get; set; }
    public DateOnly? FechaVencimiento { get; set; }
    public DateOnly? FechaRealizado { get; set; }
    public int? DiasAlerta { get; set; }
    public bool RequiereSeguimiento { get; set; } = true;
    public string? ComentarioGeneral { get; set; }
    public int CreadoPorUsuarioId { get; set; }
    public int ActualizadoPorUsuarioId { get; set; }

    public Nino? Nino { get; set; }
    public Disciplina? Disciplina { get; set; }
    public TipoRegistro? TipoRegistro { get; set; }
    public Plataforma? Plataforma { get; set; }
    public CorreoOrigen? CorreoOrigen { get; set; }
    public EstadoSeguimiento? EstadoSeguimiento { get; set; }
    public Prioridad? Prioridad { get; set; }
    public Usuario? CreadoPorUsuario { get; set; }
    public Usuario? ActualizadoPorUsuario { get; set; }
    public ICollection<RegistroUrl> Urls { get; set; } = new List<RegistroUrl>();
    public ICollection<EvidenciaRegistro> Evidencias { get; set; } = new List<EvidenciaRegistro>();
    public ICollection<RegistroAuditoria> Auditoria { get; set; } = new List<RegistroAuditoria>();
}

public sealed class RegistroUrl : EntityBase
{
    public int RegistroEscolarId { get; set; }
    public string Url { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int Orden { get; set; }
    public DateTime FechaCreacion { get; set; }

    public RegistroEscolar? RegistroEscolar { get; set; }
}

public sealed class EvidenciaRegistro : EntityBase
{
    public int RegistroEscolarId { get; set; }
    public string NombreArchivo { get; set; } = string.Empty;
    public string StorageKey { get; set; } = string.Empty;
    public string TipoMime { get; set; } = string.Empty;
    public long TamanoBytes { get; set; }
    public string? Descripcion { get; set; }
    public int SubidoPorUsuarioId { get; set; }
    public DateTime FechaCreacion { get; set; }

    public RegistroEscolar? RegistroEscolar { get; set; }
    public Usuario? SubidoPorUsuario { get; set; }
}

public sealed class RegistroAuditoria : EntityBase
{
    public int RegistroEscolarId { get; set; }
    public string Accion { get; set; } = string.Empty;
    public string? CampoModificado { get; set; }
    public string? ValorAnterior { get; set; }
    public string? ValorNuevo { get; set; }
    public string? Comentario { get; set; }
    public int UsuarioId { get; set; }
    public DateTime FechaCreacion { get; set; }

    public RegistroEscolar? RegistroEscolar { get; set; }
    public Usuario? Usuario { get; set; }
}
