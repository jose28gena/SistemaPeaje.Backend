namespace SistemaPeaje.Core.Entities;

public class ComandoPlc : BaseEntity
{
    public string TipoComando { get; set; } = string.Empty; // ABRIR_BARRERA, CERRAR_BARRERA, TEST_CONEXION
    public string IpDestino { get; set; } = string.Empty;
    public int Puerto { get; set; } = 502;
    public byte UnitId { get; set; } = 1;
    public ushort CoilAddress { get; set; }
    public bool ValorEnviado { get; set; }
    public bool ComandoExitoso { get; set; }
    public string? MensajeError { get; set; }
    public string? Observaciones { get; set; }
    public int? CarrilId { get; set; }
    public int? UsuarioId { get; set; }
    public int? EmpleadoId { get; set; }
    public DateTime FechaEjecucion { get; set; }
    public int TiempoRespuestaMs { get; set; }

    // Navigation Properties
    public virtual Carril? Carril { get; set; }
    public virtual Usuario? Usuario { get; set; }
    public virtual Empleado? Empleado { get; set; }
}
