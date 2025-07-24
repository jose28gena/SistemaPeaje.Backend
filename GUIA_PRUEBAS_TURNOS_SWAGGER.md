# 🧪 **GUÍA DE PRUEBAS - MÓDULO DE TURNOS EN SWAGGER UI**

## 📋 **Resumen de Endpoints Disponibles**

### **🎯 Endpoints Principales del Módulo de Turnos:**

#### **1. TurnosAdmin Controller**
- `GET /api/TurnosAdmin/dashboard` - Dashboard principal de turnos
- `GET /api/TurnosAdmin/configuracion` - Configuración de turnos
- `PUT /api/TurnosAdmin/configuracion` - Actualizar configuración

#### **2. TurnoAsignaciones Controller**
- `GET /api/TurnoAsignaciones` - Listar asignaciones de turnos
- `POST /api/TurnoAsignaciones` - Crear nueva asignación
- `GET /api/TurnoAsignaciones/{id}` - Obtener asignación específica
- `PUT /api/TurnoAsignaciones/{id}` - Actualizar asignación
- `DELETE /api/TurnoAsignaciones/{id}` - Eliminar asignación
- `GET /api/TurnoAsignaciones/calendario` - Vista de calendario
- `GET /api/TurnoAsignaciones/empleado/{empleadoId}` - Turnos por empleado
- `GET /api/TurnoAsignaciones/estacion/{estacionId}` - Turnos por estación

#### **3. RegistroTiempo Controller**
- `GET /api/RegistroTiempo/{empleadoId}` - Registros de tiempo por empleado
- `POST /api/RegistroTiempo/marcar-entrada` - Marcar entrada
- `POST /api/RegistroTiempo/marcar-salida` - Marcar salida
- `GET /api/RegistroTiempo/reporte/{empleadoId}` - Reporte de tiempo

#### **4. MonitorEventos Controller**
- `GET /api/MonitorEventos/turno/{turnoId}` - Eventos por turno
- `POST /api/MonitorEventos/iniciar-turno` - Iniciar turno
- `POST /api/MonitorEventos/finalizar-turno` - Finalizar turno

#### **5. ReportesTurnos Controller**
- `GET /api/ReportesTurnos/productividad` - Reporte de productividad
- `GET /api/ReportesTurnos/asistencia` - Reporte de asistencia
- `GET /api/ReportesTurnos/horas-extras` - Reporte de horas extras

#### **6. TurnoTemplates Controller**
- `GET /api/TurnoTemplates` - Listar plantillas de turnos
- `POST /api/TurnoTemplates` - Crear plantilla
- `PUT /api/TurnoTemplates/{id}` - Actualizar plantilla
- `DELETE /api/TurnoTemplates/{id}` - Eliminar plantilla

---

## 🚀 **PASOS PARA PROBAR EN SWAGGER UI**

### **Paso 1: Acceder a Swagger**
- URL: `https://localhost:5001/swagger`
- El servidor ya está ejecutándose en segundo plano

### **Paso 2: Probar Dashboard de Turnos**
1. Buscar **TurnosAdmin** en la lista de controllers
2. Expandir `GET /api/TurnosAdmin/dashboard`
3. Hacer clic en **"Try it out"**
4. Hacer clic en **"Execute"**
5. **Resultado esperado**: JSON con métricas del dashboard

### **Paso 3: Probar Configuración de Turnos**
1. Expandir `GET /api/TurnosAdmin/configuracion`
2. Hacer clic en **"Try it out"** → **"Execute"**
3. **Resultado esperado**: JSON con configuración actual

### **Paso 4: Probar Asignaciones de Turnos**
1. Buscar **TurnoAsignaciones**
2. Expandir `GET /api/TurnoAsignaciones`
3. **"Try it out"** → **"Execute"**
4. **Resultado esperado**: Lista de asignaciones (puede estar vacía)

### **Paso 5: Probar Calendario de Turnos**
1. En **TurnoAsignaciones**
2. Expandir `GET /api/TurnoAsignaciones/calendario`
3. Agregar parámetros de fecha si es necesario
4. **"Try it out"** → **"Execute"**

### **Paso 6: Probar Plantillas de Turnos**
1. Buscar **TurnoTemplates**
2. Expandir `GET /api/TurnoTemplates`
3. **"Try it out"** → **"Execute"**
4. **Resultado esperado**: Lista de plantillas disponibles

### **Paso 7: Probar Reportes**
1. Buscar **ReportesTurnos**
2. Probar `GET /api/ReportesTurnos/productividad`
3. Agregar parámetros de fecha (fechaInicio, fechaFin)
4. **"Try it out"** → **"Execute"**

---

## 📊 **EJEMPLOS DE RESPUESTAS ESPERADAS**

### **Dashboard Response:**
```json
{
  "turnosActivos": 0,
  "empleadosPresentes": 0,
  "estacionesOperativas": 2,
  "alertasPendientes": 0,
  "ultimaActualizacion": "2025-07-20T23:30:00Z"
}
```

### **Configuración Response:**
```json
{
  "horarioLaboral": {
    "inicioTurno": "06:00:00",
    "finTurno": "22:00:00"
  },
  "toleranciaTardanza": 15,
  "factorHoraExtra": 1.5
}
```

### **Calendario Response:**
```json
{
  "turnos": [],
  "fechaConsulta": "2025-07-20",
  "totalTurnos": 0
}
```

---

## ⚠️ **NOTAS IMPORTANTES**

1. **Base de Datos Nueva**: Como acabamos de crear las tablas, las respuestas iniciales pueden estar vacías
2. **Autenticación**: Algunos endpoints pueden requerir autenticación (verificar si devuelve 401)
3. **Datos de Prueba**: Considera crear datos de prueba para ver respuestas más completas
4. **CORS**: Si hay problemas, verificar configuración CORS en el backend

---

## 🔧 **CREACIÓN DE DATOS DE PRUEBA (OPCIONAL)**

Si quieres ver respuestas más completas, puedes:

1. **Crear Empleados** usando endpoints existentes
2. **Crear Estaciones** si no existen
3. **Crear Plantillas de Turnos** usando `POST /api/TurnoTemplates`
4. **Asignar Turnos** usando `POST /api/TurnoAsignaciones`

---

## ✅ **CHECKLIST DE VALIDACIÓN**

- [ ] Dashboard responde sin errores
- [ ] Configuración se puede obtener
- [ ] Lista de asignaciones es accesible
- [ ] Calendario funciona
- [ ] Plantillas de turnos responden
- [ ] Reportes se pueden consultar
- [ ] No hay errores 500 en ningún endpoint
- [ ] Responses siguen el formato JSON esperado

---

## 📞 **SOPORTE**

Si encuentras errores:
1. Verificar que el servidor esté ejecutándose
2. Revisar logs en la consola
3. Verificar que la base de datos tenga las tablas correctas
4. Confirmar que las migraciones se aplicaron correctamente

**Estado Actual**: ✅ Servidor funcionando, endpoints disponibles para pruebas
