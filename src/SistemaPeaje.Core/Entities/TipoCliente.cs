namespace SistemaPeaje.Core.Entities;

public class TipoCliente : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public bool EstaExentoPago { get; set; } = false;
    public decimal? DescuentoPorcentaje { get; set; }
    public bool RequiereValidacionDocumento { get; set; } = false;
    public string? DocumentosRequeridos { get; set; }
    public int? VigenciaMeses { get; set; }

    // Navigation Properties
    public virtual ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();
}
