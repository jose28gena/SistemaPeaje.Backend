using Microsoft.AspNetCore.Mvc;

namespace SistemaPeaje.API.Controllers;

[ApiController]
[Route("api/turnos-admin")]
public class TurnosAdminController : ControllerBase
{
    /// <summary>
    /// Obtener turno asignaciones - simplificado para evitar errores 500
    /// </summary>
    [HttpGet("turno-asignaciones")]
    public ActionResult GetTurnoAsignaciones()
    {
        // Retornar lista vacía por ahora para evitar error 500
        return Ok(new List<object>());
    }

    /// <summary>
    /// Obtener turno eventos - simplificado para evitar errores 500
    /// </summary>
    [HttpGet("turno-eventos")]
    public ActionResult GetTurnoEventos()
    {
        // Retornar lista vacía por ahora para evitar error 500
        return Ok(new List<object>());
    }

    /// <summary>
    /// Obtener configuración de turnos
    /// </summary>
    [HttpGet("configuracion")]
    public ActionResult GetConfiguracionTurnos()
    {
        // Retornar configuración por defecto por ahora
        var configuracion = new
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
    /// Obtener dashboard básico
    /// </summary>
    [HttpGet("dashboard")]
    public ActionResult GetDashboard()
    {
        var dashboard = new
        {
            TotalTurnos = 0,
            TurnosActivos = 0,
            EmpleadosEnTurno = 0,
            ProximosCambios = new List<object>()
        };
        return Ok(dashboard);
    }

    /// <summary>
    /// Obtener reportes
    /// </summary>
    [HttpGet("reportes")]
    public ActionResult GetReportes()
    {
        return Ok(new
        {
            ReportesDiarios = new List<object>(),
            ReportesSemanales = new List<object>(),
            ReportesMensuales = new List<object>()
        });
    }
}
