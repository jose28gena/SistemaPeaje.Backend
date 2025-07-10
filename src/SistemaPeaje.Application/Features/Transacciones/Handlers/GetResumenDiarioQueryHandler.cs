using MediatR;
using SistemaPeaje.Application.Features.Transacciones.Queries;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;

namespace SistemaPeaje.Application.Features.Transacciones.Handlers;

public class GetResumenDiarioQueryHandler : IRequestHandler<GetResumenDiarioQuery, object>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetResumenDiarioQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<object> Handle(GetResumenDiarioQuery request, CancellationToken cancellationToken)
    {
        var fechaInicio = request.Fecha.Date;
        var fechaFin = fechaInicio.AddDays(1);

        var transacciones = await _unitOfWork.Repository<Transaccion>()
            .GetAsync(t => t.FechaTransaccion >= fechaInicio && t.FechaTransaccion < fechaFin);

        var resumen = new
        {
            Fecha = request.Fecha.Date,
            TotalTransacciones = transacciones.Count,
            MontoTotal = transacciones.Sum(t => t.Monto),
            TransaccionesPorTipoVehiculo = transacciones
                .GroupBy(t => t.TipoVehiculoId)
                .Select(g => new
                {
                    TipoVehiculoId = g.Key,
                    Cantidad = g.Count(),
                    MontoTotal = g.Sum(t => t.Monto)
                }),
            TransaccionesPorTipoPago = transacciones
                .GroupBy(t => t.TipoPagoId)
                .Select(g => new
                {
                    TipoPagoId = g.Key,
                    Cantidad = g.Count(),
                    MontoTotal = g.Sum(t => t.Monto)
                }),
            TransaccionesPorEstacion = transacciones
                .GroupBy(t => t.EstacionId)
                .Select(g => new
                {
                    EstacionId = g.Key,
                    Cantidad = g.Count(),
                    MontoTotal = g.Sum(t => t.Monto)
                })
        };

        return resumen;
    }
}
