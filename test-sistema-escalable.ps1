#!/usr/bin/env pwsh
# Script de prueba para el Sistema Escalable de PLCs
# Ejecutar desde PowerShell

Write-Host "🚀 Iniciando pruebas del Sistema Escalable de PLCs..." -ForegroundColor Green

# URL base de la API
$baseUrl = "https://localhost:51393"  # Puerto correcto según configuración

Write-Host "`n📋 1. Creando configuraciones de ejemplo..."
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/PlcConfiguracion/seed" -Method POST
    Write-Host "✅ Configuraciones de ejemplo creadas:" -ForegroundColor Green
    $response | ConvertTo-Json -Depth 2
} catch {
    Write-Host "❌ Error al crear configuraciones: $($_.Exception.Message)" -ForegroundColor Red
}

Start-Sleep -Seconds 2

Write-Host "`n📋 2. Verificando configuraciones activas..."
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/PlcConfiguracion/activas" -Method GET
    Write-Host "✅ Configuraciones activas:" -ForegroundColor Green
    foreach ($config in $response) {
        Write-Host "  - $($config.nombre) ($($config.ip):$($config.puerto))" -ForegroundColor White
    }
} catch {
    Write-Host "❌ Error al obtener configuraciones: $($_.Exception.Message)" -ForegroundColor Red
}

Start-Sleep -Seconds 2

Write-Host "`n🔍 3. Verificando estado de workers..."
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/PlcConfiguracion/workers/status" -Method GET
    Write-Host "✅ Estado de workers:" -ForegroundColor Green
    $response | ConvertTo-Json -Depth 2
} catch {
    Write-Host "❌ Error al obtener estado de workers: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n📝 4. Creando nueva configuración de PLC..."
$nuevaConfiguracion = @{
    nombre = "PLC Caseta Norte"
    ip = "192.168.1.105"
    puerto = 502
    unitId = 1
    direccionInicial = 3000
    cantidadCoils = 3
    intervaloMonitoreo = 2000
    habilitarLoggingPeriodico = $true
    estacionId = 2
    carrilId = 1
    observaciones = "Caseta de prueba automática"
    coilsConfiguracion = @(
        @{
            indice = 0
            direccion = 3000
            nombre = "PresenciaNorte"
            tipoEvento = "VEHICULO_DETECTADO_NORTE"
            generarEvento = $true
            esAlarma = $false
        },
        @{
            indice = 1
            direccion = 3001
            nombre = "BarreraNorte"
            tipoEvento = "BARRERA_NORTE_ABIERTA"
            generarEvento = $true
            esAlarma = $false
        },
        @{
            indice = 2
            direccion = 3002
            nombre = "AlarmaNorte"
            tipoEvento = "ALARMA_NORTE"
            generarEvento = $true
            esAlarma = $true
        }
    )
}

try {
    $jsonBody = $nuevaConfiguracion | ConvertTo-Json -Depth 3
    $response = Invoke-RestMethod -Uri "$baseUrl/api/PlcConfiguracion" -Method POST -Body $jsonBody -ContentType "application/json"
    Write-Host "✅ Nueva configuración creada:" -ForegroundColor Green
    Write-Host "  ID: $($response.id), Nombre: $($response.nombre)" -ForegroundColor White
    $nuevaConfigId = $response.id
} catch {
    Write-Host "❌ Error al crear nueva configuración: $($_.Exception.Message)" -ForegroundColor Red
    $nuevaConfigId = $null
}

Start-Sleep -Seconds 5

Write-Host "`n🔄 5. Verificando que el worker se inició automáticamente..."
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/PlcConfiguracion/workers/status" -Method GET
    Write-Host "✅ Estado actualizado de workers:" -ForegroundColor Green
    $response | ConvertTo-Json -Depth 2
} catch {
    Write-Host "❌ Error al verificar workers: $($_.Exception.Message)" -ForegroundColor Red
}

if ($nuevaConfigId) {
    Write-Host "`n🔧 6. Actualizando configuración para probar cambios dinámicos..."
    $configActualizada = $nuevaConfiguracion.Clone()
    $configActualizada.intervaloMonitoreo = 1500
    $configActualizada.observaciones = "Configuración actualizada dinámicamente"
    
    try {
        $jsonBody = $configActualizada | ConvertTo-Json -Depth 3
        $response = Invoke-RestMethod -Uri "$baseUrl/api/PlcConfiguracion/$nuevaConfigId" -Method PUT -Body $jsonBody -ContentType "application/json"
        Write-Host "✅ Configuración actualizada. El worker se reiniciará automáticamente." -ForegroundColor Green
    } catch {
        Write-Host "❌ Error al actualizar configuración: $($_.Exception.Message)" -ForegroundColor Red
    }

    Start-Sleep -Seconds 3

    Write-Host "`n🗑️ 7. Eliminando configuración de prueba..."
    try {
        Invoke-RestMethod -Uri "$baseUrl/api/PlcConfiguracion/$nuevaConfigId" -Method DELETE
        Write-Host "✅ Configuración eliminada. El worker se detendrá automáticamente." -ForegroundColor Green
    } catch {
        Write-Host "❌ Error al eliminar configuración: $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host "`n📊 8. Verificando eventos en base de datos..."
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/MonitorEventos/eventos-transito" -Method GET
    Write-Host "✅ Últimos eventos registrados:" -ForegroundColor Green
    
    # Mostrar solo eventos de PLC
    $plcEvents = $response | Where-Object { $_.tipoEvento -like "*PLC*" -or $_.tipoEvento -like "*CAMBIO*" -or $_.tipoEvento -like "*VEHICULO*" }
    if ($plcEvents) {
        $plcEvents | Select-Object -First 10 | ForEach-Object {
            Write-Host "  - $($_.tipoEvento): $($_.descripcion)" -ForegroundColor White
        }
    } else {
        Write-Host "ℹ️ No se encontraron eventos de PLC aún" -ForegroundColor Yellow
    }
} catch {
    Write-Host "❌ Error al verificar eventos: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n✅ Pruebas completadas. Revisar logs de la aplicación para ver el monitoreo en tiempo real." -ForegroundColor Green

Write-Host "`n📋 Para monitorear logs en tiempo real:" -ForegroundColor Cyan
Write-Host "dotnet run --project src/SistemaPeaje.API" -ForegroundColor White

Write-Host "`n📋 Ejemplos de salidas esperadas en logs:" -ForegroundColor Cyan
Write-Host "  🚀 PlcManagerService iniciado - Gestión dinámica de múltiples PLCs" -ForegroundColor White
Write-Host "  ✅ Worker iniciado para PLC PLC Caseta Principal (192.168.1.103:502)" -ForegroundColor White  
Write-Host "  ✅ Worker iniciado para PLC PLC Caseta Salida (192.168.1.104:502)" -ForegroundColor White
Write-Host "  🔍 Iniciando monitoreo de PLC PLC Caseta Principal en 192.168.1.103:502" -ForegroundColor White
Write-Host "  🔄 Reiniciando worker para PLC debido a cambios de configuración" -ForegroundColor White
Write-Host "  🛑 Worker detenido para configuración ID X" -ForegroundColor White

Write-Host "`n🎯 APIs Principales para Gestión:" -ForegroundColor Cyan
Write-Host "  GET    /api/PlcConfiguracion/activas" -ForegroundColor White
Write-Host "  POST   /api/PlcConfiguracion" -ForegroundColor White
Write-Host "  PUT    /api/PlcConfiguracion/{id}" -ForegroundColor White
Write-Host "  DELETE /api/PlcConfiguracion/{id}" -ForegroundColor White
Write-Host "  GET    /api/PlcConfiguracion/workers/status" -ForegroundColor White
