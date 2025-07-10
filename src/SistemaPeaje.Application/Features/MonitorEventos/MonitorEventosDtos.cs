namespace SistemaPeaje.Application.Features.MonitorEventos;

public class EventoTransitoDto
{
    public int Id { get; set; }
    public int EstacionId { get; set; }
    public int CarrilId { get; set; }
    public string TipoEvento { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public DateTime FechaEvento { get; set; }
    public string? DatosAdicionales { get; set; }
    public string Estado { get; set; } = string.Empty;
    public TimeSpan TiempoTranscurrido { get; set; }
}

public class EstadisticasMonitorDto
{
    public int TotalEventosHoy { get; set; }
    public int EventosPendientes { get; set; }
    public int EventosCompletados { get; set; }
    public int EventosConError { get; set; }
    public decimal TasaExito { get; set; }
    public TimeSpan TiempoPromedioProcesamientoTransaccion { get; set; }
    public List<EventoPorCarrilDto> EventosPorCarril { get; set; } = new();
}

public class EventoPorCarrilDto
{
    public int CarrilId { get; set; }
    public string? CarrilNombre { get; set; }
    public int TotalEventos { get; set; }
    public int EventosExitosos { get; set; }
    public int EventosConError { get; set; }
}
