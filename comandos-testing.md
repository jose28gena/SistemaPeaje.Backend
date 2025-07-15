# Colección de Comandos para Testing del Sistema PLC

## 1. Comando Abrir Barrera

### PowerShell
```powershell
# Configurar SSL
[System.Net.ServicePointManager]::ServerCertificateValidationCallback = {$true}

# Abrir barrera
$body = @{
    casetaIp = "127.0.0.1"
    coilAddress = 96  # Coil 000097 en simulador
    unitId = 1
    carrilId = 1
    observaciones = "Comando de prueba para abrir barrera"
} | ConvertTo-Json

Invoke-RestMethod -Uri "https://localhost:51393/api/comandos/abrir-barrera" -Method POST -Body $body -ContentType "application/json"
```

### cURL
```bash
curl -X 'POST' \
  'https://localhost:51393/api/comandos/abrir-barrera' \
  -H 'accept: */*' \
  -H 'Content-Type: application/json' \
  -d '{
  "casetaIp": "127.0.0.1",
  "coilAddress": 96,
  "unitId": 1,
  "carrilId": 1,
  "observaciones": "sad"
}'
```

## 2. Verificar Estado de Workers

### PowerShell
```powershell
Invoke-RestMethod -Uri "https://localhost:51393/api/PlcConfiguracion/workers/status" -Method GET
```

### cURL
```bash
curl -X 'GET' \
  'https://localhost:51393/api/PlcConfiguracion/workers/status' \
  -H 'accept: */*'
```

## 3. Obtener Configuraciones Activas

### PowerShell
```powershell
Invoke-RestMethod -Uri "https://localhost:51393/api/PlcConfiguracion/activas" -Method GET
```

### cURL
```bash
curl -X 'GET' \
  'https://localhost:51393/api/PlcConfiguracion/activas' \
  -H 'accept: */*'
```

## 4. Crear Nueva Configuración PLC

### PowerShell
```powershell
$config = @{
    nombre = "PLC Test"
    ip = "127.0.0.1"
    puerto = 502
    unitId = 1
    direccionInicial = 100
    cantidadCoils = 4
    intervaloMonitoreo = 1000
    habilitarLoggingPeriodico = $true
    estacionId = 1
    carrilId = 1
    observaciones = "PLC de prueba"
    coilsConfiguracion = @(
        @{
            indice = 0
            direccion = 100
            nombre = "TestCoil1"
            tipoEvento = "TEST_EVENT"
            generarEvento = $true
            esAlarma = $false
        }
    )
} | ConvertTo-Json -Depth 3

Invoke-RestMethod -Uri "https://localhost:51393/api/PlcConfiguracion" -Method POST -Body $config -ContentType "application/json"
```

### cURL
```bash
curl -X 'POST' \
  'https://localhost:51393/api/PlcConfiguracion' \
  -H 'accept: */*' \
  -H 'Content-Type: application/json' \
  -d '{
  "nombre": "PLC Test",
  "ip": "127.0.0.1",
  "puerto": 502,
  "unitId": 1,
  "direccionInicial": 100,
  "cantidadCoils": 4,
  "intervaloMonitoreo": 1000,
  "habilitarLoggingPeriodico": true,
  "estacionId": 1,
  "carrilId": 1,
  "observaciones": "PLC de prueba",
  "coilsConfiguracion": [
    {
      "indice": 0,
      "direccion": 100,
      "nombre": "TestCoil1",
      "tipoEvento": "TEST_EVENT",
      "generarEvento": true,
      "esAlarma": false
    }
  ]
}'
```

## 5. Eliminar Configuración PLC

### PowerShell
```powershell
# Eliminar PLC con ID 3 (el simulador)
Invoke-RestMethod -Uri "https://localhost:51393/api/PlcConfiguracion/3" -Method DELETE
```

### cURL
```bash
curl -X 'DELETE' \
  'https://localhost:51393/api/PlcConfiguracion/3' \
  -H 'accept: */*'
```

## 6. Comandos de Monitoreo

### Ver logs en tiempo real
```powershell
Get-Content "src\SistemaPeaje.API\logs\sistema-peaje-*.txt" -Wait | Where-Object { $_ -like "*PLC*" }
```

### Ver últimos 20 logs
```powershell
Get-Content "src\SistemaPeaje.API\logs\sistema-peaje-*.txt" | Select-Object -Last 20
```

### Filtrar logs de eventos
```powershell
Get-Content "src\SistemaPeaje.API\logs\sistema-peaje-*.txt" | Where-Object { $_ -like "*BarreraAbierta*" -or $_ -like "*PresenciaVehiculo*" }
```

## 7. Scripts de Prueba Completos

### Ejecutar configuración completa de simulador
```powershell
.\test-simulador-simple.ps1
```

### Ejecutar comando abrir barrera
```powershell
.\test-abrir-barrera.ps1
```

### Ejecutar pruebas del sistema escalable
```powershell
.\test-sistema-escalable.ps1
```

## Notas Importantes

### Mapeo de Direcciones:
- **Simulador PLC**: Base 1 (000097, 000098, 000099, 000100)
- **Código .NET**: Base 0 (96, 97, 98, 99)

### Coils del Simulador:
- **000097** (96 en código): Presencia de vehículo
- **000098** (97 en código): Barrera abierta
- **000099** (98 en código): Sentido del carril
- **000100** (99 en código): Modo emergencia

### SSL en Desarrollo:
Para PowerShell, siempre incluir:
```powershell
[System.Net.ServicePointManager]::ServerCertificateValidationCallback = {$true}
```
