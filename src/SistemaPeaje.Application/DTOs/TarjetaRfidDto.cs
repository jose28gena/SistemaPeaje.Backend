using SistemaPeaje.Core.Entities;

namespace SistemaPeaje.Application.DTOs;

/// <summary>
/// DTO para mostrar información completa de una tarjeta RFID
/// </summary>
public class TarjetaRfidDto
{
    public int Id { get; set; }
    public string NumeroTag { get; set; } = string.Empty;
    public int ClienteId { get; set; }
    public string? ClienteNombre { get; set; }
    public decimal Saldo { get; set; }
    public DateTime FechaEmision { get; set; }
    public DateTime? FechaVencimiento { get; set; }
    public string Estado { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaActualizacion { get; set; }
    
    // Estadísticas de uso
    public int TotalTransacciones { get; set; }
    public decimal MontoTotalTransacciones { get; set; }
    public DateTime? UltimaTransaccion { get; set; }
    
    // Información del cliente
    public string? ClienteEmail { get; set; }
    public string? ClienteTelefono { get; set; }
    public string? ClienteTipoDocumento { get; set; }
    public string? ClienteNumeroDocumento { get; set; }
}

/// <summary>
/// DTO para crear una nueva tarjeta RFID
/// </summary>
public class CreateTarjetaRfidDto
{
    public string NumeroTag { get; set; } = string.Empty;
    public int ClienteId { get; set; }
    public decimal SaldoInicial { get; set; } = 0;
    public DateTime? FechaVencimiento { get; set; }
    public string? Observaciones { get; set; }
}

/// <summary>
/// DTO para actualizar una tarjeta RFID
/// </summary>
public class UpdateTarjetaRfidDto
{
    public int ClienteId { get; set; }
    public DateTime? FechaVencimiento { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string? Observaciones { get; set; }
}

/// <summary>
/// DTO para operaciones de recarga
/// </summary>
public class RecargaTarjetaDto
{
    public decimal Monto { get; set; }
    public int EmpleadoId { get; set; }
    public string? MetodoPago { get; set; }
    public string? NumeroTransaccion { get; set; }
    public string? Observaciones { get; set; }
}

/// <summary>
/// DTO para el historial de transacciones de una tarjeta
/// </summary>
public class HistorialTarjetaDto
{
    public int TransaccionId { get; set; }
    public DateTime FechaTransaccion { get; set; }
    public string EstacionNombre { get; set; } = string.Empty;
    public int CarrilNumero { get; set; }
    public string TipoVehiculo { get; set; } = string.Empty;
    public decimal MontoTransaccion { get; set; }
    public decimal SaldoAnterior { get; set; }
    public decimal SaldoNuevo { get; set; }
    public string? PlacaVehiculo { get; set; }
    public string? EmpleadoNombre { get; set; }
}

/// <summary>
/// DTO para reportes de tarjetas RFID
/// </summary>
public class ReporteTarjetaRfidDto
{
    public int TotalTarjetas { get; set; }
    public int TarjetasActivas { get; set; }
    public int TarjetasBloqueadas { get; set; }
    public int TarjetasVencidas { get; set; }
    public decimal SaldoTotalSistema { get; set; }
    public decimal MontoTotalTransacciones { get; set; }
    public int TransaccionesTotales { get; set; }
    public List<TarjetaTopDto> TarjetasMasUsadas { get; set; } = new();
    public List<ClienteTopDto> ClientesTopSaldo { get; set; } = new();
}

/// <summary>
/// DTO para las tarjetas más usadas
/// </summary>
public class TarjetaTopDto
{
    public int Id { get; set; }
    public string NumeroTag { get; set; } = string.Empty;
    public string ClienteNombre { get; set; } = string.Empty;
    public int TotalTransacciones { get; set; }
    public decimal MontoTotal { get; set; }
}

/// <summary>
/// DTO para clientes con mayor saldo
/// </summary>
public class ClienteTopDto
{
    public int ClienteId { get; set; }
    public string ClienteNombre { get; set; } = string.Empty;
    public int TotalTarjetas { get; set; }
    public decimal SaldoTotal { get; set; }
}

/// <summary>
/// DTO para el resumen de liquidación por tarjetas RFID
/// </summary>
public class LiquidacionRfidDto
{
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public int TotalTransaccionesRfid { get; set; }
    public decimal MontoTotalRfid { get; set; }
    public decimal MontoRecargasRfid { get; set; }
    public decimal SaldoInicialPeriodo { get; set; }
    public decimal SaldoFinalPeriodo { get; set; }
    public int TarjetasUtilizadas { get; set; }
    public List<MovimientoRfidDto> MovimientosDetallados { get; set; } = new();
}

/// <summary>
/// DTO para movimientos detallados de RFID
/// </summary>
public class MovimientoRfidDto
{
    public DateTime Fecha { get; set; }
    public string NumeroTag { get; set; } = string.Empty;
    public string ClienteNombre { get; set; } = string.Empty;
    public string TipoMovimiento { get; set; } = string.Empty; // Transacción, Recarga, Bloqueo, etc.
    public decimal MontoMovimiento { get; set; }
    public decimal SaldoAnterior { get; set; }
    public decimal SaldoNuevo { get; set; }
    public string? Descripcion { get; set; }
    public string? EmpleadoNombre { get; set; }
}

/// <summary>
/// DTO para validación de tarjetas RFID
/// </summary>
public class ValidacionRfidDto
{
    public string NumeroTag { get; set; } = string.Empty;
    public bool EsValida { get; set; }
    public string? MensajeError { get; set; }
    public decimal SaldoActual { get; set; }
    public string EstadoTarjeta { get; set; } = string.Empty;
    public DateTime? FechaVencimiento { get; set; }
    public ClienteDto? Cliente { get; set; }
}

/// <summary>
/// DTO para búsqueda de tarjetas
/// </summary>
public class BusquedaTarjetaDto
{
    public string? NumeroTag { get; set; }
    public int? ClienteId { get; set; }
    public string? ClienteNombre { get; set; }
    public string? Estado { get; set; }
    public decimal? SaldoMinimo { get; set; }
    public decimal? SaldoMaximo { get; set; }
    public DateTime? FechaEmisionDesde { get; set; }
    public DateTime? FechaEmisionHasta { get; set; }
    public bool? ProximasAVencer { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
