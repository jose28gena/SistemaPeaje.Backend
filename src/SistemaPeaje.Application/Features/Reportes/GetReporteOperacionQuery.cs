using AutoMapper;
using MediatR;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;

namespace SistemaPeaje.Application.Features.Reportes;

public record GetReporteOperacionQuery : IRequest<ReporteOperacionDto>
{
    public DateTime FechaInicio { get; init; }
    public DateTime FechaFin { get; init; }
    public int? EstacionId { get; init; }
}

public class GetReporteOperacionHandler : IRequestHandler<GetReporteOperacionQuery, ReporteOperacionDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetReporteOperacionHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ReporteOperacionDto> Handle(GetReporteOperacionQuery request, CancellationToken cancellationToken)
    {
        var transacciones = await _unitOfWork.Repository<Transaccion>()
            .GetAsync(t => t.FechaTransaccion >= request.FechaInicio &&
                          t.FechaTransaccion <= request.FechaFin &&
                          (!request.EstacionId.HasValue || t.EstacionId == request.EstacionId));

        var turnos = await _unitOfWork.Repository<Turno>()
            .GetAsync(t => t.FechaInicio >= request.FechaInicio &&
                          t.FechaInicio <= request.FechaFin &&
                          (!request.EstacionId.HasValue || t.EstacionId == request.EstacionId));

        var reporte = new ReporteOperacionDto
        {
            FechaInicio = request.FechaInicio,
            FechaFin = request.FechaFin,
            EstacionId = request.EstacionId,
            TotalTransacciones = transacciones.Count,
            MontoTotalRecaudado = transacciones.Sum(t => t.Monto),
            PromedioTransaccionesPorHora = CalcularPromedioTransaccionesPorHora(transacciones, request.FechaInicio, request.FechaFin),
            TurnosRegistrados = turnos.Count,
            TurnosCerrados = turnos.Count(t => t.Estado == "Cerrado"),
            TransaccionesPorTipoPago = transacciones
                .GroupBy(t => t.TipoPago?.Nombre ?? "Sin Especificar")
                .ToDictionary(g => g.Key, g => g.Count()),
            MontosPorTipoPago = transacciones
                .GroupBy(t => t.TipoPago?.Nombre ?? "Sin Especificar")
                .ToDictionary(g => g.Key, g => g.Sum(x => x.Monto)),
            TransaccionesPorVehiculo = transacciones
                .GroupBy(t => t.TipoVehiculo?.Nombre ?? "Sin Especificar")
                .ToDictionary(g => g.Key, g => g.Count()),
            FechaGeneracion = DateTime.UtcNow
        };

        return reporte;
    }

    private static decimal CalcularPromedioTransaccionesPorHora(IEnumerable<Transaccion> transacciones, DateTime fechaInicio, DateTime fechaFin)
    {
        var horasTotales = (decimal)(fechaFin - fechaInicio).TotalHours;
        return horasTotales > 0 ? transacciones.Count() / horasTotales : 0;
    }
}
