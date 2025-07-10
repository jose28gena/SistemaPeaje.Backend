namespace SistemaPeaje.Core.Entities;

public class Turno : BaseEntity
{
    public int EmpleadoId { get; set; }
    public int EstacionId { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public decimal MontoInicialCaja { get; set; }
    public decimal? MontoFinalCaja { get; set; }
    public string Estado { get; set; } = "Abierto"; // Abierto, Cerrado

    // Navigation Properties
    public virtual Empleado? Empleado { get; set; }
    public virtual Estacion? Estacion { get; set; }
}
