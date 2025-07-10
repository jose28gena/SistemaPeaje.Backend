namespace SistemaPeaje.Core.Entities;

public class Cliente : BaseEntity
{
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string? NumeroDocumento { get; set; }
    public string? TipoDocumento { get; set; }
    public string? Email { get; set; }
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
    public DateTime? FechaNacimiento { get; set; }
    public int? TipoClienteId { get; set; }

    // Navigation Properties
    public virtual TipoCliente? TipoCliente { get; set; }
    public virtual ICollection<Transaccion> Transacciones { get; set; } = new List<Transaccion>();
    public virtual ICollection<TarjetaRFID> TarjetasRFID { get; set; } = new List<TarjetaRFID>();
}
