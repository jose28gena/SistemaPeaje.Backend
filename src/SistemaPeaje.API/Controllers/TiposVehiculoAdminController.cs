using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaPeaje.Infrastructure.Data;
using SistemaPeaje.Core.Entities;

namespace SistemaPeaje.API.Controllers;

[ApiController]
[Route("api/tipos-vehiculo-admin")]
public class TiposVehiculoAdminController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<TiposVehiculoAdminController> _logger;

    public TiposVehiculoAdminController(ApplicationDbContext context, ILogger<TiposVehiculoAdminController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todos los tipos de vehículo
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<TipoVehiculoDto>>> GetTiposVehiculo()
    {
        try
        {
            var tipos = await _context.TiposVehiculo
                .Where(t => t.EsActivo)
                .OrderBy(t => t.Categoria)
                .ThenBy(t => t.Nombre)
                .Select(t => new TipoVehiculoDto
                {
                    Id = t.Id,
                    Nombre = t.Nombre,
                    Descripcion = t.Descripcion,
                    NumeroEjes = t.NumeroEjes,
                    TarifaBase = t.TarifaBase,
                    Categoria = t.Categoria,
                    EsActivo = t.EsActivo,
                    FechaCreacion = t.FechaCreacion,
                    FechaModificacion = t.FechaActualizacion
                })
                .ToListAsync();

            _logger.LogInformation("Obtenidos {Count} tipos de vehículo", tipos.Count);
            return Ok(tipos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener tipos de vehículo");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene tipos de vehículo activos para formularios
    /// </summary>
    [HttpGet("activos")]
    public async Task<ActionResult<List<TipoVehiculoDto>>> GetTiposVehiculoActivos()
    {
        try
        {
            var tipos = await _context.TiposVehiculo
                .Where(t => t.EsActivo)
                .OrderBy(t => t.Categoria)
                .ThenBy(t => t.Nombre)
                .Select(t => new TipoVehiculoDto
                {
                    Id = t.Id,
                    Nombre = t.Nombre,
                    Descripcion = t.Descripcion,
                    NumeroEjes = t.NumeroEjes,
                    TarifaBase = t.TarifaBase,
                    Categoria = t.Categoria,
                    EsActivo = t.EsActivo
                })
                .ToListAsync();

            return Ok(tipos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener tipos de vehículo activos");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Crea un nuevo tipo de vehículo
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<TipoVehiculoDto>> CreateTipoVehiculo(CreateTipoVehiculoRequest request)
    {
        try
        {
            // Validar que no exista un tipo con el mismo nombre
            var existeTipo = await _context.TiposVehiculo
                .AnyAsync(t => t.Nombre.ToLower() == request.Nombre.ToLower());

            if (existeTipo)
            {
                return BadRequest("Ya existe un tipo de vehículo con ese nombre");
            }

            var tipoVehiculo = new TipoVehiculo
            {
                Nombre = request.Nombre,
                Descripcion = request.Descripcion,
                NumeroEjes = request.NumeroEjes,
                TarifaBase = request.TarifaBase,
                Categoria = request.Categoria,
                EsActivo = true,
                FechaCreacion = DateTime.UtcNow
            };

            _context.TiposVehiculo.Add(tipoVehiculo);
            await _context.SaveChangesAsync();

            var dto = new TipoVehiculoDto
            {
                Id = tipoVehiculo.Id,
                Nombre = tipoVehiculo.Nombre,
                Descripcion = tipoVehiculo.Descripcion,
                NumeroEjes = tipoVehiculo.NumeroEjes,
                TarifaBase = tipoVehiculo.TarifaBase,
                Categoria = tipoVehiculo.Categoria,
                EsActivo = tipoVehiculo.EsActivo,
                FechaCreacion = tipoVehiculo.FechaCreacion
            };

            _logger.LogInformation("Creado tipo de vehículo: {Nombre}", tipoVehiculo.Nombre);
            return CreatedAtAction(nameof(GetTiposVehiculo), new { id = tipoVehiculo.Id }, dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear tipo de vehículo");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Actualiza un tipo de vehículo existente
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<TipoVehiculoDto>> UpdateTipoVehiculo(int id, UpdateTipoVehiculoRequest request)
    {
        try
        {
            var tipoVehiculo = await _context.TiposVehiculo.FindAsync(id);
            if (tipoVehiculo == null)
            {
                return NotFound("Tipo de vehículo no encontrado");
            }

            // Validar que no exista otro tipo con el mismo nombre
            var existeOtroTipo = await _context.TiposVehiculo
                .AnyAsync(t => t.Id != id && t.Nombre.ToLower() == request.Nombre.ToLower());

            if (existeOtroTipo)
            {
                return BadRequest("Ya existe otro tipo de vehículo con ese nombre");
            }

            tipoVehiculo.Nombre = request.Nombre;
            tipoVehiculo.Descripcion = request.Descripcion;
            tipoVehiculo.NumeroEjes = request.NumeroEjes;
            tipoVehiculo.TarifaBase = request.TarifaBase;
            tipoVehiculo.Categoria = request.Categoria;
            tipoVehiculo.EsActivo = request.EsActivo;
            tipoVehiculo.FechaActualizacion = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var dto = new TipoVehiculoDto
            {
                Id = tipoVehiculo.Id,
                Nombre = tipoVehiculo.Nombre,
                Descripcion = tipoVehiculo.Descripcion,
                NumeroEjes = tipoVehiculo.NumeroEjes,
                TarifaBase = tipoVehiculo.TarifaBase,
                Categoria = tipoVehiculo.Categoria,
                EsActivo = tipoVehiculo.EsActivo,
                FechaCreacion = tipoVehiculo.FechaCreacion,
                FechaModificacion = tipoVehiculo.FechaActualizacion
            };

            _logger.LogInformation("Actualizado tipo de vehículo: {Nombre}", tipoVehiculo.Nombre);
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar tipo de vehículo {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Elimina (desactiva) un tipo de vehículo
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTipoVehiculo(int id)
    {
        try
        {
            var tipoVehiculo = await _context.TiposVehiculo.FindAsync(id);
            if (tipoVehiculo == null)
            {
                return NotFound("Tipo de vehículo no encontrado");
            }

            // Verificar si hay transacciones asociadas
            var tieneTransacciones = await _context.Transacciones
                .AnyAsync(t => t.TipoVehiculoId == id);

            if (tieneTransacciones)
            {
                // Solo desactivar si tiene transacciones asociadas
                tipoVehiculo.EsActivo = false;
                tipoVehiculo.FechaActualizacion = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Desactivado tipo de vehículo: {Nombre}", tipoVehiculo.Nombre);
                return Ok("Tipo de vehículo desactivado");
            }
            else
            {
                // Eliminar físicamente si no tiene transacciones
                _context.TiposVehiculo.Remove(tipoVehiculo);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Eliminado tipo de vehículo: {Nombre}", tipoVehiculo.Nombre);
                return Ok("Tipo de vehículo eliminado");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar tipo de vehículo {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene los catálogos predefinidos de tipos de vehículo
    /// </summary>
    [HttpGet("catalogo")]
    public ActionResult<List<CatalogoTipoVehiculoDto>> GetCatalogo()
    {
        var catalogo = new List<CatalogoTipoVehiculoDto>
        {
            new() { Nombre = "Auto", Descripcion = "Automóvil de pasajeros", NumeroEjes = 2, Categoria = "LIVIANO" },
            new() { Nombre = "Moto", Descripcion = "Motocicleta", NumeroEjes = 2, Categoria = "LIVIANO" },
            new() { Nombre = "Bus", Descripcion = "Autobús de pasajeros", NumeroEjes = 2, Categoria = "PESADO" },
            new() { Nombre = "Camión Rígido", Descripcion = "Camión de carga rígido", NumeroEjes = 2, Categoria = "PESADO" },
            new() { Nombre = "Camión Articulado", Descripcion = "Camión articulado con remolque", NumeroEjes = 5, Categoria = "ESPECIAL" }
        };

        return Ok(catalogo);
    }
}

// DTOs
public class TipoVehiculoDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public int NumeroEjes { get; set; }
    public decimal TarifaBase { get; set; }
    public string Categoria { get; set; } = string.Empty;
    public bool EsActivo { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaModificacion { get; set; }
}

public class CreateTipoVehiculoRequest
{
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public int NumeroEjes { get; set; }
    public decimal TarifaBase { get; set; }
    public string Categoria { get; set; } = string.Empty;
}

public class UpdateTipoVehiculoRequest
{
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public int NumeroEjes { get; set; }
    public decimal TarifaBase { get; set; }
    public string Categoria { get; set; } = string.Empty;
    public bool EsActivo { get; set; }
}

public class CatalogoTipoVehiculoDto
{
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public int NumeroEjes { get; set; }
    public string Categoria { get; set; } = string.Empty;
}
