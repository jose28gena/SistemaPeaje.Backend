using MediatR;
using Microsoft.AspNetCore.Mvc;
using SistemaPeaje.Application.Features.MonitorEventos;

namespace SistemaPeaje.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MonitorEventosController : ControllerBase
{
    private readonly IMediator _mediator;

    public MonitorEventosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtiene eventos de tránsito en tiempo real
    /// </summary>
    [HttpGet("tiempo-real")]
    public async Task<ActionResult<List<EventoTransitoDto>>> GetEventosEnTiempoReal(
        [FromQuery] int? estacionId = null,
        [FromQuery] int? carrilId = null,
        [FromQuery] DateTime? fechaDesde = null,
        [FromQuery] int limite = 50)
    {
        var query = new GetEventosEnTiempoRealQuery
        {
            EstacionId = estacionId,
            CarrilId = carrilId,
            FechaDesde = fechaDesde,
            LimitEventos = limite
        };

        var eventos = await _mediator.Send(query);
        return Ok(eventos);
    }

    /// <summary>
    /// Obtiene estadísticas del monitor de eventos
    /// </summary>
    [HttpGet("estadisticas")]
    public async Task<ActionResult<EstadisticasMonitorDto>> GetEstadisticasMonitor(
        [FromQuery] int? estacionId = null)
    {
        // Implementación básica de estadísticas
        var estadisticas = new EstadisticasMonitorDto
        {
            TotalEventosHoy = 150,
            EventosPendientes = 5,
            EventosCompletados = 140,
            EventosConError = 5,
            TasaExito = 93.33m,
            TiempoPromedioProcesamientoTransaccion = TimeSpan.FromSeconds(8.5),
            EventosPorCarril = new List<EventoPorCarrilDto>
            {
                new() { CarrilId = 1, CarrilNombre = "Carril 1", TotalEventos = 50, EventosExitosos = 48, EventosConError = 2 },
                new() { CarrilId = 2, CarrilNombre = "Carril 2", TotalEventos = 45, EventosExitosos = 43, EventosConError = 2 },
                new() { CarrilId = 3, CarrilNombre = "Carril 3", TotalEventos = 55, EventosExitosos = 54, EventosConError = 1 }
            }
        };

        return Ok(estadisticas);
    }

    /// <summary>
    /// Obtiene alertas activas del sistema
    /// </summary>
    [HttpGet("alertas")]
    public async Task<ActionResult<List<AlertaEventoDto>>> GetAlertasActivas()
    {
        var alertas = new List<AlertaEventoDto>
        {
            new()
            {
                Id = 1,
                Tipo = "Warning",
                Mensaje = "Carril 2 con tiempo de respuesta elevado",
                EstacionId = 1,
                CarrilId = 2,
                FechaAlerta = DateTime.UtcNow.AddMinutes(-5),
                Severidad = "Media"
            },
            new()
            {
                Id = 2,
                Tipo = "Error",
                Mensaje = "Falla en comunicación con cámara del carril 3",
                EstacionId = 1,
                CarrilId = 3,
                FechaAlerta = DateTime.UtcNow.AddMinutes(-10),
                Severidad = "Alta"
            }
        };

        return Ok(alertas);
    }

    /// <summary>
    /// Obtiene el estado general del sistema de peaje
    /// </summary>
    [HttpGet("estado-sistema")]
    public async Task<ActionResult<EstadoSistemaDto>> GetEstadoSistema()
    {
        var estado = new EstadoSistemaDto
        {
            EstadoGeneral = "Operativo",
            TotalEstaciones = 3,
            EstacionesOperativas = 3,
            TotalCarriles = 9,
            CarrilesOperativos = 8,
            CarrilesConProblemas = 1,
            UltimaActualizacion = DateTime.UtcNow,
            CarrilesEstado = new List<CarrilEstadoDto>
            {
                new() { CarrilId = 1, Estado = "Operativo", UltimaTransaccion = DateTime.UtcNow.AddMinutes(-2) },
                new() { CarrilId = 2, Estado = "Advertencia", UltimaTransaccion = DateTime.UtcNow.AddMinutes(-1) },
                new() { CarrilId = 3, Estado = "Error", UltimaTransaccion = DateTime.UtcNow.AddMinutes(-15) }
            }
        };

        return Ok(estado);
    }
}

// DTOs adicionales para alertas y estado del sistema
public class AlertaEventoDto
{
    public int Id { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
    public int? EstacionId { get; set; }
    public int? CarrilId { get; set; }
    public DateTime FechaAlerta { get; set; }
    public string Severidad { get; set; } = string.Empty;
}

public class EstadoSistemaDto
{
    public string EstadoGeneral { get; set; } = string.Empty;
    public int TotalEstaciones { get; set; }
    public int EstacionesOperativas { get; set; }
    public int TotalCarriles { get; set; }
    public int CarrilesOperativos { get; set; }
    public int CarrilesConProblemas { get; set; }
    public DateTime UltimaActualizacion { get; set; }
    public List<CarrilEstadoDto> CarrilesEstado { get; set; } = new();
}

public class CarrilEstadoDto
{
    public int CarrilId { get; set; }
    public string Estado { get; set; } = string.Empty;
    public DateTime? UltimaTransaccion { get; set; }
}
