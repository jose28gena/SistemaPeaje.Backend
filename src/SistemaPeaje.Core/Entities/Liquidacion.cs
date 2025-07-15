namespace SistemaPeaje.Core.Entities;

/// <summary>
/// Entidad base para las liquidaciones del sistema
/// </summary>
public class Liquidacion : BaseEntity
{
    public Guid NumeroLiquidacion { get; set; } = Guid.NewGuid();
    public TipoLiquidacion TipoLiquidacion { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public DateTime FechaGeneracion { get; set; } = DateTime.UtcNow;
    
    // Relaciones
    public int? EstacionId { get; set; }
    public int? EmpleadoId { get; set; }
    public int? TurnoId { get; set; }
    
    // Información financiera
    public decimal MontoTotalTransacciones { get; set; }
    public decimal MontoTotalRecaudado { get; set; }
    public decimal DiferenciaCaja { get; set; }
    public int TotalTransacciones { get; set; }
    
    // Estado y validación
    public EstadoLiquidacion Estado { get; set; } = EstadoLiquidacion.Generada;
    public bool RequiereAprobacion { get; set; }
    public DateTime? FechaAprobacion { get; set; }
    public int? AprobadoPorEmpleadoId { get; set; }
    
    // Observaciones y notas
    public string? Observaciones { get; set; }
    public string? NotasAprobacion { get; set; }
    
    // Datos de auditoría
    public int CreadoPorEmpleadoId { get; set; }
    public DateTime? FechaUltimaActualizacion { get; set; }
    
    // Navigation Properties
    public virtual Estacion? Estacion { get; set; }
    public virtual Empleado? Empleado { get; set; }
    public virtual Turno? Turno { get; set; }
    public virtual Empleado? CreadoPorEmpleado { get; set; }
    public virtual Empleado? AprobadoPorEmpleado { get; set; }
    
    // Colecciones relacionadas
    public virtual ICollection<LiquidacionDetalle> Detalles { get; set; } = new List<LiquidacionDetalle>();
    public virtual ICollection<LiquidacionDiscrepancia> Discrepancias { get; set; } = new List<LiquidacionDiscrepancia>();
}

/// <summary>
/// Detalle de las transacciones incluidas en la liquidación
/// </summary>
public class LiquidacionDetalle : BaseEntity
{
    public int LiquidacionId { get; set; }
    public int TransaccionId { get; set; }
    public decimal Monto { get; set; }
    public string TipoPago { get; set; } = string.Empty;
    public string TipoVehiculo { get; set; } = string.Empty;
    public DateTime FechaTransaccion { get; set; }
    public int CarrilId { get; set; }
    public bool Validado { get; set; } = true;
    public string? ObservacionesValidacion { get; set; }
    
    // Navigation Properties
    public virtual Liquidacion Liquidacion { get; set; } = null!;
    public virtual Transaccion Transaccion { get; set; } = null!;
}

/// <summary>
/// Discrepancias encontradas durante la liquidación
/// </summary>
public class LiquidacionDiscrepancia : BaseEntity
{
    public int LiquidacionId { get; set; }
    public TipoDiscrepancia TipoDiscrepancia { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public decimal MontoDiscrepancia { get; set; }
    public SeveridadDiscrepancia Severidad { get; set; }
    public bool Resuelta { get; set; } = false;
    public DateTime? FechaResolucion { get; set; }
    public string? NotasResolucion { get; set; }
    public int? ResueltoPorEmpleadoId { get; set; }
    
    // Navigation Properties
    public virtual Liquidacion Liquidacion { get; set; } = null!;
    public virtual Empleado? ResueltoPorEmpleado { get; set; }
}

/// <summary>
/// Tipos de liquidación disponibles en el sistema
/// </summary>
public enum TipoLiquidacion
{
    Cajero = 1,
    Turno = 2,
    Dia = 3,
    Estacion = 4
}

/// <summary>
/// Estados posibles de una liquidación
/// </summary>
public enum EstadoLiquidacion
{
    Generada = 1,
    EnRevision = 2,
    Aprobada = 3,
    Rechazada = 4,
    Cerrada = 5
}

/// <summary>
/// Tipos de discrepancias que pueden encontrarse
/// </summary>
public enum TipoDiscrepancia
{
    DiferenciaCaja = 1,
    TransaccionSinEmpleado = 2,
    MontoIrregular = 3,
    TurnoIncompleto = 4,
    HorarioInvalido = 5,
    DuplicadoTransaccion = 6
}

/// <summary>
/// Severidad de las discrepancias
/// </summary>
public enum SeveridadDiscrepancia
{
    Baja = 1,
    Media = 2,
    Alta = 3,
    Critica = 4
}
