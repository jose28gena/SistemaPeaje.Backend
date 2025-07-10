using MediatR;
using Microsoft.AspNetCore.Mvc;
using SistemaPeaje.Application.DTOs.Comandos;
using SistemaPeaje.Application.Features.ComandosPlc;
using System.Security.Claims;

namespace SistemaPeaje.API.Controllers;

[ApiController]
[Route("api/comandos")]
public class ComandosController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ComandosController> _logger;

    public ComandosController(IMediator mediator, ILogger<ComandosController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Abre la barrera de un carril específico enviando comando al PLC
    /// </summary>
    /// <param name="dto">Datos del comando para abrir barrera</param>
    /// <returns>Resultado de la ejecución del comando</returns>
    [HttpPost("abrir-barrera")]
    // [Authorize(Roles = "Operador,Admin")] // TODO: Descomentar cuando se implemente autenticación
    public async Task<IActionResult> AbrirBarrera([FromBody] ComandoAbrirBarreraDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Usuario solicita abrir barrera - IP: {IP}, Coil: {Coil}", 
                dto.CasetaIp, dto.CoilAddress);

            var command = new EjecutarComandoAbrirBarreraCommand
            {
                CasetaIp = dto.CasetaIp,
                CoilAddress = dto.CoilAddress,
                UnitId = dto.UnitId,
                CarrilId = dto.CarrilId,
                Observaciones = dto.Observaciones,
                // UsuarioId = GetCurrentUserId(), // TODO: Implementar cuando se tenga autenticación
                // EmpleadoId = GetCurrentEmpleadoId()
            };

            var resultado = await _mediator.Send(command);

            if (resultado.Exitoso)
            {
                return Ok(resultado);
            }
            else
            {
                return StatusCode(500, resultado);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error no controlado en AbrirBarrera");
            return StatusCode(500, new ResultadoComandoDto
            {
                Exitoso = false,
                Mensaje = "❌ Error interno del servidor",
                CodigoError = "INTERNAL_ERROR"
            });
        }
    }

    /// <summary>
    /// Cierra la barrera de un carril específico enviando comando al PLC
    /// </summary>
    /// <param name="dto">Datos del comando para cerrar barrera</param>
    /// <returns>Resultado de la ejecución del comando</returns>
    [HttpPost("cerrar-barrera")]
    // [Authorize(Roles = "Operador,Admin")]
    public async Task<IActionResult> CerrarBarrera([FromBody] ComandoCerrarBarreraDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Usuario solicita cerrar barrera - IP: {IP}, Coil: {Coil}", 
                dto.CasetaIp, dto.CoilAddress);

            var command = new EjecutarComandoCerrarBarreraCommand
            {
                CasetaIp = dto.CasetaIp,
                CoilAddress = dto.CoilAddress,
                UnitId = dto.UnitId,
                CarrilId = dto.CarrilId,
                Observaciones = dto.Observaciones,
                // UsuarioId = GetCurrentUserId(),
                // EmpleadoId = GetCurrentEmpleadoId()
            };

            var resultado = await _mediator.Send(command);

            if (resultado.Exitoso)
            {
                return Ok(resultado);
            }
            else
            {
                return StatusCode(500, resultado);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error no controlado en CerrarBarrera");
            return StatusCode(500, new ResultadoComandoDto
            {
                Exitoso = false,
                Mensaje = "❌ Error interno del servidor",
                CodigoError = "INTERNAL_ERROR"
            });
        }
    }

    /// <summary>
    /// Realiza un test de conexión con el PLC para verificar conectividad
    /// </summary>
    /// <param name="dto">Datos de conexión para el test</param>
    /// <returns>Resultado del test de conexión</returns>
    [HttpPost("test-conexion")]
    // [Authorize(Roles = "Operador,Admin,Tecnico")]
    public async Task<IActionResult> TestConexion([FromBody] ComandoTestConexionDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Usuario solicita test de conexión - IP: {IP}:{Puerto}", 
                dto.CasetaIp, dto.Puerto);

            var command = new EjecutarTestConexionCommand
            {
                CasetaIp = dto.CasetaIp,
                Puerto = dto.Puerto,
                UnitId = dto.UnitId,
                // UsuarioId = GetCurrentUserId(),
                // EmpleadoId = GetCurrentEmpleadoId()
            };

            var resultado = await _mediator.Send(command);

            return Ok(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error no controlado en TestConexion");
            return StatusCode(500, new ResultadoComandoDto
            {
                Exitoso = false,
                Mensaje = "❌ Error interno del servidor",
                CodigoError = "INTERNAL_ERROR"
            });
        }
    }

    /// <summary>
    /// Obtiene el historial de comandos ejecutados con filtros opcionales
    /// </summary>
    /// <param name="ip">Filtrar por IP del PLC</param>
    /// <param name="tipoComando">Filtrar por tipo de comando</param>
    /// <param name="fechaDesde">Fecha desde (formato: yyyy-MM-dd)</param>
    /// <param name="fechaHasta">Fecha hasta (formato: yyyy-MM-dd)</param>
    /// <param name="page">Número de página (default: 1)</param>
    /// <param name="pageSize">Tamaño de página (default: 50, max: 200)</param>
    /// <returns>Lista paginada de comandos ejecutados</returns>
    [HttpGet("historial")]
    // [Authorize(Roles = "Admin,Supervisor")]
    public async Task<IActionResult> GetHistorialComandos(
        [FromQuery] string? ip = null,
        [FromQuery] string? tipoComando = null,
        [FromQuery] DateTime? fechaDesde = null,
        [FromQuery] DateTime? fechaHasta = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        try
        {
            // TODO: Implementar query para obtener historial con filtros y paginación
            _logger.LogInformation("Consultando historial de comandos PLC");
            
            await Task.CompletedTask; // Fix async warning
            
            return Ok(new { 
                Message = "Endpoint en construcción - Historial de comandos PLC",
                Filters = new { ip, tipoComando, fechaDesde, fechaHasta, page, pageSize }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al consultar historial de comandos");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    // Métodos helper para obtener datos del usuario autenticado
    // TODO: Implementar cuando se tenga el sistema de autenticación completo
    /*
    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
        {
            return userId;
        }
        return null;
    }

    private int? GetCurrentEmpleadoId()
    {
        var empleadoIdClaim = User.FindFirst("EmpleadoId");
        if (empleadoIdClaim != null && int.TryParse(empleadoIdClaim.Value, out int empleadoId))
        {
            return empleadoId;
        }
        return null;
    }
    */
}
