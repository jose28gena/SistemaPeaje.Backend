using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using SistemaPeaje.Infrastructure.Services;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;

namespace SistemaPeaje.Infrastructure.Workers;

/// <summary>
/// Servicio de monitoreo continuo del PLC para detectar eventos en tiempo real
/// </summary>
public class PlcMonitorWorker : BackgroundService
{
    private readonly ILogger<PlcMonitorWorker> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly PlcMonitorOptions _options;
    private PlcModbusReaderService? _plcReader;
    
    public PlcMonitorWorker(
        ILogger<PlcMonitorWorker> logger,
        IServiceProvider serviceProvider,
        IOptions<PlcMonitorOptions> options)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("🚀 PlcMonitorWorker iniciado - Monitoreando PLC {IP}:{Port}", 
            _options.PlcIp, _options.PlcPort);
        
        // Crear el lector de PLC
        using var scope = _serviceProvider.CreateScope();
        var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var readerLogger = loggerFactory.CreateLogger<PlcModbusReaderService>();
        
        _plcReader = new PlcModbusReaderService(
            readerLogger,
            _options.PlcIp, 
            _options.PlcPort, 
            _options.UnitId);

        // Variables para detectar cambios
        bool[] estadosAnteriores = new bool[_options.CoilCount];
        bool conexionAnterior = false;
        int errorCount = 0;

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Verificar conexión primero
                var conexionActual = await _plcReader.VerificarConexionAsync();
                
                if (conexionActual != conexionAnterior)
                {
                    if (conexionActual)
                    {
                        _logger.LogInformation("✅ Conexión con PLC restaurada {IP}:{Port}", 
                            _options.PlcIp, _options.PlcPort);
                        errorCount = 0;
                    }
                    else
                    {
                        _logger.LogWarning("❌ Conexión con PLC perdida {IP}:{Port}", 
                            _options.PlcIp, _options.PlcPort);
                        errorCount++;
                    }
                    
                    conexionAnterior = conexionActual;
                    await RegistrarEventoConexion(conexionActual);
                }

                if (conexionActual)
                {
                    // Leer estado de los coils
                    var estadosActuales = await _plcReader.LeerCoilsAsync(
                        _options.StartAddress, 
                        _options.CoilCount);

                    // Detectar cambios y registrar eventos
                    await ProcesarCambiosDeEstado(estadosAnteriores, estadosActuales);
                    
                    // Logging periódico del estado
                    if (_options.EnablePeriodicLogging)
                    {
                        LogearEstadoActual(estadosActuales);
                    }
                    
                    estadosAnteriores = estadosActuales;
                    errorCount = 0;
                }
                else
                {
                    errorCount++;
                    if (errorCount % 30 == 0) // Log cada 30 intentos fallidos
                    {
                        _logger.LogWarning("PLC {IP}:{Port} sin conexión por {ErrorCount} intentos", 
                            _options.PlcIp, _options.PlcPort, errorCount);
                    }
                }
            }
            catch (Exception ex)
            {
                errorCount++;
                _logger.LogError(ex, "Error en PlcMonitorWorker (intento #{ErrorCount})", errorCount);
                
                if (errorCount > 100)
                {
                    _logger.LogCritical("Demasiados errores consecutivos. Pausando monitoreo por 30 segundos");
                    await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
                    errorCount = 0;
                }
            }

            await Task.Delay(_options.MonitorInterval, stoppingToken);
        }

        _logger.LogInformation("🛑 PlcMonitorWorker detenido");
    }

    private async Task ProcesarCambiosDeEstado(bool[] estadosAnteriores, bool[] estadosActuales)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();
            
            for (int i = 0; i < Math.Min(estadosAnteriores.Length, estadosActuales.Length); i++)
            {
                if (estadosAnteriores[i] != estadosActuales[i])
                {
                    var coilAddress = (ushort)(_options.StartAddress + i);
                    var nombreCoil = ObtenerNombreCoil(i);
                    
                    _logger.LogInformation("🔄 Cambio detectado en {NombreCoil} (Coil {Address}): {EstadoAnterior} → {EstadoActual}",
                        nombreCoil, coilAddress, estadosAnteriores[i], estadosActuales[i]);

                    // Registrar evento en base de datos si está disponible
                    if (unitOfWork != null)
                    {
                        await RegistrarEventoTransito(unitOfWork, coilAddress, nombreCoil, 
                            estadosAnteriores[i], estadosActuales[i]);
                    }

                    // Procesar eventos especiales
                    await ProcesarEventoEspecial(i, estadosActuales[i]);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al procesar cambios de estado");
        }
    }

    private async Task RegistrarEventoTransito(IUnitOfWork unitOfWork, ushort coilAddress, 
        string nombreCoil, bool estadoAnterior, bool estadoActual)
    {
        try
        {
            var evento = new EventoTransito
            {
                EstacionId = _options.EstacionId, // Se configurará en appsettings
                CarrilId = _options.CarrilId, // Se configurará en appsettings
                TipoEvento = $"CAMBIO_{nombreCoil.ToUpper()}",
                Descripcion = $"{nombreCoil}: {estadoAnterior} → {estadoActual} (Coil {coilAddress})",
                DatosAdicionales = $"{{\"PlcIp\":\"{_options.PlcIp}\",\"CoilAddress\":{coilAddress},\"ValorAnterior\":{estadoAnterior.ToString().ToLower()},\"ValorActual\":{estadoActual.ToString().ToLower()}}}",
                SensorId = $"PLC_{_options.PlcIp}_COIL_{coilAddress}",
                FechaEvento = DateTime.UtcNow,
                EstadoEvento = "PROCESADO",
                FechaCreacion = DateTime.UtcNow,
                Activo = true
            };

            await unitOfWork.Repository<EventoTransito>().AddAsync(evento);
            await unitOfWork.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al registrar evento de tránsito");
        }
    }

    private async Task RegistrarEventoConexion(bool conectado)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();
            
            if (unitOfWork != null)
            {
                var evento = new EventoTransito
                {
                    EstacionId = _options.EstacionId,
                    CarrilId = _options.CarrilId,
                    TipoEvento = conectado ? "PLC_CONECTADO" : "PLC_DESCONECTADO",
                    Descripcion = $"PLC {_options.PlcIp}:{_options.PlcPort} {(conectado ? "conectado" : "desconectado")}",
                    DatosAdicionales = $"{{\"PlcIp\":\"{_options.PlcIp}\",\"Puerto\":{_options.PlcPort},\"Conectado\":{conectado.ToString().ToLower()}}}",
                    SensorId = $"PLC_{_options.PlcIp}_CONNECTION",
                    FechaEvento = DateTime.UtcNow,
                    EstadoEvento = "PROCESADO",
                    FechaCreacion = DateTime.UtcNow,
                    Activo = true
                };

                await unitOfWork.Repository<EventoTransito>().AddAsync(evento);
                await unitOfWork.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al registrar evento de conexión");
        }
    }

    private async Task ProcesarEventoEspecial(int coilIndex, bool nuevoEstado)
    {
        // Procesar eventos que requieren acciones especiales
        switch (coilIndex)
        {
            case 0: // Presencia de vehículo
                if (nuevoEstado)
                {
                    _logger.LogInformation("🚗 Vehículo detectado en caseta");
                }
                break;
            
            case 3: // Alarma
                if (nuevoEstado)
                {
                    _logger.LogWarning("🚨 ALARMA activada en PLC {IP}", _options.PlcIp);
                    // Aquí se podría enviar notificación, email, etc.
                }
                break;
        }
        
        await Task.CompletedTask;
    }

    private void LogearEstadoActual(bool[] estados)
    {
        var estadosTexto = new List<string>();
        
        for (int i = 0; i < estados.Length; i++)
        {
            var nombre = ObtenerNombreCoil(i);
            estadosTexto.Add($"{nombre}: {estados[i]}");
        }
        
        _logger.LogInformation("[PLC {IP}] {Estados}", 
            _options.PlcIp, string.Join(", ", estadosTexto));
    }

    private string ObtenerNombreCoil(int index)
    {
        // Mapeo personalizable de índices a nombres descriptivos
        return _options.CoilNames.TryGetValue(index, out var nombre) 
            ? nombre 
            : $"Coil{index}";
    }
}
