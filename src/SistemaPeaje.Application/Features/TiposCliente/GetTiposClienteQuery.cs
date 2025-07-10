using AutoMapper;
using MediatR;
using SistemaPeaje.Application.DTOs;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;

namespace SistemaPeaje.Application.Features.TiposCliente;

public record GetTiposClienteQuery : IRequest<List<TipoClienteDto>>;

public class GetTiposClienteHandler : IRequestHandler<GetTiposClienteQuery, List<TipoClienteDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetTiposClienteHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<TipoClienteDto>> Handle(GetTiposClienteQuery request, CancellationToken cancellationToken)
    {
        var tiposCliente = await _unitOfWork.Repository<TipoCliente>().GetAllAsync();
        return _mapper.Map<List<TipoClienteDto>>(tiposCliente);
    }
}
