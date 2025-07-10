using MediatR;

namespace SistemaPeaje.Application.Features.Transacciones.Commands;

public class DeleteTransaccionCommand : IRequest<Unit>
{
    public int Id { get; set; }
}
