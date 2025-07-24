namespace SistemaPeaje.Application.DTOs.TurnosAdmin;

/// <summary>
/// DTOs especializados para reportes de turnos
/// </summary>

public class ReporteProductividadEmpleadosDto
{
    public int EmpleadoId { get; set; }
    public string EmpleadoNombre { get; set; } = string.Empty;
    public decimal HorasTrabajadas { get; set; }
    public decimal HorasExtras { get; set; }
    public int TurnosCompletados { get; set; }
    public int TurnosLlegadaTardia { get; set; }
    public decimal IndicePuntualidad { get; set; }
    public decimal IndiceBProductividad { get; set; }
}

public class ReporteRendimientoTurnosDto
{
    public DateTime Fecha { get; set; }
    public int TotalTurnosProgramados { get; set; }
    public int TurnosCompletados { get; set; }
    public int TurnosCancelados { get; set; }
    public decimal PorcentajeCompletados { get; set; }
    public decimal HorasPlaneadas { get; set; }
    public decimal HorasReales { get; set; }
    public decimal EficienciaHoraria { get; set; }
}

public class ReporteAsistenciaPuntualidadDto
{
    public DateTime Periodo { get; set; }
    public int EmpleadoId { get; set; }
    public string EmpleadoNombre { get; set; } = string.Empty;
    public int TotalTurnos { get; set; }
    public int TurnosAPuntos { get; set; }
    public int LlegadasTardias { get; set; }
    public int AusenciasJustificadas { get; set; }
    public int AusenciasInjustificadas { get; set; }
    public decimal PorcentajeAsistencia { get; set; }
    public decimal PorcentajePuntualidad { get; set; }
}

public class ReporteHorasExtrasDto
{
    public int EmpleadoId { get; set; }
    public string EmpleadoNombre { get; set; } = string.Empty;
    public DateTime Semana { get; set; }
    public decimal HorasRegulares { get; set; }
    public decimal HorasExtras { get; set; }
    public decimal TotalHoras { get; set; }
    public decimal CostoHorasExtras { get; set; }
    public int TurnosConHorasExtras { get; set; }
}

public class ReporteCostosLaboralesDto
{
    public DateTime Periodo { get; set; }
    public decimal CostoHorasRegulares { get; set; }
    public decimal CostoHorasExtras { get; set; }
    public decimal CostoBeneficios { get; set; }
    public decimal CostoTotal { get; set; }
    public int NumeroEmpleados { get; set; }
    public decimal PromedioHorasPorEmpleado { get; set; }
}

public class ReporteIncidenciasEventosDto
{
    public DateTime Fecha { get; set; }
    public string TipoEvento { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public string EstacionMasAfectada { get; set; } = string.Empty;
    public string EmpleadoMasIncidencias { get; set; } = string.Empty;
    public decimal TiempoPromedioResolucion { get; set; }
    public int EventosResueltos { get; set; }
    public int EventosPendientes { get; set; }
}

public class DashboardEjecutivoDto
{
    public DateTime FechaActualizacion { get; set; }
    public int EmpleadosActivos { get; set; }
    public int TurnosHoy { get; set; }
    public int TurnosEnCurso { get; set; }
    public decimal HorasTrabajadasHoy { get; set; }
    public decimal IndicePuntualidadGeneral { get; set; }
    public int IncidenciasAbiertas { get; set; }
    public decimal CostoLaboralMensual { get; set; }
    public List<EstacionEstadoDto> EstadoEstaciones { get; set; } = new();
}

public class EstacionEstadoDto
{
    public int EstacionId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int EmpleadosPresentes { get; set; }
    public int EmpleadosAsignados { get; set; }
    public string Estado { get; set; } = string.Empty; // Completa, Parcial, Sin_Personal
}

public class ReporteCoberturaDto
{
    public DateTime Fecha { get; set; }
    public int EstacionId { get; set; }
    public string EstacionNombre { get; set; } = string.Empty;
    public decimal HorasRequeridas { get; set; }
    public decimal HorasCubiertas { get; set; }
    public decimal PorcentajeCobertura { get; set; }
    public int TurnosSinAsignar { get; set; }
    public List<string> HorariosSinCobertura { get; set; } = new();
}

public class AnalisisComparativoDto
{
    public string Periodo { get; set; } = string.Empty;
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public decimal HorasTrabajadasPeriodo { get; set; }
    public decimal HorasTrabajadasPeriodoAnterior { get; set; }
    public decimal VariacionHoras { get; set; }
    public int IncidenciasPeriodo { get; set; }
    public int IncidenciasPeriodoAnterior { get; set; }
    public decimal VariacionIncidencias { get; set; }
    public decimal CostoPeriodo { get; set; }
    public decimal CostoPeriodoAnterior { get; set; }
    public decimal VariacionCosto { get; set; }
}

public class MetricasTiempoRealDto
{
    public DateTime HoraActual { get; set; }
    public int EmpleadosConectados { get; set; }
    public int TurnosActivos { get; set; }
    public int IncidenciasUltimaHora { get; set; }
    public List<TurnoActivoDto> TurnosEnCurso { get; set; } = new();
    public List<IncidenciaRecienteDto> IncidenciasRecientes { get; set; } = new();
}

public class TurnoActivoDto
{
    public int TurnoId { get; set; }
    public string EmpleadoNombre { get; set; } = string.Empty;
    public string EstacionNombre { get; set; } = string.Empty;
    public DateTime HoraInicio { get; set; }
    public TimeSpan TiempoTranscurrido { get; set; }
    public string Estado { get; set; } = string.Empty;
}

public class IncidenciaRecienteDto
{
    public int EventoId { get; set; }
    public string TipoEvento { get; set; } = string.Empty;
    public string EmpleadoNombre { get; set; } = string.Empty;
    public string EstacionNombre { get; set; } = string.Empty;
    public DateTime FechaEvento { get; set; }
    public string Estado { get; set; } = string.Empty;
}

public class ProyeccionesTendenciasDto
{
    public DateTime FechaProyeccion { get; set; }
    public decimal TendenciaHorasExtras { get; set; }
    public decimal TendenciaIncidencias { get; set; }
    public decimal TendenciaCostos { get; set; }
    public List<ProyeccionMensualDto> ProyeccionesMensuales { get; set; } = new();
    public List<RecomendacionDto> Recomendaciones { get; set; } = new();
}

public class ProyeccionMensualDto
{
    public int Mes { get; set; }
    public int Año { get; set; }
    public decimal HorasProyectadas { get; set; }
    public decimal CostoProyectado { get; set; }
    public int EmpleadosRequeridos { get; set; }
}

public class RecomendacionDto
{
    public string Categoria { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string Prioridad { get; set; } = string.Empty;
    public decimal ImpactoEstimado { get; set; }
}

// DTOs adicionales para funcionalidades específicas

public class CalendarioTurnosDto
{
    public DateTime Fecha { get; set; }
    public List<TurnoCalendarioDto> Turnos { get; set; } = new();
    public bool TieneConflictos { get; set; }
    public List<string> Conflictos { get; set; } = new();
}

public class TurnoCalendarioDto
{
    public int TurnoId { get; set; }
    public string EmpleadoNombre { get; set; } = string.Empty;
    public string EstacionNombre { get; set; } = string.Empty;
    public TimeSpan HoraInicio { get; set; }
    public TimeSpan HoraFin { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
}

public class ResumenAsistenciaDto
{
    public int EmpleadoId { get; set; }
    public string EmpleadoNombre { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public DateTime? HoraEntrada { get; set; }
    public DateTime? HoraSalida { get; set; }
    public TimeSpan? TiempoTrabajado { get; set; }
    public string Estado { get; set; } = string.Empty;
    public bool TieneIncidencias { get; set; }
}

public class EstadoActualEmpleadoDto
{
    public int EmpleadoId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string EstadoActual { get; set; } = string.Empty; // Trabajando, Descanso, Fuera_Servicio
    public DateTime? HoraInicioEstado { get; set; }
    public string? EstacionActual { get; set; }
    public int? TurnoAsignacionId { get; set; }
    public TimeSpan? TiempoEnEstado { get; set; }
}

public class ReporteHorasTrabajadasDto
{
    public DateTime PeriodoInicio { get; set; }
    public DateTime PeriodoFin { get; set; }
    public int EmpleadoId { get; set; }
    public string EmpleadoNombre { get; set; } = string.Empty;
    public decimal HorasRegulares { get; set; }
    public decimal HorasExtras { get; set; }
    public decimal TotalHoras { get; set; }
    public List<DetalleHorasDiaDto> DetallesPorDia { get; set; } = new();
}

public class DetalleHorasDiaDto
{
    public DateTime Fecha { get; set; }
    public decimal HorasRegulares { get; set; }
    public decimal HorasExtras { get; set; }
    public string? Observaciones { get; set; }
}

public class ResumenEventosPorTipoDto
{
    public string TipoEvento { get; set; } = string.Empty;
    public int CantidadTotal { get; set; }
    public int CantidadResueltos { get; set; }
    public int CantidadPendientes { get; set; }
    public decimal TiempoPromedioResolucion { get; set; }
    public string EmpleadoMasAfectado { get; set; } = string.Empty;
    public string EstacionMasAfectada { get; set; } = string.Empty;
}
