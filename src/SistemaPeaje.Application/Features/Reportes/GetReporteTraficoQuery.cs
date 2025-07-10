using MediatR;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;

namespace SistemaPeaje.Application.Features.Reportes;

public record GetReporteTraficoQuery : IRequest<ReporteTraficoDto>
{
    public DateTime FechaInicio { get; init; }
    public DateTime FechaFin { get; init; }
    public int? EstacionId { get; init; }
}

public class GetReporteTraficoHandler : IRequestHandler<GetReporteTraficoQuery, ReporteTraficoDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetReporteTraficoHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ReporteTraficoDto> Handle(GetReporteTraficoQuery request, CancellationToken cancellationToken)
    {
        var transacciones = await _unitOfWork.Repository<Transaccion>()
            .GetAsync(t => t.FechaTransaccion >= request.FechaInicio &&
                          t.FechaTransaccion <= request.FechaFin &&
                          (!request.EstacionId.HasValue || t.EstacionId == request.EstacionId));

        var vehiculosPorHora = transacciones
            .GroupBy(t => new { 
                Fecha = t.FechaTransaccion.Date, 
                Hora = t.FechaTransaccion.Hour 
            })
            .Select(g => new VolumenHorario
            {
                Fecha = g.Key.Fecha,
                Hora = g.Key.Hora,
                Vehiculos = g.Count(),
                MontoRecaudado = g.Sum(x => x.Monto)
            })
            .OrderBy(v => v.Fecha)
            .ThenBy(v => v.Hora)
            .ToList();

        var reporte = new ReporteTraficoDto
        {
            FechaInicio = request.FechaInicio,
            FechaFin = request.FechaFin,
            EstacionId = request.EstacionId,
            TotalVehiculos = transacciones.Count,
            PromedioVehiculosPorDia = CalcularPromedioVehiculosPorDia(transacciones, request.FechaInicio, request.FechaFin),
            PicoMaximoVehiculos = vehiculosPorHora.Any() ? vehiculosPorHora.Max(v => v.Vehiculos) : 0,
            HorarioPico = vehiculosPorHora.Any() ? 
                vehiculosPorHora.OrderByDescending(v => v.Vehiculos).First() : null,
            VolumenPorHora = vehiculosPorHora,
            VehiculosPorTipo = transacciones
                .GroupBy(t => t.TipoVehiculo?.Nombre ?? "Sin Especificar")
                .ToDictionary(g => g.Key, g => g.Count()),
            DistribucionPorDia = transacciones
                .GroupBy(t => t.FechaTransaccion.Date)
                .ToDictionary(g => g.Key, g => g.Count()),
            FechaGeneracion = DateTime.UtcNow
        };

        return reporte;
    }

    private static decimal CalcularPromedioVehiculosPorDia(IEnumerable<Transaccion> transacciones, DateTime fechaInicio, DateTime fechaFin)
    {
        var diasTotales = (decimal)(fechaFin.Date - fechaInicio.Date).TotalDays + 1;
        return diasTotales > 0 ? transacciones.Count() / diasTotales : 0;
    }
}
