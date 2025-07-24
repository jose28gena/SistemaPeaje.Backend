using MediatR;
using Microsoft.AspNetCore.Mvc;
using SistemaPeaje.Application.Features.TurnosAdmin.RegistroTiempo;
using SistemaPeaje.Application.DTOs.TurnosAdmin;

namespace SistemaPeaje.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RegistroTiempoController : ControllerBase
{
    private readonly IMediator _mediator;

    public RegistroTiempoController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Registrar entrada de empleado
    /// </summary>
    [HttpPost("entrada")]
    public async Task<ActionResult<RegistroTiempoDto>> RegistrarEntrada([FromBody] RegistrarEntradaCommand command)
    {
        var resultado = await _mediator.Send(command);
        return Ok(resultado);
    }

    /// <summary>
    /// Registrar salida de empleado
    /// </summary>
    [HttpPost("salida")]
    public async Task<ActionResult<RegistroTiempoDto>> RegistrarSalida([FromBody] RegistrarSalidaCommand command)
    {
        var resultado = await _mediator.Send(command);
        return Ok(resultado);
    }

    /// <summary>
    /// Registrar inicio de descanso
    /// </summary>
    [HttpPost("inicio-descanso")]
    public async Task<ActionResult<RegistroTiempoDto>> RegistrarInicioDescanso([FromBody] RegistrarInicioDescansoCommand command)
    {
        var resultado = await _mediator.Send(command);
        return Ok(resultado);
    }

    /// <summary>
    /// Registrar fin de descanso
    /// </summary>
    [HttpPost("fin-descanso")]
    public async Task<ActionResult<RegistroTiempoDto>> RegistrarFinDescanso([FromBody] RegistrarFinDescansoCommand command)
    {
        var resultado = await _mediator.Send(command);
        return Ok(resultado);
    }

    /// <summary>
    /// Obtener registros de tiempo
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<RegistroTiempoDto>>> GetRegistrosTiempo([FromQuery] int? empleadoId = null, [FromQuery] DateTime? fecha = null)
    {
        var query = new GetRegistrosTiempoQuery(empleadoId, fecha);
        var resultado = await _mediator.Send(query);
        return Ok(resultado);
    }

    /// <summary>
    /// Obtener resumen de asistencia
    /// </summary>
    [HttpGet("resumen-asistencia")]
    public async Task<ActionResult<List<ResumenAsistenciaDto>>> GetResumenAsistencia([FromQuery] int? empleadoId = null, [FromQuery] DateTime? fechaInicio = null, [FromQuery] DateTime? fechaFin = null)
    {
        var query = new GetResumenAsistenciaQuery(empleadoId, fechaInicio, fechaFin);
        var resultado = await _mediator.Send(query);
        return Ok(resultado);
    }

    /// <summary>
    /// Corregir registro de tiempo
    /// </summary>
    [HttpPut("{id}/corregir")]
    public async Task<ActionResult<RegistroTiempoDto>> CorregirRegistroTiempo(int id, [FromBody] CorregirRegistroTiempoCommand command)
    {
        var resultado = await _mediator.Send(command);
        return Ok(resultado);
    }

    /// <summary>
    /// Obtener estado actual del empleado
    /// </summary>
    [HttpGet("estado-actual/{empleadoId}")]
    public async Task<ActionResult<EstadoActualEmpleadoDto>> GetEstadoActualEmpleado(int empleadoId)
    {
        var query = new GetEstadoActualEmpleadoQuery(empleadoId);
        var resultado = await _mediator.Send(query);
        return Ok(resultado);
    }

    /// <summary>
    /// Obtener reporte de horas trabajadas
    /// </summary>
    [HttpGet("reporte-horas")]
    public async Task<ActionResult<List<ReporteHorasTrabajadasDto>>> GetReporteHorasTrabajadas([FromQuery] int? empleadoId = null, [FromQuery] DateTime? fechaInicio = null, [FromQuery] DateTime? fechaFin = null)
    {
        var query = new GetReporteHorasTrabajadasQuery(empleadoId, fechaInicio, fechaFin);
        var resultado = await _mediator.Send(query);
        return Ok(resultado);
    }
}
