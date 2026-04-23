namespace SeguimientoColegio.Web.Dtos;

public sealed class RegistroListaDto
{
    public int Id { get; init; }
    public string Titulo { get; init; } = string.Empty;
    public string Nino { get; init; } = string.Empty;
    public string? Disciplina { get; init; }
    public string TipoRegistro { get; init; } = string.Empty;
    public string? Plataforma { get; init; }
    public string Estado { get; init; } = string.Empty;
    public string EstadoColorCss { get; init; } = "#7a828f";
    public int EstadoSeguimientoId { get; init; }
    public string? Prioridad { get; init; }
    public string? PrioridadColorCss { get; init; }
    public int? PrioridadId { get; init; }
    public DateOnly FechaRecibido { get; init; }
    public DateOnly? FechaVencimiento { get; init; }
    public DateOnly? FechaRealizado { get; init; }
    public DateTime FechaActualizacion { get; init; }
    public string? ResponsablePrincipal { get; init; }
    public string? CorreoOrigenAsunto { get; init; }
    public int UrlsCount { get; init; }
    public int EvidenciasCount { get; init; }
    public int AntiguedadDias { get; init; }
    public bool EstaVencido { get; init; }
    public IReadOnlyList<AlertaBadgeDto> Alertas { get; init; } = Array.Empty<AlertaBadgeDto>();
}

public sealed class RegistroUrlDto
{
    public int Id { get; init; }
    public string Url { get; init; } = string.Empty;
    public string? Descripcion { get; init; }
    public int Orden { get; init; }
}

public sealed class EvidenciaDto
{
    public int Id { get; init; }
    public string NombreArchivo { get; init; } = string.Empty;
    public string TipoMime { get; init; } = string.Empty;
    public long TamanoBytes { get; init; }
    public string? Descripcion { get; init; }
    public string SubidoPor { get; init; } = string.Empty;
    public DateTime FechaCreacion { get; init; }
}

public sealed class RegistroAuditoriaDto
{
    public int Id { get; init; }
    public string Accion { get; init; } = string.Empty;
    public string? CampoModificado { get; init; }
    public string? ValorAnterior { get; init; }
    public string? ValorNuevo { get; init; }
    public string? Comentario { get; init; }
    public string Usuario { get; init; } = string.Empty;
    public DateTime FechaCreacion { get; init; }
}

public sealed class RegistroDetalleDto
{
    public int Id { get; init; }
    public string Titulo { get; init; } = string.Empty;
    public string? Descripcion { get; init; }
    public string Nino { get; init; } = string.Empty;
    public int NinoId { get; init; }
    public string? Disciplina { get; init; }
    public int? DisciplinaId { get; init; }
    public string TipoRegistro { get; init; } = string.Empty;
    public int TipoRegistroId { get; init; }
    public string? Plataforma { get; init; }
    public int? PlataformaId { get; init; }
    public string Estado { get; init; } = string.Empty;
    public int EstadoSeguimientoId { get; init; }
    public string EstadoColorCss { get; init; } = "#7a828f";
    public string? Prioridad { get; init; }
    public int? PrioridadId { get; init; }
    public string? PrioridadColorCss { get; init; }
    public string? CorreoOrigenAsunto { get; init; }
    public int? CorreoOrigenId { get; init; }
    public string? ResponsablePrincipal { get; init; }
    public DateOnly FechaRecibido { get; init; }
    public DateOnly? FechaVencimiento { get; init; }
    public DateOnly? FechaRealizado { get; init; }
    public int? DiasAlerta { get; init; }
    public bool RequiereSeguimiento { get; init; }
    public string? ComentarioGeneral { get; init; }
    public string CreadoPor { get; init; } = string.Empty;
    public string ActualizadoPor { get; init; } = string.Empty;
    public DateTime FechaCreacion { get; init; }
    public DateTime FechaActualizacion { get; init; }
    public bool Activo { get; init; }
    public IReadOnlyList<RegistroUrlDto> Urls { get; init; } = Array.Empty<RegistroUrlDto>();
    public IReadOnlyList<EvidenciaDto> Evidencias { get; init; } = Array.Empty<EvidenciaDto>();
    public IReadOnlyList<RegistroAuditoriaDto> Historial { get; init; } = Array.Empty<RegistroAuditoriaDto>();
    public IReadOnlyList<AlertaBadgeDto> Alertas { get; init; } = Array.Empty<AlertaBadgeDto>();
}
