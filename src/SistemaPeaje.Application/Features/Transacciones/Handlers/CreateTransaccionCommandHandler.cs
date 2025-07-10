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
        // Calcular el monto considerando el tipo de cliente
        var montoFinal = await CalcularMontoConTipoCliente(request);

        var transaccion = new Transaccion
        {
            EstacionId = request.EstacionId,
            CarrilId = request.CarrilId,
            TipoVehiculoId = request.TipoVehiculoId,
            TipoPagoId = request.TipoPagoId,
            Monto = montoFinal,
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

    private async Task<decimal> CalcularMontoConTipoCliente(CreateTransaccionCommand request)
    {
        // Si no hay cliente, usar el monto original
        if (!request.ClienteId.HasValue)
            return request.Monto;

        // Obtener el cliente con su tipo
        var cliente = await _unitOfWork.Repository<Cliente>().GetByIdAsync(request.ClienteId.Value);
        
        if (cliente == null)
            return request.Monto;

        // Si no tiene tipo de cliente asignado, usar monto original
        if (!cliente.TipoClienteId.HasValue)
            return request.Monto;

        // Obtener el tipo de cliente
        var tipoCliente = await _unitOfWork.Repository<TipoCliente>().GetByIdAsync(cliente.TipoClienteId.Value);
        
        if (tipoCliente == null)
            return request.Monto;

        // Si el cliente está exento de pago (ej: residente), monto = 0
        if (tipoCliente.EstaExentoPago)
            return 0;

        // Si tiene descuento porcentual, aplicarlo
        if (tipoCliente.DescuentoPorcentaje.HasValue)
        {
            var descuento = tipoCliente.DescuentoPorcentaje.Value / 100;
            return request.Monto * (1 - descuento);
        }

        // Si no tiene descuento ni exención, usar monto original
        return request.Monto;
    }

    private string GenerateTicketNumber()
    {
        return $"TKT-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
    }
}
