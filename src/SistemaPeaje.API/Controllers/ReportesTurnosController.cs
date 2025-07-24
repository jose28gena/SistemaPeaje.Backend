using MediatR;
using Microsoft.AspNetCore.Mvc;
using SistemaPeaje.Application.Features.TurnosAdmin.Reportes;
using SistemaPeaje.Application.DTOs.TurnosAdmin;

namespace SistemaPeaje.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportesTurnosController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReportesTurnosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtener reporte de productividad de empleados
    /// </summary>
    [HttpGet("productividad-empleados")]
    public async Task<ActionResult<List<ReporteProductividadEmpleadosDto>>> GetReporteProductividadEmpleados([FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin)
    {
        var query = new GetReporteProductividadEmpleadosQuery(fechaInicio, fechaFin);
        var resultado = await _mediator.Send(query);
        return Ok(resultado);
    }

    /// <summary>
    /// Obtener reporte de rendimiento de turnos
    /// </summary>
    [HttpGet("rendimiento-turnos")]
    public async Task<ActionResult<List<ReporteRendimientoTurnosDto>>> GetReporteRendimientoTurnos([FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin)
    {
        var query = new GetReporteRendimientoTurnosQuery(fechaInicio, fechaFin);
        var resultado = await _mediator.Send(query);
        return Ok(resultado);
    }

    /// <summary>
    /// Obtener reporte de asistencia y puntualidad
    /// </summary>
    [HttpGet("asistencia-puntualidad")]
    public async Task<ActionResult<List<ReporteAsistenciaPuntualidadDto>>> GetReporteAsistenciaPuntualidad([FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin, [FromQuery] int? empleadoId = null)
    {
        var query = new GetReporteAsistenciaPuntualidadQuery(fechaInicio, fechaFin, empleadoId);
        var resultado = await _mediator.Send(query);
        return Ok(resultado);
    }

    /// <summary>
    /// Obtener reporte de horas extras
    /// </summary>
    [HttpGet("horas-extras")]
    public async Task<ActionResult<List<ReporteHorasExtrasDto>>> GetReporteHorasExtras([FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin, [FromQuery] int? empleadoId = null)
    {
        var query = new GetReporteHorasExtrasQuery(fechaInicio, fechaFin, empleadoId);
        var resultado = await _mediator.Send(query);
        return Ok(resultado);
    }

    /// <summary>
    /// Obtener reporte de costos laborales
    /// </summary>
    [HttpGet("costos-laborales")]
    public async Task<ActionResult<ReporteCostosLaboralesDto>> GetReporteCostosLaborales([FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin)
    {
        var query = new GetReporteCostosLaboralesQuery(fechaInicio, fechaFin);
        var resultado = await _mediator.Send(query);
        return Ok(resultado);
    }

    /// <summary>
    /// Obtener dashboard ejecutivo
    /// </summary>
    [HttpGet("dashboard-ejecutivo")]
    public async Task<ActionResult<DashboardEjecutivoDto>> GetDashboardEjecutivo()
    {
        var query = new GetDashboardEjecutivoQuery();
        var resultado = await _mediator.Send(query);
        return Ok(resultado);
    }

    /// <summary>
    /// Obtener métricas en tiempo real
    /// </summary>
    [HttpGet("metricas-tiempo-real")]
    public async Task<ActionResult<MetricasTiempoRealDto>> GetMetricasTiempoReal()
    {
        var query = new GetMetricasTiempoRealQuery();
        var resultado = await _mediator.Send(query);
        return Ok(resultado);
    }
}
