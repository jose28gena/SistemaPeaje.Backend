-- ===================================================================
-- SCRIPT SIMPLE DE INSERCIÓN DE TARIFAS - CASOS ESPECÍFICOS
-- ===================================================================
-- Fecha: 09 de Agosto 2025
-- Propósito: Scripts simples para casos de uso comunes
-- ===================================================================

-- ===================================================================
-- CASO 1: INSERTAR UNA TARIFA SIMPLE (GENERAL)
-- ===================================================================
-- Ejemplo: Nueva tarifa para Pick-up/SUV

INSERT INTO Tarifas (
    TipoVehiculoId,    -- ID del tipo de vehículo (consultar tabla TiposVehiculo)
    EstacionId,        -- NULL = tarifa general, o ID específico de estación
    Monto,             -- Precio en pesos
    FechaVigenciaInicio,
    FechaVigenciaFin,  -- NULL = sin fecha de vencimiento
    EsVigente,
    FechaCreacion,
    Activo
) VALUES (
    2,                 -- Asumiendo que ID 2 = Automóvil/Pick-up
    NULL,              -- Tarifa general (todas las estaciones)
    2000.00,           -- $2,000.00
    GETDATE(),         -- Vigente desde ahora
    NULL,              -- Sin fecha de vencimiento
    1,                 -- Es vigente
    GETDATE(),         -- Fecha de creación
    1                  -- Activo
);

-- ===================================================================
-- CASO 2: INSERTAR TARIFA ESPECÍFICA PARA UNA ESTACIÓN
-- ===================================================================
-- Ejemplo: Tarifa especial para Estación Norte

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
    1,                 -- ID del tipo (ej: Motocicleta)
    2,                 -- ID de Estación Norte
    750.00,            -- $750.00 (precio especial)
    GETDATE(),
    NULL,
    1,
    GETDATE(),
    1
);

-- ===================================================================
-- CASO 3: INSERTAR TARIFA TEMPORAL/PROMOCIONAL
-- ===================================================================
-- Ejemplo: Tarifa promocional por 30 días

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
    NULL,                                 -- Todas las estaciones
    1200.00,                             -- $1,200.00 (precio promocional)
    GETDATE(),                           -- Desde hoy
    DATEADD(DAY, 30, GETDATE()),        -- Válido por 30 días
    1,
    GETDATE(),
    1
);

-- ===================================================================
-- CASO 4: ACTUALIZAR TARIFA EXISTENTE (CREAR NUEVA VIGENCIA)
-- ===================================================================
-- Pasos: 1) Desactivar tarifa anterior, 2) Crear nueva tarifa

-- Paso 1: Desactivar tarifa anterior
UPDATE Tarifas 
SET EsVigente = 0, 
    FechaVigenciaFin = GETDATE(), 
    FechaActualizacion = GETDATE()
WHERE TipoVehiculoId = 1      -- Motocicleta
  AND EstacionId IS NULL      -- Tarifa general
  AND EsVigente = 1;          -- Solo las vigentes

-- Paso 2: Insertar nueva tarifa
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
    1,                 -- Motocicleta
    NULL,              -- Tarifa general
    850.00,            -- $850.00 (nuevo precio)
    GETDATE(),         -- Vigente desde ahora
    NULL,              -- Sin vencimiento
    1,
    GETDATE(),
    1
);

-- ===================================================================
-- CONSULTAS ÚTILES PARA VERIFICAR
-- ===================================================================

-- Ver todas las tarifas vigentes
SELECT 
    t.Id,
    tv.Nombre AS TipoVehiculo,
    ISNULL(e.Nombre, 'GENERAL') AS Estacion,
    t.Monto,
    t.FechaVigenciaInicio,
    t.FechaVigenciaFin
FROM Tarifas t
INNER JOIN TiposVehiculo tv ON t.TipoVehiculoId = tv.Id
LEFT JOIN Estaciones e ON t.EstacionId = e.Id
WHERE t.EsVigente = 1 AND t.Activo = 1
ORDER BY tv.Nombre, e.Nombre;

-- Ver IDs disponibles de tipos de vehículo
SELECT Id, Nombre, TarifaBase FROM TiposVehiculo WHERE Activo = 1;

-- Ver IDs disponibles de estaciones
SELECT Id, Nombre, Ubicacion FROM Estaciones WHERE Activo = 1;
