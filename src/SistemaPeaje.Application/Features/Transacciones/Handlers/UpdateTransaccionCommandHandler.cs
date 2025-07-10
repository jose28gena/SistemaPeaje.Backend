using AutoMapper;
using MediatR;
using SistemaPeaje.Application.Features.Transacciones.Commands;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;

namespace SistemaPeaje.Application.Features.Transacciones.Handlers;

public class UpdateTransaccionCommandHandler : IRequestHandler<UpdateTransaccionCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTransaccionCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(UpdateTransaccionCommand request, CancellationToken cancellationToken)
    {
        var transaccion = await _unitOfWork.Repository<Transaccion>().GetByIdAsync(request.Id);
        
        if (transaccion == null)
        {
            throw new KeyNotFoundException($"Transacción con ID {request.Id} no encontrada");
        }

        transaccion.EstacionId = request.EstacionId;
        transaccion.CarrilId = request.CarrilId;
        transaccion.TipoVehiculoId = request.TipoVehiculoId;
        transaccion.TipoPagoId = request.TipoPagoId;
        transaccion.Monto = request.Monto;
        transaccion.PlacaVehiculo = request.PlacaVehiculo;
        transaccion.TagRFID = request.TagRFID;
        transaccion.ClienteId = request.ClienteId;
        transaccion.EmpleadoId = request.EmpleadoId;
        transaccion.Observaciones = request.Observaciones;
        transaccion.FechaActualizacion = DateTime.UtcNow;

        await _unitOfWork.Repository<Transaccion>().UpdateAsync(transaccion);
        await _unitOfWork.SaveChangesAsync();

        return Unit.Value;
    }
}
