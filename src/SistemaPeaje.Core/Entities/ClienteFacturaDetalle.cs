namespace SistemaPeaje.Core.Entities;

public class ClienteFacturaDetalle : BaseEntity
{
    public int ClienteFacturaId { get; set; }
    public int? TransaccionId { get; set; }
    public string Concepto { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public decimal Cantidad { get; set; } = 1;
    public string UnidadMedida { get; set; } = "Servicio";
    public decimal PrecioUnitario { get; set; }
    public decimal? DescuentoPorcentaje { get; set; } = 0;
    public decimal? DescuentoMonto { get; set; } = 0;
    public decimal Subtotal { get; set; }
    public decimal? IVAPorcentaje { get; set; } = 16; // México
    public decimal? IVAMonto { get; set; } = 0;
    public decimal Total { get; set; }
    
    // Datos adicionales del cruce
    public DateTime? FechaCruce { get; set; }
    public string? EstacionOrigen { get; set; }
    public string? CarrilOrigen { get; set; }
    public string? PlacaVehiculo { get; set; }
    public string? TipoVehiculo { get; set; }
    public string? TagRFID { get; set; }

    // Navigation Properties
    public virtual ClienteFactura ClienteFactura { get; set; } = null!;
    public virtual Transaccion? Transaccion { get; set; }
}
