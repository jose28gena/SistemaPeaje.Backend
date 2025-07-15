# Comandos cURL para Testing del Sistema PLC

## Comando para Abrir Barrera

### cURL Command
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

### PowerShell Equivalent
```powershell
# Configurar SSL para desarrollo
[System.Net.ServicePointManager]::ServerCertificateValidationCallback = {$true}

# Comando para abrir barrera
$body = @{
    casetaIp = "127.0.0.1"
    coilAddress = 96
    unitId = 1
    carrilId = 1
    observaciones = "Comando de prueba para abrir barrera"
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri "https://localhost:51393/api/comandos/abrir-barrera" -Method POST -Body $body -ContentType "application/json"
    Write-Host "✅ Comando ejecutado exitosamente:" -ForegroundColor Green
    Write-Host $response -ForegroundColor White
} catch {
    Write-Host "❌ Error al ejecutar comando: $($_.Exception.Message)" -ForegroundColor Red
}
```

## Notas sobre el Comando

### Parámetros:
- **casetaIp**: `127.0.0.1` (IP del simulador PLC)
- **coilAddress**: `96` (Dirección base 0 que corresponde al coil 000097 del simulador)
- **unitId**: `1` (ID de la unidad Modbus)
- **carrilId**: `1` (ID del carril en la base de datos)
- **observaciones**: Texto descriptivo del comando

### Mapeo de Direcciones:
- **Código .NET**: 96 (base 0)
- **Simulador PLC**: 000097 (base 1)
- **Función**: Control de barrera

### Uso:
Este comando envía una señal para abrir la barrera del PLC simulador. 
El sistema debería:
1. Escribir `TRUE` al coil 96 (000097 en simulador)
2. Registrar el evento en la base de datos
3. Generar logs del comando ejecutado

### Para Probar:
1. Asegurarse que el simulador PLC esté corriendo en 127.0.0.1:502
2. Configurar coil 000097 en el simulador
3. Ejecutar el comando
4. Verificar que el coil cambió a TRUE en el simulador
5. Revisar logs del sistema para confirmar la ejecución
