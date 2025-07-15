# Guía de Uso del Archivo HTTP para Sistema de Peaje

## 📋 Descripción General

Este archivo contiene todas las peticiones HTTP necesarias para probar y gestionar el Sistema de Peaje, incluyendo la configuración de PLCs, control de barreras, monitoreo y reportes.

## 📂 Archivo HTTP Completo
Se ha creado el archivo `peticiones-sistema-peaje.http` que contiene:
- **42 peticiones HTTP** organizadas por categorías
- **Variables de entorno** para configuración fácil
- **Comentarios descriptivos** para cada endpoint
- **Ejemplos de payload** para peticiones POST/PUT

## 🛠️ Compatibilidad

El archivo es compatible con herramientas como:
- **REST Client** (Extensión de VS Code)
- **Thunder Client** (Extensión de VS Code)
- **Postman** (Importar como colección)
- **IntelliJ IDEA** (Soporte nativo)

## 🚀 Instalación y Uso

### 1. **Para VS Code (Recomendado)**

#### Instalar REST Client:
```bash
# Desde VS Code: Ctrl+Shift+X
# Buscar: "REST Client" by Huachao Mao
# Instalar
```

#### Usar el archivo:
1. Abrir `peticiones-sistema-peaje.http` en VS Code
2. Hacer clic en "Send Request" sobre cualquier petición
3. Ver la respuesta en el panel derecho

### 2. **Para Thunder Client**

#### Instalar Thunder Client:
```bash
# Desde VS Code: Ctrl+Shift+X
# Buscar: "Thunder Client" by Ranga Vadhineni
# Instalar
```

#### Importar colección:
1. Abrir Thunder Client
2. Importar → HTTP File → Seleccionar `peticiones-sistema-peaje.http`

## 🎯 Estructura del Archivo

### 1. **Configuración de PLCs** (`/api/PlcConfiguracion`)
```http
GET /api/PlcConfiguracion              # Listar todas
POST /api/PlcConfiguracion             # Crear nueva
PUT /api/PlcConfiguracion/{id}         # Actualizar
DELETE /api/PlcConfiguracion/{id}      # Eliminar
POST /api/PlcConfiguracion/seed        # Datos de prueba
```

### 2. **Gestión de Workers**
```http
GET /api/PlcConfiguracion/workers/status    # Estado de workers
POST /api/PlcConfiguracion/workers/{id}/restart  # Reiniciar worker
POST /api/PlcConfiguracion/workers/{id}/stop     # Detener worker
POST /api/PlcConfiguracion/workers/{id}/start    # Iniciar worker
```

### 3. **Configuración de Coils**
```http
GET /api/PlcConfiguracion/{id}/coils        # Coils de un PLC
POST /api/PlcConfiguracion/{id}/coils       # Crear coil
PUT /api/PlcConfiguracion/coils/{id}        # Actualizar coil
DELETE /api/PlcConfiguracion/coils/{id}     # Eliminar coil
```

### 4. **Comandos de Control**
```http
POST /api/comandos/abrir-barrera    # Comando principal
POST /api/comandos/coil             # Comando personalizado
```

### 5. **Monitoreo y Eventos**
```http
GET /api/PlcMonitor/status                    # Estado del sistema
GET /api/MonitorEventos/eventos-transito     # Eventos de tránsito
GET /api/logs                                 # Logs del sistema
```

## 📋 Peticiones Principales

### **Comandos PLC (Más Usados)**

#### 1. **Abrir Barrera** (Coil 98)
```http
POST https://localhost:51393/api/comandos/abrir-barrera
Content-Type: application/json

{
  "casetaIp": "127.0.0.1",
  "coilAddress": 98,
  "unitId": 1,
  "carrilId": 1,
  "observaciones": "Apertura de barrera desde prueba HTTP"
}
```

#### 2. **Verificar Estado de Workers**
```http
GET https://localhost:51393/api/PlcConfiguracion/workers/status
```

#### 3. **Crear Configuración de PLC**
```http
POST https://localhost:51393/api/PlcConfiguracion
Content-Type: application/json

{
  "nombre": "PLC_CASETA_01",
  "direccionIp": "127.0.0.1",
  "puerto": 502,
  "unitId": 1,
  "habilitado": true,
  "intervaloMonitoreo": 5000,
  "timeoutConexion": 3000,
  "intentosReconexion": 3,
  "observaciones": "PLC principal para caseta de pruebas"
}
```

## 🔧 Casos de Uso Comunes

### 1. **Configuración Inicial**
```
1. POST /api/PlcConfiguracion/seed
2. GET /api/PlcConfiguracion
3. GET /api/PlcConfiguracion/workers/status
```

### 2. **Agregar Nuevo PLC**
```
1. POST /api/PlcConfiguracion (crear PLC)
2. POST /api/PlcConfiguracion/{id}/coils (agregar coils)
3. GET /api/PlcConfiguracion/workers/status (verificar worker)
```

### 3. **Controlar Barrera**
```
1. POST /api/comandos/abrir-barrera
2. GET /api/MonitorEventos/eventos-transito
3. GET /api/logs (verificar logs)
```

### 4. **Diagnóstico de Problemas**
```
1. GET /api/PlcConfiguracion/diagnostico
2. GET /api/PlcMonitor/conexion/127.0.0.1:502
3. GET /api/logs?nivel=Error
```

## 🌐 Variables de Entorno

### Variables Principales
```
@baseUrl = https://localhost:51393
@contentType = application/json
```

### Configuración de PLC de Prueba
```
IP: 127.0.0.1
Puerto: 502
Unit ID: 1
Coils: 98, 99, 100
```

## ✅ Respuestas Esperadas

### Configuración Exitosa
```json
{
  "success": true,
  "message": "Configuración creada exitosamente",
  "data": {
    "id": 1,
    "nombre": "PLC_CASETA_01",
    "direccionIp": "127.0.0.1",
    "puerto": 502,
    "habilitado": true
  }
}
```

### Comando Exitoso
```json
{
  "success": true,
  "message": "Comando ejecutado exitosamente",
  "data": {
    "timestamp": "2025-07-14T10:30:00Z",
    "comando": "abrir-barrera",
    "resultado": "Coil 98 activado correctamente"
  }
}
```

## 🛠️ Integración con PowerShell

### Equivalencias de Comandos
```powershell
# Equivalente a POST /api/comandos/abrir-barrera
.\test-abrir-barrera.ps1

# Equivalente a GET /api/PlcConfiguracion/workers/status
.\test-sistema-escalable.ps1

# Equivalente a verificar simulador
.\test-simulador-plc.ps1
```

### Automatización
```powershell
# Ejecutar secuencia completa de pruebas
.\test-maestro.ps1

# Verificar estado del servidor
.\verificar-servidor.ps1
```

## 🚨 Troubleshooting

### Error 500 - Servidor No Responde
```
1. Verificar que el servidor esté ejecutándose
2. Comprobar puerto 51393 disponible
3. Revisar logs del servidor
```

### Error 404 - Endpoint No Encontrado
```
1. Verificar URL base correcta
2. Comprobar que el endpoint exista
3. Revisar versión de la API
```

### Error 400 - Petición Malformada
```
1. Verificar formato JSON
2. Comprobar campos requeridos
3. Validar tipos de datos
```

### Error de Conexión con PLC
```
1. Verificar IP y puerto del PLC
2. Comprobar que el simulador esté ejecutándose
3. Revisar configuración de firewall
```

## 📝 Mejores Prácticas

### 1. **Orden de Ejecución**
1. Configurar PLCs antes de crear coils
2. Verificar workers antes de enviar comandos
3. Monitorear logs después de cada comando

### 2. **Gestión de Errores**
- Siempre verificar respuestas de la API
- Revisar logs en caso de errores
- Usar endpoints de diagnóstico

### 3. **Pruebas**
- Usar datos de prueba consistentes
- Verificar resultados en base de datos
- Monitorear performance del sistema

### 4. **Seguridad**
- Usar HTTPS en producción
- Validar entrada de datos
- Implementar autenticación si es necesario

## 📊 Archivo de Logs

### Ubicación
```
src/SistemaPeaje.API/logs/sistema-peaje-YYYYMMDD[_XXX].txt
```

### Monitoreo en Tiempo Real
```powershell
Get-Content 'src\SistemaPeaje.API\logs\sistema-peaje-*.txt' -Wait
```

### Filtrado de Logs
```powershell
Select-String -Path 'src\SistemaPeaje.API\logs\sistema-peaje-*.txt' -Pattern 'ERROR'
```

## 🎯 Conclusión

Este archivo HTTP proporciona una interfaz completa para probar y gestionar el Sistema de Peaje escalable. Usar en conjunto con los scripts de PowerShell para una experiencia de testing completa.

### 📚 Documentación Relacionada
- Revisar documentación en `SISTEMA_ESCALABLE_PLC.md`
- Consultar ejemplos en `EJEMPLOS_CODIGO_PLC.md`
- Verificar errores corregidos en `ERRORES_CORREGIDOS.md`
- Consultar el archivo `MISION_CUMPLIDA.md` para resumen completo
  "casetaIp": "127.0.0.1",
  "coilAddress": 96,
  "unitId": 1,
  "carrilId": 1,
  "observaciones": "Comando desde archivo .http"
}
```

#### 2. **Verificar Estado Workers**
```http
GET https://localhost:51393/api/PlcConfiguracion/workers/status
```

#### 3. **Obtener Configuraciones Activas**
```http
GET https://localhost:51393/api/PlcConfiguracion/activas
```

### **Gestión de Configuraciones**

#### 4. **Crear Configuración Simulador**
```http
POST https://localhost:51393/api/PlcConfiguracion
Content-Type: application/json

{
  "nombre": "PLC Simulador Local HTTP",
  "ip": "127.0.0.1",
  "puerto": 502,
  "unitId": 1,
  "direccionInicial": 96,
  "cantidadCoils": 4,
  "intervaloMonitoreo": 1000,
  "habilitarLoggingPeriodico": true,
  "estacionId": 1,
  "carrilId": 1,
  "observaciones": "Simulador local",
  "coilsConfiguracion": [...]
}
```

#### 5. **Eliminar Configuración**
```http
DELETE https://localhost:51393/api/PlcConfiguracion/3
```

## 🔧 Variables de Entorno

El archivo usa variables para facilitar el mantenimiento:

```http
@baseUrl = https://localhost:51393
@simuladorIp = 127.0.0.1
```

### Para cambiar el servidor:
```http
@baseUrl = https://servidor-produccion.com
@simuladorIp = 192.168.1.100
```

## 📊 Mapeo de Direcciones

### **Código .NET ↔ Simulador PLC**

| Código .NET | Simulador PLC | Función |
|-------------|---------------|---------|
| 96 | 000097 | Presencia de vehículo |
| 97 | 000098 | Barrera abierta |
| 98 | 000099 | Sentido del carril |
| 99 | 000100 | Modo emergencia |

### **Ejemplos de Uso por Coil**

#### **Coil 96 (000097) - Presencia de Vehículo**
```http
POST {{baseUrl}}/api/comandos/abrir-barrera
Content-Type: application/json

{
  "casetaIp": "{{simuladorIp}}",
  "coilAddress": 96,
  "unitId": 1,
  "carrilId": 1,
  "observaciones": "Activar presencia de vehículo"
}
```

#### **Coil 97 (000098) - Barrera Abierta**
```http
POST {{baseUrl}}/api/comandos/abrir-barrera
Content-Type: application/json

{
  "casetaIp": "{{simuladorIp}}",
  "coilAddress": 97,
  "unitId": 1,
  "carrilId": 1,
  "observaciones": "Abrir barrera de caseta"
}
```

## 📋 Respuestas Esperadas

### **Comando Exitoso**
```json
{
  "exitoso": true,
  "mensaje": "✅ Barrera abierta correctamente",
  "fechaEjecucion": "2025-07-15T01:24:58.2047539Z",
  "datosAdicionales": {
    "TiempoRespuesta": "8ms",
    "CoilAddress": 96,
    "IP": "127.0.0.1"
  }
}
```

### **Estado de Workers**
```json
{
  "1": "Desconectado",
  "2": "Desconectado",
  "3": "Conectado"
}
```

### **Error Típico**
```json
{
  "error": "No se pudo conectar al PLC",
  "detalles": "Timeout al intentar conectar con 127.0.0.1:502"
}
```

## 🧪 Secuencia de Pruebas Recomendada

### **1. Verificación Inicial**
```http
# 1. Verificar estado del sistema
GET {{baseUrl}}/api/PlcConfiguracion/workers/status

# 2. Obtener configuraciones activas
GET {{baseUrl}}/api/PlcConfiguracion/activas
```

### **2. Configurar Simulador (si no existe)**
```http
# 3. Crear configuración de simulador
POST {{baseUrl}}/api/PlcConfiguracion
# (Usar el JSON de configuración del simulador)
```

### **3. Probar Comandos**
```http
# 4. Comando básico - Presencia de vehículo
POST {{baseUrl}}/api/comandos/abrir-barrera
# coilAddress: 96

# 5. Comando barrera - Abrir barrera
POST {{baseUrl}}/api/comandos/abrir-barrera
# coilAddress: 97
```

### **4. Verificar Resultados**
```http
# 6. Verificar estado después de comandos
GET {{baseUrl}}/api/PlcConfiguracion/workers/status

# 7. Obtener eventos generados
GET {{baseUrl}}/api/MonitorEventos/eventos-transito
```

## 🔍 Troubleshooting

### **Error: "Se ha terminado la conexión"**
- Verificar que el servidor esté corriendo: `dotnet run --project src/SistemaPeaje.API`
- Revisar el puerto: debe ser 51393 (HTTPS) o 51394 (HTTP)

### **Error: "No se pudo conectar al PLC"**
- Verificar que el simulador PLC esté corriendo en 127.0.0.1:502
- Revisar la configuración del PLC en la base de datos

### **Error: "Configuración no encontrada"**
- Ejecutar el endpoint de seed: `POST {{baseUrl}}/api/PlcConfiguracion/seed`
- Crear configuración manualmente usando el JSON del simulador

## 📂 Archivos Relacionados

- **`peticiones.http`** - Archivo principal con todas las peticiones
- **`test-barrera-abierta.ps1`** - Script PowerShell equivalente
- **`test-coils-menu.ps1`** - Menú interactivo para probar todos los coils
- **`test-maestro.ps1`** - Script maestro con todas las funcionalidades

## 🎉 Ventajas del Archivo .http

1. **Reutilizable**: Guardar y versionar peticiones
2. **Portable**: Funciona en múltiples herramientas
3. **Documentado**: Comentarios y ejemplos incluidos
4. **Variables**: Fácil cambio de entorno
5. **Completo**: Todas las peticiones del sistema en un archivo

**¡Perfecto para desarrollo, testing y documentación!** 🚀
