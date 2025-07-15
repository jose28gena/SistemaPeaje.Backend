using AutoMapper;
using MediatR;
using SistemaPeaje.Application.DTOs;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;

namespace SistemaPeaje.Application.Features.Liquidacion.Queries;

/// <summary>
/// Query para obtener liquidaciones con filtros
/// </summary>
public record GetLiquidacionesQuery : IRequest<IEnumerable<LiquidacionDto>>
{
    public TipoLiquidacion? TipoLiquidacion { get; init; }
    public EstadoLiquidacion? Estado { get; init; }
    public int? EmpleadoId { get; init; }
    public int? EstacionId { get; init; }
    public DateTime? FechaDesde { get; init; }
    public DateTime? FechaHasta { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetLiquidacionesHandler : IRequestHandler<GetLiquidacionesQuery, IEnumerable<LiquidacionDto>>
{
    private readonly ILiquidacionService _liquidacionService;
    private readonly IMapper _mapper;

    public GetLiquidacionesHandler(ILiquidacionService liquidacionService, IMapper mapper)
    {
        _liquidacionService = liquidacionService;
        _mapper = mapper;
    }

    public async Task<IEnumerable<LiquidacionDto>> Handle(GetLiquidacionesQuery request, CancellationToken cancellationToken)
    {
        var liquidaciones = await _liquidacionService.ObtenerLiquidacionesAsync(
            request.TipoLiquidacion,
            request.Estado,
            request.EmpleadoId,
            request.EstacionId,
            request.FechaDesde,
            request.FechaHasta);

        // Aplicar paginación
        var paginatedLiquidaciones = liquidaciones
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize);

        return _mapper.Map<IEnumerable<LiquidacionDto>>(paginatedLiquidaciones);
    }
}

/// <summary>
/// Query para obtener una liquidación específica por ID
/// </summary>
public record GetLiquidacionByIdQuery : IRequest<LiquidacionDto?>
{
    public int LiquidacionId { get; init; }
}

public class GetLiquidacionByIdHandler : IRequestHandler<GetLiquidacionByIdQuery, LiquidacionDto?>
{
    private readonly ILiquidacionService _liquidacionService;
    private readonly IMapper _mapper;

    public GetLiquidacionByIdHandler(ILiquidacionService liquidacionService, IMapper mapper)
    {
        _liquidacionService = liquidacionService;
        _mapper = mapper;
    }

    public async Task<LiquidacionDto?> Handle(GetLiquidacionByIdQuery request, CancellationToken cancellationToken)
    {
        var liquidacion = await _liquidacionService.ObtenerLiquidacionAsync(request.LiquidacionId);
        return liquidacion != null ? _mapper.Map<LiquidacionDto>(liquidacion) : null;
    }
}

/// <summary>
/// Query para obtener liquidaciones pendientes de aprobación
/// </summary>
public record GetLiquidacionesPendientesQuery : IRequest<IEnumerable<LiquidacionDto>>
{
    public int? EstacionId { get; init; }
    public TipoLiquidacion? TipoLiquidacion { get; init; }
}

public class GetLiquidacionesPendientesHandler : IRequestHandler<GetLiquidacionesPendientesQuery, IEnumerable<LiquidacionDto>>
{
    private readonly ILiquidacionService _liquidacionService;
    private readonly IMapper _mapper;

    public GetLiquidacionesPendientesHandler(ILiquidacionService liquidacionService, IMapper mapper)
    {
        _liquidacionService = liquidacionService;
        _mapper = mapper;
    }

    public async Task<IEnumerable<LiquidacionDto>> Handle(GetLiquidacionesPendientesQuery request, CancellationToken cancellationToken)
    {
        var liquidaciones = await _liquidacionService.ObtenerLiquidacionesAsync(
            request.TipoLiquidacion,
            EstadoLiquidacion.Generada,
            estacionId: request.EstacionId);

        var liquidacionesEnRevision = await _liquidacionService.ObtenerLiquidacionesAsync(
            request.TipoLiquidacion,
            EstadoLiquidacion.EnRevision,
            estacionId: request.EstacionId);

        var todasPendientes = liquidaciones.Concat(liquidacionesEnRevision);

        return _mapper.Map<IEnumerable<LiquidacionDto>>(todasPendientes);
    }
}

/// <summary>
/// Query para obtener resumen de liquidaciones
/// </summary>
public record GetResumenLiquidacionesQuery : IRequest<ResumenLiquidacionesDto>
{
    public DateTime FechaInicio { get; init; }
    public DateTime FechaFin { get; init; }
    public int? EstacionId { get; init; }
}

public class GetResumenLiquidacionesHandler : IRequestHandler<GetResumenLiquidacionesQuery, ResumenLiquidacionesDto>
{
    private readonly ILiquidacionService _liquidacionService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetResumenLiquidacionesHandler(ILiquidacionService liquidacionService, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _liquidacionService = liquidacionService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ResumenLiquidacionesDto> Handle(GetResumenLiquidacionesQuery request, CancellationToken cancellationToken)
    {
        var liquidaciones = await _liquidacionService.ObtenerLiquidacionesAsync(
            fechaDesde: request.FechaInicio,
            fechaHasta: request.FechaFin,
            estacionId: request.EstacionId);

        var liquidacionesList = liquidaciones.ToList();

        // Obtener información de la estación si se especifica
        string? estacionNombre = null;
        if (request.EstacionId.HasValue)
        {
            var estacion = await _unitOfWork.Repository<Estacion>().GetByIdAsync(request.EstacionId.Value);
            estacionNombre = estacion?.Nombre;
        }

        // Generar resumen por empleado
        var resumenPorEmpleado = liquidacionesList
            .Where(l => l.EmpleadoId.HasValue)
            .GroupBy(l => l.EmpleadoId!.Value)
            .Select(g => new ResumenEmpleadoDto
            {
                EmpleadoId = g.Key,
                EmpleadoNombre = g.First().Empleado != null ? $"{g.First().Empleado.Nombres} {g.First().Empleado.Apellidos}" : "Desconocido",
                TotalLiquidaciones = g.Count(),
                MontoTotalLiquidado = g.Sum(l => l.MontoTotalRecaudado),
                TotalDiscrepancias = g.SelectMany(l => l.Discrepancias).Count(),
                PromedioLiquidacion = g.Average(l => l.MontoTotalRecaudado)
            })
            .ToList();

        // Generar resumen de discrepancias
        var discrepancias = liquidacionesList.SelectMany(l => l.Discrepancias).ToList();
        var resumenDiscrepancias = discrepancias
            .GroupBy(d => d.TipoDiscrepancia.ToString())
            .Select(g => new ResumenDiscrepanciaDto
            {
                TipoDiscrepancia = g.Key,
                Cantidad = g.Count(),
                MontoTotal = g.Sum(d => d.MontoDiscrepancia),
                Severidad = g.First().Severidad.ToString(),
                Resueltas = g.Count(d => d.Resuelta),
                Pendientes = g.Count(d => !d.Resuelta)
            })
            .ToList();

        return new ResumenLiquidacionesDto
        {
            FechaInicio = request.FechaInicio,
            FechaFin = request.FechaFin,
            EstacionId = request.EstacionId,
            EstacionNombre = estacionNombre,
            TotalLiquidaciones = liquidacionesList.Count,
            LiquidacionesAprobadas = liquidacionesList.Count(l => l.Estado == EstadoLiquidacion.Aprobada),
            LiquidacionesPendientes = liquidacionesList.Count(l => l.Estado == EstadoLiquidacion.Generada || l.Estado == EstadoLiquidacion.EnRevision),
            LiquidacionesRechazadas = liquidacionesList.Count(l => l.Estado == EstadoLiquidacion.Rechazada),
            MontoTotalLiquidado = liquidacionesList.Sum(l => l.MontoTotalRecaudado),
            TotalDiscrepancias = discrepancias.Sum(d => d.MontoDiscrepancia),
            PromedioLiquidacion = liquidacionesList.Any() ? liquidacionesList.Average(l => l.MontoTotalRecaudado) : 0,
            LiquidacionesPorTipo = liquidacionesList.GroupBy(l => l.TipoLiquidacion.ToString())
                .ToDictionary(g => g.Key, g => g.Count()),
            MontosPorTipo = liquidacionesList.GroupBy(l => l.TipoLiquidacion.ToString())
                .ToDictionary(g => g.Key, g => g.Sum(l => l.MontoTotalRecaudado)),
            ResumenPorEmpleado = resumenPorEmpleado,
            DiscrepanciasMasComunes = resumenDiscrepancias
        };
    }
}

/// <summary>
/// Query para obtener liquidaciones por empleado
/// </summary>
public record GetLiquidacionesPorEmpleadoQuery : IRequest<IEnumerable<LiquidacionDto>>
{
    public int EmpleadoId { get; init; }
    public DateTime? FechaDesde { get; init; }
    public DateTime? FechaHasta { get; init; }
    public TipoLiquidacion? TipoLiquidacion { get; init; }
}

public class GetLiquidacionesPorEmpleadoHandler : IRequestHandler<GetLiquidacionesPorEmpleadoQuery, IEnumerable<LiquidacionDto>>
{
    private readonly ILiquidacionService _liquidacionService;
    private readonly IMapper _mapper;

    public GetLiquidacionesPorEmpleadoHandler(ILiquidacionService liquidacionService, IMapper mapper)
    {
        _liquidacionService = liquidacionService;
        _mapper = mapper;
    }

    public async Task<IEnumerable<LiquidacionDto>> Handle(GetLiquidacionesPorEmpleadoQuery request, CancellationToken cancellationToken)
    {
        var liquidaciones = await _liquidacionService.ObtenerLiquidacionesAsync(
            request.TipoLiquidacion,
            empleadoId: request.EmpleadoId,
            fechaDesde: request.FechaDesde,
            fechaHasta: request.FechaHasta);

        return _mapper.Map<IEnumerable<LiquidacionDto>>(liquidaciones);
    }
}

/// <summary>
/// Query para obtener liquidaciones por estación
/// </summary>
public record GetLiquidacionesPorEstacionQuery : IRequest<IEnumerable<LiquidacionDto>>
{
    public int EstacionId { get; init; }
    public DateTime? FechaDesde { get; init; }
    public DateTime? FechaHasta { get; init; }
    public TipoLiquidacion? TipoLiquidacion { get; init; }
}

public class GetLiquidacionesPorEstacionHandler : IRequestHandler<GetLiquidacionesPorEstacionQuery, IEnumerable<LiquidacionDto>>
{
    private readonly ILiquidacionService _liquidacionService;
    private readonly IMapper _mapper;

    public GetLiquidacionesPorEstacionHandler(ILiquidacionService liquidacionService, IMapper mapper)
    {
        _liquidacionService = liquidacionService;
        _mapper = mapper;
    }

    public async Task<IEnumerable<LiquidacionDto>> Handle(GetLiquidacionesPorEstacionQuery request, CancellationToken cancellationToken)
    {
        var liquidaciones = await _liquidacionService.ObtenerLiquidacionesAsync(
            request.TipoLiquidacion,
            estacionId: request.EstacionId,
            fechaDesde: request.FechaDesde,
            fechaHasta: request.FechaHasta);

        return _mapper.Map<IEnumerable<LiquidacionDto>>(liquidaciones);
    }
}

/// <summary>
/// Query para validar condiciones previas
/// </summary>
public record ValidarCondicionesPreviasQuery : IRequest<bool>
{
    public TipoLiquidacion TipoLiquidacion { get; init; }
    public int? EmpleadoId { get; init; }
    public int? EstacionId { get; init; }
    public DateTime? Fecha { get; init; }
}

public class ValidarCondicionesPreviasHandler : IRequestHandler<ValidarCondicionesPreviasQuery, bool>
{
    private readonly ILiquidacionService _liquidacionService;

    public ValidarCondicionesPreviasHandler(ILiquidacionService liquidacionService)
    {
        _liquidacionService = liquidacionService;
    }

    public async Task<bool> Handle(ValidarCondicionesPreviasQuery request, CancellationToken cancellationToken)
    {
        return await _liquidacionService.ValidarCondicionesPrevias(
            request.TipoLiquidacion,
            request.EmpleadoId,
            request.EstacionId,
            request.Fecha);
    }
}
