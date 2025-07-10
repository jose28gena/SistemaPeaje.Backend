using MediatR;

namespace SistemaPeaje.Application.Features.Transacciones.Commands;

public class UpdateTransaccionCommand : IRequest<Unit>
{
    public int Id { get; set; }
    public int EstacionId { get; set; }
    public int CarrilId { get; set; }
    public int TipoVehiculoId { get; set; }
    public int TipoPagoId { get; set; }
    public decimal Monto { get; set; }
    public string? PlacaVehiculo { get; set; }
    public string? TagRFID { get; set; }
    public int? ClienteId { get; set; }
    public int? EmpleadoId { get; set; }
    public string? Observaciones { get; set; }
}
