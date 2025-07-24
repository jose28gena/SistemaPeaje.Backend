namespace SistemaPeaje.Core.Entities;

/// <summary>
/// Eventos especiales que ocurren durante un turno
/// </summary>
public class TurnoEvento : BaseEntity
{
    public int? TurnoId { get; set; }
    public int? TurnoAsignacionId { get; set; }
    public int EmpleadoId { get; set; }
    public int? EstacionId { get; set; }
    
    // Información del evento
    public DateTime FechaHoraEvento { get; set; }
    public string TipoEvento { get; set; } = string.Empty;
    // "Incidente", "Mantenimiento", "CambioTurno", "Emergencia", "Capacitacion", 
    // "AusenciaTemporary", "ProblemaEquipo", "ClienteEspecial", "Auditoria"
    
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    
    // Severidad y prioridad
    public string Prioridad { get; set; } = "Media"; // "Baja", "Media", "Alta", "Critica"
    public string Estado { get; set; } = "Abierto"; // "Abierto", "EnProceso", "Resuelto", "Cerrado"
    
    // Tiempos del evento
    public DateTime? FechaResolucion { get; set; }
    public int? DuracionMinutos { get; set; }
    
    // Impacto operacional
    public bool AfectaOperacion { get; set; } = false;
    public string? ImpactoOperacional { get; set; }
    public decimal? MontoAfectado { get; set; }
    
    // Seguimiento
    public int? ReportadoPorId { get; set; }
    public int? AsignadoAId { get; set; }
    public int? ResueltoPorId { get; set; }
    
    // Documentación
    public string? AccionesTomadas { get; set; }
    public string? SolucionAplicada { get; set; }
    public string? MedidasPreventivas { get; set; }
    
    // Archivos adjuntos
    public string? ArchivosAdjuntos { get; set; } // JSON con rutas de archivos
    
    // Notificaciones
    public bool RequiereNotificacion { get; set; } = false;
    public string? PersonasNotificadas { get; set; } // JSON con IDs de personas notificadas
    
    // Propiedades de navegación
    public virtual Turno? Turno { get; set; }
    public virtual TurnoAsignacion? TurnoAsignacion { get; set; }
    public virtual Empleado Empleado { get; set; } = null!;
    public virtual Estacion? Estacion { get; set; }
    public virtual Empleado? ReportadoPor { get; set; }
    public virtual Empleado? AsignadoA { get; set; }
    public virtual Empleado? ResueltoPor { get; set; }
}
