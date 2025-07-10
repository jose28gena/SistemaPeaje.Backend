using AutoMapper;
using MediatR;
using SistemaPeaje.Application.Mappings;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;

namespace SistemaPeaje.Application.Features.TiposVehiculo;

public record GetTiposVehiculoQuery : IRequest<List<TipoVehiculoDto>>;

public class GetTiposVehiculoHandler : IRequestHandler<GetTiposVehiculoQuery, List<TipoVehiculoDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetTiposVehiculoHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<TipoVehiculoDto>> Handle(GetTiposVehiculoQuery request, CancellationToken cancellationToken)
    {
        var tiposVehiculo = await _unitOfWork.Repository<TipoVehiculo>().GetAllAsync();
        return _mapper.Map<List<TipoVehiculoDto>>(tiposVehiculo);
    }
}
