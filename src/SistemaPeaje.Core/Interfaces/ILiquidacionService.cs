using SistemaPeaje.Core.Entities;

namespace SistemaPeaje.Core.Interfaces;

/// <summary>
/// Interfaz para el servicio de liquidaciones
/// </summary>
public interface ILiquidacionService
{
    /// <summary>
    /// Genera una liquidación de cajero receptor
    /// </summary>
    Task<Liquidacion> GenerarLiquidacionCajeroAsync(int empleadoId, int estacionId, DateTime fechaInicio, DateTime fechaFin);
    
    /// <summary>
    /// Genera una liquidación de turno
    /// </summary>
    Task<Liquidacion> GenerarLiquidacionTurnoAsync(int turnoId);
    
    /// <summary>
    /// Genera una liquidación de día
    /// </summary>
    Task<Liquidacion> GenerarLiquidacionDiaAsync(int? estacionId, DateTime fecha);
    
    /// <summary>
    /// Aprueba una liquidación
    /// </summary>
    Task<Liquidacion> AprobarLiquidacionAsync(int liquidacionId, int empleadoId, string? notas = null);
    
    /// <summary>
    /// Rechaza una liquidación
    /// </summary>
    Task<Liquidacion> RechazarLiquidacionAsync(int liquidacionId, int empleadoId, string? notas = null);
    
    /// <summary>
    /// Obtiene una liquidación por ID
    /// </summary>
    Task<Liquidacion?> ObtenerLiquidacionAsync(int liquidacionId);
    
    /// <summary>
    /// Obtiene liquidaciones con filtros
    /// </summary>
    Task<IEnumerable<Liquidacion>> ObtenerLiquidacionesAsync(
        TipoLiquidacion? tipo = null,
        EstadoLiquidacion? estado = null,
        int? empleadoId = null,
        int? estacionId = null,
        DateTime? fechaDesde = null,
        DateTime? fechaHasta = null);
    
    /// <summary>
    /// Resuelve una discrepancia
    /// </summary>
    Task<LiquidacionDiscrepancia> ResolverDiscrepanciaAsync(int discrepanciaId, int empleadoId, string notasResolucion);
    
    /// <summary>
    /// Valida las condiciones previas para generar una liquidación
    /// </summary>
    Task<bool> ValidarCondicionesPrevias(TipoLiquidacion tipo, int? empleadoId = null, int? estacionId = null, DateTime? fecha = null);
    
    /// <summary>
    /// Obtiene el resumen de liquidaciones por período
    /// </summary>
    Task<object> ObtenerResumenLiquidacionesAsync(DateTime fechaInicio, DateTime fechaFin, int? estacionId = null);
}
