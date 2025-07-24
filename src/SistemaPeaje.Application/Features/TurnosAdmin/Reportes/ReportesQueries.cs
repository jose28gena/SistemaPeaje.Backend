using MediatR;
using SistemaPeaje.Application.DTOs.TurnosAdmin;

namespace SistemaPeaje.Application.Features.TurnosAdmin.Reportes;

/// <summary>
/// Queries para reportes de turnos
/// </summary>

public record GetReporteProductividadEmpleadosQuery(DateTime FechaInicio, DateTime FechaFin) : IRequest<List<ReporteProductividadEmpleadosDto>>;

public record GetReporteRendimientoTurnosQuery(DateTime FechaInicio, DateTime FechaFin) : IRequest<List<ReporteRendimientoTurnosDto>>;

public record GetReporteAsistenciaPuntualidadQuery(DateTime FechaInicio, DateTime FechaFin, int? EmpleadoId = null) : IRequest<List<ReporteAsistenciaPuntualidadDto>>;

public record GetReporteHorasExtrasQuery(DateTime FechaInicio, DateTime FechaFin, int? EmpleadoId = null) : IRequest<List<ReporteHorasExtrasDto>>;

public record GetReporteCostosLaboralesQuery(DateTime FechaInicio, DateTime FechaFin) : IRequest<ReporteCostosLaboralesDto>;

public record GetReporteIncidenciasEventosQuery(DateTime FechaInicio, DateTime FechaFin) : IRequest<List<ReporteIncidenciasEventosDto>>;

public record GetDashboardEjecutivoQuery() : IRequest<DashboardEjecutivoDto>;

public record GetReporteCoberturaQuery(DateTime Fecha, int? EstacionId = null) : IRequest<List<ReporteCoberturaDto>>;

public record GetAnalisisComparativoQuery(DateTime FechaInicio, DateTime FechaFin, string TipoComparacion) : IRequest<AnalisisComparativoDto>;

public record GetMetricasTiempoRealQuery() : IRequest<MetricasTiempoRealDto>;

public record GetProyeccionesTendenciasQuery(int Meses = 6) : IRequest<ProyeccionesTendenciasDto>;
