using Microsoft.EntityFrameworkCore;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Infrastructure.Data;

namespace SistemaPeaje.Infrastructure.Data;

public static class SeedData
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Verificar si ya hay datos
        if (await context.Estaciones.AnyAsync())
            return;

        // Datos de Estaciones
        var estaciones = new List<Estacion>
        {
            new() { 
                Nombre = "Estación Central", 
                Ubicacion = "Km 0 - Centro Ciudad", 
                Descripcion = "Estación principal del centro",
                FechaCreacion = DateTime.UtcNow
            },
            new() { 
                Nombre = "Estación Norte", 
                Ubicacion = "Km 15 - Entrada Norte", 
                Descripcion = "Entrada norte de la ciudad",
                FechaCreacion = DateTime.UtcNow
            },
            new() { 
                Nombre = "Estación Sur", 
                Ubicacion = "Km 20 - Salida Sur", 
                Descripcion = "Salida sur de la ciudad",
                FechaCreacion = DateTime.UtcNow
            }
        };

        context.Estaciones.AddRange(estaciones);
        await context.SaveChangesAsync();

        // Datos de Tipos de Vehículo
        var tiposVehiculo = new List<TipoVehiculo>
        {
            new() { 
                Nombre = "Automóvil", 
                Descripcion = "Vehículo de pasajeros hasta 5 personas", 
                NumeroEjes = 2, 
                TarifaBase = 50.00m,
                FechaCreacion = DateTime.UtcNow
            },
            new() { 
                Nombre = "Motocicleta", 
                Descripcion = "Vehículo de dos ruedas", 
                NumeroEjes = 2, 
                TarifaBase = 25.00m,
                FechaCreacion = DateTime.UtcNow
            },
            new() { 
                Nombre = "Camión 2 Ejes", 
                Descripcion = "Camión liviano de 2 ejes", 
                NumeroEjes = 2, 
                TarifaBase = 100.00m,
                FechaCreacion = DateTime.UtcNow
            },
            new() { 
                Nombre = "Bus", 
                Descripcion = "Vehículo de transporte público", 
                NumeroEjes = 2, 
                TarifaBase = 75.00m,
                FechaCreacion = DateTime.UtcNow
            }
        };

        context.TiposVehiculo.AddRange(tiposVehiculo);
        await context.SaveChangesAsync();

        // Datos de Tipos de Pago
        var tiposPago = new List<TipoPago>
        {
            new() { 
                Nombre = "Efectivo", 
                Descripcion = "Pago en efectivo", 
                RequiereEfectivo = true,
                FechaCreacion = DateTime.UtcNow
            },
            new() { 
                Nombre = "Tarjeta", 
                Descripcion = "Pago con tarjeta de crédito/débito", 
                RequiereTarjeta = true,
                FechaCreacion = DateTime.UtcNow
            },
            new() { 
                Nombre = "Tag RFID", 
                Descripcion = "Pago automático con tag", 
                RequiereTag = true,
                FechaCreacion = DateTime.UtcNow
            }
        };

        context.TiposPago.AddRange(tiposPago);
        await context.SaveChangesAsync();

        // Datos de Carriles
        var carriles = new List<Carril>();
        foreach (var estacion in estaciones)
        {
            for (int i = 1; i <= 3; i++)
            {
                carriles.Add(new Carril
                {
                    EstacionId = estacion.Id,
                    Numero = $"C{i:D2}",
                    Tipo = i == 1 ? "Automático" : "Manual",
                    Estado = "Activo",
                    FechaCreacion = DateTime.UtcNow
                });
            }
        }

        context.Carriles.AddRange(carriles);
        await context.SaveChangesAsync();

        // Datos de Empleados
        var empleados = new List<Empleado>
        {
            new() {
                Nombres = "Juan Carlos",
                Apellidos = "Pérez González",
                NumeroDocumento = "12345678",
                Email = "juan.perez@peaje.com",
                Telefono = "555-0001",
                Cargo = "Operador",
                EstacionId = estaciones[0].Id,
                FechaContratacion = DateTime.UtcNow.AddMonths(-6),
                FechaCreacion = DateTime.UtcNow
            },
            new() {
                Nombres = "María Elena",
                Apellidos = "Rodríguez Silva",
                NumeroDocumento = "87654321",
                Email = "maria.rodriguez@peaje.com",
                Telefono = "555-0002",
                Cargo = "Supervisor",
                EstacionId = estaciones[0].Id,
                FechaContratacion = DateTime.UtcNow.AddMonths(-12),
                FechaCreacion = DateTime.UtcNow
            }
        };

        context.Empleados.AddRange(empleados);
        await context.SaveChangesAsync();

        // Datos de Clientes
        var clientes = new List<Cliente>
        {
            new() {
                Nombres = "Ana María",
                Apellidos = "García López",
                NumeroDocumento = "98765432",
                TipoDocumento = "CC",
                Email = "ana.garcia@email.com",
                Telefono = "555-1001",
                FechaCreacion = DateTime.UtcNow
            },
            new() {
                Nombres = "Carlos Alberto",
                Apellidos = "Martínez Cruz",
                NumeroDocumento = "45612378",
                TipoDocumento = "CC",
                Email = "carlos.martinez@email.com",
                Telefono = "555-1002",
                FechaCreacion = DateTime.UtcNow
            }
        };

        context.Clientes.AddRange(clientes);
        await context.SaveChangesAsync();

        // Datos de Tarifas
        var tarifas = new List<Tarifa>();
        foreach (var tipoVehiculo in tiposVehiculo)
        {
            foreach (var estacion in estaciones)
            {
                tarifas.Add(new Tarifa
                {
                    TipoVehiculoId = tipoVehiculo.Id,
                    EstacionId = estacion.Id,
                    Monto = tipoVehiculo.TarifaBase,
                    FechaVigenciaInicio = DateTime.UtcNow.AddMonths(-1),
                    EsVigente = true,
                    FechaCreacion = DateTime.UtcNow
                });
            }
        }

        context.Tarifas.AddRange(tarifas);
        await context.SaveChangesAsync();
    }
}
