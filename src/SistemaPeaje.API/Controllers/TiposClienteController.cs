using MediatR;
using Microsoft.AspNetCore.Mvc;
using SistemaPeaje.Application.Features.TiposCliente;
using SistemaPeaje.Application.DTOs;

namespace SistemaPeaje.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TiposClienteController : ControllerBase
{
    private readonly IMediator _mediator;

    public TiposClienteController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtiene todos los tipos de cliente (Regular, Residente, VIP, etc.)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<TipoClienteDto>>> GetTiposCliente()
    {
        var tiposCliente = await _mediator.Send(new GetTiposClienteQuery());
        return Ok(tiposCliente);
    }

    /// <summary>
    /// Obtiene un tipo de cliente por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<TipoClienteDto>> GetTipoClienteById(int id)
    {
        var tipoCliente = await _mediator.Send(new GetTipoClienteByIdQuery(id));
        if (tipoCliente == null)
            return NotFound();
        return Ok(tipoCliente);
    }

    /// <summary>
    /// Crea un nuevo tipo de cliente
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<TipoClienteDto>> CreateTipoCliente(CreateTipoClienteCommand command)
    {
        var tipoCliente = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetTipoClienteById), new { id = tipoCliente.Id }, tipoCliente);
    }

    /// <summary>
    /// Actualiza un tipo de cliente
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<TipoClienteDto>> UpdateTipoCliente(int id, UpdateTipoClienteCommand command)
    {
        if (id != command.Id)
            return BadRequest();

        var tipoCliente = await _mediator.Send(command);
        return Ok(tipoCliente);
    }

    /// <summary>
    /// Obtiene tipos de cliente activos solamente
    /// </summary>
    [HttpGet("activos")]
    public async Task<ActionResult<List<TipoClienteDto>>> GetTiposClienteActivos()
    {
        var tiposCliente = await _mediator.Send(new GetTiposClienteQuery());
        var tiposActivos = tiposCliente.Where(t => t.EsActivo).ToList();
        return Ok(tiposActivos);
    }

    /// <summary>
    /// Obtiene tipos de cliente exentos de pago
    /// </summary>
    [HttpGet("exentos")]
    public async Task<ActionResult<List<TipoClienteDto>>> GetTiposClienteExentos()
    {
        var tiposCliente = await _mediator.Send(new GetTiposClienteQuery());
        var tiposExentos = tiposCliente.Where(t => t.EstaExentoPago).ToList();
        return Ok(tiposExentos);
    }

    /// <summary>
    /// Elimina (desactiva) un tipo de cliente
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTipoCliente(int id)
    {
        await _mediator.Send(new DeleteTipoClienteCommand(id));
        return NoContent();
    }
}
