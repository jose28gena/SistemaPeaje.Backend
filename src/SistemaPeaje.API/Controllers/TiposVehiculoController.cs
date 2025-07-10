using MediatR;
using Microsoft.AspNetCore.Mvc;
using SistemaPeaje.Application.Features.TiposVehiculo;
using SistemaPeaje.Application.Mappings;

namespace SistemaPeaje.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TiposVehiculoController : ControllerBase
{
    private readonly IMediator _mediator;

    public TiposVehiculoController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtiene todos los tipos de vehículo
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<TipoVehiculoDto>>> GetTiposVehiculo()
    {
        var tipos = await _mediator.Send(new GetTiposVehiculoQuery());
        return Ok(tipos);
    }

    /// <summary>
    /// Crea un nuevo tipo de vehículo
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<TipoVehiculoDto>> CreateTipoVehiculo(CreateTipoVehiculoCommand command)
    {
        var tipo = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetTiposVehiculo), tipo);
    }

    /// <summary>
    /// Obtiene tipos de vehículo activos para selección en formularios
    /// </summary>
    [HttpGet("activos")]
    public async Task<ActionResult<List<TipoVehiculoDto>>> GetTiposVehiculoActivos()
    {
        var tipos = await _mediator.Send(new GetTiposVehiculoQuery());
        var activos = tipos.Where(t => t.EsActivo).ToList();
        return Ok(activos);
    }
}
