namespace SistemaPeaje.Core.Entities;

public class ClienteRecarga : BaseEntity
{
    public int ClienteId { get; set; }
    public int? TarjetaRFIDId { get; set; }
    public decimal Monto { get; set; }
    public string MedioPago { get; set; } = string.Empty; // Efectivo, Transferencia, Tarjeta, TPV
    public string? Referencia { get; set; }
    public string? NumeroAutorizacion { get; set; }
    public DateTime FechaRecarga { get; set; }
    public string Estado { get; set; } = "Registrada"; // Registrada, Confirmada, Aplicada, Rechazada, Cancelada
    public DateTime? FechaConfirmacion { get; set; }
    public DateTime? FechaAplicacion { get; set; }
    public string? UsuarioCreacion { get; set; }
    public string? UsuarioConfirmacion { get; set; }
    public string? Observaciones { get; set; }
    public string? ComprobantePago { get; set; }
    public bool RequiereConciliacion { get; set; } = false;
    public DateTime? FechaConciliacion { get; set; }
    public string? UsuarioConciliacion { get; set; }

    // Para detección de fraude
    public string? IPOrigen { get; set; }
    public string? DispositivoOrigen { get; set; }
    public bool MarcadaFraude { get; set; } = false;
    public string? MotivoFraude { get; set; }

    // Navigation Properties
    public virtual Cliente Cliente { get; set; } = null!;
    public virtual TarjetaRFID? TarjetaRFID { get; set; }
}
