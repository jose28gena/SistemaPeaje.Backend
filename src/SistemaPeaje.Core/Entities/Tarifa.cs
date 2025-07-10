namespace SistemaPeaje.Core.Entities;

public class Tarifa : BaseEntity
{
    public int TipoVehiculoId { get; set; }
    public int? EstacionId { get; set; }
    public decimal Monto { get; set; }
    public DateTime FechaVigenciaInicio { get; set; }
    public DateTime? FechaVigenciaFin { get; set; }
    public bool EsVigente { get; set; } = true;

    // Navigation Properties
    public virtual TipoVehiculo? TipoVehiculo { get; set; }
    public virtual Estacion? Estacion { get; set; }
}
