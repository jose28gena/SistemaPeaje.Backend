# 💻 Ejemplos de Código para Integración PLC

## ⚠️ **Conversión de Direcciones: Simulador ↔ Código .NET**

### 📋 **Regla Fundamental**
- **Simulador PLC**: Direcciones base 1 (000001, 000002, 000097...)
- **NModbus4 (.NET)**: Índices base 0 (0, 1, 96...)

### 🔄 **Helper de Conversión**

```csharp
public static class DireccionHelper 
{
    /// <summary>
    /// Convierte dirección del simulador (000097) a índice .NET (96)
    /// </summary>
    public static int SimuladorAIndice(int direccionSimulador) 
    {
        return direccionSimulador - 1;
    }
    
    /// <summary>
    /// Convierte índice .NET (96) a dirección simulador (000097)
    /// </summary>
    public static int IndiceASimulador(int indice) 
    {
        return indice + 1;
    }
    
    /// <summary>
    /// Validar que la dirección está en rango válido
    /// </summary>
    public static bool EsDireccionValida(int direccion, int minimo = 1, int maximo = 65535)
    {
        return direccion >= minimo && direccion <= maximo;
    }
}

// Ejemplos de uso
int direccionSimulador = 97;  // Lo que vemos en el simulador
int indiceParaCodigo = DireccionHelper.SimuladorAIndice(direccionSimulador); // 96

// Para usar en NModbus4
await plc.LeerCoilsAsync(indiceParaCodigo, 1); // Lee coil 000097 del simulador
```

## 🏭 **Servicio de PLC Completo**

```csharp
public interface IPlcService
{
    Task<bool[]> LeerCoilsAsync(int startAddress, int count);
    Task EscribirCoilAsync(int address, bool value);
    Task<bool> TestConexionAsync();
    Task<Dictionary<string, bool>> LeerCoilsNombradosAsync(Dictionary<string, int> mapeoCoils);
}

public class PlcModbusService : IPlcService
{
    private readonly ModbusFactory _factory;
    private readonly string _ip;
    private readonly int _puerto;
    private readonly byte _unitId;
    private readonly ILogger<PlcModbusService> _logger;

    public PlcModbusService(string ip, int puerto = 502, byte unitId = 1, ILogger<PlcModbusService> logger = null)
    {
        _factory = new ModbusFactory();
        _ip = ip;
        _puerto = puerto;
        _unitId = unitId;
        _logger = logger;
    }

    public async Task<bool[]> LeerCoilsAsync(int startAddress, int count)
    {
        try
        {
            using var client = _factory.CreateMaster(new TcpClient(_ip, _puerto));
            var resultado = await client.ReadCoilsAsync(_unitId, (ushort)startAddress, (ushort)count);
            
            _logger?.LogDebug($"Leídos {count} coils desde dirección {startAddress + 1} (simulador)");
            return resultado;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, $"Error leyendo coils desde {startAddress}");
            throw;
        }
    }

    public async Task EscribirCoilAsync(int address, bool value)
    {
        try
        {
            using var client = _factory.CreateMaster(new TcpClient(_ip, _puerto));
            await client.WriteSingleCoilAsync(_unitId, (ushort)address, value);
            
            _logger?.LogDebug($"Escrito coil {address + 1} (simulador) = {value}");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, $"Error escribiendo coil {address}");
            throw;
        }
    }

    public async Task<bool> TestConexionAsync()
    {
        try
        {
            // Intentar leer 1 coil para probar conexión
            await LeerCoilsAsync(0, 1);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<Dictionary<string, bool>> LeerCoilsNombradosAsync(Dictionary<string, int> mapeoCoils)
    {
        var resultado = new Dictionary<string, bool>();
        
        foreach (var (nombre, direccionSimulador) in mapeoCoils)
        {
            try
            {
                var indice = DireccionHelper.SimuladorAIndice(direccionSimulador);
                var estado = await LeerCoilsAsync(indice, 1);
                resultado[nombre] = estado[0];
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Error leyendo coil {nombre} (dir: {direccionSimulador})");
                resultado[nombre] = false; // Valor por defecto en caso de error
            }
        }
        
        return resultado;
    }
}
```

## 🎯 **Ejemplos Prácticos de Uso**

### 📖 **Leer Estado de Caseta de Peaje**

```csharp
public class CasetaPeajeService
{
    private readonly IPlcService _plc;
    
    // Mapeo de direcciones del simulador (base 1)
    private readonly Dictionary<string, int> _mapeoCoils = new()
    {
        { "PresenciaVehiculo", 97 },    // Simulador 000097
        { "BarreraAbierta", 98 },       // Simulador 000098
        { "SentidoCarrilAB", 99 },      // Simulador 000099
        { "ModoEmergencia", 100 }       // Simulador 000100
    };

    public CasetaPeajeService(IPlcService plc)
    {
        _plc = plc;
    }

    public async Task<EstadoCaseta> LeerEstadoCompletoCasetaAsync()
    {
        var estados = await _plc.LeerCoilsNombradosAsync(_mapeoCoils);
        
        return new EstadoCaseta
        {
            TieneVehiculo = estados["PresenciaVehiculo"],
            BarreraAbierta = estados["BarreraAbierta"],
            DireccionAB = estados["SentidoCarrilAB"],
            EnEmergencia = estados["ModoEmergencia"],
            FechaLectura = DateTime.Now
        };
    }

    public async Task AbrirBarreraAsync()
    {
        var direccionBarrera = DireccionHelper.SimuladorAIndice(98); // 97 en código
        await _plc.EscribirCoilAsync(direccionBarrera, true);
    }

    public async Task CerrarBarreraAsync()
    {
        var direccionBarrera = DireccionHelper.SimuladorAIndice(98); // 97 en código
        await _plc.EscribirCoilAsync(direccionBarrera, false);
    }
}

public class EstadoCaseta
{
    public bool TieneVehiculo { get; set; }
    public bool BarreraAbierta { get; set; }
    public bool DireccionAB { get; set; }
    public bool EnEmergencia { get; set; }
    public DateTime FechaLectura { get; set; }
}
```

### 🔄 **Monitoreo Continuo con Background Service**

```csharp
public class ProcesadorEventosPeaje : BackgroundService
{
    private readonly IPlcService _plc;
    private readonly ILogger<ProcesadorEventosPeaje> _logger;
    private readonly CasetaPeajeService _caseta;
    
    public ProcesadorEventosPeaje(IPlcService plc, ILogger<ProcesadorEventosPeaje> logger)
    {
        _plc = plc;
        _logger = logger;
        _caseta = new CasetaPeajeService(plc);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        EstadoCaseta estadoAnterior = null;
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var estadoActual = await _caseta.LeerEstadoCompletoCasetaAsync();
                
                // Detectar cambios
                if (estadoAnterior != null)
                {
                    await ProcesarCambios(estadoAnterior, estadoActual);
                }
                
                estadoAnterior = estadoActual;
                await Task.Delay(1000, stoppingToken); // Leer cada segundo
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en monitoreo de caseta");
                await Task.Delay(5000, stoppingToken); // Reintentar en 5 seg
            }
        }
    }

    private async Task ProcesarCambios(EstadoCaseta anterior, EstadoCaseta actual)
    {
        // Vehículo llegó
        if (!anterior.TieneVehiculo && actual.TieneVehiculo)
        {
            _logger.LogInformation("🚗 Vehículo detectado");
            await ProcesarLlegadaVehiculo(actual);
        }
        
        // Vehículo se fue
        if (anterior.TieneVehiculo && !actual.TieneVehiculo)
        {
            _logger.LogInformation("✅ Vehículo salió");
            await ProcesarSalidaVehiculo(actual);
        }
        
        // Cambio de emergencia
        if (anterior.EnEmergencia != actual.EnEmergencia)
        {
            if (actual.EnEmergencia)
                _logger.LogWarning("🚨 MODO EMERGENCIA ACTIVADO");
            else
                _logger.LogInformation("✅ Modo emergencia desactivado");
        }
    }

    private async Task ProcesarLlegadaVehiculo(EstadoCaseta estado)
    {
        if (!estado.EnEmergencia)
        {
            // Abrir barrera automáticamente si no hay emergencia
            await _caseta.AbrirBarreraAsync();
            _logger.LogInformation("🔓 Barrera abierta automáticamente");
        }
    }

    private async Task ProcesarSalidaVehiculo(EstadoCaseta estado)
    {
        // Cerrar barrera después de que el vehículo pase
        await Task.Delay(2000); // Esperar 2 segundos
        await _caseta.CerrarBarreraAsync();
        _logger.LogInformation("🔒 Barrera cerrada");
    }
}
```

## 🧪 **Código de Prueba Completo**

```csharp
public class TestPlcSimulador
{
    public static async Task Main(string[] args)
    {
        var logger = new ConsoleLogger();
        var plc = new PlcModbusService("127.0.0.1", 502, 1, logger);
        
        Console.WriteLine("🧪 Probando conexión con simulador PLC...");
        
        // Test 1: Conexión
        if (await plc.TestConexionAsync())
        {
            Console.WriteLine("✅ Conexión exitosa");
        }
        else
        {
            Console.WriteLine("❌ Error de conexión");
            return;
        }
        
        // Test 2: Leer coils individuales
        Console.WriteLine("\n📖 Leyendo coils individuales:");
        await LeerCoilIndividual(plc, 97, "Presencia de vehículo");
        await LeerCoilIndividual(plc, 98, "Barrera abierta");
        await LeerCoilIndividual(plc, 99, "Sentido carril AB");
        await LeerCoilIndividual(plc, 100, "Modo emergencia");
        
        // Test 3: Leer múltiples coils
        Console.WriteLine("\n📚 Leyendo múltiples coils:");
        var estados = await plc.LeerCoilsAsync(96, 4); // Direcciones 97-100 del simulador
        Console.WriteLine($"Coil 000097 (Presencia): {estados[0]}");
        Console.WriteLine($"Coil 000098 (Barrera): {estados[1]}");
        Console.WriteLine($"Coil 000099 (Sentido): {estados[2]}");
        Console.WriteLine($"Coil 000100 (Emergencia): {estados[3]}");
        
        // Test 4: Escribir coils
        Console.WriteLine("\n✍️ Probando escritura de coils:");
        
        // Abrir barrera
        await plc.EscribirCoilAsync(97, true); // Dirección 000098 del simulador
        Console.WriteLine("🔓 Comando: Abrir barrera enviado");
        
        await Task.Delay(3000);
        
        // Cerrar barrera
        await plc.EscribirCoilAsync(97, false);
        Console.WriteLine("🔒 Comando: Cerrar barrera enviado");
        
        Console.WriteLine("\n🎉 Pruebas completadas. Revisar simulador para confirmar cambios.");
    }
    
    private static async Task LeerCoilIndividual(IPlcService plc, int direccionSimulador, string descripcion)
    {
        try
        {
            var indice = DireccionHelper.SimuladorAIndice(direccionSimulador);
            var estado = await plc.LeerCoilsAsync(indice, 1);
            Console.WriteLine($"  Coil {direccionSimulador:000000} ({descripcion}): {estado[0]}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  ❌ Error leyendo coil {direccionSimulador}: {ex.Message}");
        }
    }
}

public class ConsoleLogger : ILogger<PlcModbusService>
{
    public IDisposable BeginScope<TState>(TState state) => null;
    public bool IsEnabled(LogLevel logLevel) => true;
    
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        var message = formatter(state, exception);
        Console.WriteLine($"[{logLevel}] {message}");
        if (exception != null)
            Console.WriteLine($"Exception: {exception}");
    }
}
```

## 📚 **Registro de Dependencias en DI**

```csharp
// En Program.cs o Startup.cs
services.AddSingleton<IPlcService>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var logger = provider.GetRequiredService<ILogger<PlcModbusService>>();
    
    return new PlcModbusService(
        ip: config["PlcConfig:Ip"] ?? "127.0.0.1",
        puerto: config.GetValue<int>("PlcConfig:Puerto", 502),
        unitId: config.GetValue<byte>("PlcConfig:UnitId", 1),
        logger: logger
    );
});

services.AddScoped<CasetaPeajeService>();
services.AddHostedService<ProcesadorEventosPeaje>();
```

---

## 🎯 **Puntos Clave para Recordar**

1. **Siempre convertir direcciones**: Simulador base 1 → Código base 0
2. **Validar conexión antes de operaciones**: Usar `TestConexionAsync()`
3. **Manejar excepciones**: PLCs pueden desconectarse
4. **Logging detallado**: Para debug y auditoría
5. **Configuración externa**: No hardcodear IPs y puertos
6. **Testing gradual**: Probar lectura antes que escritura

**¡Con estos ejemplos ya puedes integrar cualquier PLC Modbus TCP en tu proyecto!** 🚀
