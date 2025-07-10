using AutoMapper;
using MediatR;
using SistemaPeaje.Application.Mappings;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;

namespace SistemaPeaje.Application.Features.Empleados;

public record GetEmpleadosQuery : IRequest<List<EmpleadoDto>>;

public class GetEmpleadosHandler : IRequestHandler<GetEmpleadosQuery, List<EmpleadoDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetEmpleadosHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<EmpleadoDto>> Handle(GetEmpleadosQuery request, CancellationToken cancellationToken)
    {
        var empleados = await _unitOfWork.Repository<Empleado>().GetAllAsync();
        return _mapper.Map<List<EmpleadoDto>>(empleados);
    }
}
