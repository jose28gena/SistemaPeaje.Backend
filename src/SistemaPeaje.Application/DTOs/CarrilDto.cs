namespace SistemaPeaje.Application.DTOs;

public class CarrilDto
{
    public int Id { get; set; }
    public int NumeroCarril { get; set; }
    public string Estado { get; set; } = string.Empty;
    public int EstacionId { get; set; }
    public string? EstacionNombre { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaActualizacion { get; set; }
}

public class CreateCarrilDto
{
    public int NumeroCarril { get; set; }
    public string Estado { get; set; } = string.Empty;
    public int EstacionId { get; set; }
}

public class UpdateCarrilDto
{
    public int Id { get; set; }
    public int NumeroCarril { get; set; }
    public string Estado { get; set; } = string.Empty;
    public int EstacionId { get; set; }
}
