using MediatR;
using Microsoft.AspNetCore.Mvc;
using SistemaPeaje.Application.DTOs;
using SistemaPeaje.Application.Features.Liquidacion.Commands;
using SistemaPeaje.Application.Features.Liquidacion.Queries;
using SistemaPeaje.Core.Entities;

namespace SistemaPeaje.API.Controllers;

/// <summary>
/// Controller para gestión de liquidaciones
/// Maneja los tres tipos principales:
/// - Liquidación de Cajero Receptor
/// - Liquidación de Turno
/// - Liquidación de Día
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class LiquidacionController : ControllerBase
{
    private readonly IMediator _mediator;

    public LiquidacionController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtiene todas las liquidaciones con filtros opcionales
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<LiquidacionDto>>> GetLiquidaciones(
        [FromQuery] TipoLiquidacion? tipoLiquidacion = null,
        [FromQuery] EstadoLiquidacion? estado = null,
        [FromQuery] int? empleadoId = null,
        [FromQuery] int? estacionId = null,
        [FromQuery] DateTime? fechaDesde = null,
        [FromQuery] DateTime? fechaHasta = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetLiquidacionesQuery
        {
            TipoLiquidacion = tipoLiquidacion,
            Estado = estado,
            EmpleadoId = empleadoId,
            EstacionId = estacionId,
            FechaDesde = fechaDesde,
            FechaHasta = fechaHasta,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var liquidaciones = await _mediator.Send(query);
        return Ok(liquidaciones);
    }

    /// <summary>
    /// Obtiene una liquidación específica por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<LiquidacionDto>> GetLiquidacionById(int id)
    {
        var query = new GetLiquidacionByIdQuery { LiquidacionId = id };
        var liquidacion = await _mediator.Send(query);

        if (liquidacion == null)
            return NotFound($"Liquidación con ID {id} no encontrada");

        return Ok(liquidacion);
    }

    /// <summary>
    /// Obtiene liquidaciones pendientes de aprobación
    /// </summary>
    [HttpGet("pendientes")]
    public async Task<ActionResult<IEnumerable<LiquidacionDto>>> GetLiquidacionesPendientes(
        [FromQuery] int? estacionId = null,
        [FromQuery] TipoLiquidacion? tipoLiquidacion = null)
    {
        var query = new GetLiquidacionesPendientesQuery
        {
            EstacionId = estacionId,
            TipoLiquidacion = tipoLiquidacion
        };

        var liquidaciones = await _mediator.Send(query);
        return Ok(liquidaciones);
    }

    /// <summary>
    /// Obtiene resumen de liquidaciones por período
    /// </summary>
    [HttpGet("resumen")]
    public async Task<ActionResult<ResumenLiquidacionesDto>> GetResumenLiquidaciones(
        [FromQuery] DateTime fechaInicio,
        [FromQuery] DateTime fechaFin,
        [FromQuery] int? estacionId = null)
    {
        var query = new GetResumenLiquidacionesQuery
        {
            FechaInicio = fechaInicio,
            FechaFin = fechaFin,
            EstacionId = estacionId
        };

        var resumen = await _mediator.Send(query);
        return Ok(resumen);
    }

    /// <summary>
    /// Obtiene liquidaciones por empleado
    /// </summary>
    [HttpGet("empleado/{empleadoId}")]
    public async Task<ActionResult<IEnumerable<LiquidacionDto>>> GetLiquidacionesPorEmpleado(
        int empleadoId,
        [FromQuery] DateTime? fechaDesde = null,
        [FromQuery] DateTime? fechaHasta = null,
        [FromQuery] TipoLiquidacion? tipoLiquidacion = null)
    {
        var query = new GetLiquidacionesPorEmpleadoQuery
        {
            EmpleadoId = empleadoId,
            FechaDesde = fechaDesde,
            FechaHasta = fechaHasta,
            TipoLiquidacion = tipoLiquidacion
        };

        var liquidaciones = await _mediator.Send(query);
        return Ok(liquidaciones);
    }

    /// <summary>
    /// Obtiene liquidaciones por estación
    /// </summary>
    [HttpGet("estacion/{estacionId}")]
    public async Task<ActionResult<IEnumerable<LiquidacionDto>>> GetLiquidacionesPorEstacion(
        int estacionId,
        [FromQuery] DateTime? fechaDesde = null,
        [FromQuery] DateTime? fechaHasta = null,
        [FromQuery] TipoLiquidacion? tipoLiquidacion = null)
    {
        var query = new GetLiquidacionesPorEstacionQuery
        {
            EstacionId = estacionId,
            FechaDesde = fechaDesde,
            FechaHasta = fechaHasta,
            TipoLiquidacion = tipoLiquidacion
        };

        var liquidaciones = await _mediator.Send(query);
        return Ok(liquidaciones);
    }

    // ===== MÓDULO DE LIQUIDACIÓN DE CAJERO RECEPTOR =====

    /// <summary>
    /// Genera una liquidación de cajero receptor
    /// </summary>
    [HttpPost("cajero")]
    public async Task<ActionResult<LiquidacionDto>> GenerarLiquidacionCajero(
        [FromBody] GenerarLiquidacionCajeroRequest request)
    {
        var command = new GenerarLiquidacionCajeroCommand
        {
            EmpleadoId = request.EmpleadoId,
            EstacionId = request.EstacionId,
            FechaInicio = request.FechaInicio,
            FechaFin = request.FechaFin,
            CreadoPorEmpleadoId = request.CreadoPorEmpleadoId,
            Observaciones = request.Observaciones
        };

        var liquidacion = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetLiquidacionById), new { id = liquidacion.Id }, liquidacion);
    }

    /// <summary>
    /// Valida las condiciones previas para liquidación de cajero
    /// </summary>
    [HttpPost("cajero/validar")]
    public async Task<ActionResult<bool>> ValidarCondicionesCajero(
        [FromBody] ValidarCondicionesCajeroRequest request)
    {
        var query = new ValidarCondicionesPreviasQuery
        {
            TipoLiquidacion = TipoLiquidacion.Cajero,
            EmpleadoId = request.EmpleadoId,
            EstacionId = request.EstacionId
        };

        var esValido = await _mediator.Send(query);
        return Ok(new { esValido, mensaje = esValido ? "Condiciones válidas" : "Condiciones inválidas" });
    }

    // ===== MÓDULO DE LIQUIDACIÓN DE TURNO =====

    /// <summary>
    /// Genera una liquidación de turno
    /// </summary>
    [HttpPost("turno")]
    public async Task<ActionResult<LiquidacionDto>> GenerarLiquidacionTurno(
        [FromBody] GenerarLiquidacionTurnoRequest request)
    {
        var command = new GenerarLiquidacionTurnoCommand
        {
            TurnoId = request.TurnoId,
            CreadoPorEmpleadoId = request.CreadoPorEmpleadoId,
            Observaciones = request.Observaciones
        };

        var liquidacion = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetLiquidacionById), new { id = liquidacion.Id }, liquidacion);
    }

    /// <summary>
    /// Valida las condiciones previas para liquidación de turno
    /// </summary>
    [HttpPost("turno/validar")]
    public async Task<ActionResult<bool>> ValidarCondicionesTurno(
        [FromBody] ValidarCondicionesTurnoRequest request)
    {
        var query = new ValidarCondicionesPreviasQuery
        {
            TipoLiquidacion = TipoLiquidacion.Turno,
            EmpleadoId = request.EmpleadoId,
            EstacionId = request.EstacionId
        };

        var esValido = await _mediator.Send(query);
        return Ok(new { esValido, mensaje = esValido ? "Condiciones válidas" : "Condiciones inválidas" });
    }

    // ===== MÓDULO DE LIQUIDACIÓN DE DÍA =====

    /// <summary>
    /// Genera una liquidación de día
    /// </summary>
    [HttpPost("dia")]
    public async Task<ActionResult<LiquidacionDto>> GenerarLiquidacionDia(
        [FromBody] GenerarLiquidacionDiaRequest request)
    {
        var command = new GenerarLiquidacionDiaCommand
        {
            EstacionId = request.EstacionId,
            Fecha = request.Fecha,
            CreadoPorEmpleadoId = request.CreadoPorEmpleadoId,
            Observaciones = request.Observaciones
        };

        var liquidacion = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetLiquidacionById), new { id = liquidacion.Id }, liquidacion);
    }

    /// <summary>
    /// Valida las condiciones previas para liquidación de día
    /// </summary>
    [HttpPost("dia/validar")]
    public async Task<ActionResult<bool>> ValidarCondicionesDia(
        [FromBody] ValidarCondicionesDiaRequest request)
    {
        var query = new ValidarCondicionesPreviasQuery
        {
            TipoLiquidacion = TipoLiquidacion.Dia,
            Fecha = request.Fecha
        };

        var esValido = await _mediator.Send(query);
        return Ok(new { esValido, mensaje = esValido ? "Condiciones válidas" : "Condiciones inválidas" });
    }

    // ===== GESTIÓN DE APROBACIONES =====

    /// <summary>
    /// Aprueba una liquidación
    /// </summary>
    [HttpPut("{id}/aprobar")]
    public async Task<ActionResult<LiquidacionDto>> AprobarLiquidacion(
        int id,
        [FromBody] AprobarLiquidacionRequest request)
    {
        var command = new AprobarLiquidacionCommand
        {
            LiquidacionId = id,
            EmpleadoId = request.EmpleadoId,
            NotasAprobacion = request.NotasAprobacion
        };

        var liquidacion = await _mediator.Send(command);
        return Ok(liquidacion);
    }

    /// <summary>
    /// Rechaza una liquidación
    /// </summary>
    [HttpPut("{id}/rechazar")]
    public async Task<ActionResult<LiquidacionDto>> RechazarLiquidacion(
        int id,
        [FromBody] RechazarLiquidacionRequest request)
    {
        var command = new RechazarLiquidacionCommand
        {
            LiquidacionId = id,
            EmpleadoId = request.EmpleadoId,
            NotasRechazo = request.NotasRechazo
        };

        var liquidacion = await _mediator.Send(command);
        return Ok(liquidacion);
    }

    // ===== GESTIÓN DE DISCREPANCIAS =====

    /// <summary>
    /// Resuelve una discrepancia
    /// </summary>
    [HttpPut("discrepancia/{discrepanciaId}/resolver")]
    public async Task<ActionResult<LiquidacionDiscrepanciaDto>> ResolverDiscrepancia(
        int discrepanciaId,
        [FromBody] ResolverDiscrepanciaRequest request)
    {
        var command = new ResolverDiscrepanciaCommand
        {
            DiscrepanciaId = discrepanciaId,
            EmpleadoId = request.EmpleadoId,
            NotasResolucion = request.NotasResolucion
        };

        var discrepancia = await _mediator.Send(command);
        return Ok(discrepancia);
    }

    // ===== REPORTES Y ESTADÍSTICAS =====

    /// <summary>
    /// Obtiene estadísticas de liquidaciones
    /// </summary>
    [HttpGet("estadisticas")]
    public async Task<ActionResult<object>> GetEstadisticasLiquidaciones(
        [FromQuery] DateTime? fechaDesde = null,
        [FromQuery] DateTime? fechaHasta = null,
        [FromQuery] int? estacionId = null)
    {
        var fechaInicio = fechaDesde ?? DateTime.Today.AddDays(-30);
        var fechaFin = fechaHasta ?? DateTime.Today;

        var query = new GetResumenLiquidacionesQuery
        {
            FechaInicio = fechaInicio,
            FechaFin = fechaFin,
            EstacionId = estacionId
        };

        var resumen = await _mediator.Send(query);
        
        // Crear estadísticas adicionales
        var estadisticas = new
        {
            resumen.TotalLiquidaciones,
            resumen.MontoTotalLiquidado,
            resumen.LiquidacionesAprobadas,
            resumen.LiquidacionesPendientes,
            resumen.LiquidacionesRechazadas,
            resumen.TotalDiscrepancias,
            resumen.PromedioLiquidacion,
            PorcentajeAprobacion = resumen.TotalLiquidaciones > 0 ? 
                (decimal)resumen.LiquidacionesAprobadas / resumen.TotalLiquidaciones * 100 : 0,
            PorcentajeRechazo = resumen.TotalLiquidaciones > 0 ? 
                (decimal)resumen.LiquidacionesRechazadas / resumen.TotalLiquidaciones * 100 : 0,
            resumen.LiquidacionesPorTipo,
            resumen.MontosPorTipo,
            EmpleadoConMasLiquidaciones = resumen.ResumenPorEmpleado
                .OrderByDescending(e => e.TotalLiquidaciones)
                .FirstOrDefault(),
            DiscrepanciaMasFrecuente = resumen.DiscrepanciasMasComunes
                .OrderByDescending(d => d.Cantidad)
                .FirstOrDefault()
        };

        return Ok(estadisticas);
    }
}

// ===== DTOs DE REQUEST =====

/// <summary>
/// Request para generar liquidación de cajero
/// </summary>
public class GenerarLiquidacionCajeroRequest
{
    public int EmpleadoId { get; set; }
    public int EstacionId { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public int CreadoPorEmpleadoId { get; set; }
    public string? Observaciones { get; set; }
}

/// <summary>
/// Request para validar condiciones de cajero
/// </summary>
public class ValidarCondicionesCajeroRequest
{
    public int EmpleadoId { get; set; }
    public int EstacionId { get; set; }
}

/// <summary>
/// Request para generar liquidación de turno
/// </summary>
public class GenerarLiquidacionTurnoRequest
{
    public int TurnoId { get; set; }
    public int CreadoPorEmpleadoId { get; set; }
    public string? Observaciones { get; set; }
}

/// <summary>
/// Request para validar condiciones de turno
/// </summary>
public class ValidarCondicionesTurnoRequest
{
    public int EmpleadoId { get; set; }
    public int EstacionId { get; set; }
}

/// <summary>
/// Request para generar liquidación de día
/// </summary>
public class GenerarLiquidacionDiaRequest
{
    public int? EstacionId { get; set; }
    public DateTime Fecha { get; set; }
    public int CreadoPorEmpleadoId { get; set; }
    public string? Observaciones { get; set; }
}

/// <summary>
/// Request para validar condiciones de día
/// </summary>
public class ValidarCondicionesDiaRequest
{
    public DateTime Fecha { get; set; }
}

/// <summary>
/// Request para aprobar liquidación
/// </summary>
public class AprobarLiquidacionRequest
{
    public int EmpleadoId { get; set; }
    public string? NotasAprobacion { get; set; }
}

/// <summary>
/// Request para rechazar liquidación
/// </summary>
public class RechazarLiquidacionRequest
{
    public int EmpleadoId { get; set; }
    public string? NotasRechazo { get; set; }
}

/// <summary>
/// Request para resolver discrepancia
/// </summary>
public class ResolverDiscrepanciaRequest
{
    public int EmpleadoId { get; set; }
    public string NotasResolucion { get; set; } = string.Empty;
}
