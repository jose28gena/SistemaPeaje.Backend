#!/usr/bin/env pwsh
# Script de prueba para el Background Worker Modbus TCP
# Ejecutar desde PowerShell

Write-Host "🚀 Iniciando pruebas del Background Worker Modbus TCP..." -ForegroundColor Green

# URL base de la API
$baseUrl = "https://localhost:7071"  # Ajustar según configuración

Write-Host "`n📋 1. Verificando estado del monitoreo del PLC..."
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/PlcMonitor/status" -Method GET
    Write-Host "✅ Estado obtenido:" -ForegroundColor Green
    $response | ConvertTo-Json -Depth 3
} catch {
    Write-Host "❌ Error al obtener estado: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n🔌 2. Probando conexión con el PLC..."
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/PlcMonitor/test-connection" -Method GET
    Write-Host "✅ Resultado de la prueba:" -ForegroundColor Green
    $response | ConvertTo-Json -Depth 3
} catch {
    Write-Host "❌ Error en prueba de conexión: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n📊 3. Leyendo estado actual de los coils..."
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/PlcMonitor/coils" -Method GET
    Write-Host "✅ Estados de coils:" -ForegroundColor Green
    $response | ConvertTo-Json -Depth 3
} catch {
    Write-Host "❌ Error al leer coils: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n📝 4. Verificando eventos en base de datos..."
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/MonitorEventos/eventos-transito" -Method GET
    Write-Host "✅ Últimos eventos registrados:" -ForegroundColor Green
    
    # Mostrar solo eventos de PLC
    $plcEvents = $response | Where-Object { $_.tipoEvento -like "*PLC*" -or $_.tipoEvento -like "*CAMBIO*" }
    if ($plcEvents) {
        $plcEvents | Select-Object -First 5 | ConvertTo-Json -Depth 2
    } else {
        Write-Host "ℹ️ No se encontraron eventos de PLC aún" -ForegroundColor Yellow
    }
} catch {
    Write-Host "❌ Error al verificar eventos: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n🔧 5. Ejemplo de escritura de coil (comentado por seguridad)..."
Write-Host "# Para escribir un coil, usar:" -ForegroundColor Yellow
Write-Host "# Invoke-RestMethod -Uri `"$baseUrl/api/PlcMonitor/coils/1001/write`" -Method POST -Body `"true`" -ContentType `"application/json`"" -ForegroundColor Yellow

Write-Host "`n✅ Pruebas completadas. Revisar logs de la aplicación para ver el monitoreo en tiempo real." -ForegroundColor Green

Write-Host "`n📋 Para monitorear logs en tiempo real:" -ForegroundColor Cyan
Write-Host "dotnet run --project src/SistemaPeaje.API" -ForegroundColor White

Write-Host "`n📋 Ejemplos de salidas esperadas en logs:" -ForegroundColor Cyan
Write-Host "  🚀 PlcMonitorWorker iniciado - Monitoreando PLC 192.168.1.103:502" -ForegroundColor White
Write-Host "  ✅ Conexión con PLC restaurada 192.168.1.103:502" -ForegroundColor White
Write-Host "  [PLC 192.168.1.103] Presencia: False, BarreraAbierta: False, SentidoAB: True, Alarma: False" -ForegroundColor White
Write-Host "  🔄 Cambio detectado en Presencia (Coil 1000): False → True" -ForegroundColor White
Write-Host "  🚗 Vehículo detectado en caseta" -ForegroundColor White
