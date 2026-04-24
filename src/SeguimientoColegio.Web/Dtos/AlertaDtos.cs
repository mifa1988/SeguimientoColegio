namespace SeguimientoColegio.Web.Dtos;

public sealed class AlertaBadgeDto
{
    public string Codigo { get; init; } = string.Empty;
    public string Titulo { get; init; } = string.Empty;
    public string Severidad { get; init; } = string.Empty;
    public string Mensaje { get; init; } = string.Empty;
}

public sealed class AlertaRegistroDto
{
    public int RegistroId { get; init; }
    public string Titulo { get; init; } = string.Empty;
    public string Nino { get; init; } = string.Empty;
    public string? Disciplina { get; init; }
    public string TipoRegistro { get; init; } = string.Empty;
    public string Estado { get; init; } = string.Empty;
    public string? Prioridad { get; init; }
    public DateOnly FechaRecibido { get; init; }
    public DateOnly? FechaVencimiento { get; init; }
    public DateTime FechaActualizacion { get; init; }
    public int AntiguedadDias { get; init; }
    public string SeveridadMaxima { get; init; } = "info";
    public IReadOnlyList<AlertaBadgeDto> Badges { get; init; } = Array.Empty<AlertaBadgeDto>();
}

public sealed class SnapshotAlertaRegistroDto
{
    public int RegistroId { get; init; }
    public int NinoId { get; init; }
    public string Nino { get; init; } = string.Empty;
    public int? DisciplinaId { get; init; }
    public string? Disciplina { get; init; }
    public int TipoRegistroId { get; init; }
    public string TipoRegistro { get; init; } = string.Empty;
    public int EstadoSeguimientoId { get; init; }
    public string EstadoSeguimiento { get; init; } = string.Empty;
    public int? PrioridadId { get; init; }
    public string? Prioridad { get; init; }
    public string Titulo { get; init; } = string.Empty;
    public DateOnly FechaRecibido { get; init; }
    public DateOnly? FechaVencimiento { get; init; }
    public DateOnly? FechaRealizado { get; init; }
    public DateTime FechaActualizacion { get; init; }
    public bool RequiereSeguimiento { get; init; }
    public int? DiasAlerta { get; init; }
    public bool Activo { get; init; }
}
