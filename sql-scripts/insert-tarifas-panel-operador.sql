-- ===================================================================
-- SCRIPT DE TARIFAS BASADO EN EL PANEL DEL OPERADOR
-- ===================================================================
-- Fecha: 09 de Agosto 2025
-- Basado en los tipos de vehículo mostrados en el panel del operador
-- ===================================================================

-- Primero verificamos qué tipos de vehículo tenemos disponibles
SELECT 'TIPOS DE VEHÍCULO DISPONIBLES' AS Consulta;
SELECT Id, Nombre, Descripcion, TarifaBase, Categoria FROM TiposVehiculo WHERE Activo = 1;

SELECT 'ESTACIONES DISPONIBLES' AS Consulta;
SELECT Id, Nombre, Ubicacion FROM Estaciones WHERE Activo = 1;

-- ===================================================================
-- TARIFAS SEGÚN EL PANEL DEL OPERADOR
-- ===================================================================

-- MOTOCICLETA - Categoría base
INSERT INTO Tarifas (TipoVehiculoId, EstacionId, Monto, FechaVigenciaInicio, FechaVigenciaFin, EsVigente, FechaCreacion, Activo)
SELECT 
    tv.Id,
    NULL,                    -- Tarifa general
    800.00,                  -- $800
    GETDATE(),
    NULL,
    1,
    GETDATE(),
    1
FROM TiposVehiculo tv 
WHERE tv.Nombre = 'Moto' AND tv.Activo = 1;

-- AUTOMÓVIL/PICK UP - Incluye autos, pickups, vans, SUVs
INSERT INTO Tarifas (TipoVehiculoId, EstacionId, Monto, FechaVigenciaInicio, FechaVigenciaFin, EsVigente, FechaCreacion, Activo)
SELECT 
    tv.Id,
    NULL,                    -- Tarifa general
    1500.00,                 -- $1,500
    GETDATE(),
    NULL,
    1,
    GETDATE(),
    1
FROM TiposVehiculo tv 
WHERE tv.Nombre = 'Auto' AND tv.Activo = 1;

-- AUTOBÚS DE 2 A 4 EJES - Transporte público
INSERT INTO Tarifas (TipoVehiculoId, EstacionId, Monto, FechaVigenciaInicio, FechaVigenciaFin, EsVigente, FechaCreacion, Activo)
SELECT 
    tv.Id,
    NULL,                    -- Tarifa general
    3000.00,                 -- $3,000
    GETDATE(),
    NULL,
    1,
    GETDATE(),
    1
FROM TiposVehiculo tv 
WHERE tv.Nombre = 'Bus' AND tv.Activo = 1;

-- CAMIÓN DE 2 A 4 EJES - Carga mediana
INSERT INTO Tarifas (TipoVehiculoId, EstacionId, Monto, FechaVigenciaInicio, FechaVigenciaFin, EsVigente, FechaCreacion, Activo)
SELECT 
    tv.Id,
    NULL,                    -- Tarifa general
    4500.00,                 -- $4,500
    GETDATE(),
    NULL,
    1,
    GETDATE(),
    1
FROM TiposVehiculo tv 
WHERE tv.Nombre = 'Camión Rígido' AND tv.Activo = 1;

-- CAMIÓN DE 5 A 6 EJES - Carga pesada
INSERT INTO Tarifas (TipoVehiculoId, EstacionId, Monto, FechaVigenciaInicio, FechaVigenciaFin, EsVigente, FechaCreacion, Activo)
SELECT 
    tv.Id,
    NULL,                    -- Tarifa general
    6500.00,                 -- $6,500 (ajustado para 5-6 ejes)
    GETDATE(),
    NULL,
    1,
    GETDATE(),
    1
FROM TiposVehiculo tv 
WHERE tv.Nombre = 'Camión Articulado' AND tv.Activo = 1;

-- CAMIÓN DE 7 A 9 EJES - Carga extra pesada
INSERT INTO Tarifas (TipoVehiculoId, EstacionId, Monto, FechaVigenciaInicio, FechaVigenciaFin, EsVigente, FechaCreacion, Activo)
SELECT 
    tv.Id,
    NULL,                    -- Tarifa general
    7500.00,                 -- $7,500 (tarifa máxima para articulados)
    GETDATE(),
    NULL,
    1,
    GETDATE(),
    1
FROM TiposVehiculo tv 
WHERE tv.Nombre = 'Camión Articulado' AND tv.Activo = 1 
AND NOT EXISTS (
    SELECT 1 FROM Tarifas t2 
    WHERE t2.TipoVehiculoId = tv.Id 
    AND t2.EstacionId IS NULL 
    AND t2.EsVigente = 1
);

-- ===================================================================
-- TARIFAS ESPECIALES PARA VEHÍCULOS DE EXCEPCIONES
-- ===================================================================

-- Para insertar tarifas de vehículos especiales, necesitarías crear primero 
-- los tipos de vehículo correspondientes en la tabla TiposVehiculo:

/*
-- EJE EXCEDENTE SENCILLO (+1 eje adicional)
INSERT INTO TiposVehiculo (Nombre, Descripcion, NumeroEjes, TarifaBase, Categoria, EsActivo, FechaCreacion)
VALUES ('Eje Excedente Sencillo', 'Eje adicional sencillo', 1, 1000.00, 'EXCEDENTE', 1, GETDATE());

-- EJE DOBLE/EXCEDENTE (+2 ejes adicionales)  
INSERT INTO TiposVehiculo (Nombre, Descripcion, NumeroEjes, TarifaBase, Categoria, EsActivo, FechaCreacion)
VALUES ('Eje Excedente Doble', 'Eje adicional doble', 2, 2000.00, 'EXCEDENTE', 1, GETDATE());
*/

-- Después podrías insertar las tarifas correspondientes:
/*
INSERT INTO Tarifas (TipoVehiculoId, EstacionId, Monto, FechaVigenciaInicio, FechaVigenciaFin, EsVigente, FechaCreacion, Activo)
VALUES 
    ((SELECT Id FROM TiposVehiculo WHERE Nombre = 'Eje Excedente Sencillo'), NULL, 1000.00, GETDATE(), NULL, 1, GETDATE(), 1),
    ((SELECT Id FROM TiposVehiculo WHERE Nombre = 'Eje Excedente Doble'), NULL, 2000.00, GETDATE(), NULL, 1, GETDATE(), 1);
*/

-- ===================================================================
-- VERIFICACIÓN FINAL
-- ===================================================================

PRINT 'Verificando tarifas insertadas...'

SELECT 
    'RESUMEN DE TARIFAS INSERTADAS' AS Consulta,
    COUNT(*) AS TotalTarifas
FROM Tarifas 
WHERE EsVigente = 1 AND Activo = 1;

SELECT 
    tv.Nombre AS TipoVehiculo,
    tv.Categoria,
    tv.NumeroEjes,
    t.Monto AS TarifaVigente,
    t.FechaVigenciaInicio,
    CASE 
        WHEN t.EstacionId IS NULL THEN 'GENERAL'
        ELSE e.Nombre 
    END AS Estacion
FROM Tarifas t
INNER JOIN TiposVehiculo tv ON t.TipoVehiculoId = tv.Id
LEFT JOIN Estaciones e ON t.EstacionId = e.Id
WHERE t.EsVigente = 1 AND t.Activo = 1
ORDER BY tv.Categoria, tv.NumeroEjes, tv.Nombre;

PRINT 'Script completado. Las tarifas han sido insertadas correctamente.'
