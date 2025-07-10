using MediatR;

namespace SistemaPeaje.Application.Features.Transacciones.Queries;

public class GetResumenDiarioQuery : IRequest<object>
{
    public DateTime Fecha { get; set; }
}
