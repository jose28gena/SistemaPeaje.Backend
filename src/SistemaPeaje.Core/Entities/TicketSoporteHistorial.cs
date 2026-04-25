namespace SistemaPeaje.Core.Entities;

public class TicketSoporteHistorial : BaseEntity
{
    public int TicketSoporteId { get; set; }
    public DateTime FechaAccion { get; set; }
    public string Usuario { get; set; } = string.Empty;
    public string TipoAccion { get; set; } = string.Empty; // Creacion, Asignacion, Comentario, Cambio_Estado, Resolucion, Cierre
    public string? EstadoAnterior { get; set; }
    public string? EstadoNuevo { get; set; }
    public string? Comentario { get; set; }
    public bool EsVisible { get; set; } = true; // Si es visible para el cliente
    public string? ArchivosAdjuntos { get; set; } // JSON con rutas de archivos
    public string? DatosAdicionales { get; set; } // JSON con información adicional

    // Navigation Properties
    public virtual TicketSoporte TicketSoporte { get; set; } = null!;
}
