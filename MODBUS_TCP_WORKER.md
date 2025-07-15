# Modbus TCP Background Worker - Sistema de Peaje

## 📋 Descripción

El sistema incluye un **Background Worker** que monitorea continuamente un PLC a través del protocolo **Modbus TCP**, detectando cambios en los coils y registrando eventos en tiempo real.

## 🏗️ Arquitectura

### Componentes Principales

1. **PlcModbusReaderService**: Servicio para lectura de coils via Modbus TCP
2. **PlcMonitorWorker**: Background Service que monitorea continuamente el PLC
3. **PlcModbusService**: Servicio API para operaciones síncronas
4. **PlcMonitorController**: API endpoints para gestión y monitoreo

### Diagrama de Flujo
```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   PLC Device    │◄──►│ PlcMonitorWorker│◄──►│   Database      │
│ (Modbus TCP)    │    │ (Background)    │    │ (EventoTransito)│
└─────────────────┘    └─────────────────┘    └─────────────────┘
                              │
                              ▼
                       ┌─────────────────┐
                       │    Logging      │
                       │   (Serilog)     │
                       └─────────────────┘
```

## ⚙️ Configuración

### appsettings.Development.json
```json
{
  "PlcMonitor": {
    "PlcIp": "192.168.1.103",
    "PlcPort": 502,
    "UnitId": 1,
    "StartAddress": 1000,
    "CoilCount": 4,
    "MonitorInterval": "00:00:02",
    "EnablePeriodicLogging": true,
    "EstacionId": 1,
    "CarrilId": 1,
    "CoilNames": {
      "0": "Presencia",
      "1": "BarreraAbierta", 
      "2": "SentidoAB",
      "3": "Alarma"
    }
  }
}
```

### Parámetros de Configuración

| Parámetro | Descripción | Valor por Defecto |
|-----------|-------------|-------------------|
| `PlcIp` | IP del PLC/Caseta | `192.168.1.103` |
| `PlcPort` | Puerto Modbus TCP | `502` |
| `UnitId` | ID de unidad Modbus | `1` |
| `StartAddress` | Dirección inicial de coils | `1000` |
| `CoilCount` | Cantidad de coils a leer | `4` |
| `MonitorInterval` | Intervalo de monitoreo | `00:00:02` |
| `EnablePeriodicLogging` | Logging periódico del estado | `true` |
| `EstacionId` | ID de la estación de peaje | `1` |
| `CarrilId` | ID del carril a monitorear | `1` |
| `CoilNames` | Nombres descriptivos de coils | Ver configuración |

## 🚀 Uso

### 1. Inicio Automático
El worker se inicia automáticamente al ejecutar la aplicación:

```bash
dotnet run
```

### 2. Logs Esperados
```
[21:30:15 INF] 🚀 PlcMonitorWorker iniciado - Monitoreando PLC 192.168.1.103:502
[21:30:17 INF] ✅ Conexión con PLC restaurada 192.168.1.103:502
[21:30:17 INF] [PLC 192.168.1.103] Presencia: False, BarreraAbierta: False, SentidoAB: True, Alarma: False
[21:30:19 INF] 🔄 Cambio detectado en Presencia (Coil 1000): False → True
[21:30:19 INF] 🚗 Vehículo detectado en caseta
[21:30:21 INF] 🔄 Cambio detectado en BarreraAbierta (Coil 1001): False → True
```

### 3. API Endpoints

#### Obtener Estado del Monitoreo
```http
GET /api/PlcMonitor/status
```

**Respuesta:**
```json
{
  "plcIp": "192.168.1.103",
  "plcPort": 502,
  "unitId": 1,
  "isConnected": true,
  "monitorInterval": "00:00:02",
  "startAddress": 1000,
  "coilCount": 4,
  "estacionId": 1,
  "carrilId": 1,
  "coilNames": {
    "0": "Presencia",
    "1": "BarreraAbierta", 
    "2": "SentidoAB",
    "3": "Alarma"
  },
  "timestamp": "2025-07-13T21:30:15.123Z"
}
```

#### Leer Estado de Coils
```http
GET /api/PlcMonitor/coils
```

**Respuesta:**
```json
{
  "plcIp": "192.168.1.103",
  "plcPort": 502,
  "coils": [
    {
      "index": 0,
      "address": 1000,
      "name": "Presencia",
      "state": true,
      "timestamp": "2025-07-13T21:30:15.123Z"
    },
    {
      "index": 1,
      "address": 1001,
      "name": "BarreraAbierta",
      "state": false,
      "timestamp": "2025-07-13T21:30:15.123Z"
    }
  ],
  "readTime": "2025-07-13T21:30:15.123Z"
}
```

#### Escribir Coil
```http
POST /api/PlcMonitor/coils/1001/write
Content-Type: application/json

true
```

#### Probar Conexión
```http
GET /api/PlcMonitor/test-connection
```

## 📊 Eventos en Base de Datos

Los cambios de estado se registran automáticamente en la tabla `EventosTransito`:

```sql
SELECT 
    TipoEvento,
    Descripcion,
    DatosAdicionales,
    SensorId,
    FechaEvento
FROM EventosTransito 
WHERE TipoEvento LIKE 'CAMBIO_%' 
ORDER BY FechaEvento DESC;
```

### Tipos de Eventos Registrados

| Tipo Evento | Descripción |
|-------------|-------------|
| `CAMBIO_PRESENCIA` | Cambio en sensor de presencia |
| `CAMBIO_BARRERAABIERTA` | Cambio en estado de barrera |
| `CAMBIO_SENTIDOAB` | Cambio en sentido de tránsito |
| `CAMBIO_ALARMA` | Activación/desactivación de alarma |
| `PLC_CONECTADO` | PLC reconectado |
| `PLC_DESCONECTADO` | PLC desconectado |

## 🔧 Personalización

### Múltiples PLCs
Para monitorear múltiples PLCs, crea múltiples configuraciones:

```json
{
  "PlcMonitor1": { "PlcIp": "192.168.1.103", "EstacionId": 1 },
  "PlcMonitor2": { "PlcIp": "192.168.1.104", "EstacionId": 2 }
}
```

Y registra múltiples workers en `Program.cs`:
```csharp
builder.Services.Configure<PlcMonitorOptions>("PlcMonitor1", 
    builder.Configuration.GetSection("PlcMonitor1"));
builder.Services.Configure<PlcMonitorOptions>("PlcMonitor2", 
    builder.Configuration.GetSection("PlcMonitor2"));
```

### Eventos Personalizados
Modifica el método `ProcesarEventoEspecial` en `PlcMonitorWorker`:

```csharp
private async Task ProcesarEventoEspecial(int coilIndex, bool nuevoEstado)
{
    switch (coilIndex)
    {
        case 0: // Presencia
            if (nuevoEstado)
                await EnviarNotificacion("Vehículo detectado");
            break;
        case 3: // Alarma crítica
            if (nuevoEstado)
                await EnviarAlertaUrgente("¡ALARMA ACTIVADA!");
            break;
    }
}
```

## 🐛 Troubleshooting

### Problemas Comunes

1. **Error de Conexión**
   - Verificar IP y puerto del PLC
   - Comprobar conectividad de red: `ping 192.168.1.103`
   - Verificar que el puerto 502 esté abierto

2. **Worker no Inicia**
   - Revisar logs de aplicación
   - Verificar configuración en appsettings
   - Comprobar que el servicio esté registrado en DI

3. **No se Registran Eventos**
   - Verificar conexión a base de datos
   - Comprobar que `IUnitOfWork` esté registrado
   - Revisar logs para errores de persistencia

### Logs de Diagnóstico

```csharp
// Habilitar logs detallados en appsettings.json
{
  "Logging": {
    "LogLevel": {
      "SistemaPeaje.Infrastructure.Workers": "Debug",
      "SistemaPeaje.Infrastructure.Services": "Debug"
    }
  }
}
```

## 📈 Monitoreo y Métricas

### Dashboard Sugerido
- Estado de conexión PLC en tiempo real
- Contadores de eventos por tipo
- Histórico de cambios de estado
- Alertas de desconexión
- Métricas de rendimiento del worker

### Alertas Recomendadas
- PLC desconectado > 30 segundos
- Errores consecutivos > 10
- Alarmas críticas activadas
- Worker detenido inesperadamente

---

## 🔗 Enlaces Útiles

- [Protocolo Modbus TCP](https://www.modbus.org/docs/Modbus_Application_Protocol_V1_1b3.pdf)
- [Background Services en .NET](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services)
- [Serilog Configuration](https://serilog.net/)

---
**Desarrollado para Sistema de Peaje** 🚧
