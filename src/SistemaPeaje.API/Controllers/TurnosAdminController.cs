using MediatR;
using Microsoft.AspNetCore.Mvc;
using SistemaPeaje.Application.Features.TurnosAdmin.TurnoTemplates;
using SistemaPeaje.Application.DTOs.TurnosAdmin;

namespace SistemaPeaje.API.Controllers;

[ApiController]
[Route("api/turnos-admin")]
public class TurnosAdminController : ControllerBase
{
    private readonly IMediator _mediator;

    public TurnosAdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtener configuración de turnos
    /// </summary>
    [HttpGet("configuracion")]
    public async Task<ActionResult<ConfiguracionTurnosDto>> GetConfiguracionTurnos()
    {
        // Retornar configuración por defecto por ahora
        var configuracion = new ConfiguracionTurnosDto
        {
            MaxHorasSemanales = 40,
            MinDescansoEntreTurnos = 8,
            PermitirHorasExtras = true,
            MaxHorasExtrasDiarias = 4,
            RequiereAprobacionCambios = true
        };
        return Ok(configuracion);
    }

    /// <summary>
    /// Actualizar configuración de turnos
    /// </summary>
    [HttpPut("configuracion")]
    public async Task<ActionResult<ConfiguracionTurnosDto>> UpdateConfiguracionTurnos([FromBody] UpdateConfiguracionTurnosCommand command)
    {
        var resultado = await _mediator.Send(command);
        return Ok(resultado);
    }

    /// <summary>
    /// Obtener dashboard básico
    /// </summary>
    [HttpGet("dashboard")]
    public async Task<ActionResult<object>> GetDashboard()
    {
        var dashboard = new
        {
            TotalEmpleados = 0,
            TurnosActivos = 0,
            EventosPendientes = 0,
            HorasTrabajadasHoy = 0
        };
        return Ok(dashboard);
    }
}
