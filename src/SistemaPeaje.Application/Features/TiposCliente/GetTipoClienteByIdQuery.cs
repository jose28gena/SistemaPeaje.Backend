using AutoMapper;
using MediatR;
using SistemaPeaje.Application.DTOs;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;

namespace SistemaPeaje.Application.Features.TiposCliente;

public record GetTipoClienteByIdQuery(int Id) : IRequest<TipoClienteDto?>;

public class GetTipoClienteByIdHandler : IRequestHandler<GetTipoClienteByIdQuery, TipoClienteDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetTipoClienteByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<TipoClienteDto?> Handle(GetTipoClienteByIdQuery request, CancellationToken cancellationToken)
    {
        var tipoCliente = await _unitOfWork.Repository<TipoCliente>().GetByIdAsync(request.Id);
        return tipoCliente == null ? null : _mapper.Map<TipoClienteDto>(tipoCliente);
    }
}
