namespace SistemaPeaje.Core.Entities;

public class ClienteDocumento : BaseEntity
{
    public int ClienteId { get; set; }
    public string TipoDocumento { get; set; } = string.Empty; // INE, Comprobante_Domicilio, Acta_Constitutiva, Poder_Legal, etc.
    public string NombreArchivo { get; set; } = string.Empty;
    public string RutaArchivo { get; set; } = string.Empty;
    public string? UrlArchivo { get; set; }
    public string TipoMime { get; set; } = string.Empty;
    public long TamañoArchivo { get; set; }
    public DateTime FechaSubida { get; set; }
    public DateTime? FechaVencimiento { get; set; }
    public string EstadoValidacion { get; set; } = "Pendiente"; // Pendiente, En_Revision, Aprobado, Rechazado
    public DateTime? FechaValidacion { get; set; }
    public string? UsuarioValidacion { get; set; }
    public string? ObservacionesValidacion { get; set; }
    public string? MotivoRechazo { get; set; }
    public bool EsObligatorio { get; set; } = false;
    public int VersionDocumento { get; set; } = 1;

    // Navigation Properties
    public virtual Cliente Cliente { get; set; } = null!;
}
