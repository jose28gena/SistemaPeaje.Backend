using AutoMapper;
using MediatR;
using SistemaPeaje.Application.Mappings;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;

namespace SistemaPeaje.Application.Features.Turnos;

public record AbrirTurnoCommand : IRequest<TurnoDto>
{
    public int EmpleadoId { get; set; }
    public int EstacionId { get; set; }
    public int? CarrilId { get; set; }
    public decimal MontoInicialCaja { get; set; }
}

public class AbrirTurnoHandler : IRequestHandler<AbrirTurnoCommand, TurnoDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AbrirTurnoHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<TurnoDto> Handle(AbrirTurnoCommand request, CancellationToken cancellationToken)
    {
        // Verificar que no haya turnos abiertos para el mismo empleado o estación
        var turnosAbiertos = await _unitOfWork.Repository<Turno>()
            .GetAsync(t => t.Estado == "Abierto" && 
                          (t.EmpleadoId == request.EmpleadoId || t.EstacionId == request.EstacionId));

        if (turnosAbiertos.Any())
        {
            var turnoExistente = turnosAbiertos.First();
            throw new InvalidOperationException(
                $"Ya existe un turno abierto para el empleado {turnoExistente.EmpleadoId} " +
                $"en la estación {turnoExistente.EstacionId}");
        }

        var turno = new Turno
        {
            EmpleadoId = request.EmpleadoId,
            EstacionId = request.EstacionId,
            CarrilId = request.CarrilId,
            FechaInicio = DateTime.UtcNow,
            MontoInicialCaja = request.MontoInicialCaja,
            Estado = "Abierto"
        };

        await _unitOfWork.Repository<Turno>().AddAsync(turno);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<TurnoDto>(turno);
    }
}
