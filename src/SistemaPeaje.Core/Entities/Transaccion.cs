namespace SistemaPeaje.Core.Entities;

public class Transaccion : BaseEntity
{
    public int EstacionId { get; set; }
    public int CarrilId { get; set; }
    public int TipoVehiculoId { get; set; }
    public int TipoPagoId { get; set; }
    public decimal Monto { get; set; }
    public DateTime FechaTransaccion { get; set; }
    public string? PlacaVehiculo { get; set; }
    public string? TagRFID { get; set; }
    public int? ClienteId { get; set; }
    public int? EmpleadoId { get; set; }
    public string? NumeroTicket { get; set; }
    public string? Observaciones { get; set; }

    // Navigation Properties
    public virtual Estacion? Estacion { get; set; }
    public virtual Carril? Carril { get; set; }
    public virtual TipoVehiculo? TipoVehiculo { get; set; }
    public virtual TipoPago? TipoPago { get; set; }
    public virtual Cliente? Cliente { get; set; }
    public virtual Empleado? Empleado { get; set; }
}
