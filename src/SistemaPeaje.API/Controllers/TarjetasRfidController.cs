using Microsoft.AspNetCore.Mvc;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;
using SistemaPeaje.Application.Interfaces;
using SistemaPeaje.Application.DTOs;

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
            var tarjeta = await _tarjetaRfidService.ObtenerTarjetaPorTagAsync(numeroTag);
            
            if (tarjeta == null)
            {
                return NotFound($"Tarjeta con tag {numeroTag} no encontrada");
            }

            return Ok(tarjeta);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al obtener tarjeta: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<ActionResult<TarjetaRfidDto>> CreateTarjeta(CreateTarjetaRequest request)
    {
        try
        {
            var createDto = new CreateTarjetaRfidDto
            {
                NumeroTag = request.NumeroTag,
                ClienteId = request.ClienteId,
                SaldoInicial = request.SaldoInicial
            };

            var tarjeta = await _tarjetaRfidService.CrearTarjetaAsync(createDto);
            
            return CreatedAtAction(nameof(GetTarjetaPorTag), new { numeroTag = tarjeta.NumeroTag }, tarjeta);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al crear tarjeta: {ex.Message}");
        }
    }

    [HttpPost("{id}/recargar")]
    public async Task<IActionResult> RecargarTarjeta(int id, RecargaTarjetaRequest request)
    {
        try
        {
            var recargaDto = new RecargaTarjetaDto
            {
                Monto = request.Monto,
                MetodoPago = "Efectivo", // O mapear desde request si está disponible
                Observaciones = $"Recarga de ${request.Monto}",
                EmpleadoId = 1 // Esto debería venir del usuario autenticado
            };

            var tarjeta = await _tarjetaRfidService.RecargarTarjetaAsync(id, recargaDto);
            
            return Ok(new { 
                NuevoSaldo = tarjeta.Saldo,
                Mensaje = "Recarga realizada exitosamente"
            });
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al recargar tarjeta: {ex.Message}");
        }
    }

    [HttpPost("{id}/bloquear")]
    public async Task<IActionResult> BloquearTarjeta(int id)
    {
        try
        {
            int empleadoId = 1; // Esto debería venir del usuario autenticado
            string motivo = "Bloqueado desde administración";
            
            var tarjeta = await _tarjetaRfidService.BloquearTarjetaAsync(id, empleadoId, motivo);
            
            return Ok(new { 
                Estado = tarjeta.Estado,
                Mensaje = "Tarjeta bloqueada exitosamente" 
            });
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al bloquear tarjeta: {ex.Message}");
        }
    }

    [HttpPost("{id}/activar")]
    public async Task<IActionResult> ActivarTarjeta(int id)
    {
        try
        {
            int empleadoId = 1; // Esto debería venir del usuario autenticado
            
            var tarjeta = await _tarjetaRfidService.ActivarTarjetaAsync(id, empleadoId);
            
            return Ok(new { 
                Estado = tarjeta.Estado,
                Mensaje = "Tarjeta activada exitosamente" 
            });
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al activar tarjeta: {ex.Message}");
        }
    }
    
    // Nuevos endpoints que utilizan los servicios de tarjeta RFID
    
    [HttpGet("cliente/{clienteId}")]
    public async Task<ActionResult<IEnumerable<TarjetaRfidDto>>> GetTarjetasByCliente(int clienteId)
    {
        try
        {
            var tarjetas = await _tarjetaRfidService.ObtenerTarjetasPorClienteAsync(clienteId);
            
            if (!tarjetas.Any())
                return NotFound($"No se encontraron tarjetas para el cliente con ID {clienteId}");
                
            return Ok(tarjetas);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al obtener tarjetas: {ex.Message}");
        }
    }
    
    [HttpGet("tag/{numeroTag}/saldo")]
    public async Task<ActionResult<decimal>> ConsultarSaldo(string numeroTag)
    {
        try
        {
            var saldo = await _tarjetaRfidService.ObtenerSaldoAsync(numeroTag);
            return Ok(new { Saldo = saldo });
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al consultar saldo: {ex.Message}");
        }
    }
    
    [HttpPost("tag/{numeroTag}/validar-saldo")]
    public async Task<ActionResult> ValidarSaldo(string numeroTag, [FromBody] ValidarSaldoRequest request)
    {
        try
        {
            var resultado = await _tarjetaRfidService.ValidarSaldoSuficienteAsync(numeroTag, request.Monto);
            return Ok(new { SaldoSuficiente = resultado });
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al validar saldo: {ex.Message}");
        }
    }
    
    [HttpPost("tag/{numeroTag}/debitar")]
    public async Task<ActionResult> DebitarSaldo(string numeroTag, [FromBody] DebitarSaldoRequest request)
    {
        try
        {
            // Generar un ID de transacción temporal
            int transaccionId = DateTime.Now.Millisecond; // Esto debería venir de la transacción real
            
            var resultado = await _tarjetaRfidService.DebitarSaldoAsync(numeroTag, request.Monto, transaccionId);
            
            if (!resultado)
                return BadRequest("No se pudo debitar el saldo. Verifique que la tarjeta esté activa y tenga saldo suficiente.");
                
            // Consultamos el nuevo saldo
            decimal nuevoSaldo = await _tarjetaRfidService.ObtenerSaldoAsync(numeroTag);
            
            return Ok(new { Resultado = resultado, NuevoSaldo = nuevoSaldo });
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al debitar saldo: {ex.Message}");
        }
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
