using MediatR;
using Microsoft.AspNetCore.Mvc;
using SistemaPeaje.Application.Features.Empleados;
using SistemaPeaje.Application.Mappings;

namespace SistemaPeaje.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmpleadosController : ControllerBase
{
    private readonly IMediator _mediator;

    public EmpleadosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtiene todos los empleados
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<EmpleadoDto>>> GetEmpleados()
    {
        var empleados = await _mediator.Send(new GetEmpleadosQuery());
        return Ok(empleados);
    }

    /// <summary>
    /// Obtiene un empleado por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<EmpleadoDto>> GetEmpleado(int id)
    {
        var empleado = await _mediator.Send(new GetEmpleadoByIdQuery(id));
        if (empleado == null)
            return NotFound($"Empleado con ID {id} no encontrado");

        return Ok(empleado);
    }

    /// <summary>
    /// Crea un nuevo empleado
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<EmpleadoDto>> CreateEmpleado(CreateEmpleadoCommand command)
    {
        var empleado = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetEmpleado), new { id = empleado.Id }, empleado);
    }

    /// <summary>
    /// Actualiza un empleado existente
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<EmpleadoDto>> UpdateEmpleado(int id, UpdateEmpleadoCommand command)
    {
        if (id != command.Id)
            return BadRequest("El ID del empleado no coincide");

        var empleado = await _mediator.Send(command);
        return Ok(empleado);
    }

    /// <summary>
    /// Elimina un empleado
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteEmpleado(int id)
    {
        var result = await _mediator.Send(new DeleteEmpleadoCommand(id));
        if (!result)
            return NotFound($"Empleado con ID {id} no encontrado");

        return NoContent();
    }

    /// <summary>
    /// Activa o desactiva un empleado
    /// </summary>
    [HttpPatch("{id}/estado")]
    public async Task<ActionResult<EmpleadoDto>> ToggleEmpleadoEstado(int id)
    {
        var empleado = await _mediator.Send(new GetEmpleadoByIdQuery(id));
        if (empleado == null)
            return NotFound($"Empleado con ID {id} no encontrado");

        var command = new UpdateEmpleadoCommand
        {
            Id = empleado.Id,
            Nombres = empleado.Nombres,
            Apellidos = empleado.Apellidos,
            NumeroDocumento = empleado.NumeroDocumento,
            Email = empleado.Email,
            Telefono = empleado.Telefono,
            Cargo = empleado.Cargo,
            EstacionId = empleado.EstacionId,
            EsActivo = !empleado.EsActivo
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
