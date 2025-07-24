namespace SistemaPeaje.Core.Entities;

/// <summary>
/// Plantilla de turno que define horarios estándar
/// </summary>
public class TurnoTemplate : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public TimeSpan HoraInicio { get; set; }
    public TimeSpan HoraFin { get; set; }
    public int DuracionMinutos { get; set; }
    public string Tipo { get; set; } = string.Empty; // "Mañana", "Tarde", "Noche", "Especial"
    public bool EsActivo { get; set; } = true;
    public bool PermiteHorasExtras { get; set; } = true;
    public int MaximoHorasExtras { get; set; } = 2; // Horas máximas de overtime
    public decimal FactorHoraExtra { get; set; } = 1.5m; // Multiplicador para pago de horas extras
    
    // Configuración de descansos
    public int MinutosDescanso { get; set; } = 30;
    public TimeSpan? HoraDescansoInicio { get; set; }
    
    // Días de la semana aplicables (bit flags)
    public bool Lunes { get; set; } = true;
    public bool Martes { get; set; } = true;
    public bool Miercoles { get; set; } = true;
    public bool Jueves { get; set; } = true;
    public bool Viernes { get; set; } = true;
    public bool Sabado { get; set; } = true;
    public bool Domingo { get; set; } = true;
    
    // Propiedades de navegación
    public virtual ICollection<TurnoAsignacion> TurnoAsignaciones { get; set; } = new List<TurnoAsignacion>();
}
