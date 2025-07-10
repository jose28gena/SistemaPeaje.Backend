namespace SistemaPeaje.Core.Entities;

public class Estacion : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string Ubicacion { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public double? Latitud { get; set; }
    public double? Longitud { get; set; }

    // Navigation Properties
    public virtual ICollection<Carril> Carriles { get; set; } = new List<Carril>();
    public virtual ICollection<Transaccion> Transacciones { get; set; } = new List<Transaccion>();
}
