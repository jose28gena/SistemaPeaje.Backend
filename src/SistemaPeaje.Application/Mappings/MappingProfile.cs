using AutoMapper;
using SistemaPeaje.Application.DTOs;
using SistemaPeaje.Core.Entities;

namespace SistemaPeaje.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Transaccion, TransaccionDto>()
            .ForMember(dest => dest.EstacionNombre, opt => opt.MapFrom(src => src.Estacion!.Nombre))
            .ForMember(dest => dest.CarrilNumero, opt => opt.MapFrom(src => src.Carril!.Numero))
            .ForMember(dest => dest.TipoVehiculoNombre, opt => opt.MapFrom(src => src.TipoVehiculo!.Nombre))
            .ForMember(dest => dest.TipoPagoNombre, opt => opt.MapFrom(src => src.TipoPago!.Nombre))
            .ForMember(dest => dest.ClienteNombre, opt => opt.MapFrom(src => 
                src.Cliente != null ? $"{src.Cliente.Nombres} {src.Cliente.Apellidos}" : null))
            .ForMember(dest => dest.EmpleadoNombre, opt => opt.MapFrom(src => 
                src.Empleado != null ? $"{src.Empleado.Nombres} {src.Empleado.Apellidos}" : null));

        CreateMap<Estacion, EstacionDto>();
        CreateMap<Carril, CarrilDto>()
            .ForMember(dest => dest.EstacionNombre, opt => opt.MapFrom(src => src.Estacion != null ? src.Estacion.Nombre : null));
        CreateMap<CreateCarrilDto, Carril>();
        CreateMap<UpdateCarrilDto, Carril>();
        CreateMap<TipoVehiculo, TipoVehiculoDto>();
        CreateMap<TipoPago, TipoPagoDto>();
        
        // ConfiguracionTipoPago mappings
        CreateMap<ConfiguracionTipoPago, ConfiguracionTipoPagoDto>()
            .ForMember(dest => dest.TipoPagoNombre, opt => opt.MapFrom(src => src.TipoPago != null ? src.TipoPago.Nombre : null));
        CreateMap<CreateConfiguracionTipoPagoDto, ConfiguracionTipoPago>();
        CreateMap<UpdateConfiguracionTipoPagoDto, ConfiguracionTipoPago>();
        CreateMap<TipoPago, TipoPagoConConfiguracionDto>()
            .ForMember(dest => dest.Configuracion, opt => opt.MapFrom(src => src.Configuracion));
        CreateMap<TipoCliente, TipoClienteDto>()
            .ForMember(dest => dest.EsActivo, opt => opt.MapFrom(src => src.Activo));
        CreateMap<Usuario, UsuarioDto>()
            .ForMember(dest => dest.EmpleadoNombre, opt => opt.MapFrom(src => src.Empleado != null ? $"{src.Empleado.Nombres} {src.Empleado.Apellidos}" : null))
            .ForMember(dest => dest.EstacionNombre, opt => opt.MapFrom(src => src.Estacion != null ? src.Estacion.Nombre : null));
        CreateMap<Cliente, ClienteDto>();
        CreateMap<Empleado, EmpleadoDto>()
            .ForMember(dest => dest.EstacionNombre, opt => opt.MapFrom(src => src.Estacion != null ? src.Estacion.Nombre : null));
        CreateMap<Tarifa, TarifaDto>()
            .ForMember(dest => dest.TipoVehiculoNombre, opt => opt.MapFrom(src => src.TipoVehiculo != null ? src.TipoVehiculo.Nombre : null))
            .ForMember(dest => dest.EstacionNombre, opt => opt.MapFrom(src => src.Estacion != null ? src.Estacion.Nombre : null));
        CreateMap<Turno, TurnoDto>()
            .ForMember(dest => dest.EmpleadoNombre, opt => opt.MapFrom(src => src.Empleado != null ? $"{src.Empleado.Nombres} {src.Empleado.Apellidos}" : null))
            .ForMember(dest => dest.EstacionNombre, opt => opt.MapFrom(src => src.Estacion != null ? src.Estacion.Nombre : null));
        
        // Liquidacion mappings
        CreateMap<Liquidacion, LiquidacionDto>()
            .ForMember(dest => dest.EmpleadoNombre, opt => opt.MapFrom(src => 
                src.Empleado != null ? $"{src.Empleado.Nombres} {src.Empleado.Apellidos}" : null))
            .ForMember(dest => dest.EstacionNombre, opt => opt.MapFrom(src => src.Estacion != null ? src.Estacion.Nombre : null))
            .ForMember(dest => dest.AprobadoPorEmpleadoNombre, opt => opt.MapFrom(src => 
                src.AprobadoPorEmpleado != null ? $"{src.AprobadoPorEmpleado.Nombres} {src.AprobadoPorEmpleado.Apellidos}" : null))
            .ForMember(dest => dest.CreadoPorEmpleadoNombre, opt => opt.MapFrom(src => 
                src.CreadoPorEmpleado != null ? $"{src.CreadoPorEmpleado.Nombres} {src.CreadoPorEmpleado.Apellidos}" : null));
        
        CreateMap<LiquidacionDetalle, LiquidacionDetalleDto>();
        CreateMap<LiquidacionDiscrepancia, LiquidacionDiscrepanciaDto>()
            .ForMember(dest => dest.ResueltoPorEmpleadoNombre, opt => opt.MapFrom(src => 
                src.ResueltoPorEmpleado != null ? $"{src.ResueltoPorEmpleado.Nombres} {src.ResueltoPorEmpleado.Apellidos}" : null));
        
        // TarjetaRFID mappings
        CreateMap<TarjetaRFID, TarjetaRfidDto>()
            .ForMember(dest => dest.ClienteNombre, opt => opt.MapFrom(src => 
                src.Cliente != null ? $"{src.Cliente.Nombres} {src.Cliente.Apellidos}" : null))
            .ForMember(dest => dest.ClienteEmail, opt => opt.MapFrom(src => src.Cliente != null ? src.Cliente.Email : null))
            .ForMember(dest => dest.ClienteTelefono, opt => opt.MapFrom(src => src.Cliente != null ? src.Cliente.Telefono : null))
            .ForMember(dest => dest.ClienteTipoDocumento, opt => opt.MapFrom(src => src.Cliente != null ? src.Cliente.TipoDocumento : null))
            .ForMember(dest => dest.ClienteNumeroDocumento, opt => opt.MapFrom(src => src.Cliente != null ? src.Cliente.NumeroDocumento : null));
        
        CreateMap<CreateTarjetaRfidDto, TarjetaRFID>()
            .ForMember(dest => dest.FechaEmision, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => "Activa"))
            .ForMember(dest => dest.Saldo, opt => opt.MapFrom(src => src.SaldoInicial));
    }
}

// DTOs adicionales
public class EstacionDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Ubicacion { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
}

public class CarrilDto
{
    public int Id { get; set; }
    public int EstacionId { get; set; }
    public string Numero { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string? EstacionNombre { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaActualizacion { get; set; }
}

public class TipoVehiculoDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public int NumeroEjes { get; set; }
    public decimal TarifaBase { get; set; }
    public string? Categoria { get; set; }
    public bool EsActivo { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaActualizacion { get; set; }
}

public class TipoPagoDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public bool RequiereEfectivo { get; set; }
    public bool RequiereTarjeta { get; set; }
    public bool RequiereTag { get; set; }
    public bool RequiereAutorizacion { get; set; }
    public decimal? LimiteCredito { get; set; }
    public bool EsActivo { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaActualizacion { get; set; }
}

public class ClienteDto
{
    public int Id { get; set; }
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Telefono { get; set; }
}

public class EmpleadoDto
{
    public int Id { get; set; }
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string NumeroDocumento { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string Cargo { get; set; } = string.Empty;
    public int? EstacionId { get; set; }
    public string? EstacionNombre { get; set; }
    public DateTime FechaContratacion { get; set; }
    public bool EsActivo { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaActualizacion { get; set; }
}

public class CreateCarrilDto
{
    public int EstacionId { get; set; }
    public string Numero { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string Estado { get; set; } = "Activo";
}

public class UpdateCarrilDto
{
    public int Id { get; set; }
    public int EstacionId { get; set; }
    public string Numero { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
}

public class TarifaDto
{
    public int Id { get; set; }
    public int TipoVehiculoId { get; set; }
    public string? TipoVehiculoNombre { get; set; }
    public int? EstacionId { get; set; }
    public string? EstacionNombre { get; set; }
    public decimal Monto { get; set; }
    public DateTime FechaVigenciaInicio { get; set; }
    public DateTime? FechaVigenciaFin { get; set; }
    public bool EsVigente { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaActualizacion { get; set; }
}

public class TurnoDto
{
    public int Id { get; set; }
    public int EmpleadoId { get; set; }
    public string? EmpleadoNombre { get; set; }
    public int EstacionId { get; set; }
    public string? EstacionNombre { get; set; }
    public int? CarrilId { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public decimal MontoInicialCaja { get; set; }
    public decimal? MontoFinalCaja { get; set; }
    public string Estado { get; set; } = string.Empty;
    public TimeSpan? DuracionTurno => FechaFin.HasValue ? FechaFin - FechaInicio : null;
    public decimal? DiferenciaCaja => MontoFinalCaja.HasValue ? MontoFinalCaja - MontoInicialCaja : null;
    public decimal? VentasEfectivo { get; set; }
    public decimal? EfectivoContado { get; set; }
    public decimal? VentasPrepago { get; set; }
    public int? CantidadExentos { get; set; }
    public DateTime FechaCreacion { get; set; }
}

public class UsuarioDto
{
    public int Id { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public bool EsActivo { get; set; }
    public DateTime? UltimoAcceso { get; set; }
    public int? EmpleadoId { get; set; }
    public string? EmpleadoNombre { get; set; }
    public int? EstacionId { get; set; }
    public string? EstacionNombre { get; set; }
    public string? Permisos { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaActualizacion { get; set; }
}
