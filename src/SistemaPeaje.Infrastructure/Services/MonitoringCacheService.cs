using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SistemaPeaje.Core.Entities;

namespace SistemaPeaje.Infrastructure.Services
{
    /// <summary>
    /// Servicio de caché optimizado para datos de monitoreo en tiempo real
    /// </summary>
    public interface IMonitoringCacheService
    {
        Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
        Task RemoveAsync(string key);
        Task ClearAsync();
        bool TryGetValue<T>(string key, out T? value);
        string GenerateKey(params object[] keyParts);
    }

    public class MonitoringCacheService : IMonitoringCacheService
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<MonitoringCacheService> _logger;
        private readonly MemoryCacheEntryOptions _defaultOptions;

        public MonitoringCacheService(
            IMemoryCache cache,
            ILogger<MonitoringCacheService> logger)
        {
            _cache = cache;
            _logger = logger;
            
            // Configuración por defecto para el caché
            _defaultOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30), // 30 segundos para datos en tiempo real
                SlidingExpiration = TimeSpan.FromSeconds(10), // Renovar si se accede en 10 segundos
                Priority = CacheItemPriority.High,
                Size = 1
            };
        }

        public Task<T?> GetAsync<T>(string key)
        {
            try
            {
                if (_cache.TryGetValue(key, out T? value))
                {
                    _logger.LogDebug("Cache hit para key: {Key}", key);
                    return Task.FromResult(value);
                }

                _logger.LogDebug("Cache miss para key: {Key}", key);
                return Task.FromResult<T?>(default);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener del caché key: {Key}", key);
                return Task.FromResult<T?>(default);
            }
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            try
            {
                var options = _defaultOptions;
                if (expiration.HasValue)
                {
                    options = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = expiration.Value,
                        Priority = CacheItemPriority.High,
                        Size = 1
                    };
                }

                _cache.Set(key, value, options);
                _logger.LogDebug("Valor almacenado en caché para key: {Key}", key);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar en caché key: {Key}", key);
                return Task.CompletedTask;
            }
        }

        public Task RemoveAsync(string key)
        {
            try
            {
                _cache.Remove(key);
                _logger.LogDebug("Valor removido del caché para key: {Key}", key);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al remover del caché key: {Key}", key);
                return Task.CompletedTask;
            }
        }

        public Task ClearAsync()
        {
            try
            {
                if (_cache is MemoryCache memoryCache)
                {
                    memoryCache.Clear();
                    _logger.LogInformation("Caché completamente limpiado");
                }
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al limpiar caché");
                return Task.CompletedTask;
            }
        }

        public bool TryGetValue<T>(string key, out T? value)
        {
            try
            {
                return _cache.TryGetValue(key, out value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en TryGetValue para key: {Key}", key);
                value = default;
                return false;
            }
        }

        public string GenerateKey(params object[] keyParts)
        {
            return string.Join(":", keyParts.Select(p => p?.ToString() ?? "null"));
        }
    }
}
