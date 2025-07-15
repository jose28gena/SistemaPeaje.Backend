# 🔧 Sistema Escalable de Monitoreo de PLCs - Configuración desde Base de Datos

## 📋 Descripción

Se ha actualizado el sistema para soportar **múltiples PLCs dinámicamente** desde la base de datos, reemplazando la configuración estática de `appsettings.json`. Ahora es completamente escalable y permite gestionar múltiples estaciones de peaje.

## 🏗️ Arquitectura Actualizada

### Componentes Principales

1. **PlcManagerService**: Coordina múltiples workers dinámicamente
2. **PlcConfiguracionService**: Gestiona configuraciones en base de datos
3. **PlcConfiguracion + PlcCoilConfiguracion**: Entidades para configuración dinámica
4. **PlcConfiguracionController**: API para gestión de configuraciones

### Diagrama de Flujo Escalable
```
┌─────────────────────┐    ┌─────────────────────┐    ┌─────────────────────┐
│   Base de Datos     │◄──►│  PlcManagerService  │◄──►│   Multiple PLCs     │
│ (PlcConfiguracion)  │    │   (Coordinator)     │    │  (Dynamic Workers)  │
└─────────────────────┘    └─────────────────────┘    └─────────────────────┘
                                      │
                                      ▼
                            ┌─────────────────────┐
                            │   EventoTransito    │
                            │    (Database)       │
                            └─────────────────────┘
```

## 🗄️ Estructura de Base de Datos

### Tabla PlcConfiguraciones
```sql
PlcConfiguraciones:
- Id (int, PK)
- Nombre (nvarchar(100)) -- "PLC Caseta Principal"
- Ip (nvarchar(50)) -- "192.168.1.103"  
- Puerto (int) -- 502
- UnitId (tinyint) -- 1
- DireccionInicial (int) -- 1000
- CantidadCoils (int) -- 4
- IntervaloMonitoreo (int) -- 2000 (ms)
- HabilitarLoggingPeriodico (bit)
- EstacionId (int, FK)
- CarrilId (int, FK)
- EstaConectado (bit)
- UltimaConexion (datetime2)
- Observaciones (nvarchar(500))
- FechaCreacion, FechaActualizacion, Activo
```

### Tabla PlcCoilConfiguraciones
```sql
PlcCoilConfiguraciones:
- Id (int, PK)
- PlcConfiguracionId (int, FK)
- Indice (int) -- 0, 1, 2, 3...
- Direccion (int) -- 1000, 1001, 1002...
- Nombre (nvarchar(100)) -- "Presencia", "BarreraAbierta"
- Descripcion (nvarchar(500))
- TipoEvento (nvarchar(50)) -- "VEHICULO_DETECTADO"
- GenerarEvento (bit) -- Si debe generar eventos en BD
- EsAlarma (bit) -- Si es alarma crítica
- EstadoActual (bit) -- Último estado leído
- UltimaActualizacion (datetime2)
- AccionEspecial (nvarchar(200)) -- Acción personalizada
- FechaCreacion, FechaActualizacion, Activo
```

## 🚀 Cómo Usar el Sistema

### 1. Ejecutar Migración
```bash
cd "c:\Users\Denneb - Frontend\Documents\GitHub\SistemaPeaje\SistemaPeaje.API"
dotnet ef database update --project src/SistemaPeaje.Infrastructure --startup-project src/SistemaPeaje.API
```

### 2. Crear Configuraciones de Ejemplo
```http
POST /api/PlcConfiguracion/seed
```

### 3. Iniciar la Aplicación
```bash
dotnet run --project src/SistemaPeaje.API
```

**Logs esperados:**
```
🚀 PlcManagerService iniciado - Gestión dinámica de múltiples PLCs
✅ Worker iniciado para PLC PLC Caseta Principal (192.168.1.103:502)
✅ Worker iniciado para PLC PLC Caseta Salida (192.168.1.104:502)
🔍 Iniciando monitoreo de PLC PLC Caseta Principal en 192.168.1.103:502
🔍 Iniciando monitoreo de PLC PLC Caseta Salida en 192.168.1.104:502
```

## 📊 API Endpoints

### Gestión de Configuraciones

#### 1. Obtener Todas las Configuraciones
```http
GET /api/PlcConfiguracion
```

#### 2. Obtener Configuraciones Activas
```http
GET /api/PlcConfiguracion/activas
```

#### 3. Crear Nueva Configuración de PLC
```http
POST /api/PlcConfiguracion
Content-Type: application/json

{
  "nombre": "PLC Caseta Norte",
  "ip": "192.168.1.105",
  "puerto": 502,
  "unitId": 1,
  "direccionInicial": 3000,
  "cantidadCoils": 5,
  "intervaloMonitoreo": 2000,
  "habilitarLoggingPeriodico": true,
  "estacionId": 2,
  "carrilId": 1,
  "observaciones": "Nueva caseta norte",
  "coilsConfiguracion": [
    {
      "indice": 0,
      "direccion": 3000,
      "nombre": "PresenciaNorte",
      "tipoEvento": "VEHICULO_DETECTADO_NORTE",
      "generarEvento": true,
      "esAlarma": false
    },
    {
      "indice": 1,
      "direccion": 3001,
      "nombre": "BarreraNorte",
      "tipoEvento": "BARRERA_NORTE_ABIERTA",
      "generarEvento": true,
      "esAlarma": false
    }
  ]
}
```

#### 4. Actualizar Configuración
```http
PUT /api/PlcConfiguracion/{id}
Content-Type: application/json

{
  "nombre": "PLC Caseta Norte Actualizado",
  "ip": "192.168.1.105",
  "puerto": 502,
  "unitId": 1,
  "direccionInicial": 3000,
  "cantidadCoils": 6,
  "intervaloMonitoreo": 1500,
  "habilitarLoggingPeriodico": true,
  "estacionId": 2,
  "carrilId": 1,
  "observaciones": "Configuración actualizada"
}
```

#### 5. Eliminar (Desactivar) Configuración
```http
DELETE /api/PlcConfiguracion/{id}
```

#### 6. Estado de Workers Activos
```http
GET /api/PlcConfiguracion/workers/status
```

**Respuesta:**
```json
{
  "1": "Conectado",
  "2": "Desconectado",
  "3": "Iniciando"
}
```

### Monitoreo Individual de PLCs

#### Estado de PLC Específico (usando el controlador original)
```http
GET /api/PlcMonitor/status
GET /api/PlcMonitor/coils
```

## 🔄 Funcionamiento Dinámico

### 1. Detección Automática de Cambios
- El `PlcManagerService` revisa la base de datos cada **30 segundos**
- Detecta nuevas configuraciones y las inicia automáticamente
- Detecta configuraciones modificadas y reinicia los workers
- Detecta configuraciones eliminadas y detiene los workers

### 2. Logs de Cambios Dinámicos
```
🔄 Reiniciando worker para PLC PLC Caseta Principal debido a cambios de configuración
✅ Worker iniciado para PLC Nueva Configuración (192.168.1.106:502)
🛑 Worker detenido para configuración ID 3
```

### 3. Estado en Tiempo Real
- La base de datos se actualiza con el estado de conexión de cada PLC
- Los eventos se registran automáticamente en `EventoTransito`
- Los coils mantienen su último estado en la configuración

## 🎛️ Ejemplos de Configuración

### Configuración 1: Caseta Principal
```json
{
  "nombre": "PLC Caseta Principal",
  "ip": "192.168.1.103",
  "puerto": 502,
  "unitId": 1,
  "direccionInicial": 1000,
  "cantidadCoils": 4,
  "intervaloMonitoreo": 2000,
  "estacionId": 1,
  "carrilId": 1,
  "coilsConfiguracion": [
    {
      "indice": 0,
      "direccion": 1000,
      "nombre": "Presencia",
      "tipoEvento": "VEHICULO_DETECTADO",
      "generarEvento": true,
      "esAlarma": false
    },
    {
      "indice": 1,
      "direccion": 1001,
      "nombre": "BarreraAbierta",
      "tipoEvento": "BARRERA_ABIERTA",
      "generarEvento": true,
      "esAlarma": false
    },
    {
      "indice": 2,
      "direccion": 1002,
      "nombre": "SentidoAB",
      "tipoEvento": "CAMBIO_SENTIDO",
      "generarEvento": true,
      "esAlarma": false
    },
    {
      "indice": 3,
      "direccion": 1003,
      "nombre": "Alarma",
      "tipoEvento": "ALARMA_ACTIVADA",
      "generarEvento": true,
      "esAlarma": true,
      "accionEspecial": "NOTIFICACION_URGENTE"
    }
  ]
}
```

### Configuración 2: Caseta de Salida
```json
{
  "nombre": "PLC Caseta Salida",
  "ip": "192.168.1.104", 
  "puerto": 502,
  "unitId": 1,
  "direccionInicial": 2000,
  "cantidadCoils": 3,
  "intervaloMonitoreo": 2000,
  "estacionId": 1,
  "carrilId": 2,
  "coilsConfiguracion": [
    {
      "indice": 0,
      "direccion": 2000,
      "nombre": "PresenciaSalida",
      "tipoEvento": "VEHICULO_SALIDA",
      "generarEvento": true
    },
    {
      "indice": 1,
      "direccion": 2001,
      "nombre": "BarreraSalida",
      "tipoEvento": "BARRERA_SALIDA_ABIERTA",
      "generarEvento": true
    },
    {
      "indice": 2,
      "direccion": 2002,
      "nombre": "AlarmaSalida",
      "tipoEvento": "ALARMA_SALIDA",
      "generarEvento": true,
      "esAlarma": true
    }
  ]
}
```

## 📈 Ventajas del Sistema Escalable

### ✅ Beneficios

1. **Escalabilidad**: Agregar/quitar PLCs sin reiniciar aplicación
2. **Flexibilidad**: Configuración personalizada por PLC y coil
3. **Mantenimiento**: Cambios de configuración en tiempo real
4. **Monitoreo**: Estado individual de cada PLC
5. **Auditoría**: Historial de cambios en base de datos
6. **Especialización**: Eventos y alarmas personalizadas por coil

### 🚀 Casos de Uso

- **Expansión de peaje**: Nuevas casetas automáticamente
- **Mantenimiento**: Desactivar PLCs temporalmente
- **Testing**: Configuraciones de prueba sin afectar producción
- **Diferentes protocolos**: Diferentes puertos/IPs por zona
- **Eventos específicos**: Alarmas críticas vs informativas

## 🔧 Migración desde Configuración Estática

### Pasos para Migrar

1. **Ejecutar migración de BD**
2. **Crear configuraciones vía API** (usar endpoint `/seed`)
3. **Remover configuración estática** de `appsettings.json` ✅ (ya hecho)
4. **Verificar logs** de inicio del PlcManagerService

### Diferencias Principales

| Antes (Estático) | Ahora (Dinámico) |
|------------------|------------------|
| 1 PLC en appsettings.json | N PLCs en base de datos |
| Reinicio para cambios | Cambios en tiempo real |
| PlcMonitorWorker único | PlcManagerService + múltiples workers |
| Configuración fija | Configuración por PLC y coil |
| Sin estado persistente | Estado en BD |

## 🧪 Pruebas con Simulador PLC

### ⚠️ **Diferencias en Numeración de Direcciones**

**CRÍTICO**: El índice en NModbus4 es base 0, mientras que las direcciones del simulador son base 1.

| Dirección Simulador | Índice en Código .NET | Función |
|---------------------|----------------------|---------|
| 000097 | 96 | Presencia de vehículo |
| 000098 | 97 | Barrera abierta |
| 000099 | 98 | Sentido del carril |
| 000100 | 99 | Modo emergencia |

### 💻 **Código de Ejemplo para PLC**

```csharp
// ❌ INCORRECTO: Usar dirección del simulador directamente
await plc.LeerCoilsAsync(97, 1); // Esto lee el coil 000098, no 000097

// ✅ CORRECTO: Convertir dirección a índice base 0
await plc.LeerCoilsAsync(96, 1); // Esto lee el coil 000097

// Helper para conversión
public static int SimuladorAIndice(int direccionSimulador) => direccionSimulador - 1;
public static int IndiceASimulador(int indice) => indice + 1;
```

### 🔧 **Configuración de Base de Datos**

```csharp
// Configurar coils usando índices base 0 (para el código .NET)
var plcConfig = new PlcConfiguracion 
{
    Nombre = "PLC Caseta Test",
    DireccionIp = "127.0.0.1",
    Puerto = 502,
    UnitId = 1,
    Activo = true,
    Coils = new List<PlcCoilConfiguracion>
    {
        new() { Direccion = 96, Nombre = "PresenciaVehiculo", Activo = true },  // Simula 000097
        new() { Direccion = 97, Nombre = "BarreraAbierta", Activo = true },     // Simula 000098
        new() { Direccion = 98, Nombre = "SentidoCarrilAB", Activo = true },    // Simula 000099
        new() { Direccion = 99, Nombre = "ModoEmergencia", Activo = true }      // Simula 000100
    }
};
```

### 📋 **Script de Prueba**

Usar `test-simulador-plc.ps1` para:
- Configurar PLC de prueba automáticamente
- Verificar conectividad con simulador
- Validar lectura de coils
- Probar escritura de comandos

```powershell
# Ejecutar script de prueba con simulador
.\test-simulador-plc.ps1
```

---

## 🎉 Sistema Listo para Producción

El sistema ahora soporta:
- ✅ **Múltiples PLCs dinámicos**
- ✅ **Configuración desde base de datos**
- ✅ **Gestión en tiempo real**
- ✅ **API completa de administración**
- ✅ **Escalabilidad empresarial**

**¡Perfecto para gestionar múltiples estaciones de peaje!** 🚧
