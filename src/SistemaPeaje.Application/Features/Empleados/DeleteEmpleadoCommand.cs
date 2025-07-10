using MediatR;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;

namespace SistemaPeaje.Application.Features.Empleados;

public record DeleteEmpleadoCommand(int Id) : IRequest<bool>;

public class DeleteEmpleadoHandler : IRequestHandler<DeleteEmpleadoCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteEmpleadoHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteEmpleadoCommand request, CancellationToken cancellationToken)
    {
        var empleado = await _unitOfWork.Repository<Empleado>().GetByIdAsync(request.Id);
        if (empleado == null)
            return false;

        await _unitOfWork.Repository<Empleado>().DeleteAsync(empleado);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
