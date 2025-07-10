namespace SistemaPeaje.Core.Entities;

public class Carril : BaseEntity
{
    public int EstacionId { get; set; }
    public string Numero { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty; // Manual, Automático, Mixto
    public string Estado { get; set; } = "Activo"; // Activo, Inactivo, Mantenimiento

    // Navigation Properties
    public virtual Estacion? Estacion { get; set; }
    public virtual ICollection<Transaccion> Transacciones { get; set; } = new List<Transaccion>();
}
