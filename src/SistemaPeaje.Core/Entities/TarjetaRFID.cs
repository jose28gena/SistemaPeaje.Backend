namespace SistemaPeaje.Core.Entities;

public class TarjetaRFID : BaseEntity
{
    public string NumeroTag { get; set; } = string.Empty;
    public int ClienteId { get; set; }
    public int? ClienteVehiculoId { get; set; } // Para asociar a vehículo específico
    public decimal Saldo { get; set; }
    public DateTime FechaEmision { get; set; }
    public DateTime? FechaVencimiento { get; set; }
    public string Estado { get; set; } = "Activa"; // Activa, Bloqueada, Vencida, Perdida
    public string TipoAsociacion { get; set; } = "Cuenta"; // Cuenta (monedero general) o Vehiculo (específica)
    public bool EnListaNegra { get; set; } = false;
    public DateTime? FechaBloqueo { get; set; }
    public string? MotivoBloqueo { get; set; }
    public DateTime? UltimaActividad { get; set; }
    public string? ObservacionesSeguridad { get; set; }

    // Navigation Properties
    public virtual Cliente? Cliente { get; set; }
    public virtual ClienteVehiculo? ClienteVehiculo { get; set; }
    public virtual ICollection<ClienteRecarga> Recargas { get; set; } = new List<ClienteRecarga>();
}
