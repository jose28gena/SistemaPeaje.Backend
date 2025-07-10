using AutoMapper;
using MediatR;
using SistemaPeaje.Application.DTOs;
using SistemaPeaje.Application.Features.Transacciones.Commands;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;

namespace SistemaPeaje.Application.Features.Transacciones.Handlers;

public class CreateTransaccionCommandHandler : IRequestHandler<CreateTransaccionCommand, TransaccionDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateTransaccionCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<TransaccionDto> Handle(CreateTransaccionCommand request, CancellationToken cancellationToken)
    {
        var transaccion = new Transaccion
        {
            EstacionId = request.EstacionId,
            CarrilId = request.CarrilId,
            TipoVehiculoId = request.TipoVehiculoId,
            TipoPagoId = request.TipoPagoId,
            Monto = request.Monto,
            PlacaVehiculo = request.PlacaVehiculo,
            TagRFID = request.TagRFID,
            ClienteId = request.ClienteId,
            EmpleadoId = request.EmpleadoId,
            Observaciones = request.Observaciones,
            FechaTransaccion = DateTime.UtcNow,
            FechaCreacion = DateTime.UtcNow,
            NumeroTicket = GenerateTicketNumber()
        };

        var result = await _unitOfWork.Repository<Transaccion>().AddAsync(transaccion);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<TransaccionDto>(result);
    }

    private string GenerateTicketNumber()
    {
        return $"TKT-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
    }
}
