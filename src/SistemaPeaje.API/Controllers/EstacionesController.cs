using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaPeaje.Application.Mappings;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;

namespace SistemaPeaje.API.Controllers;

[ApiController]
[Route("api/[controller]")]
//[Authorize] // Comentado temporalmente para desarrollo
public class EstacionesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public EstacionesController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EstacionDto>>> GetEstaciones()
    {
        var estaciones = await _unitOfWork.Repository<Estacion>().GetAllAsync();
        var estacionesDto = estaciones.Select(e => new EstacionDto
        {
            Id = e.Id,
            Nombre = e.Nombre,
            Ubicacion = e.Ubicacion,
            Descripcion = e.Descripcion
        });
        return Ok(estacionesDto);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EstacionDto>> GetEstacion(int id)
    {
        var estacion = await _unitOfWork.Repository<Estacion>().GetByIdAsync(id);
        if (estacion == null)
        {
            return NotFound();
        }

        var estacionDto = new EstacionDto
        {
            Id = estacion.Id,
            Nombre = estacion.Nombre,
            Ubicacion = estacion.Ubicacion,
            Descripcion = estacion.Descripcion
        };

        return Ok(estacionDto);
    }

    [HttpPost]
    public async Task<ActionResult<EstacionDto>> CreateEstacion(EstacionDto estacionDto)
    {
        var estacion = new Estacion
        {
            Nombre = estacionDto.Nombre,
            Ubicacion = estacionDto.Ubicacion,
            Descripcion = estacionDto.Descripcion,
            FechaCreacion = DateTime.UtcNow
        };

        await _unitOfWork.Repository<Estacion>().AddAsync(estacion);
        await _unitOfWork.SaveChangesAsync();

        estacionDto.Id = estacion.Id;
        return CreatedAtAction(nameof(GetEstacion), new { id = estacion.Id }, estacionDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEstacion(int id, EstacionDto estacionDto)
    {
        if (id != estacionDto.Id)
        {
            return BadRequest();
        }

        var estacion = await _unitOfWork.Repository<Estacion>().GetByIdAsync(id);
        if (estacion == null)
        {
            return NotFound();
        }

        estacion.Nombre = estacionDto.Nombre;
        estacion.Ubicacion = estacionDto.Ubicacion;
        estacion.Descripcion = estacionDto.Descripcion;
        estacion.FechaActualizacion = DateTime.UtcNow;

        await _unitOfWork.Repository<Estacion>().UpdateAsync(estacion);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("{id}/carriles")]
    public async Task<ActionResult<IEnumerable<CarrilDto>>> GetCarrilesByEstacion(int id)
    {
        var estacion = await _unitOfWork.Repository<Estacion>().GetByIdAsync(id);
        if (estacion == null)
        {
            return NotFound($"Estación con ID {id} no encontrada");
        }

        var carriles = await _unitOfWork.Repository<Carril>().GetAsync(c => c.EstacionId == id);
        var carrilesDto = carriles.Select(c => new CarrilDto
        {
            Id = c.Id,
            Numero = c.Numero,
            EstacionId = c.EstacionId,
            Tipo = c.Tipo,
            Estado = c.Estado,
            FechaCreacion = c.FechaCreacion,
            FechaActualizacion = c.FechaActualizacion
        });

        return Ok(carrilesDto);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEstacion(int id)
    {
        var estacion = await _unitOfWork.Repository<Estacion>().GetByIdAsync(id);
        if (estacion == null)
        {
            return NotFound();
        }

        await _unitOfWork.Repository<Estacion>().DeleteAsync(estacion);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }
}
