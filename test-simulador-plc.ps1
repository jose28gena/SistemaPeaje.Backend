#!/usr/bin/env pwsh
# 🧪 Script de Prueba - Simulador PLC
# ==========================================
# Este script configura automáticamente un PLC de prueba en la base de datos
# para trabajar con un simulador Modbus TCP local.

# ⚠️  IMPORTANTE: Conversión de Direcciones
# =========================================
# Simulador PLC:     000097, 000098, 000099, 000100 (base 1)
# Código .NET:       96,     97,     98,     99      (base 0)
# 
# En la configuración de BD usamos los índices base 0 para el código .NET

Write-Host "🧪 Configurando PLC para Simulador Local..." -ForegroundColor Green

# URL base de la API
$baseUrl = "https://localhost:51393"

# Configuración para simulador con direcciones convertidas a base 0
$configSimulador = @{
    nombre = "PLC Simulador Local"
    ip = "127.0.0.1"
    puerto = 502
    unitId = 1
    direccionInicial = 96  # Dirección 000097 del simulador en base 0
    cantidadCoils = 4
    intervaloMonitoreo = 1000
    habilitarLoggingPeriodico = $true
    estacionId = 1
    carrilId = 1
    observaciones = "Simulador local - Dirs: 000097-000100 -> indices 96-99"
    coilsConfiguracion = @(
        @{
            indice = 0
            direccion = 96  # 000097 en simulador -> 96 en código
            nombre = "PresenciaVehiculo"
            tipoEvento = "VEHICULO_DETECTADO"
            generarEvento = $true
            esAlarma = $false
        },
        @{
            indice = 1
            direccion = 97  # 000098 en simulador -> 97 en código
            nombre = "BarreraAbierta"
            tipoEvento = "BARRERA_ABIERTA"
            generarEvento = $true
            esAlarma = $false
        },
        @{
            indice = 2
            direccion = 98  # 000099 en simulador -> 98 en código
            nombre = "SentidoCarrilAB"
            tipoEvento = "SENTIDO_AB"
            generarEvento = $false
            esAlarma = $false
        },
        @{
            indice = 3
            direccion = 99  # 000100 en simulador -> 99 en código
            nombre = "ModoEmergencia"
            tipoEvento = "EMERGENCIA"
            generarEvento = $true
            esAlarma = $true
        }
    )
}

Write-Host "`n📝 Creando configuración para simulador..."
try {
    $jsonBody = $configSimulador | ConvertTo-Json -Depth 3
    $response = Invoke-RestMethod -Uri "$baseUrl/api/PlcConfiguracion" -Method POST -Body $jsonBody -ContentType "application/json"
    Write-Host "✅ Configuración de simulador creada:" -ForegroundColor Green
    Write-Host "  ID: $($response.id), Nombre: $($response.nombre)" -ForegroundColor White
    $simuladorId = $response.id
} catch {
    Write-Host "❌ Error al crear configuración de simulador: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "ℹ️ Puede que ya exista una configuración para 127.0.0.1:502" -ForegroundColor Yellow
    $simuladorId = $null
}

Start-Sleep -Seconds 3

Write-Host "`n🔍 Verificando estado del worker del simulador..."
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/PlcConfiguracion/workers/status" -Method GET
    Write-Host "✅ Estado de workers:" -ForegroundColor Green
    $response | ConvertTo-Json -Depth 2
    
    if ($response.PSObject.Properties.Value -contains "Conectado") {
        Write-Host "🎉 ¡Simulador conectado exitosamente!" -ForegroundColor Green
    } else {
        Write-Host "⚠️ Simulador no conectado. Verificar que esté corriendo en 127.0.0.1:502" -ForegroundColor Yellow
    }
} catch {
    Write-Host "❌ Error al verificar workers: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n📋 Información del Simulador:" -ForegroundColor Cyan
Write-Host "  🌐 IP: 127.0.0.1:502" -ForegroundColor White
Write-Host "  📍 Direcciones a configurar en simulador:" -ForegroundColor White
Write-Host "    - Coil 000097 (Presencia de vehículo)" -ForegroundColor Gray
Write-Host "    - Coil 000098 (Barrera abierta)" -ForegroundColor Gray
Write-Host "    - Coil 000099 (Sentido del carril AB)" -ForegroundColor Gray
Write-Host "    - Coil 000100 (Modo emergencia)" -ForegroundColor Gray

Write-Host "`n🔧 Para probar:" -ForegroundColor Cyan
Write-Host "  1. Asegúrate que el simulador PLC esté corriendo en 127.0.0.1:502" -ForegroundColor White
Write-Host "  2. Configura los coils 000097-000100 en el simulador" -ForegroundColor White
Write-Host "  3. Cambia los valores de los coils y observa los logs" -ForegroundColor White
Write-Host "  4. Monitorea los logs: Get-Content 'src\SistemaPeaje.API\logs\sistema-peaje-*.txt' -Wait" -ForegroundColor White

Write-Host "`n📊 Monitorear en tiempo real:" -ForegroundColor Cyan
Write-Host "  dotnet run --project src/SistemaPeaje.API" -ForegroundColor White

if ($simuladorId) {
    Write-Host "`n🗑️ Para limpiar después de las pruebas:" -ForegroundColor Yellow
    Write-Host "  Invoke-RestMethod -Uri '$baseUrl/api/PlcConfiguracion/$simuladorId' -Method DELETE" -ForegroundColor Gray
}
