namespace SistemaPeaje.Core.Entities;

public class TipoPago : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public bool RequiereEfectivo { get; set; }
    public bool RequiereTarjeta { get; set; }
    public bool RequiereTag { get; set; }
    public bool RequiereAutorizacion { get; set; } = false;
    public decimal? LimiteCredito { get; set; }
    public bool EsActivo { get; set; } = true;

    // Navigation Properties
    public virtual ICollection<Transaccion> Transacciones { get; set; } = new List<Transaccion>();
    public virtual ConfiguracionTipoPago? Configuracion { get; set; }
}
