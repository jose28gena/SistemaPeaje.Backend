namespace SistemaPeaje.Core.Entities;

public class Empleado : BaseEntity
{
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string NumeroDocumento { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string Cargo { get; set; } = string.Empty;
    public int? EstacionId { get; set; }
    public DateTime FechaContratacion { get; set; }
    public bool EsActivo { get; set; } = true;

    // Navigation Properties
    public virtual Estacion? Estacion { get; set; }
    public virtual ICollection<Transaccion> Transacciones { get; set; } = new List<Transaccion>();
    public virtual ICollection<Turno> Turnos { get; set; } = new List<Turno>();
}
