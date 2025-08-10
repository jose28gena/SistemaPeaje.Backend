-- ===================================================================
-- SCRIPT DE INSERCIÓN DIRECTA DE TARIFAS - SISTEMA DE PEAJE
-- ===================================================================
-- Fecha: 09 de Agosto 2025
-- Propósito: Insertar tarifas directamente en la tabla Tarifas
-- Nota: Estructura basada en la entidad Tarifa del sistema
-- ===================================================================

-- Verificar estructura de tabla (comentario para referencia)
/*
Estructura de tabla Tarifas:
- Id (int, IDENTITY) - Clave primaria autogenerada
- TipoVehiculoId (int, NOT NULL) - FK a TiposVehiculo
- EstacionId (int, NULL) - FK a Estaciones (puede ser NULL para tarifas generales)
- Monto (decimal(18,2), NOT NULL) - Precio de la tarifa
- FechaVigenciaInicio (datetime2, NOT NULL) - Inicio de vigencia
- FechaVigenciaFin (datetime2, NULL) - Fin de vigencia (NULL = indefinida)
- EsVigente (bit, NOT NULL) - Estado de vigencia actual
- FechaCreacion (datetime2, NOT NULL) - Fecha de creación del registro
- FechaActualizacion (datetime2, NULL) - Fecha de última actualización
- Activo (bit, NOT NULL) - Estado activo/inactivo del registro
*/

-- ===================================================================
-- PASO 1: VERIFICAR TIPOS DE VEHÍCULO DISPONIBLES
-- ===================================================================
PRINT '=== Tipos de Vehículo Disponibles ==='
SELECT 
    Id,
    Nombre,
    Descripcion,
    TarifaBase,
    Categoria,
    NumeroEjes
FROM TiposVehiculo 
WHERE Activo = 1
ORDER BY Id;

-- ===================================================================
-- PASO 2: VERIFICAR ESTACIONES DISPONIBLES
-- ===================================================================
PRINT '=== Estaciones Disponibles ==='
SELECT 
    Id,
    Nombre,
    Ubicacion,
    Descripcion
FROM Estaciones 
WHERE Activo = 1
ORDER BY Id;

-- ===================================================================
-- PASO 3: INSERTAR TARIFAS GENERALES (SIN ESTACIÓN ESPECÍFICA)
-- ===================================================================
PRINT '=== Insertando Tarifas Generales ==='

-- Tarifa para Motocicleta (Tipo 1)
INSERT INTO Tarifas (
    TipoVehiculoId,
    EstacionId,
    Monto,
    FechaVigenciaInicio,
    FechaVigenciaFin,
    EsVigente,
    FechaCreacion,
    Activo
) VALUES (
    1,                                    -- Motocicleta
    NULL,                                 -- Sin estación específica (tarifa general)
    800.00,                              -- $800.00
    '2025-08-09 00:00:00',               -- Vigente desde hoy
    NULL,                                -- Sin fecha de fin (indefinida)
    1,                                   -- Es vigente
    GETDATE(),                           -- Fecha de creación
    1                                    -- Activo
);

-- Tarifa para Automóvil (Tipo 2)
INSERT INTO Tarifas (
    TipoVehiculoId,
    EstacionId,
    Monto,
    FechaVigenciaInicio,
    FechaVigenciaFin,
    EsVigente,
    FechaCreacion,
    Activo
) VALUES (
    2,                                    -- Automóvil
    NULL,                                 -- Sin estación específica
    1500.00,                             -- $1,500.00
    '2025-08-09 00:00:00',               -- Vigente desde hoy
    NULL,                                -- Sin fecha de fin
    1,                                   -- Es vigente
    GETDATE(),                           -- Fecha de creación
    1                                    -- Activo
);

-- Tarifa para Autobús (Tipo 3)
INSERT INTO Tarifas (
    TipoVehiculoId,
    EstacionId,
    Monto,
    FechaVigenciaInicio,
    FechaVigenciaFin,
    EsVigente,
    FechaCreacion,
    Activo
) VALUES (
    3,                                    -- Autobús
    NULL,                                 -- Sin estación específica
    3000.00,                             -- $3,000.00
    '2025-08-09 00:00:00',               -- Vigente desde hoy
    NULL,                                -- Sin fecha de fin
    1,                                   -- Es vigente
    GETDATE(),                           -- Fecha de creación
    1                                    -- Activo
);

-- Tarifa para Camión Rígido (Tipo 4)
INSERT INTO Tarifas (
    TipoVehiculoId,
    EstacionId,
    Monto,
    FechaVigenciaInicio,
    FechaVigenciaFin,
    EsVigente,
    FechaCreacion,
    Activo
) VALUES (
    4,                                    -- Camión Rígido
    NULL,                                 -- Sin estación específica
    4500.00,                             -- $4,500.00
    '2025-08-09 00:00:00',               -- Vigente desde hoy
    NULL,                                -- Sin fecha de fin
    1,                                   -- Es vigente
    GETDATE(),                           -- Fecha de creación
    1                                    -- Activo
);

-- Tarifa para Camión Articulado (Tipo 5)
INSERT INTO Tarifas (
    TipoVehiculoId,
    EstacionId,
    Monto,
    FechaVigenciaInicio,
    FechaVigenciaFin,
    EsVigente,
    FechaCreacion,
    Activo
) VALUES (
    5,                                    -- Camión Articulado
    NULL,                                 -- Sin estación específica
    7500.00,                             -- $7,500.00
    '2025-08-09 00:00:00',               -- Vigente desde hoy
    NULL,                                -- Sin fecha de fin
    1,                                   -- Es vigente
    GETDATE(),                           -- Fecha de creación
    1                                    -- Activo
);

PRINT 'Tarifas generales insertadas correctamente.'

-- ===================================================================
-- PASO 4: INSERTAR TARIFAS ESPECÍFICAS POR ESTACIÓN (OPCIONAL)
-- ===================================================================
PRINT '=== Insertando Tarifas Específicas por Estación (Ejemplo) ==='

-- Ejemplo: Tarifa especial para Estación Norte (ID 2) - Descuento del 10%
INSERT INTO Tarifas (
    TipoVehiculoId,
    EstacionId,
    Monto,
    FechaVigenciaInicio,
    FechaVigenciaFin,
    EsVigente,
    FechaCreacion,
    Activo
) VALUES 
    (1, 2, 720.00, '2025-08-09 00:00:00', NULL, 1, GETDATE(), 1),  -- Moto en Est. Norte: $720 (10% desc)
    (2, 2, 1350.00, '2025-08-09 00:00:00', NULL, 1, GETDATE(), 1), -- Auto en Est. Norte: $1,350 (10% desc)
    (3, 2, 2700.00, '2025-08-09 00:00:00', NULL, 1, GETDATE(), 1); -- Bus en Est. Norte: $2,700 (10% desc)

-- Ejemplo: Tarifa premium para Estación Central (ID 1) - Recargo del 15%
INSERT INTO Tarifas (
    TipoVehiculoId,
    EstacionId,
    Monto,
    FechaVigenciaInicio,
    FechaVigenciaFin,
    EsVigente,
    FechaCreacion,
    Activo
) VALUES 
    (2, 1, 1725.00, '2025-08-09 00:00:00', NULL, 1, GETDATE(), 1), -- Auto en Est. Central: $1,725 (15% recargo)
    (4, 1, 5175.00, '2025-08-09 00:00:00', NULL, 1, GETDATE(), 1), -- Camión Rígido en Est. Central: $5,175 (15% recargo)
    (5, 1, 8625.00, '2025-08-09 00:00:00', NULL, 1, GETDATE(), 1); -- Camión Articulado en Est. Central: $8,625 (15% recargo)

PRINT 'Tarifas específicas por estación insertadas correctamente.'

-- ===================================================================
-- PASO 5: INSERTAR TARIFAS CON VIGENCIA LIMITADA (EJEMPLO)
-- ===================================================================
PRINT '=== Insertando Tarifas con Vigencia Limitada (Promocionales) ==='

-- Ejemplo: Tarifa promocional para fin de año (1 mes de vigencia)
INSERT INTO Tarifas (
    TipoVehiculoId,
    EstacionId,
    Monto,
    FechaVigenciaInicio,
    FechaVigenciaFin,
    EsVigente,
    FechaCreacion,
    Activo
) VALUES 
    (1, NULL, 600.00, '2025-12-01 00:00:00', '2025-12-31 23:59:59', 1, GETDATE(), 1), -- Moto promocional: $600
    (2, NULL, 1200.00, '2025-12-01 00:00:00', '2025-12-31 23:59:59', 1, GETDATE(), 1), -- Auto promocional: $1,200
    (3, NULL, 2400.00, '2025-12-01 00:00:00', '2025-12-31 23:59:59', 1, GETDATE(), 1); -- Bus promocional: $2,400

PRINT 'Tarifas promocionales insertadas correctamente.'

-- ===================================================================
-- PASO 6: VERIFICAR TARIFAS INSERTADAS
-- ===================================================================
PRINT '=== Verificación de Tarifas Insertadas ==='

SELECT 
    t.Id,
    tv.Nombre AS TipoVehiculo,
    e.Nombre AS Estacion,
    t.Monto,
    t.FechaVigenciaInicio,
    t.FechaVigenciaFin,
    t.EsVigente,
    t.Activo,
    t.FechaCreacion
FROM Tarifas t
INNER JOIN TiposVehiculo tv ON t.TipoVehiculoId = tv.Id
LEFT JOIN Estaciones e ON t.EstacionId = e.Id
WHERE t.Activo = 1
ORDER BY tv.Nombre, e.Nombre, t.Monto;

-- ===================================================================
-- PASO 7: RESUMEN POR TIPO DE VEHÍCULO
-- ===================================================================
PRINT '=== Resumen de Tarifas por Tipo de Vehículo ==='

SELECT 
    tv.Nombre AS TipoVehiculo,
    COUNT(*) AS CantidadTarifas,
    MIN(t.Monto) AS TarifaMinima,
    MAX(t.Monto) AS TarifaMaxima,
    AVG(t.Monto) AS TarifaPromedio
FROM Tarifas t
INNER JOIN TiposVehiculo tv ON t.TipoVehiculoId = tv.Id
WHERE t.Activo = 1 AND t.EsVigente = 1
GROUP BY tv.Id, tv.Nombre
ORDER BY tv.Nombre;

-- ===================================================================
-- COMANDOS ÚTILES PARA ADMINISTRACIÓN
-- ===================================================================

-- Para desactivar una tarifa específica:
-- UPDATE Tarifas SET EsVigente = 0, FechaActualizacion = GETDATE() WHERE Id = [ID_TARIFA];

-- Para crear una nueva vigencia (desactivar anterior y crear nueva):
-- UPDATE Tarifas SET EsVigente = 0, FechaVigenciaFin = GETDATE(), FechaActualizacion = GETDATE() 
-- WHERE TipoVehiculoId = [TIPO_ID] AND EstacionId = [ESTACION_ID] AND EsVigente = 1;

-- Para consultar tarifas vigentes por tipo de vehículo:
-- SELECT * FROM Tarifas t 
-- INNER JOIN TiposVehiculo tv ON t.TipoVehiculoId = tv.Id 
-- WHERE t.EsVigente = 1 AND t.Activo = 1 AND tv.Nombre = 'Auto';

-- Para consultar tarifas de una estación específica:
-- SELECT * FROM Tarifas t 
-- INNER JOIN Estaciones e ON t.EstacionId = e.Id 
-- WHERE t.EsVigente = 1 AND t.Activo = 1 AND e.Nombre = 'Estación Central';

PRINT '=== Script completado exitosamente ==='
