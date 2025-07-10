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

    public TarjetasRfidController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
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
        var tarjetas = await _unitOfWork.Repository<TarjetaRFID>()
            .GetAsync(t => t.NumeroTag == numeroTag);
        
        var tarjeta = tarjetas.FirstOrDefault();
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

    [HttpPost]
    public async Task<ActionResult<TarjetaRfidDto>> CreateTarjeta(CreateTarjetaRequest request)
    {
        var tarjeta = new TarjetaRFID
        {
            NumeroTag = request.NumeroTag,
            ClienteId = request.ClienteId,
            Saldo = request.SaldoInicial,
            FechaEmision = DateTime.UtcNow,
            FechaVencimiento = request.FechaVencimiento,
            Estado = "Activa",
            FechaCreacion = DateTime.UtcNow
        };

        await _unitOfWork.Repository<TarjetaRFID>().AddAsync(tarjeta);
        await _unitOfWork.SaveChangesAsync();

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
        var tarjeta = await _unitOfWork.Repository<TarjetaRFID>().GetByIdAsync(id);
        if (tarjeta == null)
            return NotFound();

        if (tarjeta.Estado != "Activa")
            return BadRequest("La tarjeta no está activa");

        tarjeta.Saldo += request.Monto;
        tarjeta.FechaActualizacion = DateTime.UtcNow;

        await _unitOfWork.Repository<TarjetaRFID>().UpdateAsync(tarjeta);
        await _unitOfWork.SaveChangesAsync();

        return Ok(new { NuevoSaldo = tarjeta.Saldo });
    }

    [HttpPost("{id}/bloquear")]
    public async Task<IActionResult> BloquearTarjeta(int id)
    {
        var tarjeta = await _unitOfWork.Repository<TarjetaRFID>().GetByIdAsync(id);
        if (tarjeta == null)
            return NotFound();

        tarjeta.Estado = "Bloqueada";
        tarjeta.FechaActualizacion = DateTime.UtcNow;

        await _unitOfWork.Repository<TarjetaRFID>().UpdateAsync(tarjeta);
        await _unitOfWork.SaveChangesAsync();

        return Ok();
    }

    [HttpPost("{id}/activar")]
    public async Task<IActionResult> ActivarTarjeta(int id)
    {
        var tarjeta = await _unitOfWork.Repository<TarjetaRFID>().GetByIdAsync(id);
        if (tarjeta == null)
            return NotFound();

        tarjeta.Estado = "Activa";
        tarjeta.FechaActualizacion = DateTime.UtcNow;

        await _unitOfWork.Repository<TarjetaRFID>().UpdateAsync(tarjeta);
        await _unitOfWork.SaveChangesAsync();

        return Ok();
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
