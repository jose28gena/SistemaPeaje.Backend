using MediatR;
using Microsoft.AspNetCore.Mvc;
using SistemaPeaje.Application.Features.Usuarios.Commands;
using SistemaPeaje.Application.Features.Usuarios.Queries;
using SistemaPeaje.Application.Mappings;

namespace SistemaPeaje.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsuariosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<UsuarioDto>>> GetUsuarios()
    {
        var usuarios = await _mediator.Send(new GetUsuariosQuery());
        return Ok(usuarios);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UsuarioDto>> GetUsuario(int id)
    {
        var usuario = await _mediator.Send(new GetUsuarioByIdQuery(id));
        return usuario != null ? Ok(usuario) : NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<UsuarioDto>> CreateUsuario(CreateUsuarioCommand command)
    {
        var usuario = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetUsuario), new { id = usuario.Id }, usuario);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<UsuarioDto>> UpdateUsuario(int id, UpdateUsuarioCommand command)
    {
        if (id != command.Id)
            return BadRequest();

        var usuario = await _mediator.Send(command);
        return Ok(usuario);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUsuario(int id)
    {
        await _mediator.Send(new DeleteUsuarioCommand(id));
        return NoContent();
    }

    [HttpPost("{id}/cambiar-password")]
    public async Task<IActionResult> CambiarPassword(int id, CambiarPasswordCommand command)
    {
        if (id != command.UsuarioId)
            return BadRequest();

        await _mediator.Send(command);
        return NoContent();
    }

    [HttpGet("por-rol/{rol}")]
    public async Task<ActionResult<List<UsuarioDto>>> GetUsuariosPorRol(string rol)
    {
        var usuarios = await _mediator.Send(new GetUsuariosPorRolQuery(rol));
        return Ok(usuarios);
    }
}
