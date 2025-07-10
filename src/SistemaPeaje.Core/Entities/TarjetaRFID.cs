namespace SistemaPeaje.Core.Entities;

public class TarjetaRFID : BaseEntity
{
    public string NumeroTag { get; set; } = string.Empty;
    public int ClienteId { get; set; }
    public decimal Saldo { get; set; }
    public DateTime FechaEmision { get; set; }
    public DateTime? FechaVencimiento { get; set; }
    public string Estado { get; set; } = "Activa"; // Activa, Bloqueada, Vencida

    // Navigation Properties
    public virtual Cliente? Cliente { get; set; }
}
