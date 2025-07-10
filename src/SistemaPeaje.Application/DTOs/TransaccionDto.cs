namespace SistemaPeaje.Application.DTOs;

public class TransaccionDto
{
    public int Id { get; set; }
    public int EstacionId { get; set; }
    public string EstacionNombre { get; set; } = string.Empty;
    public int CarrilId { get; set; }
    public string CarrilNumero { get; set; } = string.Empty;
    public int TipoVehiculoId { get; set; }
    public string TipoVehiculoNombre { get; set; } = string.Empty;
    public int TipoPagoId { get; set; }
    public string TipoPagoNombre { get; set; } = string.Empty;
    public decimal Monto { get; set; }
    public DateTime FechaTransaccion { get; set; }
    public string? PlacaVehiculo { get; set; }
    public string? TagRFID { get; set; }
    public int? ClienteId { get; set; }
    public string? ClienteNombre { get; set; }
    public int? EmpleadoId { get; set; }
    public string? EmpleadoNombre { get; set; }
    public string? NumeroTicket { get; set; }
    public string? Observaciones { get; set; }
}
