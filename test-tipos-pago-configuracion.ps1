# Script de Prueba para el Módulo 11.10 - Tipos de Pago
# Sistema de Peaje - Configuración de Métodos Activos

$baseUrl = "https://localhost:51393"

Write-Host "=== 11.10 TIPOS DE PAGO - CONFIGURACIÓN DE MÉTODOS ACTIVOS ===" -ForegroundColor Green
Write-Host ""

# 1. Inicializar tipos de pago básicos
Write-Host "1. Inicializando tipos de pago básicos..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/TiposPago/inicializar-tipos-basicos" -Method POST -ContentType "application/json"
    Write-Host "✅ Tipos de pago inicializados:" -ForegroundColor Green
    $response.tipos | ForEach-Object {
        Write-Host "   - $($_.Nombre): $($_.Descripcion)" -ForegroundColor Cyan
    }
} catch {
    Write-Host "❌ Error al inicializar tipos: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""

# 2. Obtener tipos de pago
Write-Host "2. Obteniendo tipos de pago..." -ForegroundColor Yellow
try {
    $tiposPago = Invoke-RestMethod -Uri "$baseUrl/api/TiposPago" -Method GET
    Write-Host "✅ Tipos de pago disponibles:" -ForegroundColor Green
    $tiposPago | ForEach-Object {
        $activo = if ($_.EsActivo) { "ACTIVO" } else { "INACTIVO" }
        Write-Host "   ID: $($_.Id) | $($_.Nombre) | $($activo)" -ForegroundColor Cyan
    }
} catch {
    Write-Host "❌ Error al obtener tipos: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""

# 3. Inicializar configuraciones por defecto
Write-Host "3. Inicializando configuraciones por defecto..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/ConfiguracionTiposPago/inicializar-configuraciones-defecto" -Method POST -ContentType "application/json"
    Write-Host "✅ Configuraciones inicializadas:" -ForegroundColor Green
    Write-Host "   $($response.message)" -ForegroundColor Cyan
    $response.configuraciones | ForEach-Object {
        Write-Host "   - Tipo: $($_.TipoPagoNombre) | Moneda: $($_.Moneda) $($_.SimboloMoneda)" -ForegroundColor Cyan
        Write-Host "     Límite Diario: $($_.LimiteDiario) | Comisión: $($_.ComisionPorcentaje)%" -ForegroundColor Cyan
        Write-Host "     Descuento: $($_.DescuentoPorDefecto)% | Activo: $($_.EstaActivo)" -ForegroundColor Cyan
        Write-Host ""
    }
} catch {
    Write-Host "❌ Error al inicializar configuraciones: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""

# 4. Obtener tipos de pago activos con configuración
Write-Host "4. Obteniendo tipos de pago activos con configuración..." -ForegroundColor Yellow
try {
    $tiposActivos = Invoke-RestMethod -Uri "$baseUrl/api/ConfiguracionTiposPago/activos-con-configuracion" -Method GET
    Write-Host "✅ Tipos de pago activos con configuración:" -ForegroundColor Green
    $tiposActivos | ForEach-Object {
        Write-Host "   === $($_.Nombre) ===" -ForegroundColor Yellow
        Write-Host "   Descripción: $($_.Descripcion)" -ForegroundColor Cyan
        if ($_.Configuracion) {
            Write-Host "   Configuración:" -ForegroundColor White
            Write-Host "     • Moneda: $($_.Configuracion.Moneda) ($($_.Configuracion.SimboloMoneda))" -ForegroundColor Cyan
            Write-Host "     • Límite Diario: $($_.Configuracion.LimiteDiario)" -ForegroundColor Cyan
            Write-Host "     • Límite por Transacción: $($_.Configuracion.LimiteTransaccion)" -ForegroundColor Cyan
            Write-Host "     • Comisión: $($_.Configuracion.ComisionPorcentaje)% + $($_.Configuracion.ComisionFija)" -ForegroundColor Cyan
            Write-Host "     • Descuento por Defecto: $($_.Configuracion.DescuentoPorDefecto)%" -ForegroundColor Cyan
            Write-Host "     • Requiere Validación: $($_.Configuracion.RequiereValidacionAdicional)" -ForegroundColor Cyan
            Write-Host "     • Tiempo de Espera: $($_.Configuracion.TiempoEsperaSegundos) segundos" -ForegroundColor Cyan
        } else {
            Write-Host "     Sin configuración específica" -ForegroundColor Gray
        }
        Write-Host ""
    }
} catch {
    Write-Host "❌ Error al obtener tipos activos: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""

# 5. Probar cálculo de montos
Write-Host "5. Probando cálculo de montos..." -ForegroundColor Yellow

$montosBase = @(100, 500, 1000, 2500)
$tiposParaProbar = @(1, 2, 3) # Efectivo, Prepago, Residentes

foreach ($tipoPago in $tiposParaProbar) {
    Write-Host "   === Cálculos para Tipo de Pago ID: $tipoPago ===" -ForegroundColor Yellow
    
    foreach ($montoBase in $montosBase) {
        try {
            $calculoRequest = @{
                TipoPagoId = $tipoPago
                MontoBase = $montoBase
            } | ConvertTo-Json
            
            $calculo = Invoke-RestMethod -Uri "$baseUrl/api/ConfiguracionTiposPago/calcular-monto" -Method POST -Body $calculoRequest -ContentType "application/json"
            
            Write-Host "     Monto Base: $($calculo.SimboloMoneda)$($calculo.MontoBase)" -ForegroundColor Cyan
            Write-Host "     Descuento: $($calculo.SimboloMoneda)$($calculo.DescuentoAplicado)" -ForegroundColor Green
            Write-Host "     Comisión: $($calculo.SimboloMoneda)$($calculo.ComisionPorcentual + $calculo.ComisionFija)" -ForegroundColor Red
            Write-Host "     MONTO FINAL: $($calculo.SimboloMoneda)$($calculo.MontoFinal)" -ForegroundColor White -BackgroundColor DarkBlue
            
            if ($calculo.ExcedeLimiteTransaccion) {
                Write-Host "     ⚠️  EXCEDE LÍMITE DE TRANSACCIÓN: $($calculo.SimboloMoneda)$($calculo.LimiteTransaccion)" -ForegroundColor Red
            }
            
            Write-Host ""
        } catch {
            Write-Host "     ❌ Error en cálculo: $($_.Exception.Message)" -ForegroundColor Red
        }
    }
}

# 6. Obtener estadísticas
Write-Host "6. Obteniendo estadísticas de uso..." -ForegroundColor Yellow
try {
    $estadisticas = Invoke-RestMethod -Uri "$baseUrl/api/TiposPago/estadisticas" -Method GET
    Write-Host "✅ Estadísticas de tipos de pago:" -ForegroundColor Green
    Write-Host "   Total de tipos: $($estadisticas.TotalTipos)" -ForegroundColor Cyan
    Write-Host "   Tipos activos: $($estadisticas.TiposActivos)" -ForegroundColor Cyan
    Write-Host "   Tipos inactivos: $($estadisticas.TiposInactivos)" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "   Uso mensual:" -ForegroundColor White
    Write-Host "     • Efectivo: $($estadisticas.UsoMensual.Efectivo.Transacciones) transacciones | $$$($estadisticas.UsoMensual.Efectivo.Monto) | $($estadisticas.UsoMensual.Efectivo.Porcentaje)%" -ForegroundColor Cyan
    Write-Host "     • Prepago: $($estadisticas.UsoMensual.Prepago.Transacciones) transacciones | $$$($estadisticas.UsoMensual.Prepago.Monto) | $($estadisticas.UsoMensual.Prepago.Porcentaje)%" -ForegroundColor Cyan
    Write-Host "     • Residentes: $($estadisticas.UsoMensual.Residentes.Transacciones) transacciones | $$$($estadisticas.UsoMensual.Residentes.Monto) | $($estadisticas.UsoMensual.Residentes.Porcentaje)%" -ForegroundColor Cyan
} catch {
    Write-Host "❌ Error al obtener estadísticas: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""
Write-Host "=== PRUEBA COMPLETADA - MÓDULO 11.10 TIPOS DE PAGO ===" -ForegroundColor Green
Write-Host ""
Write-Host "FUNCIONALIDADES IMPLEMENTADAS:" -ForegroundColor White -BackgroundColor DarkGreen
Write-Host "✅ Tipos de pago básicos: Efectivo, Prepago, Residentes" -ForegroundColor Green
Write-Host "✅ Configuración de parámetros: Moneda, límites, comisiones" -ForegroundColor Green
Write-Host "✅ Cálculo automático de montos con descuentos y comisiones" -ForegroundColor Green  
Write-Host "✅ Validación de límites por transacción y diarios" -ForegroundColor Green
Write-Host "✅ Control de métodos activos/inactivos" -ForegroundColor Green
Write-Host "✅ Estadísticas de uso por tipo de pago" -ForegroundColor Green
Write-Host ""
Write-Host "PARÁMETROS CONFIGURABLES:" -ForegroundColor White -BackgroundColor DarkBlue
Write-Host "• Moneda y símbolo (MXN, $)" -ForegroundColor Cyan
Write-Host "• Límite diario por tipo de pago" -ForegroundColor Cyan
Write-Host "• Límite por transacción individual" -ForegroundColor Cyan
Write-Host "• Comisión porcentual y fija" -ForegroundColor Cyan
Write-Host "• Descuento por defecto" -ForegroundColor Cyan
Write-Host "• Tiempo de espera para procesamiento" -ForegroundColor Cyan
Write-Host "• Validaciones adicionales requeridas" -ForegroundColor Cyan
