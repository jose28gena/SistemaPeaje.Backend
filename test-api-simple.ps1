# Test simple del API de clientes
Write-Host "Testeando endpoint de alta de cliente..." -ForegroundColor Green

$clienteData = @{
    tipoPersona = "Fisica"
    nombre = "Juan"
    apellidoPaterno = "Pérez"
    apellidoMaterno = "García"
    rfc = "PEGJ850315H12"
    email = "juan.perez@email.com"
    telefono = "5555123456"
    direccion = "Av. Principal 123, Col. Centro"
    ciudad = "México"
    estado = "CDMX"
    codigoPostal = "01000"
    fechaNacimiento = "1985-03-15"
    observaciones = "Cliente nuevo de prueba"
    creadoPor = "admin@sistema.com"
}

$json = $clienteData | ConvertTo-Json -Depth 10

try {
    Write-Host "Enviando petición POST..." -ForegroundColor Yellow
    $response = Invoke-RestMethod -Uri "http://localhost:5000/api/clientes-flujo/alta" -Method POST -Body $json -ContentType "application/json"
    
    Write-Host "¡Éxito! Cliente creado:" -ForegroundColor Green
    $response | ConvertTo-Json -Depth 10 | Write-Host
    
    # Guardar el ID del cliente para siguientes pruebas
    $clienteId = $response.id
    Write-Host "Cliente ID: $clienteId" -ForegroundColor Cyan
    
    # Test de obtener cliente
    Write-Host "`nObteniendo información del cliente..." -ForegroundColor Yellow
    $cliente = Invoke-RestMethod -Uri "http://localhost:5000/api/clientes-flujo/$clienteId" -Method GET
    Write-Host "Cliente obtenido:" -ForegroundColor Green
    $cliente | ConvertTo-Json -Depth 10 | Write-Host
    
} catch {
    Write-Host "Error: $_" -ForegroundColor Red
    Write-Host "Detalles del error:" -ForegroundColor Yellow
    $_.Exception | Format-List -Force
}
