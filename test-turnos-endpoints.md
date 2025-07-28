# Test Endpoints para Turnos

## Probar API de Turnos

### 1. Obtener todos los turnos
```bash
curl -X GET "https://localhost:51393/api/Turnos" -k
```

### 2. Abrir un nuevo turno
```bash
curl -X POST "https://localhost:51393/api/Turnos/abrir" \
  -H "Content-Type: application/json" \
  -d '{
    "empleadoId": 1,
    "estacionId": 1,
    "montoInicialCaja": 500.00
  }' -k
```

### 3. Obtener turnos activos
```bash
curl -X GET "https://localhost:51393/api/Turnos/activos" -k
```

### 4. Cerrar un turno (ID 1)
```bash
curl -X POST "https://localhost:51393/api/Turnos/1/cerrar" \
  -H "Content-Type: application/json" \
  -d '{
    "montoFinalCaja": 1200.00
  }' -k
```

### 5. Obtener resumen de un turno
```bash
curl -X GET "https://localhost:51393/api/Turnos/1/resumen" -k
```

## Templates de Turnos

### 6. Obtener plantillas de turnos
```bash
curl -X GET "https://localhost:51393/api/TurnoTemplates" -k
```

### 7. Crear una plantilla de turno
```bash
curl -X POST "https://localhost:51393/api/TurnoTemplates" \
  -H "Content-Type: application/json" \
  -d '{
    "nombre": "Turno Mañana",
    "descripcion": "Turno matutino estándar",
    "horaInicio": "08:00",
    "horaFin": "16:00",
    "factorHoraExtra": 1.5,
    "diasSemana": "Lunes,Martes,Miercoles,Jueves,Viernes",
    "esTurnoNocturno": false,
    "esRotativo": false
  }' -k
```

## Asignaciones de Turnos

### 8. Obtener asignaciones de turnos
```bash
curl -X GET "https://localhost:51393/api/TurnoAsignaciones" -k
```

### 9. Crear una asignación de turno
```bash
curl -X POST "https://localhost:51393/api/TurnoAsignaciones" \
  -H "Content-Type: application/json" \
  -d '{
    "empleadoId": 1,
    "estacionId": 1,
    "turnoTemplateId": 1,
    "fechaTurno": "2025-07-26",
    "horaInicio": "08:00",
    "horaFin": "16:00",
    "montoInicialCajaAsignado": 500.00
  }' -k
```

## Eventos de Turnos

### 10. Obtener eventos de turnos
```bash
curl -X GET "https://localhost:51393/api/TurnoEventos" -k
```

## Admin Dashboard

### 11. Obtener configuración de turnos admin
```bash
curl -X GET "https://localhost:51393/api/turnos-admin/configuracion" -k
```

### 12. Obtener dashboard de turnos admin
```bash
curl -X GET "https://localhost:51393/api/turnos-admin/dashboard" -k
```

## Liquidaciones de Turnos

### 13. Obtener liquidaciones
```bash
curl -X GET "https://localhost:51393/api/turno-liquidaciones" -k
```

## Catálogos necesarios para turnos

### 14. Obtener empleados
```bash
curl -X GET "https://localhost:51393/api/Empleados" -k
```

### 15. Obtener estaciones
```bash
curl -X GET "https://localhost:51393/api/Estaciones" -k
```
