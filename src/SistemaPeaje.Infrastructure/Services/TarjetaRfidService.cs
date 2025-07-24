using Microsoft.Extensions.Logging;
using SistemaPeaje.Application.DTOs;
using SistemaPeaje.Application.Interfaces;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;

namespace SistemaPeaje.Infrastructure.Services;

/// <summary>
/// Implementación del servicio de gestión de tarjetas RFID
/// </summary>
public class TarjetaRfidService : ITarjetaRfidService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TarjetaRfidService> _logger;

    public TarjetaRfidService(IUnitOfWork unitOfWork, ILogger<TarjetaRfidService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    #region Operaciones CRUD básicas

    public async Task<TarjetaRfidDto?> ObtenerTarjetaAsync(int id)
    {
        var tarjeta = await _unitOfWork.Repository<TarjetaRFID>().GetByIdAsync(id);
        if (tarjeta == null) return null;

        return await MapearTarjetaCompleta(tarjeta);
    }

    public async Task<TarjetaRfidDto?> ObtenerTarjetaPorTagAsync(string numeroTag)
    {
        var tarjetas = await _unitOfWork.Repository<TarjetaRFID>()
            .GetAsync(t => t.NumeroTag == numeroTag);
        
        var tarjeta = tarjetas.FirstOrDefault();
        if (tarjeta == null) return null;

        return await MapearTarjetaCompleta(tarjeta);
    }

    public async Task<IEnumerable<TarjetaRfidDto>> ObtenerTarjetasAsync(BusquedaTarjetaDto filtros)
    {
        var tarjetas = await _unitOfWork.Repository<TarjetaRFID>()
            .GetAsync(t => 
                (string.IsNullOrEmpty(filtros.NumeroTag) || t.NumeroTag.Contains(filtros.NumeroTag)) &&
                (!filtros.ClienteId.HasValue || t.ClienteId == filtros.ClienteId.Value) &&
                (string.IsNullOrEmpty(filtros.Estado) || t.Estado == filtros.Estado) &&
                (!filtros.SaldoMinimo.HasValue || t.Saldo >= filtros.SaldoMinimo.Value) &&
                (!filtros.SaldoMaximo.HasValue || t.Saldo <= filtros.SaldoMaximo.Value) &&
                (!filtros.FechaEmisionDesde.HasValue || t.FechaEmision >= filtros.FechaEmisionDesde.Value) &&
                (!filtros.FechaEmisionHasta.HasValue || t.FechaEmision <= filtros.FechaEmisionHasta.Value));

        var result = new List<TarjetaRfidDto>();
        foreach (var tarjeta in tarjetas)
        {
            result.Add(await MapearTarjetaCompleta(tarjeta));
        }

        return result
            .Skip((filtros.PageNumber - 1) * filtros.PageSize)
            .Take(filtros.PageSize);
    }

    public async Task<TarjetaRfidDto> CrearTarjetaAsync(CreateTarjetaRfidDto createDto)
    {
        // Validar que no exista otra tarjeta con el mismo número
        var existente = await _unitOfWork.Repository<TarjetaRFID>()
            .GetAsync(t => t.NumeroTag == createDto.NumeroTag);
        
        if (existente.Any())
            throw new InvalidOperationException($"Ya existe una tarjeta con el número {createDto.NumeroTag}");

        var tarjeta = new TarjetaRFID
        {
            NumeroTag = createDto.NumeroTag,
            ClienteId = createDto.ClienteId,
            Saldo = createDto.SaldoInicial,
            FechaEmision = DateTime.UtcNow,
            FechaVencimiento = createDto.FechaVencimiento,
            Estado = "Activa"
        };

        await _unitOfWork.Repository<TarjetaRFID>().AddAsync(tarjeta);
        await _unitOfWork.SaveChangesAsync();

        // Registrar movimiento inicial si hay saldo
        if (createDto.SaldoInicial > 0)
        {
            await RegistrarMovimientoAsync(tarjeta.Id, "RECARGA_INICIAL", createDto.SaldoInicial, 0, createDto.SaldoInicial, "Carga inicial de saldo");
        }

        _logger.LogInformation("Tarjeta RFID creada: {NumeroTag} para cliente {ClienteId}", 
            createDto.NumeroTag, createDto.ClienteId);

        return await MapearTarjetaCompleta(tarjeta);
    }

    public async Task<TarjetaRfidDto> ActualizarTarjetaAsync(int id, UpdateTarjetaRfidDto updateDto)
    {
        var tarjeta = await _unitOfWork.Repository<TarjetaRFID>().GetByIdAsync(id);
        if (tarjeta == null)
            throw new ArgumentException($"Tarjeta con ID {id} no encontrada");

        tarjeta.ClienteId = updateDto.ClienteId;
        tarjeta.FechaVencimiento = updateDto.FechaVencimiento;
        tarjeta.Estado = updateDto.Estado;
        tarjeta.FechaActualizacion = DateTime.UtcNow;

        await _unitOfWork.Repository<TarjetaRFID>().UpdateAsync(tarjeta);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Tarjeta RFID actualizada: {NumeroTag}", tarjeta.NumeroTag);

        return await MapearTarjetaCompleta(tarjeta);
    }

    public async Task<bool> EliminarTarjetaAsync(int id)
    {
        var tarjeta = await _unitOfWork.Repository<TarjetaRFID>().GetByIdAsync(id);
        if (tarjeta == null) return false;

        // Verificar que no tenga saldo
        if (tarjeta.Saldo > 0)
            throw new InvalidOperationException("No se puede eliminar una tarjeta con saldo positivo");

        await _unitOfWork.Repository<TarjetaRFID>().DeleteAsync(tarjeta);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Tarjeta RFID eliminada: {NumeroTag}", tarjeta.NumeroTag);
        return true;
    }

    #endregion

    #region Operaciones de saldo

    public async Task<decimal> ObtenerSaldoAsync(string numeroTag)
    {
        var tarjeta = await ObtenerTarjetaPorTagAsync(numeroTag);
        return tarjeta?.Saldo ?? 0;
    }

    public async Task<TarjetaRfidDto> RecargarTarjetaAsync(int id, RecargaTarjetaDto recargaDto)
    {
        var tarjeta = await _unitOfWork.Repository<TarjetaRFID>().GetByIdAsync(id);
        if (tarjeta == null)
            throw new ArgumentException($"Tarjeta con ID {id} no encontrada");

        if (tarjeta.Estado != "Activa")
            throw new InvalidOperationException("La tarjeta no está activa");

        if (recargaDto.Monto <= 0)
            throw new ArgumentException("El monto de recarga debe ser positivo");

        var saldoAnterior = tarjeta.Saldo;
        tarjeta.Saldo += recargaDto.Monto;
        tarjeta.FechaActualizacion = DateTime.UtcNow;

        await _unitOfWork.Repository<TarjetaRFID>().UpdateAsync(tarjeta);
        await _unitOfWork.SaveChangesAsync();

        // Registrar movimiento de recarga
        await RegistrarMovimientoAsync(tarjeta.Id, "RECARGA", recargaDto.Monto, saldoAnterior, tarjeta.Saldo, 
            $"Recarga - {recargaDto.MetodoPago} - {recargaDto.NumeroTransaccion}", recargaDto.EmpleadoId);

        _logger.LogInformation("Tarjeta RFID recargada: {NumeroTag} - Monto: {Monto}", 
            tarjeta.NumeroTag, recargaDto.Monto);

        return await MapearTarjetaCompleta(tarjeta);
    }

    public async Task<bool> DebitarSaldoAsync(string numeroTag, decimal monto, int transaccionId)
    {
        var tarjetas = await _unitOfWork.Repository<TarjetaRFID>()
            .GetAsync(t => t.NumeroTag == numeroTag);
        
        var tarjeta = tarjetas.FirstOrDefault();
        if (tarjeta == null) return false;

        if (tarjeta.Estado != "Activa") return false;
        if (tarjeta.Saldo < monto) return false;

        var saldoAnterior = tarjeta.Saldo;
        tarjeta.Saldo -= monto;
        tarjeta.FechaActualizacion = DateTime.UtcNow;

        await _unitOfWork.Repository<TarjetaRFID>().UpdateAsync(tarjeta);
        await _unitOfWork.SaveChangesAsync();

        // Registrar movimiento de débito
        await RegistrarMovimientoAsync(tarjeta.Id, "TRANSACCION", -monto, saldoAnterior, tarjeta.Saldo, 
            $"Transacción ID: {transaccionId}");

        _logger.LogInformation("Saldo debitado de tarjeta RFID: {NumeroTag} - Monto: {Monto}", 
            numeroTag, monto);

        return true;
    }

    public async Task<bool> ValidarSaldoSuficienteAsync(string numeroTag, decimal monto)
    {
        var saldo = await ObtenerSaldoAsync(numeroTag);
        return saldo >= monto;
    }

    #endregion

    #region Operaciones de estado

    public async Task<TarjetaRfidDto> BloquearTarjetaAsync(int id, int empleadoId, string? motivo = null)
    {
        var tarjeta = await _unitOfWork.Repository<TarjetaRFID>().GetByIdAsync(id);
        if (tarjeta == null)
            throw new ArgumentException($"Tarjeta con ID {id} no encontrada");

        tarjeta.Estado = "Bloqueada";
        tarjeta.FechaActualizacion = DateTime.UtcNow;

        await _unitOfWork.Repository<TarjetaRFID>().UpdateAsync(tarjeta);
        await _unitOfWork.SaveChangesAsync();

        // Registrar movimiento de bloqueo
        await RegistrarMovimientoAsync(tarjeta.Id, "BLOQUEO", 0, tarjeta.Saldo, tarjeta.Saldo, 
            $"Tarjeta bloqueada: {motivo}", empleadoId);

        _logger.LogInformation("Tarjeta RFID bloqueada: {NumeroTag} - Motivo: {Motivo}", 
            tarjeta.NumeroTag, motivo);

        return await MapearTarjetaCompleta(tarjeta);
    }

    public async Task<TarjetaRfidDto> ActivarTarjetaAsync(int id, int empleadoId)
    {
        var tarjeta = await _unitOfWork.Repository<TarjetaRFID>().GetByIdAsync(id);
        if (tarjeta == null)
            throw new ArgumentException($"Tarjeta con ID {id} no encontrada");

        tarjeta.Estado = "Activa";
        tarjeta.FechaActualizacion = DateTime.UtcNow;

        await _unitOfWork.Repository<TarjetaRFID>().UpdateAsync(tarjeta);
        await _unitOfWork.SaveChangesAsync();

        // Registrar movimiento de activación
        await RegistrarMovimientoAsync(tarjeta.Id, "ACTIVACION", 0, tarjeta.Saldo, tarjeta.Saldo, 
            "Tarjeta activada", empleadoId);

        _logger.LogInformation("Tarjeta RFID activada: {NumeroTag}", tarjeta.NumeroTag);

        return await MapearTarjetaCompleta(tarjeta);
    }

    public async Task<TarjetaRfidDto> RenovarTarjetaAsync(int id, DateTime nuevaFechaVencimiento)
    {
        var tarjeta = await _unitOfWork.Repository<TarjetaRFID>().GetByIdAsync(id);
        if (tarjeta == null)
            throw new ArgumentException($"Tarjeta con ID {id} no encontrada");

        var fechaAnterior = tarjeta.FechaVencimiento;
        tarjeta.FechaVencimiento = nuevaFechaVencimiento;
        tarjeta.FechaActualizacion = DateTime.UtcNow;

        await _unitOfWork.Repository<TarjetaRFID>().UpdateAsync(tarjeta);
        await _unitOfWork.SaveChangesAsync();

        // Registrar movimiento de renovación
        await RegistrarMovimientoAsync(tarjeta.Id, "RENOVACION", 0, tarjeta.Saldo, tarjeta.Saldo, 
            $"Renovación - Anterior: {fechaAnterior:dd/MM/yyyy} - Nueva: {nuevaFechaVencimiento:dd/MM/yyyy}");

        _logger.LogInformation("Tarjeta RFID renovada: {NumeroTag} - Nueva fecha: {FechaVencimiento}", 
            tarjeta.NumeroTag, nuevaFechaVencimiento);

        return await MapearTarjetaCompleta(tarjeta);
    }

    public async Task<IEnumerable<TarjetaRfidDto>> ObtenerTarjetasPorVencerAsync(int diasAnticipacion = 30)
    {
        var fechaLimite = DateTime.UtcNow.AddDays(diasAnticipacion);
        var tarjetas = await _unitOfWork.Repository<TarjetaRFID>()
            .GetAsync(t => t.FechaVencimiento.HasValue && 
                          t.FechaVencimiento.Value <= fechaLimite &&
                          t.Estado == "Activa");

        var result = new List<TarjetaRfidDto>();
        foreach (var tarjeta in tarjetas)
        {
            result.Add(await MapearTarjetaCompleta(tarjeta));
        }

        return result;
    }

    #endregion

    #region Validaciones

    public async Task<ValidacionRfidDto> ValidarTarjetaAsync(string numeroTag)
    {
        var tarjeta = await ObtenerTarjetaPorTagAsync(numeroTag);
        
        if (tarjeta == null)
        {
            return new ValidacionRfidDto
            {
                NumeroTag = numeroTag,
                EsValida = false,
                MensajeError = "Tarjeta no encontrada"
            };
        }

        var validacion = new ValidacionRfidDto
        {
            NumeroTag = numeroTag,
            SaldoActual = tarjeta.Saldo,
            EstadoTarjeta = tarjeta.Estado,
            FechaVencimiento = tarjeta.FechaVencimiento
        };

        if (tarjeta.Estado != "Activa")
        {
            validacion.EsValida = false;
            validacion.MensajeError = $"Tarjeta {tarjeta.Estado.ToLower()}";
            return validacion;
        }

        if (tarjeta.FechaVencimiento.HasValue && tarjeta.FechaVencimiento.Value < DateTime.UtcNow)
        {
            validacion.EsValida = false;
            validacion.MensajeError = "Tarjeta vencida";
            return validacion;
        }

        validacion.EsValida = true;
        return validacion;
    }

    public async Task<bool> ExisteTarjetaAsync(string numeroTag)
    {
        var tarjetas = await _unitOfWork.Repository<TarjetaRFID>()
            .GetAsync(t => t.NumeroTag == numeroTag);
        
        return tarjetas.Any();
    }

    public async Task<bool> EsTarjetaValidaParaTransaccionAsync(string numeroTag, decimal monto)
    {
        var validacion = await ValidarTarjetaAsync(numeroTag);
        return validacion.EsValida && validacion.SaldoActual >= monto;
    }

    #endregion

    #region Historial y reportes

    public async Task<IEnumerable<HistorialTarjetaDto>> ObtenerHistorialTarjetaAsync(int tarjetaId, DateTime? fechaDesde = null, DateTime? fechaHasta = null)
    {
        var transacciones = await _unitOfWork.Repository<Transaccion>()
            .GetAsync(t => t.TagRFID != null && 
                          (!fechaDesde.HasValue || t.FechaTransaccion >= fechaDesde.Value) &&
                          (!fechaHasta.HasValue || t.FechaTransaccion <= fechaHasta.Value));

        var tarjeta = await _unitOfWork.Repository<TarjetaRFID>().GetByIdAsync(tarjetaId);
        if (tarjeta == null) return new List<HistorialTarjetaDto>();

        var historial = transacciones
            .Where(t => t.TagRFID == tarjeta.NumeroTag)
            .Select(t => new HistorialTarjetaDto
            {
                TransaccionId = t.Id,
                FechaTransaccion = t.FechaTransaccion,
                EstacionNombre = t.Estacion?.Nombre ?? "Desconocida",
                CarrilNumero = int.TryParse(t.Carril?.Numero, out int carrilNum) ? carrilNum : 0,
                TipoVehiculo = t.TipoVehiculo?.Nombre ?? "Desconocido",
                MontoTransaccion = t.Monto,
                PlacaVehiculo = t.PlacaVehiculo,
                EmpleadoNombre = t.Empleado != null ? $"{t.Empleado.Nombres} {t.Empleado.Apellidos}" : null
            })
            .OrderByDescending(h => h.FechaTransaccion);

        return historial;
    }

    public async Task<ReporteTarjetaRfidDto> GenerarReporteTarjetasAsync(DateTime fechaDesde, DateTime fechaHasta)
    {
        var tarjetas = await _unitOfWork.Repository<TarjetaRFID>().GetAllAsync();
        var transacciones = await _unitOfWork.Repository<Transaccion>()
            .GetAsync(t => t.TagRFID != null &&
                          t.FechaTransaccion >= fechaDesde &&
                          t.FechaTransaccion <= fechaHasta);

        var reporte = new ReporteTarjetaRfidDto
        {
            TotalTarjetas = tarjetas.Count(),
            TarjetasActivas = tarjetas.Count(t => t.Estado == "Activa"),
            TarjetasBloqueadas = tarjetas.Count(t => t.Estado == "Bloqueada"),
            TarjetasVencidas = tarjetas.Count(t => t.FechaVencimiento.HasValue && t.FechaVencimiento.Value < DateTime.UtcNow),
            SaldoTotalSistema = tarjetas.Sum(t => t.Saldo),
            MontoTotalTransacciones = transacciones.Sum(t => t.Monto),
            TransaccionesTotales = transacciones.Count()
        };

        return reporte;
    }

    public async Task<LiquidacionRfidDto> GenerarLiquidacionRfidAsync(DateTime fechaDesde, DateTime fechaHasta, int? estacionId = null)
    {
        var transacciones = await _unitOfWork.Repository<Transaccion>()
            .GetAsync(t => t.TagRFID != null &&
                          t.FechaTransaccion >= fechaDesde &&
                          t.FechaTransaccion <= fechaHasta &&
                          (!estacionId.HasValue || t.EstacionId == estacionId.Value));

        var liquidacion = new LiquidacionRfidDto
        {
            FechaInicio = fechaDesde,
            FechaFin = fechaHasta,
            TotalTransaccionesRfid = transacciones.Count(),
            MontoTotalRfid = transacciones.Sum(t => t.Monto),
            TarjetasUtilizadas = transacciones.Select(t => t.TagRFID).Distinct().Count()
        };

        return liquidacion;
    }

    #endregion

    #region Integración con transacciones

    public async Task<bool> ProcesarTransaccionRfidAsync(string numeroTag, decimal monto, int transaccionId)
    {
        if (!await EsTarjetaValidaParaTransaccionAsync(numeroTag, monto))
            return false;

        return await DebitarSaldoAsync(numeroTag, monto, transaccionId);
    }

    public async Task<IEnumerable<TarjetaRfidDto>> ObtenerTarjetasPorClienteAsync(int clienteId)
    {
        var tarjetas = await _unitOfWork.Repository<TarjetaRFID>()
            .GetAsync(t => t.ClienteId == clienteId);

        var result = new List<TarjetaRfidDto>();
        foreach (var tarjeta in tarjetas)
        {
            result.Add(await MapearTarjetaCompleta(tarjeta));
        }

        return result;
    }

    public async Task<decimal> ObtenerMontoTotalTransaccionesAsync(int tarjetaId, DateTime? fechaDesde = null, DateTime? fechaHasta = null)
    {
        var tarjeta = await _unitOfWork.Repository<TarjetaRFID>().GetByIdAsync(tarjetaId);
        if (tarjeta == null) return 0;

        var transacciones = await _unitOfWork.Repository<Transaccion>()
            .GetAsync(t => t.TagRFID == tarjeta.NumeroTag &&
                          (!fechaDesde.HasValue || t.FechaTransaccion >= fechaDesde.Value) &&
                          (!fechaHasta.HasValue || t.FechaTransaccion <= fechaHasta.Value));

        return transacciones.Sum(t => t.Monto);
    }

    #endregion

    #region Operaciones administrativas

    public async Task<bool> MigrarTarjetaAsync(int tarjetaId, int nuevoClienteId)
    {
        var tarjeta = await _unitOfWork.Repository<TarjetaRFID>().GetByIdAsync(tarjetaId);
        if (tarjeta == null) return false;

        var clienteAnterior = tarjeta.ClienteId;
        tarjeta.ClienteId = nuevoClienteId;
        tarjeta.FechaActualizacion = DateTime.UtcNow;

        await _unitOfWork.Repository<TarjetaRFID>().UpdateAsync(tarjeta);
        await _unitOfWork.SaveChangesAsync();

        // Registrar movimiento de migración
        await RegistrarMovimientoAsync(tarjeta.Id, "MIGRACION", 0, tarjeta.Saldo, tarjeta.Saldo, 
            $"Migración de cliente {clienteAnterior} a {nuevoClienteId}");

        _logger.LogInformation("Tarjeta RFID migrada: {NumeroTag} - Cliente anterior: {ClienteAnterior} - Cliente nuevo: {ClienteNuevo}", 
            tarjeta.NumeroTag, clienteAnterior, nuevoClienteId);

        return true;
    }

    public async Task<TarjetaRfidDto> DuplicarTarjetaAsync(int tarjetaId, string nuevoNumeroTag)
    {
        var tarjetaOriginal = await _unitOfWork.Repository<TarjetaRFID>().GetByIdAsync(tarjetaId);
        if (tarjetaOriginal == null)
            throw new ArgumentException($"Tarjeta con ID {tarjetaId} no encontrada");

        var nuevaTarjeta = new TarjetaRFID
        {
            NumeroTag = nuevoNumeroTag,
            ClienteId = tarjetaOriginal.ClienteId,
            Saldo = 0, // La nueva tarjeta inicia sin saldo
            FechaEmision = DateTime.UtcNow,
            FechaVencimiento = tarjetaOriginal.FechaVencimiento,
            Estado = "Activa"
        };

        await _unitOfWork.Repository<TarjetaRFID>().AddAsync(nuevaTarjeta);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Tarjeta RFID duplicada: {NumeroTagOriginal} -> {NumeroTagNuevo}", 
            tarjetaOriginal.NumeroTag, nuevoNumeroTag);

        return await MapearTarjetaCompleta(nuevaTarjeta);
    }

    public async Task<bool> ConsolidarTarjetasAsync(int tarjetaOrigenId, int tarjetaDestinoId)
    {
        var tarjetaOrigen = await _unitOfWork.Repository<TarjetaRFID>().GetByIdAsync(tarjetaOrigenId);
        var tarjetaDestino = await _unitOfWork.Repository<TarjetaRFID>().GetByIdAsync(tarjetaDestinoId);

        if (tarjetaOrigen == null || tarjetaDestino == null) return false;

        // Transferir saldo
        tarjetaDestino.Saldo += tarjetaOrigen.Saldo;
        tarjetaOrigen.Saldo = 0;
        tarjetaOrigen.Estado = "Consolidada";

        await _unitOfWork.Repository<TarjetaRFID>().UpdateAsync(tarjetaOrigen);
        await _unitOfWork.Repository<TarjetaRFID>().UpdateAsync(tarjetaDestino);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Tarjetas RFID consolidadas: {TagOrigen} -> {TagDestino}", 
            tarjetaOrigen.NumeroTag, tarjetaDestino.NumeroTag);

        return true;
    }

    #endregion

    #region Operaciones de auditoría

    public async Task<IEnumerable<MovimientoRfidDto>> ObtenerMovimientosAsync(int? tarjetaId = null, DateTime? fechaDesde = null, DateTime? fechaHasta = null)
    {
        // Esta implementación básica devuelve una lista vacía
        // En una implementación completa, se consultaría una tabla de movimientos
        return new List<MovimientoRfidDto>();
    }

    public async Task RegistrarMovimientoAsync(int tarjetaId, string tipoMovimiento, decimal monto, decimal saldoAnterior, decimal saldoNuevo, string? descripcion = null, int? empleadoId = null)
    {
        // En una implementación completa, se guardaría en una tabla de movimientos
        _logger.LogInformation("Movimiento RFID registrado - Tarjeta: {TarjetaId}, Tipo: {TipoMovimiento}, Monto: {Monto}", 
            tarjetaId, tipoMovimiento, monto);
    }

    #endregion

    #region Métodos privados

    private async Task<TarjetaRfidDto> MapearTarjetaCompleta(TarjetaRFID tarjeta)
    {
        var cliente = await _unitOfWork.Repository<Cliente>().GetByIdAsync(tarjeta.ClienteId);
        
        var transacciones = await _unitOfWork.Repository<Transaccion>()
            .GetAsync(t => t.TagRFID == tarjeta.NumeroTag);

        return new TarjetaRfidDto
        {
            Id = tarjeta.Id,
            NumeroTag = tarjeta.NumeroTag,
            ClienteId = tarjeta.ClienteId,
            ClienteNombre = cliente != null ? $"{cliente.Nombres} {cliente.Apellidos}" : null,
            Saldo = tarjeta.Saldo,
            FechaEmision = tarjeta.FechaEmision,
            FechaVencimiento = tarjeta.FechaVencimiento,
            Estado = tarjeta.Estado,
            FechaCreacion = tarjeta.FechaCreacion,
            FechaActualizacion = tarjeta.FechaActualizacion,
            TotalTransacciones = transacciones.Count(),
            MontoTotalTransacciones = transacciones.Sum(t => t.Monto),
            UltimaTransaccion = transacciones.OrderByDescending(t => t.FechaTransaccion).FirstOrDefault()?.FechaTransaccion,
            ClienteEmail = cliente?.Email,
            ClienteTelefono = cliente?.Telefono,
            ClienteTipoDocumento = cliente?.TipoDocumento,
            ClienteNumeroDocumento = cliente?.NumeroDocumento
        };
    }

    #endregion
}
