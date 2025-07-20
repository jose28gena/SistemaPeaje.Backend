using Microsoft.AspNetCore.Mvc;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;
using SistemaPeaje.Infrastructure.Workers;

namespace SistemaPeaje.API.Controllers
{
    /// <summary>
    /// Controlador para gestión de configuraciones de PLCs
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PlcConfiguracionController : ControllerBase
    {
        private readonly ILogger<PlcConfiguracionController> _logger;
        private readonly IPlcConfiguracionService _configService;
        private readonly PlcManagerService _managerService;

        public PlcConfiguracionController(
            ILogger<PlcConfiguracionController> logger,
            IPlcConfiguracionService configService,
            PlcManagerService managerService)
        {
            _logger = logger;
            _configService = configService;
            _managerService = managerService;
        }

        /// <summary>
        /// Obtiene todas las configuraciones de PLC
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var configuraciones = await _configService.ObtenerTodasLasConfiguracionesAsync();
                return Ok(configuraciones);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener configuraciones de PLC");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene todas las configuraciones con sus coils incluidos para monitoreo
        /// </summary>
        [HttpGet("with-coils")]
        public async Task<IActionResult> GetWithCoils()
        {
            try
            {
                var configuraciones = await _configService.ObtenerTodasLasConfiguracionesAsync();
                return Ok(configuraciones);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener configuraciones con coils");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene configuraciones activas
        /// </summary>
        [HttpGet("activas")]
        public async Task<IActionResult> GetActivas()
        {
            try
            {
                var configuraciones = await _configService.ObtenerConfiguracionesActivasAsync();
                return Ok(configuraciones);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener configuraciones activas");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene una configuración por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var configuracion = await _configService.ObtenerConfiguracionPorIdAsync(id);
                if (configuracion == null)
                    return NotFound();

                return Ok(configuracion);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener configuración {Id}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Crea una nueva configuración de PLC
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PlcConfiguracionDto dto)
        {
            try
            {
                var configuracion = new PlcConfiguracion
                {
                    Nombre = dto.Nombre,
                    Ip = dto.Ip,
                    Puerto = dto.Puerto,
                    UnitId = dto.UnitId,
                    DireccionInicial = dto.DireccionInicial,
                    CantidadCoils = dto.CantidadCoils,
                    IntervaloMonitoreo = dto.IntervaloMonitoreo,
                    HabilitarLoggingPeriodico = dto.HabilitarLoggingPeriodico,
                    EstacionId = dto.EstacionId,
                    CarrilId = dto.CarrilId,
                    Observaciones = dto.Observaciones,
                    CoilsConfiguracion = dto.CoilsConfiguracion?.Select(c => new PlcCoilConfiguracion
                    {
                        Indice = c.Indice,
                        Direccion = c.Direccion,
                        Nombre = c.Nombre,
                        Descripcion = c.Descripcion,
                        TipoEvento = c.TipoEvento,
                        GenerarEvento = c.GenerarEvento,
                        EsAlarma = c.EsAlarma,
                        AccionEspecial = c.AccionEspecial
                    }).ToList() ?? new List<PlcCoilConfiguracion>()
                };

                var result = await _configService.CrearConfiguracionAsync(configuracion);
                _logger.LogInformation("Configuración de PLC creada: {Nombre} ({Ip}:{Puerto})", 
                    result.Nombre, result.Ip, result.Puerto);

                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear configuración de PLC");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Actualiza una configuración existente
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PlcConfiguracionDto dto)
        {
            try
            {
                var configuracionExistente = await _configService.ObtenerConfiguracionPorIdAsync(id);
                if (configuracionExistente == null)
                    return NotFound();

                configuracionExistente.Nombre = dto.Nombre;
                configuracionExistente.Ip = dto.Ip;
                configuracionExistente.Puerto = dto.Puerto;
                configuracionExistente.UnitId = dto.UnitId;
                configuracionExistente.DireccionInicial = dto.DireccionInicial;
                configuracionExistente.CantidadCoils = dto.CantidadCoils;
                configuracionExistente.IntervaloMonitoreo = dto.IntervaloMonitoreo;
                configuracionExistente.HabilitarLoggingPeriodico = dto.HabilitarLoggingPeriodico;
                configuracionExistente.EstacionId = dto.EstacionId;
                configuracionExistente.CarrilId = dto.CarrilId;
                configuracionExistente.Observaciones = dto.Observaciones;

                // Actualizar configuración de coils
                configuracionExistente.CoilsConfiguracion.Clear();
                if (dto.CoilsConfiguracion != null)
                {
                    foreach (var coilDto in dto.CoilsConfiguracion)
                    {
                        configuracionExistente.CoilsConfiguracion.Add(new PlcCoilConfiguracion
                        {
                            PlcConfiguracionId = id,
                            Indice = coilDto.Indice,
                            Direccion = coilDto.Direccion,
                            Nombre = coilDto.Nombre,
                            Descripcion = coilDto.Descripcion,
                            TipoEvento = coilDto.TipoEvento,
                            GenerarEvento = coilDto.GenerarEvento,
                            EsAlarma = coilDto.EsAlarma,
                            AccionEspecial = coilDto.AccionEspecial
                        });
                    }
                }

                var result = await _configService.ActualizarConfiguracionAsync(configuracionExistente);
                _logger.LogInformation("Configuración de PLC actualizada: {Nombre} ({Ip}:{Puerto})", 
                    result.Nombre, result.Ip, result.Puerto);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar configuración {Id}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Elimina (desactiva) una configuración
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _configService.EliminarConfiguracionAsync(id);
                if (!success)
                    return NotFound();

                _logger.LogInformation("Configuración de PLC eliminada: {Id}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar configuración {Id}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene el estado de todos los workers activos
        /// </summary>
        [HttpGet("workers/status")]
        public async Task<IActionResult> GetWorkersStatus()
        {
            try
            {
                var estados = await _managerService.ObtenerEstadoWorkersAsync();
                return Ok(estados);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estado de workers");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Crea configuraciones de ejemplo para testing
        /// </summary>
        [HttpPost("seed")]
        public async Task<IActionResult> SeedData()
        {
            try
            {
                // Configuración PLC 1 - Caseta Principal
                var config1 = new PlcConfiguracion
                {
                    Nombre = "PLC Caseta Principal",
                    Ip = "192.168.1.103",
                    Puerto = 502,
                    UnitId = 1,
                    DireccionInicial = 1000,
                    CantidadCoils = 4,
                    IntervaloMonitoreo = 2000,
                    HabilitarLoggingPeriodico = true,
                    EstacionId = 1,
                    CarrilId = 1,
                    Observaciones = "Caseta principal de entrada",
                    CoilsConfiguracion = new List<PlcCoilConfiguracion>
                    {
                        new() { Indice = 0, Direccion = 1000, Nombre = "Presencia", TipoEvento = "VEHICULO_DETECTADO", GenerarEvento = true },
                        new() { Indice = 1, Direccion = 1001, Nombre = "BarreraAbierta", TipoEvento = "BARRERA_ABIERTA", GenerarEvento = true },
                        new() { Indice = 2, Direccion = 1002, Nombre = "SentidoAB", TipoEvento = "CAMBIO_SENTIDO", GenerarEvento = true },
                        new() { Indice = 3, Direccion = 1003, Nombre = "Alarma", TipoEvento = "ALARMA_ACTIVADA", GenerarEvento = true, EsAlarma = true }
                    }
                };

                // Configuración PLC 2 - Caseta Salida  
                var config2 = new PlcConfiguracion
                {
                    Nombre = "PLC Caseta Salida",
                    Ip = "192.168.1.104",
                    Puerto = 502,
                    UnitId = 1,
                    DireccionInicial = 2000,
                    CantidadCoils = 3,
                    IntervaloMonitoreo = 2000,
                    HabilitarLoggingPeriodico = true,
                    EstacionId = 1,
                    CarrilId = 2,
                    Observaciones = "Caseta de salida",
                    CoilsConfiguracion = new List<PlcCoilConfiguracion>
                    {
                        new() { Indice = 0, Direccion = 2000, Nombre = "PresenciaSalida", TipoEvento = "VEHICULO_SALIDA", GenerarEvento = true },
                        new() { Indice = 1, Direccion = 2001, Nombre = "BarreraSalida", TipoEvento = "BARRERA_SALIDA_ABIERTA", GenerarEvento = true },
                        new() { Indice = 2, Direccion = 2002, Nombre = "AlarmaSalida", TipoEvento = "ALARMA_SALIDA", GenerarEvento = true, EsAlarma = true }
                    }
                };

                await _configService.CrearConfiguracionAsync(config1);
                await _configService.CrearConfiguracionAsync(config2);

                _logger.LogInformation("Datos de ejemplo creados para configuraciones de PLC");
                return Ok(new { message = "Configuraciones de ejemplo creadas exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear datos de ejemplo");
                return StatusCode(500, new { message = "Error al crear datos de ejemplo" });
            }
        }

        /// <summary>
        /// Obtiene información de monitoreo en tiempo real de todas las estaciones
        /// </summary>
        [HttpGet("monitoring")]
        public async Task<IActionResult> GetMonitoringData()
        {
            try
            {
                // Timeout de 15 segundos para toda la operación
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));
                
                var configuraciones = await _configService.ObtenerConfiguracionesActivasAsync();
                
                var monitoringData = configuraciones.Select(config => new StationMonitoringDto
                {
                    Id = config.Id,
                    Nombre = config.Nombre,
                    Ip = config.Ip,
                    Puerto = config.Puerto,
                    EstacionNombre = config.Estacion?.Nombre ?? "Sin estación",
                    CarrilNombre = config.Carril?.Numero.ToString() ?? "Sin carril",
                    EstaConectado = config.EstaConectado,
                    UltimaConexion = config.UltimaConexion,
                    WorkerActivo = _managerService.IsWorkerRunning(config.Id),
                    EstadoWorker = "Verificando...", // Se actualizará después
                    CoilsConfiguracion = config.CoilsConfiguracion?.Select(c => new PlcCoilConfiguracionDto
                    {
                        Indice = c.Indice,
                        Direccion = c.Direccion,
                        Nombre = c.Nombre,
                        Descripcion = c.Descripcion,
                        TipoEvento = c.TipoEvento,
                        GenerarEvento = c.GenerarEvento,
                        EsAlarma = c.EsAlarma,
                        AccionEspecial = c.AccionEspecial,
                        EstadoActual = c.EstadoActual, // Incluir estado actual
                        UltimaActualizacion = c.UltimaActualizacion // Incluir última actualización
                    }).ToList()
                }).ToList();

                // Obtener estados de workers con timeout
                try
                {
                    var estadosWorkers = await _managerService.ObtenerEstadoWorkersAsync().WaitAsync(cts.Token);
                    
                    foreach (var item in monitoringData)
                    {
                        if (estadosWorkers.TryGetValue(item.Id, out var estado))
                        {
                            item.EstadoWorker = estado;
                        }
                        else
                        {
                            item.EstadoWorker = item.WorkerActivo ? "Desconocido" : "Inactivo";
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    _logger.LogWarning("Timeout al obtener estados de workers");
                    foreach (var item in monitoringData)
                    {
                        item.EstadoWorker = "Timeout";
                    }
                }

                return Ok(monitoringData);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Timeout en endpoint de monitoreo");
                return StatusCode(408, new { message = "Timeout al obtener datos de monitoreo" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener datos de monitoreo");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }

    /// <summary>
    /// DTO para monitoreo de estaciones
    /// </summary>
    public class StationMonitoringDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Ip { get; set; } = string.Empty;
        public int Puerto { get; set; }
        public string EstacionNombre { get; set; } = string.Empty;
        public string CarrilNombre { get; set; } = string.Empty;
        public bool EstaConectado { get; set; }
        public DateTime? UltimaConexion { get; set; }
        public bool WorkerActivo { get; set; }
        public string EstadoWorker { get; set; } = "Desconocido";
        public List<PlcCoilConfiguracionDto>? CoilsConfiguracion { get; set; }
    }

    /// <summary>
    /// DTO para crear/actualizar configuraciones de PLC
    /// </summary>
    public class PlcConfiguracionDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Ip { get; set; } = string.Empty;
        public int Puerto { get; set; } = 502;
        public byte UnitId { get; set; } = 1;
        public ushort DireccionInicial { get; set; } = 1000;
        public ushort CantidadCoils { get; set; } = 4;
        public int IntervaloMonitoreo { get; set; } = 2000;
        public bool HabilitarLoggingPeriodico { get; set; } = true;
        public int EstacionId { get; set; }
        public int CarrilId { get; set; }
        public string? Observaciones { get; set; }
        public List<PlcCoilConfiguracionDto>? CoilsConfiguracion { get; set; }
    }

    public class PlcCoilConfiguracionDto
    {
        public int Indice { get; set; }
        public ushort Direccion { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string? TipoEvento { get; set; }
        public bool GenerarEvento { get; set; } = true;
        public bool EsAlarma { get; set; } = false;
        public string? AccionEspecial { get; set; }
        public bool EstadoActual { get; set; } = false; // Estado actual del coil
        public DateTime? UltimaActualizacion { get; set; } // Última actualización del estado
    }
}
