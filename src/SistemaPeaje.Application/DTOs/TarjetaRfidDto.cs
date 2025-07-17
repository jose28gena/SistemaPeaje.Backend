namespace SistemaPeaje.Application.DTOs;

public class TarjetaRfidDto
{
    public int Id { get; set; }
    public string NumeroTag { get; set; } = string.Empty;
    public int ClienteId { get; set; }
    public decimal Saldo { get; set; }
    public string Estado { get; set; } = string.Empty;
    public DateTime FechaEmision { get; set; }
    public DateTime? FechaVencimiento { get; set; }
    public string? NombreCliente { get; set; }
}
