namespace SistemaPeaje.Application.DTOs;

public class TipoClienteDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public bool EstaExentoPago { get; set; }
    public decimal? DescuentoPorcentaje { get; set; }
    public bool RequiereValidacionDocumento { get; set; }
    public string? DocumentosRequeridos { get; set; }
    public int? VigenciaMeses { get; set; }
    public bool EsActivo { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaActualizacion { get; set; }
}
