namespace SistemaPeaje.Core.Entities;

/// <summary>
/// Asignación programada de turnos para empleados
/// </summary>
public class TurnoAsignacion : BaseEntity
{
    public int EmpleadoId { get; set; }
    public int EstacionId { get; set; }
    public int? TurnoTemplateId { get; set; }
    
    // Fecha y horario específico
    public DateTime FechaTurno { get; set; }
    public TimeSpan HoraInicioPrograma { get; set; }
    public TimeSpan HoraFinPrograma { get; set; }
    
    // Estado de la asignación
    public string Estado { get; set; } = "Programado"; // "Programado", "Confirmado", "EnCurso", "Completado", "Cancelado", "NoPresento"
    
    // Información de confirmación/rechazo
    public DateTime? FechaConfirmacion { get; set; }
    public bool ConfirmadoPorEmpleado { get; set; } = false;
    public string? MotivoRechazo { get; set; }
    
    // Información de sustitución
    public int? EmpleadoSustitutoId { get; set; }
    public string? MotivoSustitucion { get; set; }
    public DateTime? FechaSustitucion { get; set; }
    
    // Notas adicionales
    public string? Notas { get; set; }
    public string? Observaciones { get; set; }
    
    // Configuración específica del turno
    public bool RequiereSupervisor { get; set; } = false;
    public decimal? MontoInicialCajaAsignado { get; set; }
    public string? InstruccionesEspeciales { get; set; }
    
    // Propiedades de navegación
    public virtual Empleado Empleado { get; set; } = null!;
    public virtual Estacion Estacion { get; set; } = null!;
    public virtual TurnoTemplate? TurnoTemplate { get; set; }
    public virtual Empleado? EmpleadoSustituto { get; set; }
    public virtual Turno? TurnoEjecutado { get; set; } // Relación con el turno real que se ejecutó
    public virtual ICollection<RegistroTiempo> RegistrosTiempo { get; set; } = new List<RegistroTiempo>();
    public virtual ICollection<TurnoEvento> Eventos { get; set; } = new List<TurnoEvento>();
}
