namespace SeguimientoColegio.Web.Dtos;

public sealed class ConteoAgrupadoDto
{
    public string Etiqueta { get; init; } = string.Empty;
    public int Total { get; init; }
    public string? Detalle { get; init; }
}

public sealed class RegistroBreveDto
{
    public int Id { get; init; }
    public string Titulo { get; init; } = string.Empty;
    public string Nino { get; init; } = string.Empty;
    public string? Disciplina { get; init; }
    public string TipoRegistro { get; init; } = string.Empty;
    public string Estado { get; init; } = string.Empty;
    public string EstadoColorCss { get; init; } = "#7a828f";
    public string? Prioridad { get; init; }
    public string? PrioridadColorCss { get; init; }
    public DateOnly FechaRecibido { get; init; }
    public DateTime FechaActualizacion { get; init; }
    public int AntiguedadDias { get; init; }
    public bool EstaVencido { get; init; }
}

public sealed class DashboardResumenDto
{
    public int TotalPendientes { get; init; }
    public int TotalEnProceso { get; init; }
    public int TotalConcluidos { get; init; }
    public int TotalVencidos { get; init; }
    public int PendientesAntiguos { get; init; }
    public int AcuerdosPendientes { get; init; }
    public IReadOnlyList<ConteoAgrupadoDto> PendientesPorNino { get; init; } = Array.Empty<ConteoAgrupadoDto>();
    public IReadOnlyList<ConteoAgrupadoDto> PendientesPorDisciplina { get; init; } = Array.Empty<ConteoAgrupadoDto>();
    public IReadOnlyList<RegistroBreveDto> UltimosRegistrosCreados { get; init; } = Array.Empty<RegistroBreveDto>();
    public IReadOnlyList<RegistroBreveDto> UltimosRegistrosActualizados { get; init; } = Array.Empty<RegistroBreveDto>();
    public IReadOnlyList<AlertaRegistroDto> AlertasCriticas { get; init; } = Array.Empty<AlertaRegistroDto>();
}
