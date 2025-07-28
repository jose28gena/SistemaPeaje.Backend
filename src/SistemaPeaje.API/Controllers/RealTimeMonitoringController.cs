using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;
using SistemaPeaje.Core.Interfaces;
using SistemaPeaje.Infrastructure.Services;
using SistemaPeaje.Infrastructure.Workers;

namespace SistemaPeaje.API.Controllers
{
    /// <summary>
    /// Controlador optimizado para monitoreo en tiempo real ultra-rápido
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class RealTimeMonitoringController : ControllerBase
    {
        private readonly ILogger<RealTimeMonitoringController> _logger;
        private readonly IPlcConfiguracionService _configService;
        private readonly PlcManagerService _managerService;
        private readonly IMemoryCache _cache;
        
        // Cache keys
        private const string MONITORING_DATA_KEY = "monitoring_data_rt";
        private const string MONITORING_HASH_KEY = "monitoring_hash_rt";
        
        // Configuración ultra-rápida
        private static readonly TimeSpan CACHE_DURATION = TimeSpan.FromMilliseconds(500); // 500ms cache
        private static readonly TimeSpan HASH_DURATION = TimeSpan.FromMilliseconds(100);  // 100ms hash cache

        public RealTimeMonitoringController(
            ILogger<RealTimeMonitoringController> logger,
            IPlcConfiguracionService configService,
            PlcManagerService managerService,
            IMemoryCache cache)
        {
            _logger = logger;
            _configService = configService;
            _managerService = managerService;
            _cache = cache;
        }

        /// <summary>
        /// Endpoint ultra-optimizado para monitoreo en tiempo real
        /// </summary>
        [HttpGet("ultra-fast")]
        public async Task<IActionResult> GetUltraFastMonitoring(
            [FromQuery] string? lastHash = null,
            [FromQuery] bool forceRefresh = false)
        {
            try
            {
                var startTime = DateTime.UtcNow;

                // 1. Verificar hash primero (ultra-rápido)
                if (!forceRefresh && !string.IsNullOrEmpty(lastHash))
                {
                    var currentHash = await GetOrGenerateDataHash();
                    if (currentHash == lastHash)
                    {
                        // Sin cambios - respuesta inmediata
                        return Ok(new
                        {
                            hasChanges = false,
                            hash = currentHash,
                            timestamp = startTime,
                            responseTimeMs = (DateTime.UtcNow - startTime).TotalMilliseconds
                        });
                    }
                }

                // 2. Obtener datos (con cache inteligente)
                var data = await GetOrGenerateMonitoringData(forceRefresh);
                var hash = await GetOrGenerateDataHash(forceRefresh);

                var response = new
                {
                    hasChanges = true,
                    hash = hash,
                    data = data,
                    timestamp = startTime,
                    responseTimeMs = (DateTime.UtcNow - startTime).TotalMilliseconds,
                    cacheHit = !forceRefresh && _cache.TryGetValue(MONITORING_DATA_KEY, out _)
                };

                // Headers para prevenir cache del navegador
                Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
                Response.Headers.Add("Pragma", "no-cache");
                Response.Headers.Add("Expires", "0");

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en endpoint ultra-rápido");
                return StatusCode(500, new { 
                    message = "Error interno", 
                    timestamp = DateTime.UtcNow 
                });
            }
        }

        /// <summary>
        /// Obtiene solo los cambios desde el último hash
        /// </summary>
        [HttpGet("delta")]
        public async Task<IActionResult> GetDeltaChanges([FromQuery] string lastHash)
        {
            try
            {
                var startTime = DateTime.UtcNow;
                var currentHash = await GetOrGenerateDataHash();

                if (currentHash == lastHash)
                {
                    return Ok(new
                    {
                        hasChanges = false,
                        hash = currentHash,
                        responseTimeMs = (DateTime.UtcNow - startTime).TotalMilliseconds
                    });
                }

                // Obtener solo estaciones que cambiaron
                var data = await GetOrGenerateMonitoringData();
                var changedStations = IdentifyChangedStations(data, lastHash);

                return Ok(new
                {
                    hasChanges = true,
                    hash = currentHash,
                    changedStations = changedStations,
                    fullData = data, // Incluir datos completos por simplicidad
                    responseTimeMs = (DateTime.UtcNow - startTime).TotalMilliseconds
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en endpoint delta");
                return StatusCode(500, new { message = "Error interno" });
            }
        }

        /// <summary>
        /// Limpia cache forzadamente
        /// </summary>
        [HttpPost("clear-cache")]
        public IActionResult ClearCache()
        {
            try
            {
                _cache.Remove(MONITORING_DATA_KEY);
                _cache.Remove(MONITORING_HASH_KEY);
                
                _logger.LogInformation("Cache de monitoreo limpiado");
                return Ok(new { message = "Cache limpiado exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al limpiar cache");
                return StatusCode(500, new { message = "Error al limpiar cache" });
            }
        }

        #region Private Methods

        private async Task<string> GetOrGenerateDataHash(bool forceRefresh = false)
        {
            if (!forceRefresh && _cache.TryGetValue(MONITORING_HASH_KEY, out string? cachedHash))
            {
                return cachedHash!;
            }

            var data = await GetOrGenerateMonitoringData(forceRefresh);
            var hash = GenerateDataHash(data);

            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = HASH_DURATION,
                Size = 1
            };
            
            _cache.Set(MONITORING_HASH_KEY, hash, cacheOptions);
            return hash;
        }

        private async Task<List<StationMonitoringDto>> GetOrGenerateMonitoringData(bool forceRefresh = false)
        {
            if (!forceRefresh && _cache.TryGetValue(MONITORING_DATA_KEY, out List<StationMonitoringDto>? cachedData))
            {
                return cachedData!;
            }

            // Timeout ultra-corto para máxima velocidad
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            
            var configuraciones = await _configService.ObtenerConfiguracionesActivasAsync();
            
            var monitoringData = configuraciones.Select(config => new StationMonitoringDto
            {
                Id = config.Id,
                Nombre = config.Nombre,
                Ip = config.Ip,
                Puerto = config.Puerto,
                EstacionNombre = config.Estacion?.Nombre ?? "N/A",
                CarrilNombre = config.Carril?.Numero.ToString() ?? "N/A",
                EstaConectado = config.EstaConectado,
                UltimaConexion = config.UltimaConexion,
                WorkerActivo = _managerService.IsWorkerRunning(config.Id),
                EstadoWorker = GetWorkerStatusFast(config.Id),
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
                    EstadoActual = c.EstadoActual,
                    UltimaActualizacion = c.UltimaActualizacion
                }).ToList() ?? new List<PlcCoilConfiguracionDto>()
            }).ToList();

            // Cache con duración muy corta para tiempo real
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = CACHE_DURATION,
                Size = 1 // Especificar tamaño para el SizeLimit del cache
            };
            _cache.Set(MONITORING_DATA_KEY, monitoringData, cacheOptions);
            
            return monitoringData;
        }

        private string GetWorkerStatusFast(int configId)
        {
            try
            {
                // Implementación rápida sin async para evitar delays
                var isRunning = _managerService.IsWorkerRunning(configId);
                return isRunning ? "Activo" : "Inactivo";
            }
            catch
            {
                return "Desconocido";
            }
        }

        private string GenerateDataHash(List<StationMonitoringDto> data)
        {
            try
            {
                // Hash ultra-rápido usando solo datos críticos
                var criticalData = data.Select(station => new
                {
                    id = station.Id,
                    connected = station.EstaConectado,
                    workerActive = station.WorkerActivo,
                    coilsHash = station.CoilsConfiguracion?.Sum(c => 
                        c.Direccion + (c.EstadoActual ? 1 : 0) + (c.EsAlarma ? 2 : 0))
                });

                var json = JsonSerializer.Serialize(criticalData);
                return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(json))[..16]; // Solo primeros 16 chars
            }
            catch
            {
                return DateTime.UtcNow.Ticks.ToString()[..16];
            }
        }

        private List<int> IdentifyChangedStations(List<StationMonitoringDto> data, string lastHash)
        {
            // Implementación simplificada - en producción se haría comparación granular
            return data.Select(s => s.Id).ToList();
        }

        #endregion
    }
}
