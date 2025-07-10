using MediatR;
using Microsoft.AspNetCore.Mvc;
using SistemaPeaje.Application.Features.Turnos;
using SistemaPeaje.Application.Mappings;

namespace SistemaPeaje.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TurnosController : ControllerBase
{
    private readonly IMediator _mediator;

    public TurnosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtiene turnos con filtros opcionales
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<TurnoDto>>> GetTurnos(
        [FromQuery] int? empleadoId = null,
        [FromQuery] int? estacionId = null,
        [FromQuery] string? estado = null)
    {
        var query = new GetTurnosQuery
        {
            EmpleadoId = empleadoId,
            EstacionId = estacionId,
            Estado = estado
        };

        var turnos = await _mediator.Send(query);
        return Ok(turnos);
    }

    /// <summary>
    /// Obtiene los turnos activos (abiertos)
    /// </summary>
    [HttpGet("activos")]
    public async Task<ActionResult<List<TurnoDto>>> GetTurnosActivos()
    {
        var query = new GetTurnosQuery { Estado = "Abierto" };
        var turnos = await _mediator.Send(query);
        return Ok(turnos);
    }

    /// <summary>
    /// Abre un nuevo turno
    /// </summary>
    [HttpPost("abrir")]
    public async Task<ActionResult<TurnoDto>> AbrirTurno(AbrirTurnoCommand command)
    {
        try
        {
            var turno = await _mediator.Send(command);
            return Ok(turno);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Cierra un turno existente
    /// </summary>
    [HttpPost("{id}/cerrar")]
    public async Task<ActionResult<TurnoDto>> CerrarTurno(int id, [FromBody] CerrarTurnoRequest request)
    {
        try
        {
            var command = new CerrarTurnoCommand
            {
                TurnoId = id,
                MontoFinalCaja = request.MontoFinalCaja
            };

            var turno = await _mediator.Send(command);
            return Ok(turno);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene el resumen de un turno (transacciones, totales, etc.)
    /// </summary>
    [HttpGet("{id}/resumen")]
    public async Task<ActionResult<ResumenTurnoDto>> GetResumenTurno(int id)
    {
        var turno = await _mediator.Send(new GetTurnosQuery());
        var turnoEspecifico = turno.FirstOrDefault(t => t.Id == id);

        if (turnoEspecifico == null)
            return NotFound($"Turno con ID {id} no encontrado");

        // Aquí podrías agregar lógica para obtener las transacciones del turno
        // y calcular totales, estadísticas, etc.
        var resumen = new ResumenTurnoDto
        {
            Turno = turnoEspecifico,
            TotalTransacciones = 0, // Implementar lógica
            MontoTotalRecaudado = 0, // Implementar lógica
            TransaccionesPorTipoPago = new Dictionary<string, int>(), // Implementar lógica
            MontosPorTipoPago = new Dictionary<string, decimal>() // Implementar lógica
        };

        return Ok(resumen);
    }
}

// DTOs auxiliares
public class CerrarTurnoRequest
{
    public decimal MontoFinalCaja { get; set; }
}

public class ResumenTurnoDto
{
    public TurnoDto Turno { get; set; } = new();
    public int TotalTransacciones { get; set; }
    public decimal MontoTotalRecaudado { get; set; }
    public Dictionary<string, int> TransaccionesPorTipoPago { get; set; } = new();
    public Dictionary<string, decimal> MontosPorTipoPago { get; set; } = new();
}
