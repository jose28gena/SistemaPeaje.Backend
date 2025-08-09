namespace SistemaPeaje.Core.Entities;

public class Turno : BaseEntity
{
    public int EmpleadoId { get; set; }
    public int EstacionId { get; set; }
    public int? CarrilId { get; set; } // Carril asignado al turno
    public int? TurnoAsignacionId { get; set; } // Relación con la asignación programada
    public DateTime FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public decimal MontoInicialCaja { get; set; }
    public decimal? MontoFinalCaja { get; set; }
    public string Estado { get; set; } = "Abierto"; // Abierto, Cerrado, Pausado, Suspendido
    
    // Información adicional del turno
    public DateTime? HoraInicioReal { get; set; } // Hora real de inicio (puede diferir de FechaInicio)
    public DateTime? HoraFinReal { get; set; } // Hora real de fin
    public bool TieneHorasExtras { get; set; } = false;
    public int MinutosHorasExtras { get; set; } = 0;
    public decimal? MontoHorasExtras { get; set; }
    
    // Control de descansos
    public DateTime? InicioDescanso { get; set; }
    public DateTime? FinDescanso { get; set; }
    public int MinutosDescanso { get; set; } = 0;
    
    // Métricas del turno
    public int TotalTransacciones { get; set; } = 0;
    public decimal TotalRecaudado { get; set; } = 0;
    public int TotalVehiculos { get; set; } = 0;
    
    // Cuadre/cierre por medios de pago
    public decimal? VentasEfectivo { get; set; }
    public decimal? EfectivoContado { get; set; }
    public decimal? VentasPrepago { get; set; }
    public int? CantidadExentos { get; set; }
    
    // Observaciones y notas
    public string? Observaciones { get; set; }
    public string? MotivoCierre { get; set; }
    public string? NotasAdministrativas { get; set; }

    // Navigation Properties
    public virtual Empleado? Empleado { get; set; }
    public virtual Estacion? Estacion { get; set; }
    public virtual Carril? Carril { get; set; }
    public virtual TurnoAsignacion? TurnoAsignacion { get; set; }
    public virtual ICollection<RegistroTiempo> RegistrosTiempo { get; set; } = new List<RegistroTiempo>();
    public virtual ICollection<TurnoEvento> Eventos { get; set; } = new List<TurnoEvento>();
}
