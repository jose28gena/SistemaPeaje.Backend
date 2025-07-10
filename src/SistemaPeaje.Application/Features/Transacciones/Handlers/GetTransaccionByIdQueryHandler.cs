using AutoMapper;
using MediatR;
using SistemaPeaje.Application.DTOs;
using SistemaPeaje.Application.Features.Transacciones.Queries;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;

namespace SistemaPeaje.Application.Features.Transacciones.Handlers;

public class GetTransaccionByIdQueryHandler : IRequestHandler<GetTransaccionByIdQuery, TransaccionDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetTransaccionByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<TransaccionDto> Handle(GetTransaccionByIdQuery request, CancellationToken cancellationToken)
    {
        var transaccion = await _unitOfWork.Repository<Transaccion>().GetByIdAsync(request.Id);
        
        if (transaccion == null)
        {
            throw new KeyNotFoundException($"Transacción con ID {request.Id} no encontrada");
        }

        return _mapper.Map<TransaccionDto>(transaccion);
    }
}
