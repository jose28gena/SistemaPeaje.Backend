namespace SistemaPeaje.Core.Entities;

public class ClienteFactura : BaseEntity
{
    public int ClienteId { get; set; }
    public string NumeroFactura { get; set; } = string.Empty;
    public string? Serie { get; set; }
    public string? Folio { get; set; }
    public DateTime FechaFactura { get; set; }
    public DateTime PeriodoInicio { get; set; }
    public DateTime PeriodoFin { get; set; }
    
    // Importes
    public decimal Subtotal { get; set; }
    public decimal? Descuento { get; set; } = 0;
    public decimal? IVA { get; set; } = 0;
    public decimal? OtrosImpuestos { get; set; } = 0;
    public decimal Total { get; set; }
    
    // Estado y Tipo
    public string TipoFactura { get; set; } = "Ingreso"; // Ingreso, Egreso, Nota_Credito, Nota_Debito
    public string Estado { get; set; } = "Borrador"; // Borrador, Emitida, Timbrada, Enviada, Pagada, Cancelada
    public string? MotivoFacturacion { get; set; } = "Servicios de Peaje";
    
    // CFDI (México)
    public string? UUID { get; set; }
    public DateTime? FechaTimbrado { get; set; }
    public string? CadenaOriginal { get; set; }
    public string? SelloDigital { get; set; }
    public string? RFCProveedorCertificacion { get; set; }
    public string? NumeroCertificadoSAT { get; set; }
    public string? SelloSAT { get; set; }
    
    // Forma y Método de Pago
    public string FormaPago { get; set; } = "PUE"; // PUE (Pago en una exhibición), PPD (Pago en parcialidades)
    public string MetodoPago { get; set; } = "01"; // Catálogo SAT
    public string? UsoCFDI { get; set; } = "G03"; // Gastos en general por defecto
    
    // Vencimiento (para PPD)
    public DateTime? FechaVencimiento { get; set; }
    public int? DiasCredito { get; set; }
    
    // Archivos
    public string? RutaPDF { get; set; }
    public string? RutaXML { get; set; }
    public string? EmailEnvio { get; set; }
    public DateTime? FechaEnvio { get; set; }
    
    // Observaciones
    public string? Observaciones { get; set; }
    public string? UsuarioCreacion { get; set; }
    public string? UsuarioTimbrado { get; set; }

    // Navigation Properties
    public virtual Cliente Cliente { get; set; } = null!;
    public virtual ICollection<ClienteFacturaDetalle> Detalles { get; set; } = new List<ClienteFacturaDetalle>();
    public virtual ICollection<ClientePago> Pagos { get; set; } = new List<ClientePago>();
}
