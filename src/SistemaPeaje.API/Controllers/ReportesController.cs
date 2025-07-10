using MediatR;
using Microsoft.AspNetCore.Mvc;
using SistemaPeaje.Application.Features.Reportes;

namespace SistemaPeaje.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReportesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Genera reporte de operación con transacciones, turnos y estadísticas
    /// </summary>
    [HttpGet("operacion")]
    public async Task<ActionResult<ReporteOperacionDto>> GetReporteOperacion(
        [FromQuery] DateTime fechaInicio,
        [FromQuery] DateTime fechaFin,
        [FromQuery] int? estacionId = null)
    {
        var query = new GetReporteOperacionQuery
        {
            FechaInicio = fechaInicio,
            FechaFin = fechaFin,
            EstacionId = estacionId
        };

        var reporte = await _mediator.Send(query);
        return Ok(reporte);
    }

    /// <summary>
    /// Genera reporte de tráfico con volúmenes y aforo vehicular
    /// </summary>
    [HttpGet("trafico")]
    public async Task<ActionResult<ReporteTraficoDto>> GetReporteTrafico(
        [FromQuery] DateTime fechaInicio,
        [FromQuery] DateTime fechaFin,
        [FromQuery] int? estacionId = null)
    {
        var query = new GetReporteTraficoQuery
        {
            FechaInicio = fechaInicio,
            FechaFin = fechaFin,
            EstacionId = estacionId
        };

        var reporte = await _mediator.Send(query);
        return Ok(reporte);
    }

    /// <summary>
    /// Genera reporte de ingresos y recaudación
    /// </summary>
    [HttpGet("ingresos")]
    public async Task<ActionResult<ReporteIngresosDto>> GetReporteIngresos(
        [FromQuery] DateTime fechaInicio,
        [FromQuery] DateTime fechaFin,
        [FromQuery] int? estacionId = null)
    {
        // Reutilizar la lógica del reporte de operación para ingresos
        var operacionQuery = new GetReporteOperacionQuery
        {
            FechaInicio = fechaInicio,
            FechaFin = fechaFin,
            EstacionId = estacionId
        };

        var reporteOperacion = await _mediator.Send(operacionQuery);
        
        var reporteIngresos = new ReporteIngresosDto
        {
            FechaInicio = fechaInicio,
            FechaFin = fechaFin,
            EstacionId = estacionId,
            IngresoTotal = reporteOperacion.MontoTotalRecaudado,
            IngresoPromedioDiario = CalcularIngresoDiario(reporteOperacion.MontoTotalRecaudado, fechaInicio, fechaFin),
            IngresosPorTipoPago = reporteOperacion.MontosPorTipoPago,
            TransaccionPromedio = reporteOperacion.TotalTransacciones > 0 ? 
                reporteOperacion.MontoTotalRecaudado / reporteOperacion.TotalTransacciones : 0,
            MetaPorcentaje = CalcularCumplimientoMeta(reporteOperacion.MontoTotalRecaudado),
            FechaGeneracion = DateTime.UtcNow
        };

        return Ok(reporteIngresos);
    }

    /// <summary>
    /// Genera reporte ejecutivo con KPIs y métricas clave
    /// </summary>
    [HttpGet("ejecutivo")]
    public async Task<ActionResult<ReporteEjecutivoDto>> GetReporteEjecutivo(
        [FromQuery] DateTime fechaInicio,
        [FromQuery] DateTime fechaFin,
        [FromQuery] int? estacionId = null)
    {
        var operacionQuery = new GetReporteOperacionQuery
        {
            FechaInicio = fechaInicio,
            FechaFin = fechaFin,
            EstacionId = estacionId
        };

        var traficoQuery = new GetReporteTraficoQuery
        {
            FechaInicio = fechaInicio,
            FechaFin = fechaFin,
            EstacionId = estacionId
        };

        var reporteOperacion = await _mediator.Send(operacionQuery);
        var reporteTrafico = await _mediator.Send(traficoQuery);

        var reporteEjecutivo = new ReporteEjecutivoDto
        {
            FechaInicio = fechaInicio,
            FechaFin = fechaFin,
            EstacionId = estacionId,
            KPIs = new KPIsDto
            {
                TotalTransacciones = reporteOperacion.TotalTransacciones,
                IngresoTotal = reporteOperacion.MontoTotalRecaudado,
                TotalVehiculos = reporteTrafico.TotalVehiculos,
                PromedioTransaccionesPorHora = reporteOperacion.PromedioTransaccionesPorHora,
                PromedioVehiculosPorDia = reporteTrafico.PromedioVehiculosPorDia,
                TasaEficiencia = CalcularTasaEficiencia(reporteOperacion),
                IndiceCumplimiento = CalcularCumplimientoMeta(reporteOperacion.MontoTotalRecaudado)
            },
            TendenciasTrafico = reporteTrafico.VolumenPorHora.Take(24).ToList(),
            DistribucionIngresos = reporteOperacion.MontosPorTipoPago,
            AlertasOperativas = GenerarAlertas(reporteOperacion, reporteTrafico),
            FechaGeneracion = DateTime.UtcNow
        };

        return Ok(reporteEjecutivo);
    }

    /// <summary>
    /// Exporta reporte en formato CSV
    /// </summary>
    [HttpGet("exportar/csv")]
    public async Task<IActionResult> ExportarReporteCSV(
        [FromQuery] string tipoReporte,
        [FromQuery] DateTime fechaInicio,
        [FromQuery] DateTime fechaFin,
        [FromQuery] int? estacionId = null)
    {
        // Implementar lógica de exportación a CSV
        var csvContent = "Fecha,Tipo,Monto,Estacion\n"; // Ejemplo básico
        
        var fileName = $"reporte_{tipoReporte}_{fechaInicio:yyyyMMdd}_{fechaFin:yyyyMMdd}.csv";
        
        return File(System.Text.Encoding.UTF8.GetBytes(csvContent), "text/csv", fileName);
    }

    // Métodos auxiliares
    private static decimal CalcularIngresoDiario(decimal montoTotal, DateTime fechaInicio, DateTime fechaFin)
    {
        var dias = (decimal)(fechaFin.Date - fechaInicio.Date).TotalDays + 1;
        return dias > 0 ? montoTotal / dias : 0;
    }

    private static decimal CalcularCumplimientoMeta(decimal montoActual)
    {
        // Meta ejemplo: $1,000,000 por mes
        var metaMensual = 1000000m;
        return (montoActual / metaMensual) * 100;
    }

    private static decimal CalcularTasaEficiencia(ReporteOperacionDto reporte)
    {
        // Eficiencia basada en turnos cerrados vs abiertos
        return reporte.TurnosRegistrados > 0 ? 
            (decimal)reporte.TurnosCerrados / reporte.TurnosRegistrados * 100 : 0;
    }

    private static List<string> GenerarAlertas(ReporteOperacionDto operacion, ReporteTraficoDto trafico)
    {
        var alertas = new List<string>();

        if (operacion.MontoTotalRecaudado < 50000) // Ejemplo de umbral
            alertas.Add("Ingresos por debajo del promedio esperado");

        if (trafico.TotalVehiculos < 100) // Ejemplo de umbral
            alertas.Add("Volumen de tráfico bajo");

        if (operacion.TurnosCerrados < operacion.TurnosRegistrados)
            alertas.Add("Turnos sin cerrar detectados");

        return alertas;
    }
}
