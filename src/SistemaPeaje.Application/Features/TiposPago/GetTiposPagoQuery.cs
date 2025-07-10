using AutoMapper;
using MediatR;
using SistemaPeaje.Application.Mappings;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;

namespace SistemaPeaje.Application.Features.TiposPago;

public record GetTiposPagoQuery : IRequest<List<TipoPagoDto>>;

public class GetTiposPagoHandler : IRequestHandler<GetTiposPagoQuery, List<TipoPagoDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetTiposPagoHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<TipoPagoDto>> Handle(GetTiposPagoQuery request, CancellationToken cancellationToken)
    {
        var tiposPago = await _unitOfWork.Repository<TipoPago>().GetAllAsync();
        return _mapper.Map<List<TipoPagoDto>>(tiposPago);
    }
}
