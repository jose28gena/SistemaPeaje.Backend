using MediatR;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;

namespace SistemaPeaje.Application.Features.TiposCliente;

public record DeleteTipoClienteCommand(int Id) : IRequest;

public class DeleteTipoClienteHandler : IRequestHandler<DeleteTipoClienteCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteTipoClienteHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteTipoClienteCommand request, CancellationToken cancellationToken)
    {
        var tipoCliente = await _unitOfWork.Repository<TipoCliente>().GetByIdAsync(request.Id);
        if (tipoCliente == null)
            throw new ArgumentException($"TipoCliente con ID {request.Id} no encontrado");

        // En lugar de eliminar físicamente, desactivamos el tipo de cliente
        tipoCliente.Activo = false;
        tipoCliente.FechaActualizacion = DateTime.UtcNow;

        await _unitOfWork.Repository<TipoCliente>().UpdateAsync(tipoCliente);
        await _unitOfWork.SaveChangesAsync();
    }
}
