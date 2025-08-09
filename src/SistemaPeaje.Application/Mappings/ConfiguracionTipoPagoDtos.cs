namespace SistemaPeaje.Application.Mappings;

/// <summary>
/// DTO para configuración de tipos de pago
/// </summary>
public class ConfiguracionTipoPagoDto
{
    public int Id { get; set; }
    public int TipoPagoId { get; set; }
    public string TipoPagoNombre { get; set; } = string.Empty;
    public string Moneda { get; set; } = string.Empty;
    public string SimboloMoneda { get; set; } = string.Empty;
    public decimal? LimiteDiario { get; set; }
    public decimal? LimiteTransaccion { get; set; }
    public decimal ComisionPorcentaje { get; set; }
    public decimal ComisionFija { get; set; }
    public bool EstaActivo { get; set; }
    public bool RequiereValidacionAdicional { get; set; }
    public int TiempoEsperaSegundos { get; set; }
    public decimal DescuentoPorDefecto { get; set; }
    public bool PermiteTransaccionesParcialeS { get; set; }
    public string? ConfiguracionEspecifica { get; set; }
    public string? Observaciones { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaActualizacion { get; set; }
}

/// <summary>
/// DTO para crear configuración de tipo de pago
/// </summary>
public class CreateConfiguracionTipoPagoDto
{
    public int TipoPagoId { get; set; }
    public string Moneda { get; set; } = "MXN";
    public string SimboloMoneda { get; set; } = "$";
    public decimal? LimiteDiario { get; set; }
    public decimal? LimiteTransaccion { get; set; }
    public decimal ComisionPorcentaje { get; set; } = 0;
    public decimal ComisionFija { get; set; } = 0;
    public bool EstaActivo { get; set; } = true;
    public bool RequiereValidacionAdicional { get; set; } = false;
    public int TiempoEsperaSegundos { get; set; } = 30;
    public decimal DescuentoPorDefecto { get; set; } = 0;
    public bool PermiteTransaccionesParcialeS { get; set; } = false;
    public string? ConfiguracionEspecifica { get; set; }
    public string? Observaciones { get; set; }
}

/// <summary>
/// DTO para actualizar configuración de tipo de pago
/// </summary>
public class UpdateConfiguracionTipoPagoDto : CreateConfiguracionTipoPagoDto
{
    public int Id { get; set; }
}

/// <summary>
/// DTO para tipos de pago con configuración completa
/// </summary>
public class TipoPagoConConfiguracionDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public bool RequiereEfectivo { get; set; }
    public bool RequiereTarjeta { get; set; }
    public bool RequiereTag { get; set; }
    public bool RequiereAutorizacion { get; set; }
    public decimal? LimiteCredito { get; set; }
    public bool EsActivo { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaActualizacion { get; set; }
    
    // Configuración específica
    public ConfiguracionTipoPagoDto? Configuracion { get; set; }
}
