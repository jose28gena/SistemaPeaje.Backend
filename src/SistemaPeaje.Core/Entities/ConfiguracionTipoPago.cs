using System.ComponentModel.DataAnnotations;

namespace SistemaPeaje.Core.Entities;

/// <summary>
/// Configuración específica para cada tipo de pago en el sistema
/// Permite definir parámetros como moneda, límites diarios, comisiones, etc.
/// </summary>
public class ConfiguracionTipoPago : BaseEntity
{
    /// <summary>
    /// Relación con el tipo de pago
    /// </summary>
    public int TipoPagoId { get; set; }

    /// <summary>
    /// Moneda utilizada para este tipo de pago
    /// </summary>
    [Required]
    [StringLength(10)]
    public string Moneda { get; set; } = "MXN"; // Pesos mexicanos por defecto

    /// <summary>
    /// Símbolo de la moneda
    /// </summary>
    [StringLength(5)]
    public string SimboloMoneda { get; set; } = "$";

    /// <summary>
    /// Límite diario para transacciones de este tipo de pago
    /// </summary>
    public decimal? LimiteDiario { get; set; }

    /// <summary>
    /// Límite por transacción individual
    /// </summary>
    public decimal? LimiteTransaccion { get; set; }

    /// <summary>
    /// Comisión aplicada como porcentaje (ej: 2.5 = 2.5%)
    /// </summary>
    public decimal ComisionPorcentaje { get; set; } = 0;

    /// <summary>
    /// Comisión fija por transacción
    /// </summary>
    public decimal ComisionFija { get; set; } = 0;

    /// <summary>
    /// Indica si este tipo de pago está activo para nuevas transacciones
    /// </summary>
    public bool EstaActivo { get; set; } = true;

    /// <summary>
    /// Requiere validación adicional (ej: PIN, firma, etc.)
    /// </summary>
    public bool RequiereValidacionAdicional { get; set; } = false;

    /// <summary>
    /// Tiempo de espera para procesamiento en segundos
    /// </summary>
    public int TiempoEsperaSegundos { get; set; } = 30;

    /// <summary>
    /// Descuento aplicable por defecto (porcentaje)
    /// </summary>
    public decimal DescuentoPorDefecto { get; set; } = 0;

    /// <summary>
    /// Permite transacciones parciales
    /// </summary>
    public bool PermiteTransaccionesParcialeS { get; set; } = false;

    /// <summary>
    /// Configuración específica en formato JSON
    /// Para almacenar parámetros adicionales específicos del tipo de pago
    /// </summary>
    [StringLength(1000)]
    public string? ConfiguracionEspecifica { get; set; }

    /// <summary>
    /// Observaciones adicionales
    /// </summary>
    [StringLength(500)]
    public string? Observaciones { get; set; }

    // Navigation Properties
    public virtual TipoPago TipoPago { get; set; } = null!;
}
