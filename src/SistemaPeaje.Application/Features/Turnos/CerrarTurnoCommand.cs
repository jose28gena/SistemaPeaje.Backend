using AutoMapper;
using MediatR;
using SistemaPeaje.Application.Mappings;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;

namespace SistemaPeaje.Application.Features.Turnos;

public record CerrarTurnoCommand : IRequest<TurnoDto>
{
    public int TurnoId { get; set; }
    public decimal MontoFinalCaja { get; set; }
    public decimal? VentasEfectivo { get; set; }
    public decimal? EfectivoContado { get; set; }
    public decimal? VentasPrepago { get; set; }
    public int? CantidadExentos { get; set; }
}

public class CerrarTurnoHandler : IRequestHandler<CerrarTurnoCommand, TurnoDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CerrarTurnoHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<TurnoDto> Handle(CerrarTurnoCommand request, CancellationToken cancellationToken)
    {
        var turno = await _unitOfWork.Repository<Turno>().GetByIdAsync(request.TurnoId);
        if (turno == null)
            throw new KeyNotFoundException($"Turno with ID {request.TurnoId} not found");

        if (turno.Estado != "Abierto")
            throw new InvalidOperationException("El turno ya está cerrado");

        turno.FechaFin = DateTime.UtcNow;
        turno.MontoFinalCaja = request.MontoFinalCaja;
    turno.VentasEfectivo = request.VentasEfectivo;
    turno.EfectivoContado = request.EfectivoContado;
    turno.VentasPrepago = request.VentasPrepago;
    turno.CantidadExentos = request.CantidadExentos;
        turno.Estado = "Cerrado";

        await _unitOfWork.Repository<Turno>().UpdateAsync(turno);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<TurnoDto>(turno);
    }
}
