using MediatR;
using SistemaPeaje.Core.Interfaces;
using SistemaPeaje.Core.Entities;

namespace SistemaPeaje.Application.Features.Usuarios.Commands;

public record DeleteUsuarioCommand(int Id) : IRequest;

public class DeleteUsuarioHandler : IRequestHandler<DeleteUsuarioCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteUsuarioHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteUsuarioCommand request, CancellationToken cancellationToken)
    {
        var usuario = await _unitOfWork.Repository<Usuario>().GetByIdAsync(request.Id);
        if (usuario == null)
            throw new ArgumentException("Usuario no encontrado", nameof(request.Id));

        await _unitOfWork.Repository<Usuario>().DeleteAsync(usuario);
        await _unitOfWork.SaveChangesAsync();
    }
}
