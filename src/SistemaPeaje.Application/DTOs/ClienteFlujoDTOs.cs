namespace SistemaPeaje.Application.DTOs;

// DTO Principal del Cliente Extendido
public class ClienteCompletaDto
{
    public int Id { get; set; }
    
    // Datos Básicos
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string? NumeroDocumento { get; set; }
    public string? TipoDocumento { get; set; }
    public string? Email { get; set; }
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
    public DateTime? FechaNacimiento { get; set; }
    
    // Datos Fiscales
    public string? RFC { get; set; }
    public string? RazonSocial { get; set; }
    public string? RegimenFiscal { get; set; }
    public string? DomicilioFiscal { get; set; }
    public string? EmailFacturacion { get; set; }
    public string? UsoCFDI { get; set; }
    public string? PreferenciaFacturacion { get; set; }
    
    // Estado del Cliente
    public string EstadoCliente { get; set; } = "Prospecto";
    public DateTime? FechaAprobacion { get; set; }
    public DateTime? FechaSuspension { get; set; }
    public string? MotivoSuspension { get; set; }
    public DateTime? FechaCierre { get; set; }
    public string? MotivoCierre { get; set; }
    
    // Configuración Comercial
    public string ModeloCuenta { get; set; } = "Prepago";
    public decimal? TopeCredito { get; set; }
    public int? DiasCredito { get; set; }
    public DateTime? FechaCorteCredito { get; set; }
    public decimal? SaldoMinimo { get; set; }
    public bool AlertaSaldoBajo { get; set; }
    public decimal? MontoAlertaSaldo { get; set; }
    
    // Facturación
    public string? PeriodicidadFacturacion { get; set; }
    public string? SerieFacturacion { get; set; }
    
    // KYC
    public bool DocumentosValidados { get; set; }
    public DateTime? FechaValidacionDocumentos { get; set; }
    public string? ObservacionesKYC { get; set; }
    public DateTime? FechaVencimientoDocumentos { get; set; }
    
    // Tipo Cliente
    public int? TipoClienteId { get; set; }
    public string? TipoClienteNombre { get; set; }
    
    // Auditoría
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaActualizacion { get; set; }
    public bool Activo { get; set; }
    public string? UsuarioCreacion { get; set; }
    public string? UsuarioAprobacion { get; set; }
    public DateTime? FechaConsentimiento { get; set; }
    
    // Datos de navegación
    public List<ClienteVehiculoDto> Vehiculos { get; set; } = new();
    public List<TarjetaRfidDto> TarjetasRFID { get; set; } = new();
    public List<ClienteDocumentoDto> Documentos { get; set; } = new();
    public List<TicketSoporteDto> TicketsAbiertos { get; set; } = new();
    
    // Resumen de estado
    public ResumenClienteDto Resumen { get; set; } = new();
}

// DTO para vehículos del cliente
public class ClienteVehiculoDto
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public string Placa { get; set; } = string.Empty;
    public string? Marca { get; set; }
    public string? Modelo { get; set; }
    public int? Año { get; set; }
    public string? Color { get; set; }
    public string? VIN { get; set; }
    public string ClaseVehiculo { get; set; } = string.Empty;
    public string SubClaseVehiculo { get; set; } = string.Empty;
    public string? Propietario { get; set; }
    public bool EsPrincipal { get; set; }
    public string Estado { get; set; } = "Activo";
    public DateTime? FechaRegistro { get; set; }
    public DateTime? FechaVencimientoDocumentos { get; set; }
    public List<TarjetaRfidDto> TarjetasAsociadas { get; set; } = new();
}

// DTO para documentos del cliente
public class ClienteDocumentoDto
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public string TipoDocumento { get; set; } = string.Empty;
    public string NombreArchivo { get; set; } = string.Empty;
    public string? UrlArchivo { get; set; }
    public DateTime FechaSubida { get; set; }
    public DateTime? FechaVencimiento { get; set; }
    public string EstadoValidacion { get; set; } = "Pendiente";
    public DateTime? FechaValidacion { get; set; }
    public string? UsuarioValidacion { get; set; }
    public string? ObservacionesValidacion { get; set; }
    public string? MotivoRechazo { get; set; }
    public bool EsObligatorio { get; set; }
    public int VersionDocumento { get; set; }
}

// DTO para tickets de soporte
public class TicketSoporteDto
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public string NumeroTicket { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string Prioridad { get; set; } = "Media";
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string Estado { get; set; } = "Abierto";
    public DateTime? FechaApertura { get; set; }
    public DateTime? FechaResolucion { get; set; }
    public string? UsuarioAsignado { get; set; }
    public string? TipoSolucion { get; set; }
}

// DTO para recargas
public class ClienteRecargaDto
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public int? TarjetaRFIDId { get; set; }
    public string? NumeroTag { get; set; }
    public decimal Monto { get; set; }
    public string MedioPago { get; set; } = string.Empty;
    public string? Referencia { get; set; }
    public DateTime FechaRecarga { get; set; }
    public string Estado { get; set; } = "Registrada";
    public DateTime? FechaConfirmacion { get; set; }
    public string? UsuarioCreacion { get; set; }
    public string? Observaciones { get; set; }
}

// DTO para resumen del estado del cliente
public class ResumenClienteDto
{
    public int TotalVehiculos { get; set; }
    public int TotalTarjetas { get; set; }
    public int TarjetasActivas { get; set; }
    public int TarjetasBloqueadas { get; set; }
    public decimal SaldoTotal { get; set; }
    public int DocumentosPendientes { get; set; }
    public int DocumentosAprobados { get; set; }
    public int TicketsAbiertos { get; set; }
    public decimal MontoRecargadoMes { get; set; }
    public int CrucesMes { get; set; }
    public DateTime? UltimaActividad { get; set; }
    public bool RequiereAtencion { get; set; }
    public string? MotivoAtencion { get; set; }
}
