# Script de Verificacion del Estado del Servidor
# Verifica si el servidor API esta funcionando

Write-Host "Verificando estado del servidor API..." -ForegroundColor Green

# Configurar SSL
[System.Net.ServicePointManager]::ServerCertificateValidationCallback = {$true}

$baseUrl = "https://localhost:51393"

# Probar endpoint basico
try {
    Write-Host "Probando endpoint de salud..." -ForegroundColor Cyan
    $response = Invoke-RestMethod -Uri "$baseUrl/api/health" -Method GET -TimeoutSec 5
    Write-Host "Servidor disponible: $response" -ForegroundColor Green
} catch {
    Write-Host "Error al acceder al servidor: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Verifica que el servidor este corriendo con: dotnet run --project src/SistemaPeaje.API" -ForegroundColor Yellow
}

# Probar endpoint de workers
try {
    Write-Host "Probando endpoint de workers..." -ForegroundColor Cyan
    $response = Invoke-RestMethod -Uri "$baseUrl/api/PlcConfiguracion/workers/status" -Method GET -TimeoutSec 5
    Write-Host "Workers disponibles:" -ForegroundColor Green
    $response | ConvertTo-Json -Depth 2
} catch {
    Write-Host "Error al acceder a workers: $($_.Exception.Message)" -ForegroundColor Red
    
    # Verificar si el servidor esta corriendo
    Write-Host "Verificando si el proceso dotnet esta corriendo..." -ForegroundColor Yellow
    try {
        $processes = Get-Process -Name "dotnet" -ErrorAction Stop
        if ($processes) {
            Write-Host "Procesos dotnet encontrados:" -ForegroundColor Green
            $processes | ForEach-Object { Write-Host "- PID: $($_.Id), CPU: $($_.CPU)" -ForegroundColor Gray }
        }
    } catch {
        Write-Host "No se encontraron procesos dotnet corriendo" -ForegroundColor Red
    }
}

Write-Host "`nPara iniciar el servidor:" -ForegroundColor Cyan
Write-Host "cd src/SistemaPeaje.API" -ForegroundColor White
Write-Host "dotnet run" -ForegroundColor White
