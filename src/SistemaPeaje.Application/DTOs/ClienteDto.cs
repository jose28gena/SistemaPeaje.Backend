namespace SistemaPeaje.Application.DTOs;

public class ClienteDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Apellido { get; set; }
    public string? Email { get; set; }
    public string? Telefono { get; set; }
    public string? Documento { get; set; }
    public int TipoClienteId { get; set; }
    public string TipoClienteNombre { get; set; } = string.Empty;
    public bool Activo { get; set; }
    public DateTime FechaCreacion { get; set; }
}
