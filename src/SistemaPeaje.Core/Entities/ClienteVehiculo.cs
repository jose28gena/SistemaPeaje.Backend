namespace SistemaPeaje.Core.Entities;

public class ClienteVehiculo : BaseEntity
{
    public int ClienteId { get; set; }
    public string Placa { get; set; } = string.Empty;
    public string? Marca { get; set; }
    public string? Modelo { get; set; }
    public int? Año { get; set; }
    public string? Color { get; set; }
    public string? VIN { get; set; }
    public string? NumeroSerie { get; set; }
    public string ClaseVehiculo { get; set; } = string.Empty; // Livianos, Pesados, Motocicletas
    public string SubClaseVehiculo { get; set; } = string.Empty; // Más específico
    public string? Propietario { get; set; }
    public bool EsPrincipal { get; set; } = false;
    public string Estado { get; set; } = "Activo"; // Activo, Inactivo, Bloqueado
    public DateTime? FechaRegistro { get; set; }
    public DateTime? FechaVencimientoDocumentos { get; set; }
    public string? Observaciones { get; set; }

    // Navigation Properties
    public virtual Cliente Cliente { get; set; } = null!;
    public virtual ICollection<TarjetaRFID> TarjetasAsociadas { get; set; } = new List<TarjetaRFID>();
}
