namespace SistemaPeaje.Core.Entities;

public class TipoVehiculo : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public int NumeroEjes { get; set; }
    public decimal TarifaBase { get; set; }
    public string Categoria { get; set; } = string.Empty; // "LIVIANO", "PESADO", "ESPECIAL"
    public bool EsActivo { get; set; } = true;

    // Navigation Properties
    public virtual ICollection<Transaccion> Transacciones { get; set; } = new List<Transaccion>();
    public virtual ICollection<Tarifa> Tarifas { get; set; } = new List<Tarifa>();
}
