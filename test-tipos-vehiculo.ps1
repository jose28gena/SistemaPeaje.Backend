# Script de prueba para Tipos de Vehículo - Sistema de Peaje
# Ejecutar desde PowerShell en la raíz del proyecto

Write-Host "=== PRUEBAS DE TIPOS DE VEHÍCULO ===" -ForegroundColor Green

$baseUrl = "http://localhost:51394/api/tipos-vehiculo-admin"

Write-Host "`n1. Probando obtener catálogo maestro..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "$baseUrl/catalogo" -Method GET -Headers @{"Accept"="application/json"}
    Write-Host "✅ Catálogo obtenido exitosamente:" -ForegroundColor Green
    $catalogo = $response.Content | ConvertFrom-Json
    $catalogo | ForEach-Object {
        Write-Host "   - $($_.Nombre): $($_.Descripcion) [$($_.Categoria)] - $($_.NumeroEjes) ejes" -ForegroundColor Cyan
    }
} catch {
    Write-Host "❌ Error al obtener catálogo: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n2. Probando obtener tipos de vehículo..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "$baseUrl" -Method GET -Headers @{"Accept"="application/json"}
    Write-Host "✅ Tipos de vehículo obtenidos (Status: $($response.StatusCode))" -ForegroundColor Green
    $tipos = $response.Content | ConvertFrom-Json
    Write-Host "   Total de tipos: $($tipos.Count)" -ForegroundColor Cyan
} catch {
    Write-Host "❌ Error al obtener tipos: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n3. Probando obtener tipos activos..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "$baseUrl/activos" -Method GET -Headers @{"Accept"="application/json"}
    Write-Host "✅ Tipos activos obtenidos (Status: $($response.StatusCode))" -ForegroundColor Green
    $activos = $response.Content | ConvertFrom-Json
    Write-Host "   Total de tipos activos: $($activos.Count)" -ForegroundColor Cyan
} catch {
    Write-Host "❌ Error al obtener tipos activos: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n4. Probando crear tipo de vehículo (Auto)..." -ForegroundColor Yellow
$nuevoTipo = @{
    nombre = "Auto"
    descripcion = "Automóvil de pasajeros"
    numeroEjes = 2
    tarifaBase = 1500.00
    categoria = "LIVIANO"
} | ConvertTo-Json

try {
    $response = Invoke-WebRequest -Uri "$baseUrl" -Method POST -Body $nuevoTipo -ContentType "application/json"
    Write-Host "✅ Tipo 'Auto' creado exitosamente (Status: $($response.StatusCode))" -ForegroundColor Green
    $tipoCreado = $response.Content | ConvertFrom-Json
    Write-Host "   ID: $($tipoCreado.id) - Nombre: $($tipoCreado.nombre)" -ForegroundColor Cyan
} catch {
    if ($_.Exception.Response.StatusCode -eq 400) {
        Write-Host "⚠️ El tipo 'Auto' ya existe" -ForegroundColor Yellow
    } else {
        Write-Host "❌ Error al crear tipo: $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host "`n5. Probando crear tipo de vehículo (Moto)..." -ForegroundColor Yellow
$nuevoMoto = @{
    nombre = "Moto"
    descripcion = "Motocicleta"
    numeroEjes = 2
    tarifaBase = 800.00
    categoria = "LIVIANO"
} | ConvertTo-Json

try {
    $response = Invoke-WebRequest -Uri "$baseUrl" -Method POST -Body $nuevoMoto -ContentType "application/json"
    Write-Host "✅ Tipo 'Moto' creado exitosamente (Status: $($response.StatusCode))" -ForegroundColor Green
    $motoCreada = $response.Content | ConvertFrom-Json
    Write-Host "   ID: $($motoCreada.id) - Nombre: $($motoCreada.nombre)" -ForegroundColor Cyan
} catch {
    if ($_.Exception.Response.StatusCode -eq 400) {
        Write-Host "⚠️ El tipo 'Moto' ya existe" -ForegroundColor Yellow
    } else {
        Write-Host "❌ Error al crear moto: $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host "`n6. Probando crear tipo de vehículo (Bus)..." -ForegroundColor Yellow
$nuevoBus = @{
    nombre = "Bus"
    descripcion = "Autobús de pasajeros"
    numeroEjes = 2
    tarifaBase = 3000.00
    categoria = "PESADO"
} | ConvertTo-Json

try {
    $response = Invoke-WebRequest -Uri "$baseUrl" -Method POST -Body $nuevoBus -ContentType "application/json"
    Write-Host "✅ Tipo 'Bus' creado exitosamente (Status: $($response.StatusCode))" -ForegroundColor Green
    $busCreado = $response.Content | ConvertFrom-Json
    Write-Host "   ID: $($busCreado.id) - Nombre: $($busCreado.nombre)" -ForegroundColor Cyan
} catch {
    if ($_.Exception.Response.StatusCode -eq 400) {
        Write-Host "⚠️ El tipo 'Bus' ya existe" -ForegroundColor Yellow
    } else {
        Write-Host "❌ Error al crear bus: $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host "`n7. Probando crear tipo de vehículo (Camión Rígido)..." -ForegroundColor Yellow
$nuevoCamion = @{
    nombre = "Camión Rígido"
    descripcion = "Camión de carga rígido"
    numeroEjes = 2
    tarifaBase = 4500.00
    categoria = "PESADO"
} | ConvertTo-Json

try {
    $response = Invoke-WebRequest -Uri "$baseUrl" -Method POST -Body $nuevoCamion -ContentType "application/json"
    Write-Host "✅ Tipo 'Camión Rígido' creado exitosamente (Status: $($response.StatusCode))" -ForegroundColor Green
    $camionCreado = $response.Content | ConvertFrom-Json
    Write-Host "   ID: $($camionCreado.id) - Nombre: $($camionCreado.nombre)" -ForegroundColor Cyan
} catch {
    if ($_.Exception.Response.StatusCode -eq 400) {
        Write-Host "⚠️ El tipo 'Camión Rígido' ya existe" -ForegroundColor Yellow
    } else {
        Write-Host "❌ Error al crear camión: $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host "`n8. Probando crear tipo de vehículo (Camión Articulado)..." -ForegroundColor Yellow
$nuevoArticulado = @{
    nombre = "Camión Articulado"
    descripcion = "Camión articulado con remolque"
    numeroEjes = 5
    tarifaBase = 7500.00
    categoria = "ESPECIAL"
} | ConvertTo-Json

try {
    $response = Invoke-WebRequest -Uri "$baseUrl" -Method POST -Body $nuevoArticulado -ContentType "application/json"
    Write-Host "✅ Tipo 'Camión Articulado' creado exitosamente (Status: $($response.StatusCode))" -ForegroundColor Green
    $articuladoCreado = $response.Content | ConvertFrom-Json
    Write-Host "   ID: $($articuladoCreado.id) - Nombre: $($articuladoCreado.nombre)" -ForegroundColor Cyan
} catch {
    if ($_.Exception.Response.StatusCode -eq 400) {
        Write-Host "⚠️ El tipo 'Camión Articulado' ya existe" -ForegroundColor Yellow
    } else {
        Write-Host "❌ Error al crear articulado: $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host "`n9. Verificando tipos creados..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "$baseUrl" -Method GET -Headers @{"Accept"="application/json"}
    $tipos = $response.Content | ConvertFrom-Json
    Write-Host "✅ Tipos de vehículo en el sistema:" -ForegroundColor Green
    $tipos | ForEach-Object {
        $estado = if ($_.esActivo) { "Activo" } else { "Inactivo" }
        Write-Host "   - ID: $($_.id) | $($_.nombre) | $($_.categoria) | $($estado) | Tarifa: $($_.tarifaBase)" -ForegroundColor Cyan
    }
} catch {
    Write-Host "❌ Error al verificar tipos: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n=== PRUEBAS COMPLETADAS ===" -ForegroundColor Green
Write-Host "Los endpoints de tipos de vehículo están listos para usar." -ForegroundColor White
Write-Host "Base URL: $baseUrl" -ForegroundColor Gray
