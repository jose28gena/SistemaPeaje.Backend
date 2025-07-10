using AutoMapper;
using MediatR;
using SistemaPeaje.Application.Mappings;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;

namespace SistemaPeaje.Application.Features.Carriles;

public record GetCarrilesQuery : IRequest<List<CarrilDto>>;

public class GetCarrilesHandler : IRequestHandler<GetCarrilesQuery, List<CarrilDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetCarrilesHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<CarrilDto>> Handle(GetCarrilesQuery request, CancellationToken cancellationToken)
    {
        var carriles = await _unitOfWork.Repository<Carril>().GetAllAsync();
        return _mapper.Map<List<CarrilDto>>(carriles);
    }
}
