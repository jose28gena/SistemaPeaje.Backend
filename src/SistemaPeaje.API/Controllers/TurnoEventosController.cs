using MediatR;
using Microsoft.AspNetCore.Mvc;
using SistemaPeaje.Application.Features.TurnosAdmin.TurnoEventos;
using SistemaPeaje.Application.DTOs.TurnosAdmin;

namespace SistemaPeaje.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TurnoEventosController : ControllerBase
{
    private readonly IMediator _mediator;

    public TurnoEventosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtener todos los eventos de turnos
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<TurnoEventoDto>>> GetTurnoEventos([FromQuery] int? turnoAsignacionId = null, [FromQuery] string? tipoEvento = null, [FromQuery] bool? esResuelto = null)
    {
        var query = new GetTurnoEventosQuery(turnoAsignacionId, tipoEvento, esResuelto);
        var resultado = await _mediator.Send(query);
        return Ok(resultado);
    }

    /// <summary>
    /// Obtener evento por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<TurnoEventoDto>> GetTurnoEventoById(int id)
    {
        var query = new GetTurnoEventoByIdQuery(id);
        var resultado = await _mediator.Send(query);
        if (resultado == null)
            return NotFound();
        return Ok(resultado);
    }

    /// <summary>
    /// Crear nuevo evento de turno
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<TurnoEventoDto>> CreateTurnoEvento([FromBody] CreateTurnoEventoRequest request)
    {
        var command = new CreateTurnoEventoCommand(request.TurnoAsignacionId, request.TipoEvento, request.Descripcion, request.DetallesAdicionales);
        var resultado = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetTurnoEventoById), new { id = resultado.Id }, resultado);
    }

    /// <summary>
    /// Actualizar evento de turno
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<TurnoEventoDto>> UpdateTurnoEvento(int id, [FromBody] UpdateTurnoEventoRequest request)
    {
        var command = new UpdateTurnoEventoCommand(id, request.Descripcion, request.DetallesAdicionales);
        var resultado = await _mediator.Send(command);
        return Ok(resultado);
    }

    /// <summary>
    /// Resolver evento de turno
    /// </summary>
    [HttpPut("{id}/resolver")]
    public async Task<ActionResult<TurnoEventoDto>> ResolverTurnoEvento(int id, [FromBody] ResolverEventoRequest request)
    {
        var command = new ResolverTurnoEventoCommand(id, request.SolucionAplicada, request.ResueltoPort);
        var resultado = await _mediator.Send(command);
        return Ok(resultado);
    }

    /// <summary>
    /// Eliminar evento de turno
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult<bool>> DeleteTurnoEvento(int id)
    {
        var command = new DeleteTurnoEventoCommand(id);
        var resultado = await _mediator.Send(command);
        return Ok(resultado);
    }

    /// <summary>
    /// Obtener eventos pendientes
    /// </summary>
    [HttpGet("pendientes")]
    public async Task<ActionResult<List<TurnoEventoDto>>> GetEventosPendientes()
    {
        var query = new GetEventosPendientesQuery();
        var resultado = await _mediator.Send(query);
        return Ok(resultado);
    }

    /// <summary>
    /// Obtener resumen de eventos por tipo
    /// </summary>
    [HttpGet("resumen-por-tipo")]
    public async Task<ActionResult<List<ResumenEventosPorTipoDto>>> GetResumenEventosPorTipo([FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin)
    {
        var query = new GetResumenEventosPorTipoQuery(fechaInicio, fechaFin);
        var resultado = await _mediator.Send(query);
        return Ok(resultado);
    }

    /// <summary>
    /// Obtener eventos por empleado
    /// </summary>
    [HttpGet("empleado/{empleadoId}")]
    public async Task<ActionResult<List<TurnoEventoDto>>> GetEventosPorEmpleado(int empleadoId, [FromQuery] DateTime? fechaInicio = null, [FromQuery] DateTime? fechaFin = null)
    {
        var query = new GetEventosPorEmpleadoQuery(empleadoId, fechaInicio, fechaFin);
        var resultado = await _mediator.Send(query);
        return Ok(resultado);
    }

    /// <summary>
    /// Obtener eventos por estación
    /// </summary>
    [HttpGet("estacion/{estacionId}")]
    public async Task<ActionResult<List<TurnoEventoDto>>> GetEventosPorEstacion(int estacionId, [FromQuery] DateTime? fechaInicio = null, [FromQuery] DateTime? fechaFin = null)
    {
        var query = new GetEventosPorEstacionQuery(estacionId, fechaInicio, fechaFin);
        var resultado = await _mediator.Send(query);
        return Ok(resultado);
    }
}

// DTOs para requests
public class CreateTurnoEventoRequest
{
    public int TurnoAsignacionId { get; set; }
    public string TipoEvento { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string? DetallesAdicionales { get; set; }
}

public class UpdateTurnoEventoRequest
{
    public string? Descripcion { get; set; }
    public string? DetallesAdicionales { get; set; }
}

public class ResolverEventoRequest
{
    public string SolucionAplicada { get; set; } = string.Empty;
    public string ResueltoPort { get; set; } = string.Empty;
}
