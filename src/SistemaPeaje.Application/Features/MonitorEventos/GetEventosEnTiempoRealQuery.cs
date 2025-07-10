using MediatR;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;

namespace SistemaPeaje.Application.Features.MonitorEventos;

public record GetEventosEnTiempoRealQuery : IRequest<List<EventoTransitoDto>>
{
    public int? EstacionId { get; init; }
    public int? CarrilId { get; init; }
    public DateTime? FechaDesde { get; init; }
    public int LimitEventos { get; init; } = 50;
}

public class GetEventosEnTiempoRealHandler : IRequestHandler<GetEventosEnTiempoRealQuery, List<EventoTransitoDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetEventosEnTiempoRealHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<EventoTransitoDto>> Handle(GetEventosEnTiempoRealQuery request, CancellationToken cancellationToken)
    {
        var fechaFiltro = request.FechaDesde ?? DateTime.UtcNow.AddMinutes(-30);
        
        var eventos = await _unitOfWork.Repository<EventoTransito>()
            .GetAsync(e => e.FechaEvento >= fechaFiltro &&
                          (!request.EstacionId.HasValue || e.EstacionId == request.EstacionId) &&
                          (!request.CarrilId.HasValue || e.CarrilId == request.CarrilId));

        var eventosOrdenados = eventos
            .OrderByDescending(e => e.FechaEvento)
            .Take(request.LimitEventos)
            .Select(e => new EventoTransitoDto
            {
                Id = e.Id,
                EstacionId = e.EstacionId,
                CarrilId = e.CarrilId,
                TipoEvento = e.TipoEvento,
                Descripcion = e.Descripcion,
                FechaEvento = e.FechaEvento,
                DatosAdicionales = e.DatosAdicionales,
                Estado = DeterminarEstadoEvento(e),
                TiempoTranscurrido = DateTime.UtcNow - e.FechaEvento
            })
            .ToList();

        return eventosOrdenados;
    }

    private static string DeterminarEstadoEvento(EventoTransito evento)
    {
        var tiempoTranscurrido = DateTime.UtcNow - evento.FechaEvento;
        
        return evento.TipoEvento switch
        {
            "VehiculoDetectado" when tiempoTranscurrido.TotalSeconds < 30 => "Procesando",
            "VehiculoDetectado" when tiempoTranscurrido.TotalSeconds >= 30 => "Timeout",
            "PagoRealizado" => "Completado",
            "ErrorLectura" => "Error",
            _ => "Pendiente"
        };
    }
}
