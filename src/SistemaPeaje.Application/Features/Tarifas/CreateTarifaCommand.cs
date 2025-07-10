using AutoMapper;
using MediatR;
using SistemaPeaje.Application.Mappings;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;

namespace SistemaPeaje.Application.Features.Tarifas;

public record CreateTarifaCommand : IRequest<TarifaDto>
{
    public int TipoVehiculoId { get; set; }
    public int? EstacionId { get; set; }
    public decimal Monto { get; set; }
    public DateTime FechaVigenciaInicio { get; set; }
    public DateTime? FechaVigenciaFin { get; set; }
}

public class CreateTarifaHandler : IRequestHandler<CreateTarifaCommand, TarifaDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateTarifaHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<TarifaDto> Handle(CreateTarifaCommand request, CancellationToken cancellationToken)
    {
        // Check if there's an overlapping active tariff for the same vehicle type and station
        var existingTarifas = await _unitOfWork.Repository<Tarifa>()
            .GetAsync(t => t.TipoVehiculoId == request.TipoVehiculoId &&
                          t.EstacionId == request.EstacionId &&
                          t.EsVigente &&
                          t.FechaVigenciaInicio <= request.FechaVigenciaInicio &&
                          (!t.FechaVigenciaFin.HasValue || t.FechaVigenciaFin >= request.FechaVigenciaInicio));

        // Deactivate overlapping tariffs
        foreach (var tarifa in existingTarifas)
        {
            tarifa.EsVigente = false;
            tarifa.FechaVigenciaFin = request.FechaVigenciaInicio.AddDays(-1);
            await _unitOfWork.Repository<Tarifa>().UpdateAsync(tarifa);
        }

        var nuevaTarifa = new Tarifa
        {
            TipoVehiculoId = request.TipoVehiculoId,
            EstacionId = request.EstacionId,
            Monto = request.Monto,
            FechaVigenciaInicio = request.FechaVigenciaInicio,
            FechaVigenciaFin = request.FechaVigenciaFin,
            EsVigente = true
        };

        await _unitOfWork.Repository<Tarifa>().AddAsync(nuevaTarifa);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<TarifaDto>(nuevaTarifa);
    }
}
