using MediatR;
using Microsoft.AspNetCore.Mvc;
using SistemaPeaje.Application.Features.Carriles;
using SistemaPeaje.Application.Mappings;

namespace SistemaPeaje.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CarrilesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CarrilesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtiene todos los carriles
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<CarrilDto>>> GetCarriles()
    {
        var carriles = await _mediator.Send(new GetCarrilesQuery());
        return Ok(carriles);
    }

    /// <summary>
    /// Obtiene un carril por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<CarrilDto>> GetCarril(int id)
    {
        var carril = await _mediator.Send(new GetCarrilByIdQuery(id));
        if (carril == null)
            return NotFound($"Carril con ID {id} no encontrado");

        return Ok(carril);
    }

    /// <summary>
    /// Crea un nuevo carril
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<CarrilDto>> CreateCarril(CreateCarrilCommand command)
    {
        var carril = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetCarril), new { id = carril.Id }, carril);
    }

    /// <summary>
    /// Actualiza un carril existente
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<CarrilDto>> UpdateCarril(int id, UpdateCarrilCommand command)
    {
        if (id != command.Id)
            return BadRequest("El ID del carril no coincide");

        var carril = await _mediator.Send(command);
        return Ok(carril);
    }

    /// <summary>
    /// Elimina un carril
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCarril(int id)
    {
        var result = await _mediator.Send(new DeleteCarrilCommand(id));
        if (!result)
            return NotFound($"Carril con ID {id} no encontrado");

        return NoContent();
    }
}
