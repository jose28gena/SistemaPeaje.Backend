using AutoMapper;
using MediatR;
using SistemaPeaje.Application.Mappings;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;

namespace SistemaPeaje.Application.Features.Empleados;

public record GetEmpleadoByIdQuery(int Id) : IRequest<EmpleadoDto?>;

public class GetEmpleadoByIdHandler : IRequestHandler<GetEmpleadoByIdQuery, EmpleadoDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetEmpleadoByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<EmpleadoDto?> Handle(GetEmpleadoByIdQuery request, CancellationToken cancellationToken)
    {
        var empleado = await _unitOfWork.Repository<Empleado>().GetByIdAsync(request.Id);
        return empleado == null ? null : _mapper.Map<EmpleadoDto>(empleado);
    }
}
