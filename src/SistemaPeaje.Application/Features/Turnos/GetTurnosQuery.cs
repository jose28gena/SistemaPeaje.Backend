using AutoMapper;
using MediatR;
using SistemaPeaje.Application.Mappings;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;

namespace SistemaPeaje.Application.Features.Turnos;

public record GetTurnosQuery : IRequest<List<TurnoDto>>
{
    public int? EmpleadoId { get; init; }
    public int? EstacionId { get; init; }
    public string? Estado { get; init; }
}

public class GetTurnosHandler : IRequestHandler<GetTurnosQuery, List<TurnoDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetTurnosHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<TurnoDto>> Handle(GetTurnosQuery request, CancellationToken cancellationToken)
    {
        var turnos = await _unitOfWork.Repository<Turno>()
            .GetAsync(t => (!request.EmpleadoId.HasValue || t.EmpleadoId == request.EmpleadoId) &&
                          (!request.EstacionId.HasValue || t.EstacionId == request.EstacionId) &&
                          (string.IsNullOrEmpty(request.Estado) || t.Estado == request.Estado));

        return _mapper.Map<List<TurnoDto>>(turnos);
    }
}
