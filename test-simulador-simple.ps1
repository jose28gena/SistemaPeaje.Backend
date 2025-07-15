# Script de Prueba - Simulador PLC
# Configuracion automatica para simulador local

Write-Host "Configurando PLC para Simulador Local..." -ForegroundColor Green

$baseUrl = "https://localhost:51393"

# Configuracion para simulador
$configSimulador = @{
    nombre = "PLC Simulador Local"
    ip = "127.0.0.1"
    puerto = 502
    unitId = 1
    direccionInicial = 96
    cantidadCoils = 4
    intervaloMonitoreo = 1000
    habilitarLoggingPeriodico = $true
    estacionId = 1
    carrilId = 1
    observaciones = "Simulador local - Dirs: 000097-000100 -> indices 96-99"
    coilsConfiguracion = @(
        @{
            indice = 0
            direccion = 96
            nombre = "PresenciaVehiculo"
            tipoEvento = "VEHICULO_DETECTADO"
            generarEvento = $true
            esAlarma = $false
        },
        @{
            indice = 1
            direccion = 97
            nombre = "BarreraAbierta"
            tipoEvento = "BARRERA_ABIERTA"
            generarEvento = $true
            esAlarma = $false
        },
        @{
            indice = 2
            direccion = 98
            nombre = "SentidoCarrilAB"
            tipoEvento = "SENTIDO_AB"
            generarEvento = $false
            esAlarma = $false
        },
        @{
            indice = 3
            direccion = 99
            nombre = "ModoEmergencia"
            tipoEvento = "EMERGENCIA_ACTIVADA"
            generarEvento = $true
            esAlarma = $true
        }
    )
}

Write-Host "Intentando crear configuracion de simulador..."

# Configurar SSL
[System.Net.ServicePointManager]::ServerCertificateValidationCallback = {$true}

try {
    $jsonBody = $configSimulador | ConvertTo-Json -Depth 3
    $response = Invoke-RestMethod -Uri "$baseUrl/api/PlcConfiguracion" -Method POST -Body $jsonBody -ContentType "application/json"
    Write-Host "Configuracion de simulador creada:" -ForegroundColor Green
    Write-Host "ID: $($response.id), Nombre: $($response.nombre)" -ForegroundColor White
    $simuladorId = $response.id
} catch {
    Write-Host "Error al crear configuracion: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Puede que ya exista una configuracion para 127.0.0.1:502" -ForegroundColor Yellow
    $simuladorId = $null
}

Start-Sleep -Seconds 3

Write-Host "Verificando estado del worker del simulador..."
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/PlcConfiguracion/workers/status" -Method GET
    Write-Host "Estado de workers:" -ForegroundColor Green
    $response | ConvertTo-Json -Depth 2
    
    if ($response.PSObject.Properties.Value -contains "Conectado") {
        Write-Host "Simulador conectado exitosamente!" -ForegroundColor Green
    } else {
        Write-Host "Simulador no conectado. Verificar que este corriendo en 127.0.0.1:502" -ForegroundColor Yellow
    }
} catch {
    Write-Host "Error al verificar workers: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""
Write-Host "Informacion del Simulador:" -ForegroundColor Cyan
Write-Host "IP: 127.0.0.1:502" -ForegroundColor White
Write-Host "Direcciones a configurar en simulador:" -ForegroundColor White
Write-Host "- Coil 000097 (Presencia de vehiculo)" -ForegroundColor Gray
Write-Host "- Coil 000098 (Barrera abierta)" -ForegroundColor Gray
Write-Host "- Coil 000099 (Sentido del carril AB)" -ForegroundColor Gray
Write-Host "- Coil 000100 (Modo emergencia)" -ForegroundColor Gray

Write-Host ""
Write-Host "Para probar:" -ForegroundColor Cyan
Write-Host "1. Asegurate que el simulador PLC este corriendo en 127.0.0.1:502" -ForegroundColor White
Write-Host "2. Configura los coils 000097-000100 en el simulador" -ForegroundColor White
Write-Host "3. Cambia los valores de los coils y observa los logs" -ForegroundColor White

Write-Host ""
Write-Host "Monitorear en tiempo real:" -ForegroundColor Cyan
Write-Host "dotnet run --project src/SistemaPeaje.API" -ForegroundColor White

if ($simuladorId) {
    Write-Host ""
    Write-Host "Para limpiar despues de las pruebas:" -ForegroundColor Yellow
    Write-Host "Invoke-RestMethod -Uri '$baseUrl/api/PlcConfiguracion/$simuladorId' -Method DELETE" -ForegroundColor Gray
}
