# Script Maestro de Pruebas - Sistema PLC Escalable
# Incluye todos los comandos de testing disponibles

param(
    [string]$Action = "menu"
)

# Configurar SSL para desarrollo
[System.Net.ServicePointManager]::ServerCertificateValidationCallback = {$true}

$baseUrl = "https://localhost:51393"

function Show-Menu {
    Write-Host "`nSISTEMA PLC ESCALABLE - MENU DE PRUEBAS" -ForegroundColor Green
    Write-Host "=============================================" -ForegroundColor Gray
    Write-Host "1. Configurar simulador PLC" -ForegroundColor Cyan
    Write-Host "2. Abrir barrera (comando almacenado)" -ForegroundColor Cyan
    Write-Host "3. Ver estado de workers" -ForegroundColor Cyan
    Write-Host "4. Ver configuraciones activas" -ForegroundColor Cyan
    Write-Host "5. Ver logs del sistema" -ForegroundColor Cyan
    Write-Host "6. Ejecutar pruebas completas" -ForegroundColor Cyan
    Write-Host "7. Limpiar configuracion de simulador" -ForegroundColor Yellow
    Write-Host "0. Salir" -ForegroundColor Red
    Write-Host "=============================================" -ForegroundColor Gray
    
    $choice = Read-Host "Selecciona una opcion (0-7)"
    
    switch ($choice) {
        "1" { ConfigurarSimulador }
        "2" { AbrirBarrera }
        "3" { VerEstadoWorkers }
        "4" { VerConfiguracionesActivas }
        "5" { VerLogs }
        "6" { EjecutarPruebasCompletas }
        "7" { LimpiarSimulador }
        "0" { Write-Host "Saliendo..." -ForegroundColor Green; exit }
        default { Write-Host "Opcion invalida" -ForegroundColor Red; Show-Menu }
    }
}

function ConfigurarSimulador {
    Write-Host "`nConfigurando simulador PLC..." -ForegroundColor Green
    try {
        & ".\test-simulador-simple.ps1"
    } catch {
        Write-Host "Error al ejecutar script de simulador: $($_.Exception.Message)" -ForegroundColor Red
    }
    PressAnyKey
}

function AbrirBarrera {
    Write-Host "`nEjecutando comando abrir barrera..." -ForegroundColor Green
    
    $body = @{
        casetaIp = "127.0.0.1"
        coilAddress = 96
        unitId = 1
        carrilId = 1
        observaciones = "Comando ejecutado desde script maestro"
    } | ConvertTo-Json
    
    try {
        $response = Invoke-RestMethod -Uri "$baseUrl/api/comandos/abrir-barrera" -Method POST -Body $body -ContentType "application/json"
        Write-Host "Comando ejecutado exitosamente:" -ForegroundColor Green
        Write-Host ($response | ConvertTo-Json -Depth 3) -ForegroundColor White
    } catch {
        Write-Host "Error al ejecutar comando: $($_.Exception.Message)" -ForegroundColor Red
        if ($_.Exception.Response) {
            Write-Host "Status: $($_.Exception.Response.StatusCode)" -ForegroundColor Yellow
        }
    }
    PressAnyKey
}

function VerEstadoWorkers {
    Write-Host "`nVerificando estado de workers..." -ForegroundColor Green
    try {
        $response = Invoke-RestMethod -Uri "$baseUrl/api/PlcConfiguracion/workers/status" -Method GET
        Write-Host "Estado de workers:" -ForegroundColor Green
        $response | ConvertTo-Json -Depth 2
    } catch {
        Write-Host "Error al verificar workers: $($_.Exception.Message)" -ForegroundColor Red
    }
    PressAnyKey
}

function VerConfiguracionesActivas {
    Write-Host "`nObteniendo configuraciones activas..." -ForegroundColor Green
    try {
        $response = Invoke-RestMethod -Uri "$baseUrl/api/PlcConfiguracion/activas" -Method GET
        Write-Host "Configuraciones activas:" -ForegroundColor Green
        foreach ($config in $response) {
            Write-Host "- ID: $($config.id), Nombre: $($config.nombre), IP: $($config.ip):$($config.puerto), Conectado: $($config.estaConectado)" -ForegroundColor White
        }
    } catch {
        Write-Host "Error al obtener configuraciones: $($_.Exception.Message)" -ForegroundColor Red
    }
    PressAnyKey
}

function VerLogs {
    Write-Host "`nMostrando ultimos logs del sistema..." -ForegroundColor Green
    try {
        $logs = Get-Content "src\SistemaPeaje.API\logs\sistema-peaje-*.txt" | Select-Object -Last 15
        $logs | ForEach-Object { Write-Host $_ -ForegroundColor Gray }
    } catch {
        Write-Host "Error al leer logs: $($_.Exception.Message)" -ForegroundColor Red
    }
    PressAnyKey
}

function EjecutarPruebasCompletas {
    Write-Host "`nEjecutando pruebas completas del sistema..." -ForegroundColor Green
    try {
        & ".\test-sistema-escalable.ps1"
    } catch {
        Write-Host "Error al ejecutar pruebas completas: $($_.Exception.Message)" -ForegroundColor Red
    }
    PressAnyKey
}

function LimpiarSimulador {
    Write-Host "`nLimpiando configuracion de simulador..." -ForegroundColor Yellow
    try {
        Invoke-RestMethod -Uri "$baseUrl/api/PlcConfiguracion/3" -Method DELETE
        Write-Host "Configuracion de simulador eliminada" -ForegroundColor Green
    } catch {
        Write-Host "Error al eliminar configuracion: $($_.Exception.Message)" -ForegroundColor Red
    }
    PressAnyKey
}

function PressAnyKey {
    Write-Host "`nPresiona cualquier tecla para continuar..." -ForegroundColor Gray
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
    Show-Menu
}

# Punto de entrada
switch ($Action) {
    "configurar" { ConfigurarSimulador }
    "abrir-barrera" { AbrirBarrera }
    "workers" { VerEstadoWorkers }
    "activas" { VerConfiguracionesActivas }
    "logs" { VerLogs }
    "pruebas" { EjecutarPruebasCompletas }
    "limpiar" { LimpiarSimulador }
    default { Show-Menu }
}
