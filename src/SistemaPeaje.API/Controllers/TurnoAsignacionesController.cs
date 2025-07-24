using MediatR;
using Microsoft.AspNetCore.Mvc;
using SistemaPeaje.Application.Features.TurnosAdmin.TurnoAsignaciones;
using SistemaPeaje.Application.DTOs.TurnosAdmin;

namespace SistemaPeaje.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TurnoAsignacionesController : ControllerBase
{
    private readonly IMediator _mediator;

    public TurnoAsignacionesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtener todas las asignaciones de turnos
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<TurnoAsignacionDto>>> GetTurnoAsignaciones([FromQuery] DateTime? fechaInicio = null, [FromQuery] DateTime? fechaFin = null, [FromQuery] int? empleadoId = null, [FromQuery] int? estacionId = null)
    {
        var query = new GetTurnoAsignacionesQuery(fechaInicio, fechaFin, empleadoId, estacionId);
        var resultado = await _mediator.Send(query);
        return Ok(resultado);
    }

    /// <summary>
    /// Obtener asignación de turno por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<TurnoAsignacionDto>> GetTurnoAsignacionById(int id)
    {
        var query = new GetTurnoAsignacionByIdQuery(id);
        var resultado = await _mediator.Send(query);
        if (resultado == null)
            return NotFound();
        return Ok(resultado);
    }

    /// <summary>
    /// Crear nueva asignación de turno
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<TurnoAsignacionDto>> CreateTurnoAsignacion([FromBody] CreateTurnoAsignacionDto asignacion)
    {
        var command = new CreateTurnoAsignacionCommand(asignacion);
        var resultado = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetTurnoAsignacionById), new { id = resultado.Id }, resultado);
    }

    /// <summary>
    /// Crear múltiples asignaciones de turnos
    /// </summary>
    [HttpPost("lote")]
    public async Task<ActionResult<List<TurnoAsignacionDto>>> CreateTurnoAsignacionesLote([FromBody] List<CreateTurnoAsignacionDto> asignaciones)
    {
        var command = new CreateTurnoAsignacionesLoteCommand(asignaciones);
        var resultado = await _mediator.Send(command);
        return Ok(resultado);
    }

    /// <summary>
    /// Actualizar asignación de turno
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<TurnoAsignacionDto>> UpdateTurnoAsignacion(int id, [FromBody] CreateTurnoAsignacionDto asignacion)
    {
        var command = new UpdateTurnoAsignacionCommand(id, asignacion);
        var resultado = await _mediator.Send(command);
        return Ok(resultado);
    }

    /// <summary>
    /// Cancelar asignación de turno
    /// </summary>
    [HttpPut("{id}/cancelar")]
    public async Task<ActionResult<TurnoAsignacionDto>> CancelarTurnoAsignacion(int id, [FromBody] string motivo)
    {
        var command = new CancelarTurnoAsignacionCommand(id, motivo);
        var resultado = await _mediator.Send(command);
        return Ok(resultado);
    }

    /// <summary>
    /// Eliminar asignación de turno
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult<bool>> DeleteTurnoAsignacion(int id)
    {
        var command = new DeleteTurnoAsignacionCommand(id);
        var resultado = await _mediator.Send(command);
        return Ok(resultado);
    }

    /// <summary>
    /// Obtener calendario de turnos
    /// </summary>
    [HttpGet("calendario")]
    public async Task<ActionResult<List<CalendarioTurnosDto>>> GetCalendarioTurnos([FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin, [FromQuery] int? estacionId = null)
    {
        var query = new GetCalendarioTurnosQuery(fechaInicio, fechaFin, estacionId);
        var resultado = await _mediator.Send(query);
        return Ok(resultado);
    }

    /// <summary>
    /// Obtener turnos de un empleado
    /// </summary>
    [HttpGet("empleado/{empleadoId}")]
    public async Task<ActionResult<List<TurnoAsignacionDto>>> GetTurnosEmpleado(int empleadoId, [FromQuery] DateTime? fechaInicio = null, [FromQuery] DateTime? fechaFin = null)
    {
        var query = new GetTurnosEmpleadoQuery(empleadoId, fechaInicio, fechaFin);
        var resultado = await _mediator.Send(query);
        return Ok(resultado);
    }

    /// <summary>
    /// Obtener turnos de una estación
    /// </summary>
    [HttpGet("estacion/{estacionId}")]
    public async Task<ActionResult<List<TurnoAsignacionDto>>> GetTurnosEstacion(int estacionId, [FromQuery] DateTime? fecha = null)
    {
        var query = new GetTurnosEstacionQuery(estacionId, fecha);
        var resultado = await _mediator.Send(query);
        return Ok(resultado);
    }
}
