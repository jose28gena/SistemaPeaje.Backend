using MediatR;
using SistemaPeaje.Application.DTOs.TurnosAdmin;

namespace SistemaPeaje.Application.Features.TurnosAdmin.TurnoAsignaciones;

/// <summary>
/// Commands para asignaciones de turnos
/// </summary>

public record CreateTurnoAsignacionCommand(CreateTurnoAsignacionDto Asignacion) : IRequest<TurnoAsignacionDto>;

public record CreateTurnoAsignacionesLoteCommand(List<CreateTurnoAsignacionDto> Asignaciones) : IRequest<List<TurnoAsignacionDto>>;

public record UpdateTurnoAsignacionCommand(int Id, CreateTurnoAsignacionDto Asignacion) : IRequest<TurnoAsignacionDto>;

public record DeleteTurnoAsignacionCommand(int Id) : IRequest<bool>;

public record CancelarTurnoAsignacionCommand(int Id, string Motivo) : IRequest<TurnoAsignacionDto>;

/// <summary>
/// Queries para asignaciones de turnos
/// </summary>

public record GetTurnoAsignacionesQuery(DateTime? FechaInicio = null, DateTime? FechaFin = null, int? EmpleadoId = null, int? EstacionId = null) : IRequest<List<TurnoAsignacionDto>>;

public record GetTurnoAsignacionByIdQuery(int Id) : IRequest<TurnoAsignacionDto?>;

public record GetCalendarioTurnosQuery(DateTime FechaInicio, DateTime FechaFin, int? EstacionId = null) : IRequest<List<CalendarioTurnosDto>>;

public record GetTurnosEmpleadoQuery(int EmpleadoId, DateTime? FechaInicio = null, DateTime? FechaFin = null) : IRequest<List<TurnoAsignacionDto>>;

public record GetTurnosEstacionQuery(int EstacionId, DateTime? Fecha = null) : IRequest<List<TurnoAsignacionDto>>;
