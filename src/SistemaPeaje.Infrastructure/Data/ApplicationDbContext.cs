using Microsoft.EntityFrameworkCore;
using SistemaPeaje.Core.Entities;

namespace SistemaPeaje.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Transaccion> Transacciones { get; set; }
    public DbSet<Estacion> Estaciones { get; set; }
    public DbSet<Carril> Carriles { get; set; }
    public DbSet<TipoVehiculo> TiposVehiculo { get; set; }
    public DbSet<TipoPago> TiposPago { get; set; }
    public DbSet<TipoCliente> TiposCliente { get; set; }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Empleado> Empleados { get; set; }
    public DbSet<Tarifa> Tarifas { get; set; }
    public DbSet<TarjetaRFID> TarjetasRFID { get; set; }
    public DbSet<Turno> Turnos { get; set; }
    public DbSet<EventoTransito> EventosTransito { get; set; }
    public DbSet<ComandoPlc> ComandosPlc { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Deshabilitar cascade delete globalmente para evitar ciclos
        foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.Restrict;
        }

        // Configuración de Transaccion
        modelBuilder.Entity<Transaccion>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Monto).HasColumnType("decimal(18,2)");
            entity.Property(e => e.PlacaVehiculo).HasMaxLength(10);
            entity.Property(e => e.TagRFID).HasMaxLength(50);
            entity.Property(e => e.NumeroTicket).HasMaxLength(50);
            entity.Property(e => e.Observaciones).HasMaxLength(500);

            entity.HasOne(e => e.Estacion)
                .WithMany(e => e.Transacciones)
                .HasForeignKey(e => e.EstacionId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Carril)
                .WithMany(e => e.Transacciones)
                .HasForeignKey(e => e.CarrilId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.TipoVehiculo)
                .WithMany(e => e.Transacciones)
                .HasForeignKey(e => e.TipoVehiculoId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.TipoPago)
                .WithMany(e => e.Transacciones)
                .HasForeignKey(e => e.TipoPagoId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Cliente)
                .WithMany(e => e.Transacciones)
                .HasForeignKey(e => e.ClienteId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Empleado)
                .WithMany(e => e.Transacciones)
                .HasForeignKey(e => e.EmpleadoId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configuración de Estacion
        modelBuilder.Entity<Estacion>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Ubicacion).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Descripcion).HasMaxLength(500);
        });

        // Configuración de Carril
        modelBuilder.Entity<Carril>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Numero).HasMaxLength(10).IsRequired();
            entity.Property(e => e.Tipo).HasMaxLength(20).IsRequired();
            entity.Property(e => e.Estado).HasMaxLength(20).IsRequired();

            entity.HasOne(e => e.Estacion)
                .WithMany(e => e.Carriles)
                .HasForeignKey(e => e.EstacionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuración de TipoVehiculo
        modelBuilder.Entity<TipoVehiculo>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Descripcion).HasMaxLength(200).IsRequired();
            entity.Property(e => e.TarifaBase).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Categoria).HasMaxLength(20).IsRequired();
        });

        // Configuración de TipoPago
        modelBuilder.Entity<TipoPago>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Descripcion).HasMaxLength(200).IsRequired();
            entity.Property(e => e.LimiteCredito).HasColumnType("decimal(18,2)");
        });

        // Configuración de Cliente
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombres).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Apellidos).HasMaxLength(100).IsRequired();
            entity.Property(e => e.NumeroDocumento).HasMaxLength(20);
            entity.Property(e => e.TipoDocumento).HasMaxLength(10);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Telefono).HasMaxLength(20);
            entity.Property(e => e.Direccion).HasMaxLength(200);

            entity.HasOne(e => e.TipoCliente)
                .WithMany(tc => tc.Clientes)
                .HasForeignKey(e => e.TipoClienteId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuración de TipoCliente
        modelBuilder.Entity<TipoCliente>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Descripcion).HasMaxLength(200);
            entity.Property(e => e.DescuentoPorcentaje).HasColumnType("decimal(5,2)");
            entity.Property(e => e.DocumentosRequeridos).HasMaxLength(500);
        });

        // Configuración de Empleado
        modelBuilder.Entity<Empleado>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombres).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Apellidos).HasMaxLength(100).IsRequired();
            entity.Property(e => e.NumeroDocumento).HasMaxLength(20).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Telefono).HasMaxLength(20);
            entity.Property(e => e.Cargo).HasMaxLength(50).IsRequired();

            entity.HasOne(e => e.Estacion)
                .WithMany()
                .HasForeignKey(e => e.EstacionId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configuración de Tarifa
        modelBuilder.Entity<Tarifa>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Monto).HasColumnType("decimal(18,2)");

            entity.HasOne(e => e.TipoVehiculo)
                .WithMany(e => e.Tarifas)
                .HasForeignKey(e => e.TipoVehiculoId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Estacion)
                .WithMany()
                .HasForeignKey(e => e.EstacionId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configuración de TarjetaRFID
        modelBuilder.Entity<TarjetaRFID>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.NumeroTag).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Saldo).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Estado).HasMaxLength(20).IsRequired();

            entity.HasOne(e => e.Cliente)
                .WithMany(e => e.TarjetasRFID)
                .HasForeignKey(e => e.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuración de Turno
        modelBuilder.Entity<Turno>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.MontoInicialCaja).HasColumnType("decimal(18,2)");
            entity.Property(e => e.MontoFinalCaja).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Estado).HasMaxLength(20).IsRequired();

            entity.HasOne(e => e.Empleado)
                .WithMany(e => e.Turnos)
                .HasForeignKey(e => e.EmpleadoId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Estacion)
                .WithMany()
                .HasForeignKey(e => e.EstacionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuración de EventoTransito
        modelBuilder.Entity<EventoTransito>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TipoEvento).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Descripcion).HasMaxLength(500);
            entity.Property(e => e.DatosAdicionales).HasMaxLength(1000);
            entity.Property(e => e.PlacaVehiculo).HasMaxLength(20);
            entity.Property(e => e.CodigoRfid).HasMaxLength(100);
            entity.Property(e => e.Observaciones).HasMaxLength(500);
            entity.Property(e => e.RutaImagen).HasMaxLength(200);
            entity.Property(e => e.EstadoEvento).HasMaxLength(50);
            entity.Property(e => e.VelocidadVehiculo).HasColumnType("decimal(10,2)");
            entity.Property(e => e.SensorId).HasMaxLength(100);

            entity.HasOne(e => e.Estacion)
                .WithMany()
                .HasForeignKey(e => e.EstacionId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Carril)
                .WithMany()
                .HasForeignKey(e => e.CarrilId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.TipoVehiculo)
                .WithMany()
                .HasForeignKey(e => e.TipoVehiculoId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Transaccion)
                .WithMany()
                .HasForeignKey(e => e.TransaccionId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configuración de ComandoPlc
        // Configuración de ComandoPlc
        modelBuilder.Entity<ComandoPlc>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TipoComando).HasMaxLength(50).IsRequired();
            entity.Property(e => e.IpDestino).HasMaxLength(15).IsRequired();
            entity.Property(e => e.MensajeError).HasMaxLength(500);
            entity.Property(e => e.Observaciones).HasMaxLength(200);

            entity.HasOne(e => e.Carril)
                .WithMany()
                .HasForeignKey(e => e.CarrilId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Usuario)
                .WithMany()
                .HasForeignKey(e => e.UsuarioId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Empleado)
                .WithMany()
                .HasForeignKey(e => e.EmpleadoId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuración de Usuario
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.NombreUsuario).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(100).IsRequired();
            entity.Property(e => e.PasswordHash).HasMaxLength(256).IsRequired();
            entity.Property(e => e.Nombres).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Apellidos).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Rol).HasMaxLength(20).IsRequired();
            entity.Property(e => e.Telefono).HasMaxLength(20);
            entity.Property(e => e.Permisos).HasMaxLength(500);

            entity.HasIndex(e => e.NombreUsuario).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();

            entity.HasOne(e => e.Empleado)
                .WithMany()
                .HasForeignKey(e => e.EmpleadoId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Estacion)
                .WithMany()
                .HasForeignKey(e => e.EstacionId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
