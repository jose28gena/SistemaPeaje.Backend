using MediatR;
using SistemaPeaje.Application.DTOs.TurnosAdmin;

namespace SistemaPeaje.Application.Features.TurnosAdmin.RegistroTiempo;

/// <summary>
/// Commands para registro de tiempo
/// </summary>

public record RegistrarEntradaCommand(int EmpleadoId, int EstacionId, DateTime? HoraEntrada = null) : IRequest<RegistroTiempoDto>;

public record RegistrarSalidaCommand(int EmpleadoId, DateTime? HoraSalida = null) : IRequest<RegistroTiempoDto>;

public record RegistrarInicioDescansoCommand(int EmpleadoId) : IRequest<RegistroTiempoDto>;

public record RegistrarFinDescansoCommand(int EmpleadoId) : IRequest<RegistroTiempoDto>;

public record CorregirRegistroTiempoCommand(int RegistroId, DateTime? HoraEntrada, DateTime? HoraSalida, string? Observaciones) : IRequest<RegistroTiempoDto>;

/// <summary>
/// Queries para registro de tiempo
/// </summary>

public record GetRegistrosTiempoQuery(int? EmpleadoId = null, DateTime? Fecha = null) : IRequest<List<RegistroTiempoDto>>;

public record GetResumenAsistenciaQuery(int? EmpleadoId = null, DateTime? FechaInicio = null, DateTime? FechaFin = null) : IRequest<List<ResumenAsistenciaDto>>;

public record GetEstadoActualEmpleadoQuery(int EmpleadoId) : IRequest<EstadoActualEmpleadoDto>;

public record GetReporteHorasTrabajadasQuery(int? EmpleadoId = null, DateTime? FechaInicio = null, DateTime? FechaFin = null) : IRequest<List<ReporteHorasTrabajadasDto>>;
