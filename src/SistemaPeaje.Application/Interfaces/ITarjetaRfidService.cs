using SistemaPeaje.Application.DTOs;
using SistemaPeaje.Core.Entities;

namespace SistemaPeaje.Application.Interfaces;

/// <summary>
/// Interfaz para el servicio de gestión de tarjetas RFID
/// </summary>
public interface ITarjetaRfidService
{
    // Operaciones CRUD básicas
    Task<TarjetaRfidDto?> ObtenerTarjetaAsync(int id);
    Task<TarjetaRfidDto?> ObtenerTarjetaPorTagAsync(string numeroTag);
    Task<IEnumerable<TarjetaRfidDto>> ObtenerTarjetasAsync(BusquedaTarjetaDto filtros);
    Task<TarjetaRfidDto> CrearTarjetaAsync(CreateTarjetaRfidDto createDto);
    Task<TarjetaRfidDto> ActualizarTarjetaAsync(int id, UpdateTarjetaRfidDto updateDto);
    Task<bool> EliminarTarjetaAsync(int id);

    // Operaciones de saldo
    Task<decimal> ObtenerSaldoAsync(string numeroTag);
    Task<TarjetaRfidDto> RecargarTarjetaAsync(int id, RecargaTarjetaDto recargaDto);
    Task<bool> DebitarSaldoAsync(string numeroTag, decimal monto, int transaccionId);
    Task<bool> ValidarSaldoSuficienteAsync(string numeroTag, decimal monto);

    // Operaciones de estado
    Task<TarjetaRfidDto> BloquearTarjetaAsync(int id, int empleadoId, string? motivo = null);
    Task<TarjetaRfidDto> ActivarTarjetaAsync(int id, int empleadoId);
    Task<TarjetaRfidDto> RenovarTarjetaAsync(int id, DateTime nuevaFechaVencimiento);
    Task<IEnumerable<TarjetaRfidDto>> ObtenerTarjetasPorVencerAsync(int diasAnticipacion = 30);

    // Validaciones
    Task<ValidacionRfidDto> ValidarTarjetaAsync(string numeroTag);
    Task<bool> ExisteTarjetaAsync(string numeroTag);
    Task<bool> EsTarjetaValidaParaTransaccionAsync(string numeroTag, decimal monto);

    // Historial y reportes
    Task<IEnumerable<HistorialTarjetaDto>> ObtenerHistorialTarjetaAsync(int tarjetaId, DateTime? fechaDesde = null, DateTime? fechaHasta = null);
    Task<ReporteTarjetaRfidDto> GenerarReporteTarjetasAsync(DateTime fechaDesde, DateTime fechaHasta);
    Task<LiquidacionRfidDto> GenerarLiquidacionRfidAsync(DateTime fechaDesde, DateTime fechaHasta, int? estacionId = null);

    // Integración con transacciones
    Task<bool> ProcesarTransaccionRfidAsync(string numeroTag, decimal monto, int transaccionId);
    Task<IEnumerable<TarjetaRfidDto>> ObtenerTarjetasPorClienteAsync(int clienteId);
    Task<decimal> ObtenerMontoTotalTransaccionesAsync(int tarjetaId, DateTime? fechaDesde = null, DateTime? fechaHasta = null);

    // Operaciones administrativas
    Task<bool> MigrarTarjetaAsync(int tarjetaId, int nuevoClienteId);
    Task<TarjetaRfidDto> DuplicarTarjetaAsync(int tarjetaId, string nuevoNumeroTag);
    Task<bool> ConsolidarTarjetasAsync(int tarjetaOrigenId, int tarjetaDestinoId);
    
    // Operaciones de auditoría
    Task<IEnumerable<MovimientoRfidDto>> ObtenerMovimientosAsync(int? tarjetaId = null, DateTime? fechaDesde = null, DateTime? fechaHasta = null);
    Task RegistrarMovimientoAsync(int tarjetaId, string tipoMovimiento, decimal monto, decimal saldoAnterior, decimal saldoNuevo, string? descripcion = null, int? empleadoId = null);
}
