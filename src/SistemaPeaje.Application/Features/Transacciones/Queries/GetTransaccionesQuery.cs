using MediatR;
using SistemaPeaje.Application.DTOs;

namespace SistemaPeaje.Application.Features.Transacciones.Queries;

public class GetTransaccionesQuery : IRequest<IEnumerable<TransaccionDto>>
{
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public int? EstacionId { get; set; }
    public int? TipoVehiculoId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
