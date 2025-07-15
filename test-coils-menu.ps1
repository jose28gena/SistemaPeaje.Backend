# Comandos para Diferentes Coils del Simulador PLC
# Cada comando afecta un coil diferente según el mapeo

Write-Host "=== COMANDOS PARA DIFERENTES COILS ===" -ForegroundColor Green
Write-Host "Mapeo: Codigo .NET -> Simulador PLC" -ForegroundColor Yellow
Write-Host "96 -> 000097 (Presencia vehiculo)" -ForegroundColor Gray
Write-Host "97 -> 000098 (Barrera abierta)" -ForegroundColor Gray
Write-Host "98 -> 000099 (Sentido carril)" -ForegroundColor Gray
Write-Host "99 -> 000100 (Modo emergencia)" -ForegroundColor Gray

# Configurar SSL
[System.Net.ServicePointManager]::ServerCertificateValidationCallback = {$true}

$baseUrl = "https://localhost:51393"

# Función para enviar comando
function Send-PlcCommand {
    param(
        [int]$CoilAddress,
        [string]$Descripcion,
        [string]$CoilSimulador,
        [string]$Observaciones
    )
    
    Write-Host "`n--- $Descripcion ---" -ForegroundColor Cyan
    Write-Host "Enviando comando a coil $CoilAddress (Simulador: $CoilSimulador)" -ForegroundColor Yellow
    
    $body = @{
        casetaIp = "127.0.0.1"
        coilAddress = $CoilAddress
        unitId = 1
        carrilId = 1
        observaciones = $Observaciones
    } | ConvertTo-Json
    
    try {
        $response = Invoke-RestMethod -Uri "$baseUrl/api/comandos/abrir-barrera" -Method POST -Body $body -ContentType "application/json"
        Write-Host "Comando ejecutado exitosamente:" -ForegroundColor Green
        Write-Host ($response | ConvertTo-Json -Depth 3) -ForegroundColor White
        Write-Host "Verificar coil $CoilSimulador en el simulador" -ForegroundColor Gray
    } catch {
        Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    }
}

# Menú interactivo
do {
    Write-Host "`n=== MENU DE COMANDOS PLC ===" -ForegroundColor Green
    Write-Host "1. Activar Presencia Vehiculo (Coil 000097)" -ForegroundColor Cyan
    Write-Host "2. Abrir Barrera (Coil 000098)" -ForegroundColor Cyan
    Write-Host "3. Cambiar Sentido Carril (Coil 000099)" -ForegroundColor Cyan
    Write-Host "4. Activar Emergencia (Coil 000100)" -ForegroundColor Cyan
    Write-Host "5. Enviar a todos los coils" -ForegroundColor Yellow
    Write-Host "0. Salir" -ForegroundColor Red
    
    $choice = Read-Host "Selecciona una opcion (0-5)"
    
    switch ($choice) {
        "1" {
            Send-PlcCommand -CoilAddress 96 -Descripcion "PRESENCIA VEHICULO" -CoilSimulador "000097" -Observaciones "Activar presencia de vehiculo"
        }
        "2" {
            Send-PlcCommand -CoilAddress 97 -Descripcion "BARRERA ABIERTA" -CoilSimulador "000098" -Observaciones "Abrir barrera de caseta"
        }
        "3" {
            Send-PlcCommand -CoilAddress 98 -Descripcion "SENTIDO CARRIL" -CoilSimulador "000099" -Observaciones "Cambiar sentido del carril"
        }
        "4" {
            Send-PlcCommand -CoilAddress 99 -Descripcion "MODO EMERGENCIA" -CoilSimulador "000100" -Observaciones "Activar modo emergencia"
        }
        "5" {
            Write-Host "`nEnviando comandos a todos los coils..." -ForegroundColor Yellow
            Send-PlcCommand -CoilAddress 96 -Descripcion "PRESENCIA VEHICULO" -CoilSimulador "000097" -Observaciones "Activar presencia"
            Start-Sleep -Seconds 2
            Send-PlcCommand -CoilAddress 97 -Descripcion "BARRERA ABIERTA" -CoilSimulador "000098" -Observaciones "Abrir barrera"
            Start-Sleep -Seconds 2
            Send-PlcCommand -CoilAddress 98 -Descripcion "SENTIDO CARRIL" -CoilSimulador "000099" -Observaciones "Cambiar sentido"
            Start-Sleep -Seconds 2
            Send-PlcCommand -CoilAddress 99 -Descripcion "MODO EMERGENCIA" -CoilSimulador "000100" -Observaciones "Activar emergencia"
        }
        "0" {
            Write-Host "Saliendo..." -ForegroundColor Green
            break
        }
        default {
            Write-Host "Opcion invalida" -ForegroundColor Red
        }
    }
    
    if ($choice -ne "0") {
        Write-Host "`nPresiona Enter para continuar..." -ForegroundColor Gray
        Read-Host
    }
    
} while ($choice -ne "0")

Write-Host "`nPara monitorear los cambios:" -ForegroundColor Cyan
Write-Host "Get-Content 'src\SistemaPeaje.API\logs\sistema-peaje-*.txt' -Wait" -ForegroundColor White
