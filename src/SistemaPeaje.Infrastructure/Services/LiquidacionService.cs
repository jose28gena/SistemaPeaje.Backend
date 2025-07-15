using Microsoft.Extensions.Logging;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;

namespace SistemaPeaje.Infrastructure.Services;

/// <summary>
/// Servicio de liquidaciones que maneja los tres tipos principales:
/// - Liquidación de Cajero Receptor
/// - Liquidación de Turno
/// - Liquidación de Día
/// </summary>
public class LiquidacionService : ILiquidacionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<LiquidacionService> _logger;

    public LiquidacionService(IUnitOfWork unitOfWork, ILogger<LiquidacionService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Genera una liquidación de cajero receptor
    /// </summary>
    public async Task<Liquidacion> GenerarLiquidacionCajeroAsync(int empleadoId, int estacionId, DateTime fechaInicio, DateTime fechaFin)
    {
        _logger.LogInformation("Generando liquidación de cajero para empleado {EmpleadoId} en estación {EstacionId}", 
            empleadoId, estacionId);

        // Obtener transacciones del empleado en el período
        var transacciones = await _unitOfWork.Repository<Transaccion>()
            .GetAsync(t => t.EmpleadoId == empleadoId &&
                          t.EstacionId == estacionId &&
                          t.FechaTransaccion >= fechaInicio &&
                          t.FechaTransaccion <= fechaFin);

        // Obtener turnos del empleado
        var turnos = await _unitOfWork.Repository<Turno>()
            .GetAsync(t => t.EmpleadoId == empleadoId &&
                          t.EstacionId == estacionId &&
                          t.FechaInicio >= fechaInicio &&
                          t.FechaInicio <= fechaFin);

        var liquidacion = new Liquidacion
        {
            TipoLiquidacion = TipoLiquidacion.Cajero,
            FechaInicio = fechaInicio,
            FechaFin = fechaFin,
            EstacionId = estacionId,
            EmpleadoId = empleadoId,
            MontoTotalTransacciones = transacciones.Sum(t => t.Monto),
            MontoTotalRecaudado = transacciones.Sum(t => t.Monto),
            TotalTransacciones = transacciones.Count,
            CreadoPorEmpleadoId = empleadoId,
            RequiereAprobacion = ShouldRequireApproval(transacciones, turnos)
        };

        // Calcular diferencia de caja
        liquidacion.DiferenciaCaja = CalcularDiferenciaCaja(transacciones, turnos);

        // Agregar detalles
        await AgregarDetallesLiquidacion(liquidacion, transacciones);

        // Identificar discrepancias
        await IdentificarDiscrepancias(liquidacion, transacciones, turnos);

        // Guardar en base de datos
        await _unitOfWork.Repository<Liquidacion>().AddAsync(liquidacion);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Liquidación de cajero generada exitosamente: {LiquidacionId}", liquidacion.Id);
        return liquidacion;
    }

    /// <summary>
    /// Genera una liquidación de turno
    /// </summary>
    public async Task<Liquidacion> GenerarLiquidacionTurnoAsync(int turnoId)
    {
        _logger.LogInformation("Generando liquidación de turno {TurnoId}", turnoId);

        var turno = await _unitOfWork.Repository<Turno>()
            .GetByIdAsync(turnoId);

        if (turno == null)
            throw new ArgumentException($"Turno con ID {turnoId} no encontrado");

        // Obtener transacciones del turno
        var transacciones = await _unitOfWork.Repository<Transaccion>()
            .GetAsync(t => t.EmpleadoId == turno.EmpleadoId &&
                          t.EstacionId == turno.EstacionId &&
                          t.FechaTransaccion >= turno.FechaInicio &&
                          (!turno.FechaFin.HasValue || t.FechaTransaccion <= turno.FechaFin));

        var liquidacion = new Liquidacion
        {
            TipoLiquidacion = TipoLiquidacion.Turno,
            FechaInicio = turno.FechaInicio,
            FechaFin = turno.FechaFin ?? DateTime.Now,
            EstacionId = turno.EstacionId,
            EmpleadoId = turno.EmpleadoId,
            TurnoId = turnoId,
            MontoTotalTransacciones = transacciones.Sum(t => t.Monto),
            MontoTotalRecaudado = transacciones.Sum(t => t.Monto),
            TotalTransacciones = transacciones.Count,
            CreadoPorEmpleadoId = turno.EmpleadoId,
            RequiereAprobacion = turno.Estado != "Cerrado" || 
                               (turno.MontoFinalCaja.HasValue && Math.Abs(turno.MontoFinalCaja.Value - turno.MontoInicialCaja - transacciones.Sum(t => t.Monto)) > 100)
        };

        // Calcular diferencia de caja específica del turno
        if (turno.MontoFinalCaja.HasValue)
        {
            liquidacion.DiferenciaCaja = turno.MontoFinalCaja.Value - turno.MontoInicialCaja - transacciones.Sum(t => t.Monto);
        }

        // Agregar detalles
        await AgregarDetallesLiquidacion(liquidacion, transacciones);

        // Identificar discrepancias específicas del turno
        await IdentificarDiscrepanciasTurno(liquidacion, transacciones, turno);

        // Guardar en base de datos
        await _unitOfWork.Repository<Liquidacion>().AddAsync(liquidacion);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Liquidación de turno generada exitosamente: {LiquidacionId}", liquidacion.Id);
        return liquidacion;
    }

    /// <summary>
    /// Genera una liquidación de día
    /// </summary>
    public async Task<Liquidacion> GenerarLiquidacionDiaAsync(int? estacionId, DateTime fecha)
    {
        _logger.LogInformation("Generando liquidación de día para fecha {Fecha} en estación {EstacionId}", 
            fecha.Date, estacionId);

        var fechaInicio = fecha.Date;
        var fechaFin = fecha.Date.AddDays(1).AddTicks(-1);

        // Obtener todas las transacciones del día
        var transacciones = await _unitOfWork.Repository<Transaccion>()
            .GetAsync(t => t.FechaTransaccion >= fechaInicio &&
                          t.FechaTransaccion <= fechaFin &&
                          (!estacionId.HasValue || t.EstacionId == estacionId.Value));

        // Obtener todos los turnos del día
        var turnos = await _unitOfWork.Repository<Turno>()
            .GetAsync(t => t.FechaInicio >= fechaInicio &&
                          t.FechaInicio <= fechaFin &&
                          (!estacionId.HasValue || t.EstacionId == estacionId.Value));

        var liquidacion = new Liquidacion
        {
            TipoLiquidacion = TipoLiquidacion.Dia,
            FechaInicio = fechaInicio,
            FechaFin = fechaFin,
            EstacionId = estacionId,
            MontoTotalTransacciones = transacciones.Sum(t => t.Monto),
            MontoTotalRecaudado = transacciones.Sum(t => t.Monto),
            TotalTransacciones = transacciones.Count,
            CreadoPorEmpleadoId = 1, // Usuario del sistema
            RequiereAprobacion = transacciones.Sum(t => t.Monto) > 50000 || // Monto alto
                               turnos.Any(t => t.Estado != "Cerrado") || // Turnos abiertos
                               Math.Abs(CalcularDiferenciaCaja(transacciones, turnos)) > 500 // Diferencia significativa
        };

        // Calcular diferencia de caja total del día
        liquidacion.DiferenciaCaja = CalcularDiferenciaCaja(transacciones, turnos);

        // Agregar detalles
        await AgregarDetallesLiquidacion(liquidacion, transacciones);

        // Identificar discrepancias del día
        await IdentificarDiscrepanciasDia(liquidacion, transacciones, turnos);

        // Guardar en base de datos
        await _unitOfWork.Repository<Liquidacion>().AddAsync(liquidacion);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Liquidación de día generada exitosamente: {LiquidacionId}", liquidacion.Id);
        return liquidacion;
    }

    /// <summary>
    /// Aprueba una liquidación
    /// </summary>
    public async Task<Liquidacion> AprobarLiquidacionAsync(int liquidacionId, int empleadoId, string? notas = null)
    {
        var liquidacion = await _unitOfWork.Repository<Liquidacion>().GetByIdAsync(liquidacionId);
        if (liquidacion == null)
            throw new ArgumentException($"Liquidación con ID {liquidacionId} no encontrada");

        if (liquidacion.Estado != EstadoLiquidacion.Generada && liquidacion.Estado != EstadoLiquidacion.EnRevision)
            throw new InvalidOperationException("La liquidación no puede ser aprobada en su estado actual");

        liquidacion.Estado = EstadoLiquidacion.Aprobada;
        liquidacion.FechaAprobacion = DateTime.UtcNow;
        liquidacion.AprobadoPorEmpleadoId = empleadoId;
        liquidacion.NotasAprobacion = notas;
        liquidacion.FechaUltimaActualizacion = DateTime.UtcNow;

        await _unitOfWork.Repository<Liquidacion>().UpdateAsync(liquidacion);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Liquidación {LiquidacionId} aprobada por empleado {EmpleadoId}", 
            liquidacionId, empleadoId);

        return liquidacion;
    }

    /// <summary>
    /// Rechaza una liquidación
    /// </summary>
    public async Task<Liquidacion> RechazarLiquidacionAsync(int liquidacionId, int empleadoId, string? notas = null)
    {
        var liquidacion = await _unitOfWork.Repository<Liquidacion>().GetByIdAsync(liquidacionId);
        if (liquidacion == null)
            throw new ArgumentException($"Liquidación con ID {liquidacionId} no encontrada");

        if (liquidacion.Estado != EstadoLiquidacion.Generada && liquidacion.Estado != EstadoLiquidacion.EnRevision)
            throw new InvalidOperationException("La liquidación no puede ser rechazada en su estado actual");

        liquidacion.Estado = EstadoLiquidacion.Rechazada;
        liquidacion.AprobadoPorEmpleadoId = empleadoId;
        liquidacion.NotasAprobacion = notas;
        liquidacion.FechaUltimaActualizacion = DateTime.UtcNow;

        await _unitOfWork.Repository<Liquidacion>().UpdateAsync(liquidacion);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Liquidación {LiquidacionId} rechazada por empleado {EmpleadoId}", 
            liquidacionId, empleadoId);

        return liquidacion;
    }

    /// <summary>
    /// Obtiene una liquidación por ID
    /// </summary>
    public async Task<Liquidacion?> ObtenerLiquidacionAsync(int liquidacionId)
    {
        return await _unitOfWork.Repository<Liquidacion>().GetByIdAsync(liquidacionId);
    }

    /// <summary>
    /// Obtiene liquidaciones con filtros
    /// </summary>
    public async Task<IEnumerable<Liquidacion>> ObtenerLiquidacionesAsync(
        TipoLiquidacion? tipo = null,
        EstadoLiquidacion? estado = null,
        int? empleadoId = null,
        int? estacionId = null,
        DateTime? fechaDesde = null,
        DateTime? fechaHasta = null)
    {
        return await _unitOfWork.Repository<Liquidacion>()
            .GetAsync(l => (!tipo.HasValue || l.TipoLiquidacion == tipo.Value) &&
                          (!estado.HasValue || l.Estado == estado.Value) &&
                          (!empleadoId.HasValue || l.EmpleadoId == empleadoId.Value) &&
                          (!estacionId.HasValue || l.EstacionId == estacionId.Value) &&
                          (!fechaDesde.HasValue || l.FechaInicio >= fechaDesde.Value) &&
                          (!fechaHasta.HasValue || l.FechaFin <= fechaHasta.Value));
    }

    /// <summary>
    /// Resuelve una discrepancia
    /// </summary>
    public async Task<LiquidacionDiscrepancia> ResolverDiscrepanciaAsync(int discrepanciaId, int empleadoId, string notasResolucion)
    {
        var discrepancia = await _unitOfWork.Repository<LiquidacionDiscrepancia>().GetByIdAsync(discrepanciaId);
        if (discrepancia == null)
            throw new ArgumentException($"Discrepancia con ID {discrepanciaId} no encontrada");

        discrepancia.Resuelta = true;
        discrepancia.FechaResolucion = DateTime.UtcNow;
        discrepancia.ResueltoPorEmpleadoId = empleadoId;
        discrepancia.NotasResolucion = notasResolucion;

        await _unitOfWork.Repository<LiquidacionDiscrepancia>().UpdateAsync(discrepancia);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Discrepancia {DiscrepanciaId} resuelta por empleado {EmpleadoId}", 
            discrepanciaId, empleadoId);

        return discrepancia;
    }

    /// <summary>
    /// Valida las condiciones previas para generar una liquidación
    /// </summary>
    public async Task<bool> ValidarCondicionesPrevias(TipoLiquidacion tipo, int? empleadoId = null, int? estacionId = null, DateTime? fecha = null)
    {
        switch (tipo)
        {
            case TipoLiquidacion.Cajero:
                return empleadoId.HasValue && estacionId.HasValue;
            
            case TipoLiquidacion.Turno:
                return empleadoId.HasValue && estacionId.HasValue;
            
            case TipoLiquidacion.Dia:
                return fecha.HasValue;
            
            default:
                return false;
        }
    }

    /// <summary>
    /// Obtiene el resumen de liquidaciones por período
    /// </summary>
    public async Task<object> ObtenerResumenLiquidacionesAsync(DateTime fechaInicio, DateTime fechaFin, int? estacionId = null)
    {
        var liquidaciones = await ObtenerLiquidacionesAsync(
            fechaDesde: fechaInicio,
            fechaHasta: fechaFin,
            estacionId: estacionId);

        return new
        {
            TotalLiquidaciones = liquidaciones.Count(),
            LiquidacionesAprobadas = liquidaciones.Count(l => l.Estado == EstadoLiquidacion.Aprobada),
            LiquidacionesPendientes = liquidaciones.Count(l => l.Estado == EstadoLiquidacion.Generada || l.Estado == EstadoLiquidacion.EnRevision),
            MontoTotalLiquidado = liquidaciones.Sum(l => l.MontoTotalRecaudado),
            TotalDiscrepancias = liquidaciones.SelectMany(l => l.Discrepancias).Count(),
            DiscrepanciasPendientes = liquidaciones.SelectMany(l => l.Discrepancias).Count(d => !d.Resuelta),
            PorTipo = liquidaciones.GroupBy(l => l.TipoLiquidacion.ToString())
                .ToDictionary(g => g.Key, g => g.Count())
        };
    }

    // Métodos privados auxiliares

    private static bool ShouldRequireApproval(IEnumerable<Transaccion> transacciones, IEnumerable<Turno> turnos)
    {
        var montoTotal = transacciones.Sum(t => t.Monto);
        var diferenciaCaja = Math.Abs(CalcularDiferenciaCaja(transacciones, turnos));
        
        return montoTotal > 10000 || diferenciaCaja > 100 || turnos.Any(t => t.Estado != "Cerrado");
    }

    private static decimal CalcularDiferenciaCaja(IEnumerable<Transaccion> transacciones, IEnumerable<Turno> turnos)
    {
        var montoTransacciones = transacciones.Sum(t => t.Monto);
        var diferenciaTurnos = turnos.Where(t => t.MontoFinalCaja.HasValue)
            .Sum(t => t.MontoFinalCaja!.Value - t.MontoInicialCaja);
        
        return diferenciaTurnos - montoTransacciones;
    }

    private async Task AgregarDetallesLiquidacion(Liquidacion liquidacion, IEnumerable<Transaccion> transacciones)
    {
        foreach (var transaccion in transacciones)
        {
            var detalle = new LiquidacionDetalle
            {
                LiquidacionId = liquidacion.Id,
                TransaccionId = transaccion.Id,
                Monto = transaccion.Monto,
                TipoPago = transaccion.TipoPago?.Nombre ?? "Sin especificar",
                TipoVehiculo = transaccion.TipoVehiculo?.Nombre ?? "Sin especificar",
                FechaTransaccion = transaccion.FechaTransaccion,
                CarrilId = transaccion.CarrilId,
                Validado = true
            };

            liquidacion.Detalles.Add(detalle);
        }
    }

    private async Task IdentificarDiscrepancias(Liquidacion liquidacion, IEnumerable<Transaccion> transacciones, IEnumerable<Turno> turnos)
    {
        // Discrepancias comunes
        await AgregarDiscrepanciasComunes(liquidacion, transacciones, turnos);
    }

    private async Task IdentificarDiscrepanciasTurno(Liquidacion liquidacion, IEnumerable<Transaccion> transacciones, Turno turno)
    {
        // Discrepancias específicas del turno
        if (turno.Estado != "Cerrado")
        {
            liquidacion.Discrepancias.Add(new LiquidacionDiscrepancia
            {
                LiquidacionId = liquidacion.Id,
                TipoDiscrepancia = TipoDiscrepancia.TurnoIncompleto,
                Descripcion = $"El turno {turno.Id} no está cerrado",
                MontoDiscrepancia = 0,
                Severidad = SeveridadDiscrepancia.Media
            });
        }

        await AgregarDiscrepanciasComunes(liquidacion, transacciones, new[] { turno });
    }

    private async Task IdentificarDiscrepanciasDia(Liquidacion liquidacion, IEnumerable<Transaccion> transacciones, IEnumerable<Turno> turnos)
    {
        // Discrepancias específicas del día
        var turnosAbiertos = turnos.Where(t => t.Estado != "Cerrado").ToList();
        if (turnosAbiertos.Any())
        {
            liquidacion.Discrepancias.Add(new LiquidacionDiscrepancia
            {
                LiquidacionId = liquidacion.Id,
                TipoDiscrepancia = TipoDiscrepancia.TurnoIncompleto,
                Descripcion = $"{turnosAbiertos.Count} turnos permanecen abiertos",
                MontoDiscrepancia = 0,
                Severidad = SeveridadDiscrepancia.Alta
            });
        }

        await AgregarDiscrepanciasComunes(liquidacion, transacciones, turnos);
    }

    private async Task AgregarDiscrepanciasComunes(Liquidacion liquidacion, IEnumerable<Transaccion> transacciones, IEnumerable<Turno> turnos)
    {
        // Transacciones sin empleado
        var transaccionesSinEmpleado = transacciones.Where(t => !t.EmpleadoId.HasValue).ToList();
        if (transaccionesSinEmpleado.Any())
        {
            liquidacion.Discrepancias.Add(new LiquidacionDiscrepancia
            {
                LiquidacionId = liquidacion.Id,
                TipoDiscrepancia = TipoDiscrepancia.TransaccionSinEmpleado,
                Descripcion = $"{transaccionesSinEmpleado.Count} transacciones sin empleado asignado",
                MontoDiscrepancia = transaccionesSinEmpleado.Sum(t => t.Monto),
                Severidad = SeveridadDiscrepancia.Media
            });
        }

        // Diferencias significativas en caja
        var diferenciaCaja = Math.Abs(CalcularDiferenciaCaja(transacciones, turnos));
        if (diferenciaCaja > 500)
        {
            liquidacion.Discrepancias.Add(new LiquidacionDiscrepancia
            {
                LiquidacionId = liquidacion.Id,
                TipoDiscrepancia = TipoDiscrepancia.DiferenciaCaja,
                Descripcion = $"Diferencia significativa en caja: ${diferenciaCaja:F2}",
                MontoDiscrepancia = diferenciaCaja,
                Severidad = diferenciaCaja > 1000 ? SeveridadDiscrepancia.Alta : SeveridadDiscrepancia.Media
            });
        }

        // Montos irregulares (muy altos o muy bajos)
        var transaccionesIrregulares = transacciones.Where(t => t.Monto > 1000 || t.Monto < 5).ToList();
        if (transaccionesIrregulares.Any())
        {
            liquidacion.Discrepancias.Add(new LiquidacionDiscrepancia
            {
                LiquidacionId = liquidacion.Id,
                TipoDiscrepancia = TipoDiscrepancia.MontoIrregular,
                Descripcion = $"{transaccionesIrregulares.Count} transacciones con montos irregulares",
                MontoDiscrepancia = transaccionesIrregulares.Sum(t => t.Monto),
                Severidad = SeveridadDiscrepancia.Baja
            });
        }
    }
}
