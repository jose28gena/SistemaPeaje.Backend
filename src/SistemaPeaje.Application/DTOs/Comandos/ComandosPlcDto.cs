using System.ComponentModel.DataAnnotations;

namespace SistemaPeaje.Application.DTOs.Comandos;

public class ComandoAbrirBarreraDto
{
    [Required(ErrorMessage = "La IP de la caseta es requerida")]
    [RegularExpression(@"^(?:[0-9]{1,3}\.){3}[0-9]{1,3}$", ErrorMessage = "Formato de IP inválido")]
    public string CasetaIp { get; set; } = string.Empty;

    [Required(ErrorMessage = "La dirección del coil es requerida")]
    [Range(0, 65535, ErrorMessage = "La dirección del coil debe estar entre 0 y 65535")]
    public ushort CoilAddress { get; set; }

    [Range(1, 247, ErrorMessage = "El Unit ID debe estar entre 1 y 247")]
    public byte UnitId { get; set; } = 1;

    public int? CarrilId { get; set; }
    public string? Observaciones { get; set; }
}

public class ComandoCerrarBarreraDto
{
    [Required(ErrorMessage = "La IP de la caseta es requerida")]
    [RegularExpression(@"^(?:[0-9]{1,3}\.){3}[0-9]{1,3}$", ErrorMessage = "Formato de IP inválido")]
    public string CasetaIp { get; set; } = string.Empty;

    [Required(ErrorMessage = "La dirección del coil es requerida")]
    [Range(0, 65535, ErrorMessage = "La dirección del coil debe estar entre 0 y 65535")]
    public ushort CoilAddress { get; set; }

    [Range(1, 247, ErrorMessage = "El Unit ID debe estar entre 1 y 247")]
    public byte UnitId { get; set; } = 1;

    public int? CarrilId { get; set; }
    public string? Observaciones { get; set; }
}

public class ComandoTestConexionDto
{
    [Required(ErrorMessage = "La IP de la caseta es requerida")]
    [RegularExpression(@"^(?:[0-9]{1,3}\.){3}[0-9]{1,3}$", ErrorMessage = "Formato de IP inválido")]
    public string CasetaIp { get; set; } = string.Empty;

    [Range(1, 65535, ErrorMessage = "El puerto debe estar entre 1 y 65535")]
    public int Puerto { get; set; } = 502;

    [Range(1, 247, ErrorMessage = "El Unit ID debe estar entre 1 y 247")]
    public byte UnitId { get; set; } = 1;
}

public class ResultadoComandoDto
{
    public bool Exitoso { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public DateTime FechaEjecucion { get; set; } = DateTime.UtcNow;
    public string? CodigoError { get; set; }
    public Dictionary<string, object>? DatosAdicionales { get; set; }
}
