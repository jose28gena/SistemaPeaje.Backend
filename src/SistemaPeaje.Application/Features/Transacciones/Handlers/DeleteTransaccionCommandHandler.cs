using MediatR;
using SistemaPeaje.Application.Features.Transacciones.Commands;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;

namespace SistemaPeaje.Application.Features.Transacciones.Handlers;

public class DeleteTransaccionCommandHandler : IRequestHandler<DeleteTransaccionCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteTransaccionCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeleteTransaccionCommand request, CancellationToken cancellationToken)
    {
        var transaccion = await _unitOfWork.Repository<Transaccion>().GetByIdAsync(request.Id);
        
        if (transaccion == null)
        {
            throw new KeyNotFoundException($"Transacción con ID {request.Id} no encontrada");
        }

        await _unitOfWork.Repository<Transaccion>().DeleteAsync(transaccion);
        await _unitOfWork.SaveChangesAsync();

        return Unit.Value;
    }
}
