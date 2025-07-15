# 🎯 Sistema de Peaje - Archivo HTTP Completo

## 📋 Resumen de la Implementación

Se ha creado un archivo HTTP completo para el Sistema de Peaje que incluye **42 peticiones HTTP** organizadas y documentadas para facilitar el testing y la gestión del sistema.

## 📂 Archivos Creados

### 1. **peticiones-sistema-peaje.http**
- **42 peticiones HTTP** categorizadas
- **Variables de entorno** configurables
- **Comentarios descriptivos** para cada endpoint
- **Ejemplos de payload** completos

### 2. **GUIA_PETICIONES_HTTP.md** (Actualizada)
- Instrucciones completas de uso
- Casos de uso comunes
- Troubleshooting
- Mejores prácticas

## 🎯 Categorías de Peticiones

### 1. **Configuración de PLCs** (10 peticiones)
- Listar, crear, actualizar, eliminar configuraciones
- Inicializar datos de prueba (seed)
- Gestión de workers (estado, reiniciar, detener, iniciar)

### 2. **Configuración de Coils** (4 peticiones)
- Gestión completa de coils por PLC
- Crear, actualizar, eliminar coils

### 3. **Comandos de Control** (6 peticiones)
- Comando principal `abrir-barrera`
- Comandos personalizados de coils
- Múltiples pruebas de apertura

### 4. **Monitoreo y Eventos** (5 peticiones)
- Estado del sistema
- Eventos de tránsito
- Logs del sistema con filtros

### 5. **Entidades del Sistema** (8 peticiones)
- Carriles, estaciones, transacciones
- Tipos de vehículo, tipos de pago, tarifas
- Usuarios, empleados, turnos

### 6. **Reportes** (2 peticiones)
- Reportes por fecha
- Reportes por carril

### 7. **Diagnóstico** (7 peticiones)
- Verificación de salud del sistema
- Diagnóstico de PLCs
- Verificación de conectividad

## 🚀 Características Destacadas

### Variables de Entorno
```http
@baseUrl = https://localhost:51393
@contentType = application/json
```

### Peticiones Más Importantes

#### 1. **Inicialización del Sistema**
```http
POST {{baseUrl}}/api/PlcConfiguracion/seed
```

#### 2. **Comando Principal - Abrir Barrera**
```http
POST {{baseUrl}}/api/comandos/abrir-barrera
Content-Type: {{contentType}}

{
  "casetaIp": "127.0.0.1",
  "coilAddress": 98,
  "unitId": 1,
  "carrilId": 1,
  "observaciones": "Apertura de barrera desde prueba HTTP"
}
```

#### 3. **Estado de Workers**
```http
GET {{baseUrl}}/api/PlcConfiguracion/workers/status
```

#### 4. **Monitoreo de Eventos**
```http
GET {{baseUrl}}/api/MonitorEventos/eventos-transito
```

## 🛠️ Compatibilidad

### Herramientas Soportadas
- **REST Client** (VS Code) - Recomendado
- **Thunder Client** (VS Code)
- **Postman** (Importar colección)
- **IntelliJ IDEA** (Soporte nativo)

### Integración con PowerShell
- Equivalencias documentadas con scripts existentes
- Automatización con `test-maestro.ps1`
- Verificación con `verificar-servidor.ps1`

## 🎯 Flujo de Trabajo Recomendado

### 1. **Configuración Inicial**
```
1. POST /api/PlcConfiguracion/seed
2. GET /api/PlcConfiguracion
3. GET /api/PlcConfiguracion/workers/status
```

### 2. **Prueba de Funcionalidad**
```
1. POST /api/comandos/abrir-barrera
2. GET /api/MonitorEventos/eventos-transito
3. GET /api/logs
```

### 3. **Diagnóstico**
```
1. GET /api/health
2. GET /api/PlcConfiguracion/diagnostico
3. GET /api/PlcMonitor/conexion/127.0.0.1:502
```

## 📊 Ventajas del Archivo HTTP

### 1. **Facilidad de Uso**
- Sin necesidad de configurar Postman
- Directamente en VS Code
- Ejecución con un clic

### 2. **Documentación Integrada**
- Comentarios descriptivos
- Ejemplos de payload
- Casos de uso explicados

### 3. **Mantenibilidad**
- Variables centralizadas
- Fácil actualización
- Versionado con Git

### 4. **Pruebas Completas**
- Cobertura de todos los endpoints
- Pruebas de estrés incluidas
- Verificación de resultados

## 🔧 Configuración Necesaria

### Servidor
```
URL: https://localhost:51393
Estado: Debe estar ejecutándose
Base de datos: Debe estar accesible
```

### Simulador PLC (Opcional)
```
IP: 127.0.0.1
Puerto: 502
Unit ID: 1
Coils: 98, 99, 100
```

## 📝 Instrucciones de Uso

### En VS Code
1. Instalar extensión "REST Client"
2. Abrir archivo `peticiones-sistema-peaje.http`
3. Hacer clic en "Send Request" sobre cualquier petición
4. Ver respuesta en panel derecho

### En Thunder Client
1. Instalar extensión "Thunder Client"
2. Importar → HTTP File → Seleccionar archivo
3. Ejecutar peticiones desde la interfaz

## 🚨 Solución de Problemas

### Error 500 - Servidor No Responde
```
✅ Verificar que el servidor esté ejecutándose
✅ Comprobar puerto 51393 disponible
✅ Revisar logs del servidor
```

### Error 404 - Endpoint No Encontrado
```
✅ Verificar URL base correcta
✅ Comprobar que el endpoint exista
✅ Revisar versión de la API
```

### Error de Conexión con PLC
```
✅ Verificar IP y puerto del PLC
✅ Comprobar que el simulador esté ejecutándose
✅ Revisar configuración de firewall
```

## 🎯 Resultados Esperados

### Respuesta Exitosa
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

### Estado del Worker
```json
{
  "success": true,
  "data": [
    {
      "plcId": 1,
      "nombre": "PLC_CASETA_01",
      "estado": "Activo",
      "ultimaConexion": "2025-07-14T10:30:00Z",
      "errores": 0
    }
  ]
}
```

## 📚 Documentación Relacionada

- `SISTEMA_ESCALABLE_PLC.md` - Documentación técnica completa
- `EJEMPLOS_CODIGO_PLC.md` - Ejemplos de código .NET
- `ERRORES_CORREGIDOS.md` - Historial de correcciones
- `MISION_CUMPLIDA.md` - Resumen del proyecto
- `comandos-curl.md` - Comandos cURL alternativos
- `comandos-testing.md` - Comandos de testing

## 🎉 Conclusión

El archivo HTTP completo proporciona:
- **Testing completo** del sistema
- **Documentación integrada** 
- **Facilidad de uso** en VS Code
- **Cobertura total** de endpoints
- **Integración** con herramientas existentes

**¡El sistema está listo para uso en producción y testing!**
