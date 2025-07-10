using AutoMapper;
using MediatR;
using SistemaPeaje.Application.DTOs;
using SistemaPeaje.Application.Features.Transacciones.Queries;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;

namespace SistemaPeaje.Application.Features.Transacciones.Handlers;

public class GetTransaccionesQueryHandler : IRequestHandler<GetTransaccionesQuery, IEnumerable<TransaccionDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetTransaccionesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TransaccionDto>> Handle(GetTransaccionesQuery request, CancellationToken cancellationToken)
    {
        var transacciones = await _unitOfWork.Repository<Transaccion>().GetAllAsync();
        
        // Aplicar filtros si están presentes
        if (request.FechaInicio.HasValue)
        {
            transacciones = transacciones.Where(t => t.FechaTransaccion >= request.FechaInicio.Value).ToList();
        }
        
        if (request.FechaFin.HasValue)
        {
            transacciones = transacciones.Where(t => t.FechaTransaccion <= request.FechaFin.Value).ToList();
        }
        
        if (request.EstacionId.HasValue)
        {
            transacciones = transacciones.Where(t => t.EstacionId == request.EstacionId.Value).ToList();
        }
        
        if (request.TipoVehiculoId.HasValue)
        {
            transacciones = transacciones.Where(t => t.TipoVehiculoId == request.TipoVehiculoId.Value).ToList();
        }

        // Paginación
        var pagedTransacciones = transacciones
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize);

        return _mapper.Map<IEnumerable<TransaccionDto>>(pagedTransacciones);
    }
}
