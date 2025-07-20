using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SistemaPeaje.Core.Interfaces;
using SistemaPeaje.Infrastructure.Services;
using System.Collections.Concurrent;

namespace SistemaPeaje.Infrastructure.Workers
{
    /// <summary>
    /// Manager que coordina múltiples workers de PLC dinámicamente desde base de datos
    /// </summary>
    public class PlcManagerService : BackgroundService
    {
        private readonly ILogger<PlcManagerService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly ConcurrentDictionary<int, PlcWorkerInstance> _activeWorkers;
        private readonly TimeSpan _configCheckInterval = TimeSpan.FromSeconds(30); // Revisar configuración cada 30 segundos

        public PlcManagerService(
            ILogger<PlcManagerService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _activeWorkers = new ConcurrentDictionary<int, PlcWorkerInstance>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("🚀 PlcManagerService iniciado - Gestión dinámica de múltiples PLCs");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ActualizarWorkersAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al actualizar workers de PLC");
                }

                await Task.Delay(_configCheckInterval, stoppingToken);
            }

            // Detener todos los workers al finalizar
            await DetenerTodosLosWorkersAsync();
            _logger.LogInformation("🛑 PlcManagerService detenido");
        }

        private async Task ActualizarWorkersAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var configService = scope.ServiceProvider.GetRequiredService<IPlcConfiguracionService>();

            try
            {
                var configuracionesActivas = await configService.ObtenerConfiguracionesActivasAsync();
                var configIds = configuracionesActivas.Select(c => c.Id).ToHashSet();

                // Detener workers de configuraciones que ya no están activas
                var workersADetener = _activeWorkers.Keys.Where(id => !configIds.Contains(id)).ToList();
                foreach (var workerId in workersADetener)
                {
                    await DetenerWorkerAsync(workerId);
                }

                // Iniciar workers para nuevas configuraciones
                foreach (var config in configuracionesActivas)
                {
                    if (!_activeWorkers.ContainsKey(config.Id))
                    {
                        await IniciarWorkerAsync(config, stoppingToken);
                    }
                    else
                    {
                        // Verificar si la configuración cambió
                        var workerExistente = _activeWorkers[config.Id];
                        if (workerExistente.RequiereActualizacion(config))
                        {
                            _logger.LogInformation("🔄 Reiniciando worker para PLC {Nombre} debido a cambios de configuración", config.Nombre);
                            await DetenerWorkerAsync(config.Id);
                            await IniciarWorkerAsync(config, stoppingToken);
                        }
                    }
                }

                _logger.LogDebug("Workers activos: {Count}, Configuraciones activas: {ConfigCount}", 
                    _activeWorkers.Count, configuracionesActivas.Count());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener configuraciones de PLC");
            }
        }

        private async Task IniciarWorkerAsync(Core.Entities.PlcConfiguracion config, CancellationToken stoppingToken)
        {
            try
            {
                var workerInstance = new PlcWorkerInstance(config, _serviceProvider, _logger);
                
                if (_activeWorkers.TryAdd(config.Id, workerInstance))
                {
                    await workerInstance.IniciarAsync(stoppingToken);
                    _logger.LogInformation("✅ Worker iniciado para PLC {Nombre} ({Ip}:{Puerto})", 
                        config.Nombre, config.Ip, config.Puerto);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al iniciar worker para PLC {Nombre}", config.Nombre);
            }
        }

        private async Task DetenerWorkerAsync(int configId)
        {
            if (_activeWorkers.TryRemove(configId, out var worker))
            {
                try
                {
                    await worker.DetenerAsync();
                    _logger.LogInformation("🛑 Worker detenido para configuración ID {ConfigId}", configId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al detener worker para configuración ID {ConfigId}", configId);
                }
            }
        }

        private async Task DetenerTodosLosWorkersAsync()
        {
            var tasks = _activeWorkers.Values.Select(worker => worker.DetenerAsync());
            await Task.WhenAll(tasks);
            _activeWorkers.Clear();
        }

        public async Task<Dictionary<int, string>> ObtenerEstadoWorkersAsync()
        {
            var estados = new Dictionary<int, string>();
            
            // Timeout global de 8 segundos (3 PLCs x 3s cada uno máximo)
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(8));
            
            var tasks = _activeWorkers.Select(async kvp =>
            {
                try
                {
                    var estado = await kvp.Value.ObtenerEstadoAsync().WaitAsync(cts.Token);
                    return new { ConfigId = kvp.Key, Estado = estado };
                }
                catch (OperationCanceledException)
                {
                    return new { ConfigId = kvp.Key, Estado = "Timeout" };
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error al obtener estado del worker {ConfigId}", kvp.Key);
                    return new { ConfigId = kvp.Key, Estado = "Error" };
                }
            });

            var resultados = await Task.WhenAll(tasks);
            
            foreach (var resultado in resultados)
            {
                estados[resultado.ConfigId] = resultado.Estado;
            }

            return estados;
        }

        /// <summary>
        /// Verifica si un worker está activo para una configuración específica
        /// </summary>
        public bool IsWorkerRunning(int configId)
        {
            return _activeWorkers.ContainsKey(configId);
        }
    }

    /// <summary>
    /// Instancia individual de worker para un PLC específico
    /// </summary>
    public class PlcWorkerInstance
    {
        private readonly Core.Entities.PlcConfiguracion _config;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;
        private CancellationTokenSource? _cancellationTokenSource;
        private Task? _workerTask;
        private PlcModbusReaderService? _plcReader;
        private bool[] _estadosAnteriores = Array.Empty<bool>();
        private DateTime _ultimaActualizacionConfig;

        public PlcWorkerInstance(
            Core.Entities.PlcConfiguracion config, 
            IServiceProvider serviceProvider, 
            ILogger logger)
        {
            _config = config;
            _serviceProvider = serviceProvider;
            _logger = logger;
            _ultimaActualizacionConfig = config.FechaActualizacion ?? config.FechaCreacion;
        }

        public async Task IniciarAsync(CancellationToken stoppingToken)
        {
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
            
            using var scope = _serviceProvider.CreateScope();
            var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
            var readerLogger = loggerFactory.CreateLogger<PlcModbusReaderService>();
            
            _plcReader = new PlcModbusReaderService(
                readerLogger,
                _config.Ip,
                _config.Puerto,
                _config.UnitId);

            _estadosAnteriores = new bool[_config.CantidadCoils];

            // Verificar estado inicial de conexión antes de iniciar el monitoreo
            try
            {
                var conexionInicial = await _plcReader.VerificarConexionAsync();
                if (conexionInicial != _config.EstaConectado)
                {
                    _logger.LogInformation("📍 Estado inicial de conexión para PLC {Nombre}: {Estado}", 
                        _config.Nombre, conexionInicial ? "Conectado" : "Desconectado");
                    await ActualizarEstadoConexion(conexionInicial);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "No se pudo verificar estado inicial de conexión para PLC {Nombre}", _config.Nombre);
            }

            _workerTask = EjecutarMonitoreoAsync(_cancellationTokenSource.Token);
            await Task.CompletedTask;
        }

        public async Task DetenerAsync()
        {
            _cancellationTokenSource?.Cancel();
            
            if (_workerTask != null)
            {
                try
                {
                    await _workerTask;
                }
                catch (OperationCanceledException)
                {
                    // Esperado al cancelar
                }
            }

            _cancellationTokenSource?.Dispose();
        }

        public async Task<string> ObtenerEstadoAsync()
        {
            if (_workerTask == null)
                return "Detenido";

            if (_workerTask.IsCompleted)
                return _workerTask.IsFaulted ? "Error" : "Completado";

            if (_plcReader != null)
            {
                try
                {
                    // Timeout agresivo de 3 segundos para garantizar respuesta rápida del endpoint
                    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
                    var conectado = await _plcReader.VerificarConexionAsync().WaitAsync(cts.Token);
                    
                    // Actualizar estado local si es diferente
                    if (conectado != _config.EstaConectado)
                    {
                        _logger.LogDebug("Estado local inconsistente para PLC {Nombre}: Local={EstadoLocal}, Real={EstadoReal}", 
                            _config.Nombre, _config.EstaConectado, conectado);
                    }
                    
                    return conectado ? "Conectado" : "Desconectado";
                }
                catch (OperationCanceledException)
                {
                    return "Timeout";
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(ex, "Error al verificar conexión para PLC {Nombre}", _config.Nombre);
                    return "Error de conexión";
                }
            }

            return "Iniciando";
        }

        public bool RequiereActualizacion(Core.Entities.PlcConfiguracion nuevaConfig)
        {
            var fechaModificacion = nuevaConfig.FechaActualizacion ?? nuevaConfig.FechaCreacion;
            return fechaModificacion > _ultimaActualizacionConfig;
        }

        private async Task EjecutarMonitoreoAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("🔍 Iniciando monitoreo de PLC {Nombre} en {Ip}:{Puerto}", 
                _config.Nombre, _config.Ip, _config.Puerto);

            bool conexionAnterior = false;
            int errorCount = 0;
            int iterationCount = 0;

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    if (_plcReader == null)
                        break;

                    iterationCount++;

                    // Verificar conexión (usa timeout interno de 5s del PlcModbusReaderService)
                    var conexionActual = await _plcReader.VerificarConexionAsync();
                    
                    if (conexionActual != conexionAnterior)
                    {
                        await ActualizarEstadoConexion(conexionActual);
                        conexionAnterior = conexionActual;
                        errorCount = 0;
                    }

                    if (conexionActual)
                    {
                        // Leer coils (usa timeout interno de 5s del PlcModbusReaderService)
                        var estadosActuales = await _plcReader.LeerCoilsAsync(
                            _config.DireccionInicial, 
                            _config.CantidadCoils);

                        // Procesar cambios normales
                        await ProcesarCambiosDeEstado(_estadosAnteriores, estadosActuales);
                        
                        // Sincronizar todos los estados cada 10 iteraciones (aproximadamente cada 10 segundos)
                        // para asegurar que la BD esté actualizada incluso si no hay cambios
                        if (iterationCount % 10 == 0)
                        {
                            await SincronizarTodosLosEstados(estadosActuales);
                        }
                        
                        if (_config.HabilitarLoggingPeriodico)
                        {
                            LogearEstadoActual(estadosActuales);
                        }

                        _estadosAnteriores = estadosActuales;
                        errorCount = 0;
                    }
                    else
                    {
                        errorCount++;
                    }
                }
                catch (Exception ex)
                {
                    errorCount++;
                    _logger.LogError(ex, "Error en monitoreo de PLC {Nombre} (intento #{ErrorCount})", 
                        _config.Nombre, errorCount);
                    
                    if (errorCount > 50)
                    {
                        _logger.LogCritical("Demasiados errores en PLC {Nombre}. Pausando 60 segundos", _config.Nombre);
                        await Task.Delay(TimeSpan.FromSeconds(60), cancellationToken);
                        errorCount = 0;
                    }
                }

                await Task.Delay(TimeSpan.FromMilliseconds(_config.IntervaloMonitoreo), cancellationToken);
            }
        }

        private async Task ActualizarEstadoConexion(bool conectado)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var configService = scope.ServiceProvider.GetRequiredService<IPlcConfiguracionService>();
                await configService.ActualizarEstadoConexionAsync(_config.Id, conectado);

                // Actualizar también el estado local para mantener consistencia
                _config.EstaConectado = conectado;
                if (conectado)
                {
                    _config.UltimaConexion = DateTime.UtcNow;
                    // Cuando se reconecta, forzar sincronización inmediata de todos los estados
                    if (_plcReader != null)
                    {
                        _logger.LogInformation("🔄 Forzando sincronización inmediata tras reconexión para PLC {Nombre}", _config.Nombre);
                        var estadosActuales = await _plcReader.LeerCoilsAsync(
                            _config.DireccionInicial, 
                            _config.CantidadCoils);
                        await SincronizarTodosLosEstados(estadosActuales);
                    }
                }
                _config.FechaActualizacion = DateTime.UtcNow;

                _logger.LogInformation(conectado ? 
                    "✅ PLC {Nombre} conectado y estado sincronizado" : 
                    "❌ PLC {Nombre} desconectado", _config.Nombre);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar estado de conexión para PLC {Nombre}", _config.Nombre);
            }
        }

        private async Task ProcesarCambiosDeEstado(bool[] estadosAnteriores, bool[] estadosActuales)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();
                var coilsActualizados = new List<Core.Entities.PlcCoilConfiguracion>();
                
                for (int i = 0; i < Math.Min(estadosAnteriores.Length, estadosActuales.Length); i++)
                {
                    if (estadosAnteriores[i] != estadosActuales[i])
                    {
                        var coilConfig = _config.CoilsConfiguracion.FirstOrDefault(c => c.Indice == i);
                        var nombreCoil = coilConfig?.Nombre ?? $"Coil{i}";
                        
                        _logger.LogInformation("🔄 [{PLC}] Cambio en {NombreCoil}: {EstadoAnterior} → {EstadoActual}",
                            _config.Nombre, nombreCoil, estadosAnteriores[i], estadosActuales[i]);

                        // Actualizar estado en configuración
                        if (coilConfig != null)
                        {
                            coilConfig.EstadoActual = estadosActuales[i];
                            coilConfig.UltimaActualizacion = DateTime.UtcNow;
                            coilsActualizados.Add(coilConfig);
                        }

                        // Registrar evento en BD si está habilitado
                        if (coilConfig?.GenerarEvento == true && unitOfWork != null)
                        {
                            await RegistrarEventoTransito(unitOfWork, coilConfig, estadosAnteriores[i], estadosActuales[i]);
                        }

                        // Procesar eventos especiales
                        await ProcesarEventoEspecial(coilConfig, estadosActuales[i]);
                    }
                }

                // Guardar cambios de estados de coils en la base de datos
                if (coilsActualizados.Any() && unitOfWork != null)
                {
                    try
                    {
                        // Actualizar cada entidad usando el repositorio
                        var repository = unitOfWork.Repository<Core.Entities.PlcCoilConfiguracion>();
                        foreach (var coil in coilsActualizados)
                        {
                            await repository.UpdateAsync(coil);
                        }
                        
                        await unitOfWork.SaveChangesAsync();
                        _logger.LogInformation("💾 Estados de {Count} coils actualizados en BD para PLC {Nombre}", 
                            coilsActualizados.Count, _config.Nombre);
                    }
                    catch (Exception saveEx)
                    {
                        _logger.LogError(saveEx, "Error al guardar estados de coils en BD para PLC {Nombre}", _config.Nombre);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar cambios de estado en PLC {Nombre}", _config.Nombre);
            }
        }

        /// <summary>
        /// Sincroniza todos los estados de los coils con la base de datos, sin importar si cambiaron
        /// </summary>
        private async Task SincronizarTodosLosEstados(bool[] estadosActuales)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();
                
                if (unitOfWork == null) return;

                var coilsActualizados = new List<Core.Entities.PlcCoilConfiguracion>();
                
                for (int i = 0; i < Math.Min(_config.CoilsConfiguracion.Count, estadosActuales.Length); i++)
                {
                    var coilConfig = _config.CoilsConfiguracion.FirstOrDefault(c => c.Indice == i);
                    if (coilConfig != null)
                    {
                        // Actualizar siempre el estado actual y la fecha de actualización
                        coilConfig.EstadoActual = estadosActuales[i];
                        coilConfig.UltimaActualizacion = DateTime.UtcNow;
                        coilsActualizados.Add(coilConfig);
                    }
                }

                if (coilsActualizados.Any())
                {
                    try
                    {
                        // Actualizar cada entidad usando el repositorio
                        var repository = unitOfWork.Repository<Core.Entities.PlcCoilConfiguracion>();
                        foreach (var coil in coilsActualizados)
                        {
                            await repository.UpdateAsync(coil);
                        }
                        
                        await unitOfWork.SaveChangesAsync();
                        _logger.LogInformation("🔄 Sincronizados {Count} estados de coils en BD para PLC {Nombre}", 
                            coilsActualizados.Count, _config.Nombre);
                    }
                    catch (Exception saveEx)
                    {
                        _logger.LogError(saveEx, "Error al sincronizar estados de coils en BD para PLC {Nombre}", _config.Nombre);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al sincronizar todos los estados en PLC {Nombre}", _config.Nombre);
            }
        }

        private async Task RegistrarEventoTransito(IUnitOfWork unitOfWork, 
            Core.Entities.PlcCoilConfiguracion coilConfig, bool estadoAnterior, bool estadoActual)
        {
            try
            {
                var evento = new Core.Entities.EventoTransito
                {
                    EstacionId = _config.EstacionId,
                    CarrilId = _config.CarrilId,
                    TipoEvento = coilConfig.TipoEvento ?? $"CAMBIO_{coilConfig.Nombre.ToUpper()}",
                    Descripcion = $"[{_config.Nombre}] {coilConfig.Nombre}: {estadoAnterior} → {estadoActual}",
                    DatosAdicionales = System.Text.Json.JsonSerializer.Serialize(new
                    {
                        PlcNombre = _config.Nombre,
                        PlcIp = _config.Ip,
                        CoilDireccion = coilConfig.Direccion,
                        CoilIndice = coilConfig.Indice,
                        ValorAnterior = estadoAnterior,
                        ValorActual = estadoActual,
                        EsAlarma = coilConfig.EsAlarma
                    }),
                    SensorId = $"PLC_{_config.Id}_COIL_{coilConfig.Direccion}",
                    FechaEvento = DateTime.UtcNow,
                    EstadoEvento = "PROCESADO",
                    FechaCreacion = DateTime.UtcNow,
                    Activo = true
                };

                await unitOfWork.Repository<Core.Entities.EventoTransito>().AddAsync(evento);
                await unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar evento para PLC {Nombre}", _config.Nombre);
            }
        }

        private async Task ProcesarEventoEspecial(Core.Entities.PlcCoilConfiguracion? coilConfig, bool nuevoEstado)
        {
            if (coilConfig == null) return;

            try
            {
                // Procesar alarmas críticas
                if (coilConfig.EsAlarma && nuevoEstado)
                {
                    _logger.LogWarning("🚨 ALARMA activada en {PLC}: {Coil}", _config.Nombre, coilConfig.Nombre);
                    // Aquí se podrían enviar notificaciones, emails, etc.
                }

                // Procesar acciones especiales configuradas
                if (!string.IsNullOrEmpty(coilConfig.AccionEspecial))
                {
                    _logger.LogInformation("⚡ Ejecutando acción especial para {PLC}.{Coil}: {Accion}", 
                        _config.Nombre, coilConfig.Nombre, coilConfig.AccionEspecial);
                    // Aquí se podría implementar lógica para diferentes tipos de acciones
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar evento especial para {PLC}.{Coil}", 
                    _config.Nombre, coilConfig.Nombre);
            }

            await Task.CompletedTask;
        }

        private void LogearEstadoActual(bool[] estados)
        {
            var estadosTexto = new List<string>();
            
            for (int i = 0; i < estados.Length; i++)
            {
                var coilConfig = _config.CoilsConfiguracion.FirstOrDefault(c => c.Indice == i);
                var nombre = coilConfig?.Nombre ?? $"Coil{i}";
                estadosTexto.Add($"{nombre}: {estados[i]}");
            }
            
            _logger.LogInformation("[{PLC}] {Estados}", _config.Nombre, string.Join(", ", estadosTexto));
        }
    }
}
