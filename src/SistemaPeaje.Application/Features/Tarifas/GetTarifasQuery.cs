using AutoMapper;
using MediatR;
using SistemaPeaje.Application.Mappings;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;

namespace SistemaPeaje.Application.Features.Tarifas;

public record GetTarifasQuery : IRequest<List<TarifaDto>>;

public class GetTarifasHandler : IRequestHandler<GetTarifasQuery, List<TarifaDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetTarifasHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<TarifaDto>> Handle(GetTarifasQuery request, CancellationToken cancellationToken)
    {
        var tarifas = await _unitOfWork.Repository<Tarifa>().GetAllAsync();
        return _mapper.Map<List<TarifaDto>>(tarifas);
    }
}
