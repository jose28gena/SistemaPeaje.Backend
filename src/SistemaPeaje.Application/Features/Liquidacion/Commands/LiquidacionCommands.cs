using AutoMapper;
using MediatR;
using SistemaPeaje.Application.DTOs;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;

namespace SistemaPeaje.Application.Features.Liquidacion.Commands;

/// <summary>
/// Comando para generar liquidación de cajero receptor
/// </summary>
public record GenerarLiquidacionCajeroCommand : IRequest<LiquidacionDto>
{
    public int EmpleadoId { get; init; }
    public int EstacionId { get; init; }
    public DateTime FechaInicio { get; init; }
    public DateTime FechaFin { get; init; }
    public int CreadoPorEmpleadoId { get; init; }
    public string? Observaciones { get; init; }
}

public class GenerarLiquidacionCajeroHandler : IRequestHandler<GenerarLiquidacionCajeroCommand, LiquidacionDto>
{
    private readonly ILiquidacionService _liquidacionService;
    private readonly IMapper _mapper;

    public GenerarLiquidacionCajeroHandler(ILiquidacionService liquidacionService, IMapper mapper)
    {
        _liquidacionService = liquidacionService;
        _mapper = mapper;
    }

    public async Task<LiquidacionDto> Handle(GenerarLiquidacionCajeroCommand request, CancellationToken cancellationToken)
    {
        var liquidacion = await _liquidacionService.GenerarLiquidacionCajeroAsync(
            request.EmpleadoId,
            request.EstacionId,
            request.FechaInicio,
            request.FechaFin);

        if (!string.IsNullOrEmpty(request.Observaciones))
        {
            liquidacion.Observaciones = request.Observaciones;
        }

        return _mapper.Map<LiquidacionDto>(liquidacion);
    }
}

/// <summary>
/// Comando para generar liquidación de turno
/// </summary>
public record GenerarLiquidacionTurnoCommand : IRequest<LiquidacionDto>
{
    public int TurnoId { get; init; }
    public int CreadoPorEmpleadoId { get; init; }
    public string? Observaciones { get; init; }
}

public class GenerarLiquidacionTurnoHandler : IRequestHandler<GenerarLiquidacionTurnoCommand, LiquidacionDto>
{
    private readonly ILiquidacionService _liquidacionService;
    private readonly IMapper _mapper;

    public GenerarLiquidacionTurnoHandler(ILiquidacionService liquidacionService, IMapper mapper)
    {
        _liquidacionService = liquidacionService;
        _mapper = mapper;
    }

    public async Task<LiquidacionDto> Handle(GenerarLiquidacionTurnoCommand request, CancellationToken cancellationToken)
    {
        var liquidacion = await _liquidacionService.GenerarLiquidacionTurnoAsync(request.TurnoId);

        if (!string.IsNullOrEmpty(request.Observaciones))
        {
            liquidacion.Observaciones = request.Observaciones;
        }

        return _mapper.Map<LiquidacionDto>(liquidacion);
    }
}

/// <summary>
/// Comando para generar liquidación de día
/// </summary>
public record GenerarLiquidacionDiaCommand : IRequest<LiquidacionDto>
{
    public int? EstacionId { get; init; }
    public DateTime Fecha { get; init; }
    public int CreadoPorEmpleadoId { get; init; }
    public string? Observaciones { get; init; }
}

public class GenerarLiquidacionDiaHandler : IRequestHandler<GenerarLiquidacionDiaCommand, LiquidacionDto>
{
    private readonly ILiquidacionService _liquidacionService;
    private readonly IMapper _mapper;

    public GenerarLiquidacionDiaHandler(ILiquidacionService liquidacionService, IMapper mapper)
    {
        _liquidacionService = liquidacionService;
        _mapper = mapper;
    }

    public async Task<LiquidacionDto> Handle(GenerarLiquidacionDiaCommand request, CancellationToken cancellationToken)
    {
        var liquidacion = await _liquidacionService.GenerarLiquidacionDiaAsync(
            request.EstacionId,
            request.Fecha);

        if (!string.IsNullOrEmpty(request.Observaciones))
        {
            liquidacion.Observaciones = request.Observaciones;
        }

        return _mapper.Map<LiquidacionDto>(liquidacion);
    }
}

/// <summary>
/// Comando para aprobar liquidación
/// </summary>
public record AprobarLiquidacionCommand : IRequest<LiquidacionDto>
{
    public int LiquidacionId { get; init; }
    public int EmpleadoId { get; init; }
    public string? NotasAprobacion { get; init; }
}

public class AprobarLiquidacionHandler : IRequestHandler<AprobarLiquidacionCommand, LiquidacionDto>
{
    private readonly ILiquidacionService _liquidacionService;
    private readonly IMapper _mapper;

    public AprobarLiquidacionHandler(ILiquidacionService liquidacionService, IMapper mapper)
    {
        _liquidacionService = liquidacionService;
        _mapper = mapper;
    }

    public async Task<LiquidacionDto> Handle(AprobarLiquidacionCommand request, CancellationToken cancellationToken)
    {
        var liquidacion = await _liquidacionService.AprobarLiquidacionAsync(
            request.LiquidacionId,
            request.EmpleadoId,
            request.NotasAprobacion);

        return _mapper.Map<LiquidacionDto>(liquidacion);
    }
}

/// <summary>
/// Comando para rechazar liquidación
/// </summary>
public record RechazarLiquidacionCommand : IRequest<LiquidacionDto>
{
    public int LiquidacionId { get; init; }
    public int EmpleadoId { get; init; }
    public string? NotasRechazo { get; init; }
}

public class RechazarLiquidacionHandler : IRequestHandler<RechazarLiquidacionCommand, LiquidacionDto>
{
    private readonly ILiquidacionService _liquidacionService;
    private readonly IMapper _mapper;

    public RechazarLiquidacionHandler(ILiquidacionService liquidacionService, IMapper mapper)
    {
        _liquidacionService = liquidacionService;
        _mapper = mapper;
    }

    public async Task<LiquidacionDto> Handle(RechazarLiquidacionCommand request, CancellationToken cancellationToken)
    {
        var liquidacion = await _liquidacionService.RechazarLiquidacionAsync(
            request.LiquidacionId,
            request.EmpleadoId,
            request.NotasRechazo);

        return _mapper.Map<LiquidacionDto>(liquidacion);
    }
}

/// <summary>
/// Comando para resolver discrepancia
/// </summary>
public record ResolverDiscrepanciaCommand : IRequest<LiquidacionDiscrepanciaDto>
{
    public int DiscrepanciaId { get; init; }
    public int EmpleadoId { get; init; }
    public string NotasResolucion { get; init; } = string.Empty;
}

public class ResolverDiscrepanciaHandler : IRequestHandler<ResolverDiscrepanciaCommand, LiquidacionDiscrepanciaDto>
{
    private readonly ILiquidacionService _liquidacionService;
    private readonly IMapper _mapper;

    public ResolverDiscrepanciaHandler(ILiquidacionService liquidacionService, IMapper mapper)
    {
        _liquidacionService = liquidacionService;
        _mapper = mapper;
    }

    public async Task<LiquidacionDiscrepanciaDto> Handle(ResolverDiscrepanciaCommand request, CancellationToken cancellationToken)
    {
        var discrepancia = await _liquidacionService.ResolverDiscrepanciaAsync(
            request.DiscrepanciaId,
            request.EmpleadoId,
            request.NotasResolucion);

        return _mapper.Map<LiquidacionDiscrepanciaDto>(discrepancia);
    }
}
