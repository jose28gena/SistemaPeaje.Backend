# Sistema de Peaje API

Una API REST completa para la gestión de sistemas de peaje desarrollada con .NET 8 y siguiendo los principios de Clean Architecture.

## Características

- ✅ **Clean Architecture**: Separación clara de responsabilidades en capas
- ✅ **CQRS + MediatR**: Patrón Command Query Responsibility Segregation
- ✅ **Entity Framework Core**: ORM para acceso a datos
- ✅ **AutoMapper**: Mapeo automático entre entidades y DTOs
- ✅ **FluentValidation**: Validación fluida y expresiva
- ✅ **Swagger/OpenAPI**: Documentación automática de la API
- ✅ **Serilog**: Logging estructurado
- ✅ **JWT Authentication**: Autenticación basada en tokens
- ✅ **CORS**: Configurado para aplicaciones Angular
- ✅ **Unit of Work Pattern**: Gestión de transacciones
- ✅ **Repository Pattern**: Abstracción del acceso a datos
- ✅ **Global Exception Handling**: Manejo centralizado de errores
- ✅ **Unit Tests**: Cobertura completa de pruebas unitarias con xUnit, Moq y FluentAssertions (48 tests)

## Estructura del Proyecto

```
SistemaPeaje.API/
├── src/
│   ├── SistemaPeaje.API/           # Capa de presentación (Controllers, Middleware)
│   ├── SistemaPeaje.Application/   # Capa de aplicación (CQRS, DTOs, Validators)
│   ├── SistemaPeaje.Infrastructure/ # Capa de infraestructura (EF Core, Repositories)
│   └── SistemaPeaje.Core/          # Capa de dominio (Entities, Interfaces)
└── tests/
    └── SistemaPeaje.Tests/         # Pruebas unitarias
```

## Tecnologías Utilizadas

- **.NET 8**: Framework principal
- **ASP.NET Core Web API**: Para crear la API REST
- **Entity Framework Core 8**: ORM para base de datos
- **SQL Server**: Base de datos
- **MediatR**: Implementación del patrón Mediator
- **AutoMapper**: Mapeo objeto a objeto
- **FluentValidation**: Validación de datos
- **Serilog**: Logging
- **Swagger/OpenAPI**: Documentación de API
- **xUnit**: Framework de testing
- **Moq**: Framework de mocking
- **FluentAssertions**: Assertions fluidas para tests

## Endpoints Principales

### Transacciones
- `GET /api/transacciones` - Obtener todas las transacciones (con filtros)
- `GET /api/transacciones/{id}` - Obtener transacción por ID
- `POST /api/transacciones` - Crear nueva transacción
- `PUT /api/transacciones/{id}` - Actualizar transacción
- `DELETE /api/transacciones/{id}` - Eliminar transacción
- `GET /api/transacciones/resumen-diario` - Resumen diario de transacciones

### Carriles
- `GET /api/carriles` - Obtener todos los carriles
- `GET /api/carriles/{id}` - Obtener carril por ID
- `POST /api/carriles` - Crear nuevo carril
- `PUT /api/carriles/{id}` - Actualizar carril
- `DELETE /api/carriles/{id}` - Eliminar carril

### Tarifas
- `GET /api/tarifas` - Obtener todas las tarifas
- `GET /api/tarifas/{id}` - Obtener tarifa por ID
- `POST /api/tarifas` - Crear nueva tarifa
- `PUT /api/tarifas/{id}` - Actualizar tarifa
- `DELETE /api/tarifas/{id}` - Eliminar tarifa

### Turnos
- `POST /api/turnos/abrir` - Abrir nuevo turno
- `PUT /api/turnos/{id}/cerrar` - Cerrar turno
- `GET /api/turnos` - Obtener turnos
- `GET /api/turnos/{id}` - Obtener turno por ID

### Reportes
- `GET /api/reportes/operacion` - Reporte de operaciones
- `GET /api/reportes/resumen-diario` - Resumen diario de operaciones

### Monitor de Eventos
- `GET /api/monitor/eventos-tiempo-real` - Eventos en tiempo real

### Estaciones
- `GET /api/estaciones` - Obtener todas las estaciones
- `GET /api/estaciones/{id}` - Obtener estación por ID
- `POST /api/estaciones` - Crear nueva estación
- `PUT /api/estaciones/{id}` - Actualizar estación
- `DELETE /api/estaciones/{id}` - Eliminar estación

## Configuración

### Base de Datos

1. Configurar la cadena de conexión en `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SistemaPeajeDB;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true"
  }
}
```

2. Ejecutar migraciones para crear la base de datos:
```bash
dotnet ef database update --project src/SistemaPeaje.Infrastructure --startup-project src/SistemaPeaje.API
```

**Nota**: Las migraciones ya están disponibles e incluyen:
- ✅ **Migración Inicial**: Todas las entidades principales (Estaciones, Carriles, Transacciones, etc.)
- ✅ **EventoTransito**: Sistema de eventos en tiempo real
- ✅ **Usuario**: Entidades de autenticación y autorización

### Verificar Migraciones

Para ver las migraciones disponibles:
```bash
dotnet ef migrations list --project src/SistemaPeaje.Infrastructure --startup-project src/SistemaPeaje.API
```

Para agregar nuevas migraciones (si es necesario):
```bash
dotnet ef migrations add NombreMigracion --project src/SistemaPeaje.Infrastructure --startup-project src/SistemaPeaje.API
```

### Secretos (JWT y administrador)

Los secretos **no** se guardan en el repositorio. `appsettings.json` solo trae valores vacíos y la API se niega a arrancar si falta la clave JWT.

Define estas variables de entorno (o usa `dotnet user-secrets`):

| Variable | Descripción |
| --- | --- |
| `Jwt__Key` | Clave de firma del token, mínimo 32 caracteres |
| `Auth__AdminUsername` | Usuario para `POST /api/auth/login` |
| `Auth__AdminPassword` | Contraseña para `POST /api/auth/login` |

```powershell
# PowerShell
$env:Jwt__Key = "una-clave-larga-y-aleatoria-de-al-menos-32-caracteres"
$env:Auth__AdminUsername = "admin"
$env:Auth__AdminPassword = "<una-contraseña-segura>"
dotnet run --project src/SistemaPeaje.API
```

Con `user-secrets`:
```bash
dotnet user-secrets init --project src/SistemaPeaje.API
dotnet user-secrets set "Jwt:Key" "una-clave-larga-y-aleatoria-de-al-menos-32-caracteres" --project src/SistemaPeaje.API
dotnet user-secrets set "Auth:AdminUsername" "admin" --project src/SistemaPeaje.API
dotnet user-secrets set "Auth:AdminPassword" "<una-contraseña-segura>" --project src/SistemaPeaje.API
```

Si `Auth__AdminUsername` o `Auth__AdminPassword` no están definidos, el login de administrador queda deshabilitado. `POST /api/auth/login-empleado` solo emite token para empleados activos registrados en la base de datos.

El resto de parámetros del token (`Issuer`, `Audience`, `ExpireMinutes`) están en la sección `Jwt` de `appsettings.json`.

## Ejecución

### Configuración Rápida

1. **Clonar el repositorio**
   ```bash
   git clone <repository-url>
   cd SistemaPeaje
   ```

2. **Restaurar dependencias**:
   ```bash
   dotnet restore
   ```

3. **Configurar base de datos** (opcional - usar LocalDB por defecto):
   - Editar `appsettings.json` si se desea usar otra instancia de SQL Server
   - La configuración por defecto usa LocalDB

4. **Ejecutar migraciones**:
   ```bash
   dotnet ef database update --project src/SistemaPeaje.Infrastructure --startup-project src/SistemaPeaje.API
   ```

5. **Compilar el proyecto**:
   ```bash
   dotnet build
   ```

6. **Ejecutar la aplicación**:
   ```bash
   dotnet run --project src/SistemaPeaje.API
   ```

7. **Acceder a la documentación**: 
   - Swagger UI: `https://localhost:5001/swagger`
   - API Base: `https://localhost:5001/api`

### Ejecutar Solo las Pruebas

```bash
dotnet test tests/SistemaPeaje.Tests/SistemaPeaje.Tests.csproj
```

## Pasos Detallados

1. **Clonar el repositorio**
2. **Restaurar dependencias**:
   ```bash
   dotnet restore
   ```
3. **Compilar el proyecto**:
   ```bash
   dotnet build
   ```
4. **Ejecutar la aplicación**:
   ```bash
   dotnet run --project src/SistemaPeaje.API
   ```
5. **Acceder a Swagger**: `http://localhost:5000` o `https://localhost:5001`

## Pruebas

### Ejecutar Pruebas Unitarias

Ejecutar todas las pruebas unitarias:
```bash
dotnet test
```

Ejecutar pruebas con información detallada:
```bash
dotnet test --verbosity normal
```

Ejecutar pruebas con cobertura:
```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Cobertura de Pruebas

El proyecto cuenta con **48 pruebas unitarias** que cubren todos los handlers principales:

#### Módulos Cubiertos:
- ✅ **Carriles**: CreateCarrilCommandHandler, GetCarrilesQueryHandler
- ✅ **Tarifas**: CreateTarifaCommandHandler
- ✅ **Turnos**: AbrirTurnoCommandHandler, CerrarTurnoCommandHandler
- ✅ **Transacciones**: CreateTransaccionCommandHandler, UpdateTransaccionCommandHandler, DeleteTransaccionCommandHandler, GetTransaccionesQueryHandler, GetTransaccionByIdQueryHandler
- ✅ **Reportes**: GetReporteOperacionQueryHandler
- ✅ **Monitor de Eventos**: GetEventosEnTiempoRealQueryHandler

#### Casos de Prueba Incluidos:
- ✅ Casos exitosos (happy path)
- ✅ Validación de datos inválidos
- ✅ Manejo de entidades no encontradas
- ✅ Validación de reglas de negocio
- ✅ Pruebas de mapeo de datos
- ✅ Validación de ordenamiento y filtros
- ✅ Pruebas de estados y cálculos automáticos

#### Tecnologías de Testing:
- **xUnit**: Framework de testing principal
- **Moq**: Mocking de dependencias
- **FluentAssertions**: Assertions expresivas y legibles
- **AutoMapper**: Testing de mapeos

## Entidades Principales

- **Transaccion**: Registro de cada cobro de peaje
- **Estacion**: Estaciones de peaje
- **Carril**: Carriles de una estación
- **TipoVehiculo**: Clasificación de vehículos
- **TipoPago**: Métodos de pago (efectivo, tarjeta, tag)
- **Cliente**: Información de clientes
- **Empleado**: Operadores del sistema
- **Tarifa**: Tarifas por tipo de vehículo
- **TarjetaRFID**: Tags RFID para pagos automáticos
- **EventoTransito**: Eventos del sistema en tiempo real
- **Turno**: Turnos de trabajo de empleados

## Clean Architecture

El proyecto sigue los principios de Clean Architecture:

1. **Core (Dominio)**: Entidades de negocio e interfaces
2. **Application**: Lógica de aplicación, casos de uso, DTOs
3. **Infrastructure**: Implementación de acceso a datos, servicios externos
4. **API**: Controladores, middleware, configuración

## Patrones Implementados

- **CQRS**: Separación de comandos y consultas con MediatR
- **Repository Pattern**: Abstracción del acceso a datos
- **Unit of Work**: Gestión de transacciones
- **Dependency Injection**: Inversión de control
- **Mediator Pattern**: Desacoplamiento entre capas
- **Global Exception Handling**: Manejo centralizado de errores
- **Validation Pattern**: Validación con FluentValidation
- **Mapping Pattern**: AutoMapper para transformación de datos

## Estado del Proyecto

### ✅ Completado
- Arquitectura base implementada
- Todos los endpoints principales funcionales
- Base de datos completamente estructurada
- **Migraciones de Entity Framework implementadas y listas**
- Sistema de autenticación JWT
- Manejo global de excepciones
- Logging con Serilog
- **48 pruebas unitarias implementadas y funcionando al 100%**
- Documentación Swagger completa

### 🔄 En Desarrollo
- Integración con frontend Angular
- Pruebas de integración
- Despliegue en contenedores Docker

### 📋 Próximos Pasos
- Implementar caching con Redis
- Agregar métricas y monitoreo
- Implementar notificaciones en tiempo real con SignalR
- Agregar más validaciones de negocio
