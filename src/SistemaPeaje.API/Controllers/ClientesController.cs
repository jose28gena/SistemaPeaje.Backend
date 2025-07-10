using Microsoft.AspNetCore.Mvc;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;
using SistemaPeaje.Application.Mappings;

namespace SistemaPeaje.API.Controllers;

[ApiController]
[Route("api/[controller]")]
//[Authorize] // Comentado temporalmente para desarrollo
public class ClientesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public ClientesController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClienteDto>>> GetClientes()
    {
        var clientes = await _unitOfWork.Repository<Cliente>().GetAllAsync();
        var clientesDto = clientes.Select(c => new ClienteDto
        {
            Id = c.Id,
            Nombres = c.Nombres,
            Apellidos = c.Apellidos,
            Email = c.Email,
            Telefono = c.Telefono
        });
        return Ok(clientesDto);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ClienteDto>> GetCliente(int id)
    {
        var cliente = await _unitOfWork.Repository<Cliente>().GetByIdAsync(id);
        if (cliente == null)
            return NotFound();

        var clienteDto = new ClienteDto
        {
            Id = cliente.Id,
            Nombres = cliente.Nombres,
            Apellidos = cliente.Apellidos,
            Email = cliente.Email,
            Telefono = cliente.Telefono
        };

        return Ok(clienteDto);
    }

    [HttpPost]
    public async Task<ActionResult<ClienteDto>> CreateCliente(CreateClienteRequest request)
    {
        var cliente = new Cliente
        {
            Nombres = request.Nombres,
            Apellidos = request.Apellidos,
            NumeroDocumento = request.NumeroDocumento,
            TipoDocumento = request.TipoDocumento,
            Email = request.Email,
            Telefono = request.Telefono,
            Direccion = request.Direccion,
            FechaNacimiento = request.FechaNacimiento,
            FechaCreacion = DateTime.UtcNow
        };

        await _unitOfWork.Repository<Cliente>().AddAsync(cliente);
        await _unitOfWork.SaveChangesAsync();

        var clienteDto = new ClienteDto
        {
            Id = cliente.Id,
            Nombres = cliente.Nombres,
            Apellidos = cliente.Apellidos,
            Email = cliente.Email,
            Telefono = cliente.Telefono
        };

        return CreatedAtAction(nameof(GetCliente), new { id = cliente.Id }, clienteDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCliente(int id, UpdateClienteRequest request)
    {
        var cliente = await _unitOfWork.Repository<Cliente>().GetByIdAsync(id);
        if (cliente == null)
            return NotFound();

        cliente.Nombres = request.Nombres;
        cliente.Apellidos = request.Apellidos;
        cliente.NumeroDocumento = request.NumeroDocumento;
        cliente.TipoDocumento = request.TipoDocumento;
        cliente.Email = request.Email;
        cliente.Telefono = request.Telefono;
        cliente.Direccion = request.Direccion;
        cliente.FechaNacimiento = request.FechaNacimiento;
        cliente.FechaActualizacion = DateTime.UtcNow;

        await _unitOfWork.Repository<Cliente>().UpdateAsync(cliente);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCliente(int id)
    {
        var cliente = await _unitOfWork.Repository<Cliente>().GetByIdAsync(id);
        if (cliente == null)
            return NotFound();

        await _unitOfWork.Repository<Cliente>().DeleteAsync(cliente);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("{id}/tarjetas")]
    public async Task<ActionResult<IEnumerable<TarjetaRfidDto>>> GetTarjetasCliente(int id)
    {
        var tarjetas = await _unitOfWork.Repository<TarjetaRFID>()
            .GetAsync(t => t.ClienteId == id);

        var tarjetasDto = tarjetas.Select(t => new TarjetaRfidDto
        {
            Id = t.Id,
            NumeroTag = t.NumeroTag,
            Saldo = t.Saldo,
            Estado = t.Estado,
            FechaEmision = t.FechaEmision,
            FechaVencimiento = t.FechaVencimiento
        });

        return Ok(tarjetasDto);
    }
}

public class CreateClienteRequest
{
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string? NumeroDocumento { get; set; }
    public string? TipoDocumento { get; set; }
    public string? Email { get; set; }
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
    public DateTime? FechaNacimiento { get; set; }
}

public class UpdateClienteRequest
{
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string? NumeroDocumento { get; set; }
    public string? TipoDocumento { get; set; }
    public string? Email { get; set; }
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
    public DateTime? FechaNacimiento { get; set; }
}

public class TarjetaRfidDto
{
    public int Id { get; set; }
    public string NumeroTag { get; set; } = string.Empty;
    public int ClienteId { get; set; }
    public decimal Saldo { get; set; }
    public string Estado { get; set; } = string.Empty;
    public DateTime FechaEmision { get; set; }
    public DateTime? FechaVencimiento { get; set; }
    public string? ClienteNombre { get; set; }
}
