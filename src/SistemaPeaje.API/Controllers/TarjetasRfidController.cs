using Microsoft.AspNetCore.Mvc;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;

namespace SistemaPeaje.API.Controllers;

[ApiController]
[Route("api/[controller]")]
//[Authorize] // Comentado temporalmente para desarrollo
public class TarjetasRfidController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITarjetaRfidService _tarjetaRfidService;

    public TarjetasRfidController(
        IUnitOfWork unitOfWork,
        ITarjetaRfidService tarjetaRfidService)
    {
        _unitOfWork = unitOfWork;
        _tarjetaRfidService = tarjetaRfidService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TarjetaRfidDto>>> GetTarjetas()
    {
        var tarjetas = await _unitOfWork.Repository<TarjetaRFID>().GetAllAsync();
        var tarjetasDto = tarjetas.Select(t => new TarjetaRfidDto
        {
            Id = t.Id,
            NumeroTag = t.NumeroTag,
            ClienteId = t.ClienteId,
            Saldo = t.Saldo,
            Estado = t.Estado,
            FechaEmision = t.FechaEmision,
            FechaVencimiento = t.FechaVencimiento
        });
        return Ok(tarjetasDto);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TarjetaRfidDto>> GetTarjeta(int id)
    {
        var tarjeta = await _unitOfWork.Repository<TarjetaRFID>().GetByIdAsync(id);
        if (tarjeta == null)
            return NotFound();

        var tarjetaDto = new TarjetaRfidDto
        {
            Id = tarjeta.Id,
            NumeroTag = tarjeta.NumeroTag,
            ClienteId = tarjeta.ClienteId,
            Saldo = tarjeta.Saldo,
            Estado = tarjeta.Estado,
            FechaEmision = tarjeta.FechaEmision,
            FechaVencimiento = tarjeta.FechaVencimiento
        };

        return Ok(tarjetaDto);
    }

    [HttpGet("tag/{numeroTag}")]
    public async Task<ActionResult<TarjetaRfidDto>> GetTarjetaPorTag(string numeroTag)
    {
        try 
        {
            var tarjeta = await _tarjetaRfidService.GetByNumeroTagAsync(numeroTag);
            
            var tarjetaDto = new TarjetaRfidDto
            {
                Id = tarjeta.Id,
                NumeroTag = tarjeta.NumeroTag,
                ClienteId = tarjeta.ClienteId,
                Saldo = tarjeta.Saldo,
                Estado = tarjeta.Estado,
                FechaEmision = tarjeta.FechaEmision,
                FechaVencimiento = tarjeta.FechaVencimiento
            };

            return Ok(tarjetaDto);
        }
        catch (KeyNotFoundException)
        {
            return NotFound($"Tarjeta con tag {numeroTag} no encontrada");
        }
    }

    [HttpPost]
    public async Task<ActionResult<TarjetaRfidDto>> CreateTarjeta(CreateTarjetaRequest request)
    {
        var tarjeta = await _tarjetaRfidService.CrearTarjetaAsync(
            request.NumeroTag,
            request.ClienteId,
            request.SaldoInicial);
            
        if (tarjeta.Id == 0) // Verificamos si se trata de un objeto vacío (error)
        {
            return BadRequest("No se pudo crear la tarjeta. Verifique que el número de tag sea único y el cliente exista.");
        }

        var tarjetaDto = new TarjetaRfidDto
        {
            Id = tarjeta.Id,
            NumeroTag = tarjeta.NumeroTag,
            ClienteId = tarjeta.ClienteId,
            Saldo = tarjeta.Saldo,
            Estado = tarjeta.Estado,
            FechaEmision = tarjeta.FechaEmision,
            FechaVencimiento = tarjeta.FechaVencimiento
        };

        return CreatedAtAction(nameof(GetTarjeta), new { id = tarjeta.Id }, tarjetaDto);
    }

    [HttpPost("{id}/recargar")]
    public async Task<IActionResult> RecargarTarjeta(int id, RecargaTarjetaRequest request)
    {
        // Primero obtenemos el número de tag a partir del ID
        var tarjeta = await _unitOfWork.Repository<TarjetaRFID>().GetByIdAsync(id);
        if (tarjeta == null)
            return NotFound();
            
        string numeroTag = tarjeta.NumeroTag;

        // Ahora usamos el servicio para recargar
        bool resultado = await _tarjetaRfidService.RecargarSaldoAsync(numeroTag, request.Monto);
        if (!resultado)
            return BadRequest("No se pudo realizar la recarga. La tarjeta puede estar bloqueada o no existir.");
        
        // Consultamos el nuevo saldo
        decimal nuevoSaldo = await _tarjetaRfidService.ConsultarSaldoAsync(numeroTag);
        
        return Ok(new { NuevoSaldo = nuevoSaldo });
    }

    [HttpPost("{id}/bloquear")]
    public async Task<IActionResult> BloquearTarjeta(int id)
    {
        // Primero obtenemos el número de tag a partir del ID
        var tarjeta = await _unitOfWork.Repository<TarjetaRFID>().GetByIdAsync(id);
        if (tarjeta == null)
            return NotFound();
            
        string numeroTag = tarjeta.NumeroTag;

        bool resultado = await _tarjetaRfidService.BloquearTarjetaAsync(numeroTag);
        if (!resultado)
            return BadRequest("No se pudo bloquear la tarjeta");

        return Ok(new { Estado = "Bloqueada" });
    }

    [HttpPost("{id}/activar")]
    public async Task<IActionResult> ActivarTarjeta(int id)
    {
        // Primero obtenemos el número de tag a partir del ID
        var tarjeta = await _unitOfWork.Repository<TarjetaRFID>().GetByIdAsync(id);
        if (tarjeta == null)
            return NotFound();
            
        string numeroTag = tarjeta.NumeroTag;

        bool resultado = await _tarjetaRfidService.DesbloquearTarjetaAsync(numeroTag);
        if (!resultado)
            return BadRequest("No se pudo activar la tarjeta");

        return Ok(new { Estado = "Activa" });
    }
    
    // Nuevos endpoints que utilizan los servicios de tarjeta RFID
    
    [HttpGet("cliente/{clienteId}")]
    public async Task<ActionResult<IEnumerable<TarjetaRfidDto>>> GetTarjetasByCliente(int clienteId)
    {
        var tarjetas = await _tarjetaRfidService.GetTarjetasByClienteIdAsync(clienteId);
        
        if (!tarjetas.Any())
            return NotFound($"No se encontraron tarjetas para el cliente con ID {clienteId}");
            
        var tarjetasDto = tarjetas.Select(t => new TarjetaRfidDto
        {
            Id = t.Id,
            NumeroTag = t.NumeroTag,
            ClienteId = t.ClienteId,
            Saldo = t.Saldo,
            Estado = t.Estado,
            FechaEmision = t.FechaEmision,
            FechaVencimiento = t.FechaVencimiento
        });
        
        return Ok(tarjetasDto);
    }
    
    [HttpGet("tag/{numeroTag}/saldo")]
    public async Task<ActionResult<decimal>> ConsultarSaldo(string numeroTag)
    {
        try
        {
            var saldo = await _tarjetaRfidService.ConsultarSaldoAsync(numeroTag);
            return Ok(new { Saldo = saldo });
        }
        catch (Exception ex)
        {
            return NotFound($"Error al consultar saldo: {ex.Message}");
        }
    }
    
    [HttpPost("tag/{numeroTag}/validar-saldo")]
    public async Task<ActionResult> ValidarSaldo(string numeroTag, [FromBody] ValidarSaldoRequest request)
    {
        var resultado = await _tarjetaRfidService.ValidarSaldoSuficienteAsync(numeroTag, request.Monto);
        return Ok(new { SaldoSuficiente = resultado });
    }
    
    [HttpPost("tag/{numeroTag}/debitar")]
    public async Task<ActionResult> DebitarSaldo(string numeroTag, [FromBody] DebitarSaldoRequest request)
    {
        var resultado = await _tarjetaRfidService.DebitarSaldoAsync(numeroTag, request.Monto);
        
        if (!resultado)
            return BadRequest("No se pudo debitar el saldo. Verifique que la tarjeta esté activa y tenga saldo suficiente.");
            
        // Consultamos el nuevo saldo
        decimal nuevoSaldo = await _tarjetaRfidService.ConsultarSaldoAsync(numeroTag);
        
        return Ok(new { Resultado = resultado, NuevoSaldo = nuevoSaldo });
    }
}

public class CreateTarjetaRequest
{
    public string NumeroTag { get; set; } = string.Empty;
    public int ClienteId { get; set; }
    public decimal SaldoInicial { get; set; }
    public DateTime? FechaVencimiento { get; set; }
}

public class RecargaTarjetaRequest
{
    public decimal Monto { get; set; }
}

public class ValidarSaldoRequest
{
    public decimal Monto { get; set; }
}

public class DebitarSaldoRequest
{
    public decimal Monto { get; set; }
}
