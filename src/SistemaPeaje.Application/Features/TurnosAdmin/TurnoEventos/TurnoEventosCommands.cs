using MediatR;
using SistemaPeaje.Application.DTOs.TurnosAdmin;

namespace SistemaPeaje.Application.Features.TurnosAdmin.TurnoEventos;

/// <summary>
/// Commands para eventos de turnos
/// </summary>

public record CreateTurnoEventoCommand(int TurnoAsignacionId, string TipoEvento, string Descripcion, string? DetallesAdicionales = null) : IRequest<TurnoEventoDto>;

public record UpdateTurnoEventoCommand(int Id, string? Descripcion = null, string? DetallesAdicionales = null) : IRequest<TurnoEventoDto>;

public record ResolverTurnoEventoCommand(int Id, string SolucionAplicada, string ResueltoPort) : IRequest<TurnoEventoDto>;

public record DeleteTurnoEventoCommand(int Id) : IRequest<bool>;

/// <summary>
/// Queries para eventos de turnos
/// </summary>

public record GetTurnoEventosQuery(int? TurnoAsignacionId = null, string? TipoEvento = null, bool? EsResuelto = null) : IRequest<List<TurnoEventoDto>>;

public record GetTurnoEventoByIdQuery(int Id) : IRequest<TurnoEventoDto?>;

public record GetEventosPendientesQuery() : IRequest<List<TurnoEventoDto>>;

public record GetResumenEventosPorTipoQuery(DateTime FechaInicio, DateTime FechaFin) : IRequest<List<ResumenEventosPorTipoDto>>;

public record GetEventosPorEmpleadoQuery(int EmpleadoId, DateTime? FechaInicio = null, DateTime? FechaFin = null) : IRequest<List<TurnoEventoDto>>;

public record GetEventosPorEstacionQuery(int EstacionId, DateTime? FechaInicio = null, DateTime? FechaFin = null) : IRequest<List<TurnoEventoDto>>;
