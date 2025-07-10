using AutoMapper;
using MediatR;
using SistemaPeaje.Application.Mappings;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;

namespace SistemaPeaje.Application.Features.Tarifas;

public record GetTarifasVigentesQuery : IRequest<List<TarifaDto>>
{
    public int? EstacionId { get; init; }
    public int? TipoVehiculoId { get; init; }
}

public class GetTarifasVigentesHandler : IRequestHandler<GetTarifasVigentesQuery, List<TarifaDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetTarifasVigentesHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<TarifaDto>> Handle(GetTarifasVigentesQuery request, CancellationToken cancellationToken)
    {
        var tarifas = await _unitOfWork.Repository<Tarifa>()
            .GetAsync(t => t.EsVigente && 
                          t.FechaVigenciaInicio <= DateTime.UtcNow &&
                          (!t.FechaVigenciaFin.HasValue || t.FechaVigenciaFin >= DateTime.UtcNow) &&
                          (!request.EstacionId.HasValue || t.EstacionId == request.EstacionId) &&
                          (!request.TipoVehiculoId.HasValue || t.TipoVehiculoId == request.TipoVehiculoId));

        return _mapper.Map<List<TarifaDto>>(tarifas);
    }
}
