using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaPeaje.Application.Features.Transacciones.Commands;
using SistemaPeaje.Application.Features.Transacciones.Queries;

namespace SistemaPeaje.API.Controllers;

[ApiController]
[Route("api/[controller]")]
//[Authorize] // Comentado temporalmente para desarrollo
public class TransaccionesController : ControllerBase
{
    private readonly IMediator _mediator;

    public TransaccionesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetTransacciones([FromQuery] GetTransaccionesQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTransaccionById(int id)
    {
        var query = new GetTransaccionByIdQuery { Id = id };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTransaccion(CreateTransaccionCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetTransaccionById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTransaccion(int id, UpdateTransaccionCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest();
        }

        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTransaccion(int id)
    {
        var command = new DeleteTransaccionCommand { Id = id };
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpGet("resumen-diario")]
    public async Task<IActionResult> GetResumenDiario([FromQuery] DateTime fecha)
    {
        var query = new GetResumenDiarioQuery { Fecha = fecha };
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
