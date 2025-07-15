using SistemaPeaje.Core.Entities;

namespace SistemaPeaje.Application.DTOs;

/// <summary>
/// DTO para la liquidación
/// </summary>
public class LiquidacionDto
{
    public int Id { get; set; }
    public Guid NumeroLiquidacion { get; set; }
    public string TipoLiquidacion { get; set; } = string.Empty;
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public DateTime FechaGeneracion { get; set; }
    
    // Relaciones
    public int? EstacionId { get; set; }
    public string? EstacionNombre { get; set; }
    public int? EmpleadoId { get; set; }
    public string? EmpleadoNombre { get; set; }
    public int? TurnoId { get; set; }
    
    // Información financiera
    public decimal MontoTotalTransacciones { get; set; }
    public decimal MontoTotalRecaudado { get; set; }
    public decimal DiferenciaCaja { get; set; }
    public int TotalTransacciones { get; set; }
    
    // Estado y validación
    public string Estado { get; set; } = string.Empty;
    public bool RequiereAprobacion { get; set; }
    public DateTime? FechaAprobacion { get; set; }
    public string? AprobadoPorEmpleadoNombre { get; set; }
    
    // Observaciones y notas
    public string? Observaciones { get; set; }
    public string? NotasAprobacion { get; set; }
    
    // Datos de auditoría
    public string? CreadoPorEmpleadoNombre { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaUltimaActualizacion { get; set; }
    
    // Detalles y discrepancias
    public List<LiquidacionDetalleDto> Detalles { get; set; } = new();
    public List<LiquidacionDiscrepanciaDto> Discrepancias { get; set; } = new();
    
    // Resumen por tipos de pago
    public Dictionary<string, ResumenTipoPagoDto> ResumenPorTipoPago { get; set; } = new();
    
    // Estadísticas adicionales
    public decimal PromedioTransaccion => TotalTransacciones > 0 ? MontoTotalTransacciones / TotalTransacciones : 0;
    public TimeSpan DuracionPeriodo => FechaFin - FechaInicio;
    public int TotalDiscrepancias => Discrepancias.Count;
    public int DiscrepanciasPendientes => Discrepancias.Count(d => !d.Resuelta);
}

/// <summary>
/// DTO para el detalle de liquidación
/// </summary>
public class LiquidacionDetalleDto
{
    public int Id { get; set; }
    public int TransaccionId { get; set; }
    public decimal Monto { get; set; }
    public string TipoPago { get; set; } = string.Empty;
    public string TipoVehiculo { get; set; } = string.Empty;
    public DateTime FechaTransaccion { get; set; }
    public int CarrilId { get; set; }
    public bool Validado { get; set; }
    public string? ObservacionesValidacion { get; set; }
}

/// <summary>
/// DTO para las discrepancias de liquidación
/// </summary>
public class LiquidacionDiscrepanciaDto
{
    public int Id { get; set; }
    public string TipoDiscrepancia { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal MontoDiscrepancia { get; set; }
    public string Severidad { get; set; } = string.Empty;
    public bool Resuelta { get; set; }
    public DateTime? FechaResolucion { get; set; }
    public string? NotasResolucion { get; set; }
    public string? ResueltoPorEmpleadoNombre { get; set; }
}

/// <summary>
/// DTO para el resumen por tipo de pago
/// </summary>
public class ResumenTipoPagoDto
{
    public string TipoPago { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal Monto { get; set; }
    public decimal Porcentaje { get; set; }
}

/// <summary>
/// DTO para crear una nueva liquidación
/// </summary>
public class CrearLiquidacionDto
{
    public TipoLiquidacion TipoLiquidacion { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public int? EstacionId { get; set; }
    public int? EmpleadoId { get; set; }
    public int? TurnoId { get; set; }
    public int CreadoPorEmpleadoId { get; set; }
    public string? Observaciones { get; set; }
}

/// <summary>
/// DTO para aprobar/rechazar una liquidación
/// </summary>
public class AprobarLiquidacionDto
{
    public int LiquidacionId { get; set; }
    public int EmpleadoId { get; set; }
    public string? NotasAprobacion { get; set; }
}

/// <summary>
/// DTO para resolver una discrepancia
/// </summary>
public class ResolverDiscrepanciaDto
{
    public int DiscrepanciaId { get; set; }
    public int EmpleadoId { get; set; }
    public string NotasResolucion { get; set; } = string.Empty;
}

/// <summary>
/// DTO para el resumen de liquidaciones
/// </summary>
public class ResumenLiquidacionesDto
{
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public int? EstacionId { get; set; }
    public string? EstacionNombre { get; set; }
    
    // Estadísticas generales
    public int TotalLiquidaciones { get; set; }
    public int LiquidacionesAprobadas { get; set; }
    public int LiquidacionesPendientes { get; set; }
    public int LiquidacionesRechazadas { get; set; }
    
    // Información financiera
    public decimal MontoTotalLiquidado { get; set; }
    public decimal TotalDiscrepancias { get; set; }
    public decimal PromedioLiquidacion { get; set; }
    
    // Resumen por tipos
    public Dictionary<string, int> LiquidacionesPorTipo { get; set; } = new();
    public Dictionary<string, decimal> MontosPorTipo { get; set; } = new();
    
    // Rendimiento por empleado
    public List<ResumenEmpleadoDto> ResumenPorEmpleado { get; set; } = new();
    
    // Discrepancias más comunes
    public List<ResumenDiscrepanciaDto> DiscrepanciasMasComunes { get; set; } = new();
}

/// <summary>
/// DTO para el resumen por empleado
/// </summary>
public class ResumenEmpleadoDto
{
    public int EmpleadoId { get; set; }
    public string EmpleadoNombre { get; set; } = string.Empty;
    public int TotalLiquidaciones { get; set; }
    public decimal MontoTotalLiquidado { get; set; }
    public int TotalDiscrepancias { get; set; }
    public decimal PromedioLiquidacion { get; set; }
}

/// <summary>
/// DTO para el resumen de discrepancias
/// </summary>
public class ResumenDiscrepanciaDto
{
    public string TipoDiscrepancia { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal MontoTotal { get; set; }
    public string Severidad { get; set; } = string.Empty;
    public int Resueltas { get; set; }
    public int Pendientes { get; set; }
}
