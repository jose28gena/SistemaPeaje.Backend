namespace SistemaPeaje.Core.Entities;

public class TicketSoporte : BaseEntity
{
    public int ClienteId { get; set; }
    public string NumeroTicket { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty; // Reposicion_Tarjeta, Disputa_Cruce, Bloqueo_Fraude, Consulta_General
    public string Prioridad { get; set; } = "Media"; // Baja, Media, Alta, Critica
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string Estado { get; set; } = "Abierto"; // Abierto, En_Proceso, Resuelto, Cerrado, Cancelado
    public DateTime? FechaApertura { get; set; }
    public DateTime? FechaAsignacion { get; set; }
    public DateTime? FechaResolucion { get; set; }
    public DateTime? FechaCierre { get; set; }
    
    // Usuarios involucrados
    public string? UsuarioCreacion { get; set; }
    public string? UsuarioAsignado { get; set; }
    public string? UsuarioResolucion { get; set; }
    
    // Información específica del problema
    public string? CanalReporte { get; set; } = "Portal"; // Portal, Telefono, Email, Presencial
    public int? TarjetaRFIDAfectada { get; set; }
    public int? TransaccionAfectada { get; set; }
    public string? PlacaVehiculo { get; set; }
    public DateTime? FechaIncidente { get; set; }
    public string? EstacionIncidente { get; set; }
    public string? CarrilIncidente { get; set; }
    
    // Resolución
    public string? TipoSolucion { get; set; }
    public string? DescripcionSolucion { get; set; }
    public decimal? MontoAjuste { get; set; }
    public bool RequiereReembolso { get; set; } = false;
    public bool RequiereNuevaTargeta { get; set; } = false;
    
    // Calificación del servicio
    public int? CalificacionCliente { get; set; } // 1-5 estrellas
    public string? ComentarioCliente { get; set; }
    public DateTime? FechaCalificacion { get; set; }
    
    // Archivos adjuntos
    public string? ArchivosEvidencia { get; set; } // JSON con rutas de archivos
    
    // Observaciones internas
    public string? ObservacionesInternas { get; set; }

    // Navigation Properties
    public virtual Cliente Cliente { get; set; } = null!;
    public virtual TarjetaRFID? TarjetaRFIDAfectadaNavigation { get; set; }
    public virtual Transaccion? TransaccionAfectadaNavigation { get; set; }
    public virtual ICollection<TicketSoporteHistorial> Historial { get; set; } = new List<TicketSoporteHistorial>();
}
