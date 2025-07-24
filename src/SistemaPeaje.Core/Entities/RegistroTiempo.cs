namespace SistemaPeaje.Core.Entities;

/// <summary>
/// Registro detallado de tiempo de entrada, salida y eventos durante el turno
/// </summary>
public class RegistroTiempo : BaseEntity
{
    public int EmpleadoId { get; set; }
    public int? TurnoAsignacionId { get; set; }
    public int? TurnoId { get; set; } // Relación con el turno actual
    
    // Registro de tiempo
    public DateTime FechaHora { get; set; }
    public string TipoRegistro { get; set; } = string.Empty; 
    // "Entrada", "Salida", "InicioDescanso", "FinDescanso", "InicioHoraExtra", "FinHoraExtra"
    
    // Método de registro
    public string MetodoRegistro { get; set; } = "Manual"; // "Manual", "Automatico", "Biometrico", "TarjetaRFID"
    
    // Ubicación del registro
    public int? EstacionId { get; set; }
    public string? UbicacionGPS { get; set; }
    public string? DireccionIP { get; set; }
    
    // Validación y autorización
    public bool EsValido { get; set; } = true;
    public string? MotivoInvalidacion { get; set; }
    public int? AutorizadoPorId { get; set; } // ID del supervisor que autorizó
    public DateTime? FechaAutorizacion { get; set; }
    
    // Información adicional
    public string? Observaciones { get; set; }
    public bool EsHoraExtra { get; set; } = false;
    public bool EsAtraso { get; set; } = false;
    public int MinutosAtraso { get; set; } = 0;
    
    // Archivo adjunto (foto, documento justificativo)
    public string? ArchivoAdjunto { get; set; }
    
    // Propiedades de navegación
    public virtual Empleado Empleado { get; set; } = null!;
    public virtual TurnoAsignacion? TurnoAsignacion { get; set; }
    public virtual Turno? Turno { get; set; }
    public virtual Estacion? Estacion { get; set; }
    public virtual Empleado? AutorizadoPor { get; set; }
}
