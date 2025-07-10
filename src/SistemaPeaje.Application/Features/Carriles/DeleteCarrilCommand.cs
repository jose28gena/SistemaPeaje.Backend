using MediatR;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;

namespace SistemaPeaje.Application.Features.Carriles;

public record DeleteCarrilCommand(int Id) : IRequest<bool>;

public class DeleteCarrilHandler : IRequestHandler<DeleteCarrilCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCarrilHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteCarrilCommand request, CancellationToken cancellationToken)
    {
        var carril = await _unitOfWork.Repository<Carril>().GetByIdAsync(request.Id);
        if (carril == null)
            return false;

        await _unitOfWork.Repository<Carril>().DeleteAsync(carril);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
