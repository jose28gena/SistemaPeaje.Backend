using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SistemaPeaje.Infrastructure.Workers;
using SistemaPeaje.Infrastructure.Services;
using SistemaPeaje.Core.Interfaces;

namespace SistemaPeaje.API.Controllers
{
    /// <summary>
    /// Controlador para monitoreo y gestión del PLC
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PlcMonitorController : ControllerBase
    {
        private readonly ILogger<PlcMonitorController> _logger;
        private readonly PlcMonitorOptions _options;
        private readonly IPlcModbusService _plcService;

        public PlcMonitorController(
            ILogger<PlcMonitorController> logger,
            IOptions<PlcMonitorOptions> options,
            IPlcModbusService plcService)
        {
            _logger = logger;
            _options = options.Value;
            _plcService = plcService;
        }

        /// <summary>
        /// Obtiene el estado actual del monitoreo del PLC
        /// </summary>
        [HttpGet("status")]
        public async Task<IActionResult> GetStatus()
        {
            try
            {
                var isConnected = await _plcService.TestConexionAsync(_options.PlcIp, _options.PlcPort, _options.UnitId);
                
                var status = new
                {
                    PlcIp = _options.PlcIp,
                    PlcPort = _options.PlcPort,
                    UnitId = _options.UnitId,
                    IsConnected = isConnected,
                    MonitorInterval = _options.MonitorInterval,
                    StartAddress = _options.StartAddress,
                    CoilCount = _options.CoilCount,
                    EstacionId = _options.EstacionId,
                    CarrilId = _options.CarrilId,
                    CoilNames = _options.CoilNames,
                    Timestamp = DateTime.UtcNow
                };

                return Ok(status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el estado del PLC");
                return StatusCode(500, new { message = "Error al obtener el estado del PLC", error = ex.Message });
            }
        }

        /// <summary>
        /// Lee el estado actual de todos los coils configurados
        /// </summary>
        [HttpGet("coils")]
        public async Task<IActionResult> GetCoilStates()
        {
            try
            {
                var coilStates = new List<object>();
                
                for (ushort i = 0; i < _options.CoilCount; i++)
                {
                    var coilAddress = (ushort)(_options.StartAddress + i);
                    var state = await _plcService.LeerCoilAsync(_options.PlcIp, coilAddress, _options.PlcPort, _options.UnitId);
                    var name = _options.CoilNames.GetValueOrDefault(i, $"Coil{i}");
                    
                    coilStates.Add(new
                    {
                        Index = i,
                        Address = coilAddress,
                        Name = name,
                        State = state,
                        Timestamp = DateTime.UtcNow
                    });
                }

                return Ok(new
                {
                    PlcIp = _options.PlcIp,
                    PlcPort = _options.PlcPort,
                    Coils = coilStates,
                    ReadTime = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al leer los coils del PLC");
                return StatusCode(500, new { message = "Error al leer los coils del PLC", error = ex.Message });
            }
        }

        /// <summary>
        /// Escribe un valor a un coil específico
        /// </summary>
        [HttpPost("coils/{coilAddress}/write")]
        public async Task<IActionResult> WriteCoil(ushort coilAddress, [FromBody] bool value)
        {
            try
            {
                var success = await _plcService.EscribirCoilAsync(_options.PlcIp, coilAddress, value, _options.PlcPort, _options.UnitId);
                
                if (success)
                {
                    _logger.LogInformation("Coil {CoilAddress} escrito con valor {Value}", coilAddress, value);
                    return Ok(new
                    {
                        CoilAddress = coilAddress,
                        Value = value,
                        Success = true,
                        Timestamp = DateTime.UtcNow
                    });
                }
                else
                {
                    return BadRequest(new { message = "Error al escribir el coil", coilAddress, value });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al escribir coil {CoilAddress}", coilAddress);
                return StatusCode(500, new { message = "Error al escribir el coil", error = ex.Message });
            }
        }

        /// <summary>
        /// Prueba la conexión con el PLC
        /// </summary>
        [HttpGet("test-connection")]
        public async Task<IActionResult> TestConnection()
        {
            try
            {
                var isConnected = await _plcService.TestConexionAsync(_options.PlcIp, _options.PlcPort, _options.UnitId);
                
                return Ok(new
                {
                    PlcIp = _options.PlcIp,
                    PlcPort = _options.PlcPort,
                    UnitId = _options.UnitId,
                    IsConnected = isConnected,
                    TestTime = DateTime.UtcNow,
                    Message = isConnected ? "Conexión exitosa" : "Conexión fallida"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al probar la conexión con el PLC");
                return StatusCode(500, new { message = "Error al probar la conexión", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene información del sistema escalable de PLCs
        /// </summary>
        [HttpGet("info-escalable")]
        public async Task<IActionResult> GetInfoEscalable()
        {
            try
            {
                using var scope = HttpContext.RequestServices.CreateScope();
                var configService = scope.ServiceProvider.GetRequiredService<IPlcConfiguracionService>();
                var managerService = scope.ServiceProvider.GetRequiredService<PlcManagerService>();
                
                var configuracionesActivas = await configService.ObtenerConfiguracionesActivasAsync();
                var estadosWorkers = await managerService.ObtenerEstadoWorkersAsync();
                
                var info = new
                {
                    TotalConfiguraciones = configuracionesActivas.Count(),
                    WorkersActivos = estadosWorkers.Count,
                    Configuraciones = configuracionesActivas.Select(c => new
                    {
                        c.Id,
                        c.Nombre,
                        c.Ip,
                        c.Puerto,
                        c.EstaConectado,
                        c.UltimaConexion,
                        CantidadCoils = c.CoilsConfiguracion.Count,
                        EstadoWorker = estadosWorkers.GetValueOrDefault(c.Id, "Desconocido"),
                        NombreEstacion = c.Estacion?.Id.ToString() ?? "N/A",
                        IdCarril = c.CarrilId
                    }),
                    EstadisticasGenerales = new
                    {
                        PLCsConectados = configuracionesActivas.Count(c => c.EstaConectado),
                        PLCsDesconectados = configuracionesActivas.Count(c => !c.EstaConectado),
                        TotalCoils = configuracionesActivas.Sum(c => c.CoilsConfiguracion.Count),
                        UltimaActualizacion = DateTime.UtcNow
                    }
                };

                return Ok(info);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener información del sistema escalable");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}
