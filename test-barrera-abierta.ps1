# Script de Prueba - Comando Abrir Barrera
# Equivalente al cURL proporcionado

Write-Host "Enviando comando para abrir barrera..." -ForegroundColor Green

# Configurar SSL para desarrollo
[System.Net.ServicePointManager]::ServerCertificateValidationCallback = {$true}

# URL del endpoint
$url = "https://localhost:51393/api/comandos/abrir-barrera"

# Cuerpo de la peticion (exactamente como en el cURL)
$body = @{
    casetaIp = "127.0.0.1"
    coilAddress = 96  # Correcto: 96 en código = 000097 en simulador (Presencia de vehículo)
    unitId = 1
    carrilId = 1
    observaciones = "Comando desde script PowerShell - Coil 000097"
} | ConvertTo-Json

Write-Host "Enviando peticion a: $url" -ForegroundColor Cyan
Write-Host "Datos enviados:" -ForegroundColor Yellow
Write-Host $body -ForegroundColor White

try {
    $response = Invoke-RestMethod -Uri $url -Method POST -Body $body -ContentType "application/json"
    Write-Host "Comando ejecutado exitosamente:" -ForegroundColor Green
    Write-Host ($response | ConvertTo-Json -Depth 3) -ForegroundColor White
    
    Write-Host "`nResultado esperado:" -ForegroundColor Cyan
    Write-Host "- Coil 000097 del simulador deberia cambiar a TRUE" -ForegroundColor Gray
    Write-Host "- Se deberia registrar un evento en la base de datos" -ForegroundColor Gray
    Write-Host "- Los logs deberian mostrar la ejecucion del comando" -ForegroundColor Gray
    
} catch {
    Write-Host "Error al ejecutar comando:" -ForegroundColor Red
    Write-Host "Status: $($_.Exception.Response.StatusCode)" -ForegroundColor Yellow
    Write-Host "Message: $($_.Exception.Message)" -ForegroundColor Yellow
    
    # Intentar mostrar el contenido del error si esta disponible
    if ($_.Exception.Response) {
        $reader = [System.IO.StreamReader]::new($_.Exception.Response.GetResponseStream())
        $errorContent = $reader.ReadToEnd()
        Write-Host "Error content: $errorContent" -ForegroundColor Red
    }
}

Write-Host "`nPara verificar el resultado:" -ForegroundColor Cyan
Write-Host "1. Revisar el simulador PLC en 127.0.0.1:502" -ForegroundColor White
Write-Host "2. Verificar que el coil 000097 este en TRUE" -ForegroundColor White
Write-Host "3. Monitorear logs: Get-Content 'src\SistemaPeaje.API\logs\sistema-peaje-*.txt' -Wait" -ForegroundColor White

Write-Host "`nMapeo de direcciones:" -ForegroundColor Cyan
Write-Host "- Codigo .NET: 96 -> Simulador: 000097 (Presencia vehiculo)" -ForegroundColor Gray
Write-Host "- Codigo .NET: 97 -> Simulador: 000098 (Barrera abierta)" -ForegroundColor Gray
Write-Host "- Codigo .NET: 98 -> Simulador: 000099 (Sentido carril)" -ForegroundColor Gray
Write-Host "- Codigo .NET: 99 -> Simulador: 000100 (Modo emergencia)" -ForegroundColor Gray
