using MediatR;
using Microsoft.AspNetCore.Mvc;
using SistemaPeaje.Application.Features.TurnosAdmin.TurnoTemplates;
using SistemaPeaje.Application.DTOs.TurnosAdmin;

namespace SistemaPeaje.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TurnoTemplatesController : ControllerBase
{
    private readonly IMediator _mediator;

    public TurnoTemplatesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtiene todas las plantillas de turnos
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<TurnoTemplateDto>>> GetTurnoTemplates([FromQuery] bool? soloActivos = null)
    {
        var query = new GetTurnoTemplatesQuery { SoloActivos = soloActivos };
        var templates = await _mediator.Send(query);
        return Ok(templates);
    }

    /// <summary>
    /// Obtiene una plantilla de turno por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<TurnoTemplateDto>> GetTurnoTemplate(int id)
    {
        var query = new GetTurnoTemplateByIdQuery { Id = id };
        var template = await _mediator.Send(query);
        
        if (template == null)
            return NotFound($"Plantilla de turno con ID {id} no encontrada");
            
        return Ok(template);
    }

    /// <summary>
    /// Crea una nueva plantilla de turno
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<TurnoTemplateDto>> CreateTurnoTemplate(CreateTurnoTemplateCommand command)
    {
        try
        {
            var template = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetTurnoTemplate), new { id = template.Id }, template);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Actualiza una plantilla de turno existente
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<TurnoTemplateDto>> UpdateTurnoTemplate(int id, UpdateTurnoTemplateCommand command)
    {
        try
        {
            command.Id = id;
            var template = await _mediator.Send(command);
            return Ok(template);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Elimina (desactiva) una plantilla de turno
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTurnoTemplate(int id)
    {
        try
        {
            await _mediator.Send(new DeleteTurnoTemplateCommand { Id = id });
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene plantillas de turnos aplicables para un día específico
    /// </summary>
    [HttpGet("aplicables")]
    public async Task<ActionResult<List<TurnoTemplateDto>>> GetTurnoTemplatesAplicables([FromQuery] DayOfWeek diaSemana)
    {
        var query = new GetTurnoTemplatesAplicablesQuery { DiaSemana = diaSemana };
        var templates = await _mediator.Send(query);
        return Ok(templates);
    }

    /// <summary>
    /// Duplica una plantilla de turno existente
    /// </summary>
    [HttpPost("{id}/duplicar")]
    public async Task<ActionResult<TurnoTemplateDto>> DuplicarTurnoTemplate(int id, [FromBody] DuplicarTurnoTemplateRequest request)
    {
        try
        {
            var command = new DuplicarTurnoTemplateCommand 
            { 
                TemplateIdOriginal = id, 
                NuevoNombre = request.NuevoNombre 
            };
            var nuevoTemplate = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetTurnoTemplate), new { id = nuevoTemplate.Id }, nuevoTemplate);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }
}

// DTOs auxiliares
public class DuplicarTurnoTemplateRequest
{
    public string NuevoNombre { get; set; } = string.Empty;
}
