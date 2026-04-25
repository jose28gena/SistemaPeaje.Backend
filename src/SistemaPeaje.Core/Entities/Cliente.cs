namespace SistemaPeaje.Core.Entities;

public class Cliente : BaseEntity
{
    // Datos Básicos
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string? NumeroDocumento { get; set; }
    public string? TipoDocumento { get; set; } = "Cédula"; // Cédula, RUC, Pasaporte
    public string? Email { get; set; }
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
    public DateTime? FechaNacimiento { get; set; }
    
    // Datos Fiscales (para empresas)
    public string? RFC { get; set; }
    public string? RazonSocial { get; set; }
    public string? RegimenFiscal { get; set; }
    public string? DomicilioFiscal { get; set; }
    public string? EmailFacturacion { get; set; }
    public string? UsoCFDI { get; set; } // Para México
    public string? PreferenciaFacturacion { get; set; } = "PUE"; // PUE/PPD
    
    // Estado del Cliente (Flujo de Vida)
    public string EstadoCliente { get; set; } = "Prospecto"; // Prospecto → En validación → Aprobado → Suspendido → Cerrado
    public DateTime? FechaAprobacion { get; set; }
    public DateTime? FechaSuspension { get; set; }
    public string? MotivoSuspension { get; set; }
    public DateTime? FechaCierre { get; set; }
    public string? MotivoCierre { get; set; }
    
    // Configuración Comercial
    public string ModeloCuenta { get; set; } = "Prepago"; // Prepago, Pospago, Residente, Exento
    public decimal? TopeCredito { get; set; }
    public int? DiasCredito { get; set; }
    public DateTime? FechaCorteCredito { get; set; }
    public decimal? SaldoMinimo { get; set; } = 0;
    public bool AlertaSaldoBajo { get; set; } = true;
    public decimal? MontoAlertaSaldo { get; set; } = 50;
    
    // Facturación
    public string? PeriodicidadFacturacion { get; set; } = "Mensual"; // Diaria, Semanal, Mensual
    public string? SerieFacturacion { get; set; }
    
    // KYC y Documentos
    public bool DocumentosValidados { get; set; } = false;
    public DateTime? FechaValidacionDocumentos { get; set; }
    public string? ObservacionesKYC { get; set; }
    public DateTime? FechaVencimientoDocumentos { get; set; }
    
    // Auditoría
    public string? UsuarioCreacion { get; set; }
    public string? UsuarioAprobacion { get; set; }
    public DateTime? FechaConsentimiento { get; set; }
    
    // Tipo y Relaciones
    public int? TipoClienteId { get; set; }

    // Navigation Properties
    public virtual TipoCliente? TipoCliente { get; set; }
    public virtual ICollection<Transaccion> Transacciones { get; set; } = new List<Transaccion>();
    public virtual ICollection<TarjetaRFID> TarjetasRFID { get; set; } = new List<TarjetaRFID>();
    public virtual ICollection<ClienteVehiculo> Vehiculos { get; set; } = new List<ClienteVehiculo>();
    public virtual ICollection<ClienteDocumento> Documentos { get; set; } = new List<ClienteDocumento>();
    public virtual ICollection<ClienteRecarga> Recargas { get; set; } = new List<ClienteRecarga>();
    public virtual ICollection<ClienteFactura> Facturas { get; set; } = new List<ClienteFactura>();
    public virtual ICollection<TicketSoporte> TicketsSoporte { get; set; } = new List<TicketSoporte>();
}
