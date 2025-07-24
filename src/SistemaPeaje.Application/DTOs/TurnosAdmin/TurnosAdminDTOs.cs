namespace SistemaPeaje.Application.DTOs.TurnosAdmin;

/// <summary>
/// DTO para plantilla de turno
/// </summary>
public class TurnoTemplateDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public TimeSpan HoraInicio { get; set; }
    public TimeSpan HoraFin { get; set; }
    public int MinutosDescanso { get; set; }
    public decimal HorasEstimadas { get; set; }
    public bool EsRotativo { get; set; }
    public int? DiasRotacion { get; set; }
    public bool RequiereSupervisor { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaActualizacion { get; set; }
    
    // Propiedades calculadas
    public string DuracionTexto => $"{HorasEstimadas:F1}h";
    public string HorarioTexto => $"{HoraInicio:hh\\:mm} - {HoraFin:hh\\:mm}";
    public int NumeroAsignaciones { get; set; }
}

/// <summary>
/// DTO para crear/actualizar plantilla de turno
/// </summary>
public class CreateTurnoTemplateDto
{
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public TimeSpan HoraInicio { get; set; }
    public TimeSpan HoraFin { get; set; }
    public int MinutosDescanso { get; set; } = 30;
    public bool EsRotativo { get; set; } = false;
    public int? DiasRotacion { get; set; }
    public bool RequiereSupervisor { get; set; } = false;
}

/// <summary>
/// DTO para actualizar plantilla de turno
/// </summary>
public class UpdateTurnoTemplateDto : CreateTurnoTemplateDto
{
    public int Id { get; set; }
}

/// <summary>
/// DTO para asignación de turno
/// </summary>
public class TurnoAsignacionDto
{
    public int Id { get; set; }
    public int EmpleadoId { get; set; }
    public string EmpleadoNombre { get; set; } = string.Empty;
    public int EstacionId { get; set; }
    public string EstacionNombre { get; set; } = string.Empty;
    public DateTime FechaTurno { get; set; }
    public TimeSpan HoraInicioPrograma { get; set; }
    public TimeSpan HoraFinPrograma { get; set; }
    public DateTime? HoraInicioReal { get; set; }
    public DateTime? HoraFinReal { get; set; }
    public string Estado { get; set; } = string.Empty;
    public bool RequiereSupervisor { get; set; }
    public int? TurnoTemplateId { get; set; }
    public string? TurnoTemplateName { get; set; }
    public string? Notas { get; set; }
    public DateTime FechaCreacion { get; set; }
    
    // Propiedades calculadas
    public string HorarioProgramado => $"{HoraInicioPrograma:hh\\:mm} - {HoraFinPrograma:hh\\:mm}";
    public string EstadoDisplay => Estado switch
    {
        "Programado" => "📅 Programado",
        "EnCurso" => "🟢 En Curso",
        "Completado" => "✅ Completado",
        "Cancelado" => "❌ Cancelado",
        _ => Estado
    };
}

/// <summary>
/// DTO para crear asignación de turno
/// </summary>
public class CreateTurnoAsignacionDto
{
    public int EmpleadoId { get; set; }
    public int EstacionId { get; set; }
    public DateTime FechaTurno { get; set; }
    public TimeSpan HoraInicioPrograma { get; set; }
    public TimeSpan HoraFinPrograma { get; set; }
    public bool RequiereSupervisor { get; set; } = false;
    public int? TurnoTemplateId { get; set; }
    public string? Notas { get; set; }
}

/// <summary>
/// DTO para registro de tiempo
/// </summary>
public class RegistroTiempoDto
{
    public int Id { get; set; }
    public int EmpleadoId { get; set; }
    public string EmpleadoNombre { get; set; } = string.Empty;
    public int EstacionId { get; set; }
    public string EstacionNombre { get; set; } = string.Empty;
    public DateTime FechaTurno { get; set; }
    public DateTime? HoraEntrada { get; set; }
    public DateTime? HoraSalida { get; set; }
    public int MinutosDescanso { get; set; }
    public DateTime? InicioDescanso { get; set; }
    public DateTime? FinDescanso { get; set; }
    public decimal? HorasExtras { get; set; }
    public string TipoRegistro { get; set; } = string.Empty;
    public string? Observaciones { get; set; }
    public int? TurnoAsignacionId { get; set; }
    
    // Propiedades calculadas
    public TimeSpan? TiempoTrabajado => HoraSalida.HasValue && HoraEntrada.HasValue 
        ? HoraSalida.Value - HoraEntrada.Value - TimeSpan.FromMinutes(MinutosDescanso)
        : null;
    public string TiempoTrabajoTexto => TiempoTrabajado?.ToString(@"hh\:mm") ?? "--:--";
}

/// <summary>
/// DTO para evento de turno
/// </summary>
public class TurnoEventoDto
{
    public int Id { get; set; }
    public int TurnoAsignacionId { get; set; }
    public string TipoEvento { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public DateTime FechaHoraEvento { get; set; }
    public string? DetallesAdicionales { get; set; }
    public bool EsResuelto { get; set; }
    public DateTime? FechaResolucion { get; set; }
    public string? ResueltoPorte { get; set; }
    public string? SolucionAplicada { get; set; }
    
    // Propiedades calculadas
    public string TipoEventoDisplay => TipoEvento switch
    {
        "Llegada_Tardia" => "⏰ Llegada Tardía",
        "Salida_Temprana" => "⏰ Salida Temprana",
        "Problema_Tecnico" => "⚠️ Problema Técnico",
        "Emergencia" => "🚨 Emergencia",
        "Capacitacion" => "📚 Capacitación",
        "Reunion" => "👥 Reunión",
        _ => TipoEvento
    };
    public string EstadoDisplay => EsResuelto ? "✅ Resuelto" : "⏳ Pendiente";
}
