namespace SistemaPeaje.Core.Entities;

public class ClientePago : BaseEntity
{
    public int ClienteId { get; set; }
    public int? ClienteFacturaId { get; set; }
    public string NumeroOperacion { get; set; } = string.Empty;
    public decimal Monto { get; set; }
    public DateTime FechaPago { get; set; }
    public string FormaPago { get; set; } = string.Empty; // Efectivo, Transferencia, Tarjeta, Cheque
    public string? Banco { get; set; }
    public string? Referencia { get; set; }
    public string? CuentaOrigen { get; set; }
    public string? CuentaDestino { get; set; }
    public string Estado { get; set; } = "Registrado"; // Registrado, Confirmado, Aplicado, Rechazado
    public DateTime? FechaConfirmacion { get; set; }
    public DateTime? FechaAplicacion { get; set; }
    public string? UsuarioRegistro { get; set; }
    public string? UsuarioConfirmacion { get; set; }
    public string? ComprobantePago { get; set; }
    public string? Observaciones { get; set; }
    
    // Para complemento de pago (México CFDI)
    public string? UUIDComplemento { get; set; }
    public DateTime? FechaTimbradoComplemento { get; set; }
    public string? XMLComplemento { get; set; }

    // Navigation Properties
    public virtual Cliente Cliente { get; set; } = null!;
    public virtual ClienteFactura? ClienteFactura { get; set; }
}
