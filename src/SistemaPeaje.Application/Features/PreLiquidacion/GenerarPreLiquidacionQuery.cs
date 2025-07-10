using MediatR;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;

namespace SistemaPeaje.Application.Features.PreLiquidacion;

public record GenerarPreLiquidacionQuery : IRequest<PreLiquidacionDto>
{
    public DateTime FechaInicio { get; init; }
    public DateTime FechaFin { get; init; }
    public int? EstacionId { get; init; }
    public int? EmpleadoId { get; init; }
    public TipoPreLiquidacion Tipo { get; init; } = TipoPreLiquidacion.Turno;
}

public class GenerarPreLiquidacionHandler : IRequestHandler<GenerarPreLiquidacionQuery, PreLiquidacionDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GenerarPreLiquidacionHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PreLiquidacionDto> Handle(GenerarPreLiquidacionQuery request, CancellationToken cancellationToken)
    {
        var transacciones = await _unitOfWork.Repository<Transaccion>()
            .GetAsync(t => t.FechaTransaccion >= request.FechaInicio &&
                          t.FechaTransaccion <= request.FechaFin &&
                          (!request.EstacionId.HasValue || t.EstacionId == request.EstacionId) &&
                          (!request.EmpleadoId.HasValue || t.EmpleadoId == request.EmpleadoId));

        var turnos = await _unitOfWork.Repository<Turno>()
            .GetAsync(t => t.FechaInicio >= request.FechaInicio &&
                          t.FechaInicio <= request.FechaFin &&
                          (!request.EstacionId.HasValue || t.EstacionId == request.EstacionId) &&
                          (!request.EmpleadoId.HasValue || t.EmpleadoId == request.EmpleadoId));

        var preLiquidacion = new PreLiquidacionDto
        {
            Id = Guid.NewGuid(),
            TipoLiquidacion = request.Tipo.ToString(),
            FechaInicio = request.FechaInicio,
            FechaFin = request.FechaFin,
            EstacionId = request.EstacionId,
            EmpleadoId = request.EmpleadoId,
            FechaGeneracion = DateTime.UtcNow,

            // Resumen de transacciones
            TotalTransacciones = transacciones.Count,
            MontoTotalRecaudado = transacciones.Sum(t => t.Monto),
            TransaccionesPorTipoPago = transacciones
                .GroupBy(t => t.TipoPago?.Nombre ?? "Sin especificar")
                .ToDictionary(g => g.Key, g => new ResumenTipoPago
                {
                    Cantidad = g.Count(),
                    Monto = g.Sum(x => x.Monto)
                }),

            // Resumen de turnos
            TotalTurnos = turnos.Count,
            TurnosCerrados = turnos.Count(t => t.Estado == "Cerrado"),
            DiferenciaCaja = turnos.Where(t => t.MontoFinalCaja.HasValue)
                .Sum(t => t.MontoFinalCaja!.Value - t.MontoInicialCaja),

            // Análisis de discrepancias
            Discrepancias = IdentificarDiscrepancias(transacciones, turnos),

            // Estado de validación
            Estado = "Generada",
            RequiereAprobacion = request.Tipo == TipoPreLiquidacion.Dia || 
                               Math.Abs(CalcularDiferencias(transacciones, turnos)) > 1000,

            // Detalles adicionales
            DetallesPorCarril = GenerarDetallesPorCarril(transacciones),
            DetallesPorEmpleado = GenerarDetallesPorEmpleado(transacciones)
        };

        return preLiquidacion;
    }

    private static List<DiscrepanciaDto> IdentificarDiscrepancias(IEnumerable<Transaccion> transacciones, IEnumerable<Turno> turnos)
    {
        var discrepancias = new List<DiscrepanciaDto>();

        // Verificar transacciones sin turno asociado
        var transaccionesSinTurno = transacciones.Where(t => t.EmpleadoId == null).ToList();
        if (transaccionesSinTurno.Any())
        {
            discrepancias.Add(new DiscrepanciaDto
            {
                Tipo = "Transacciones sin empleado",
                Descripcion = $"{transaccionesSinTurno.Count} transacciones sin empleado asignado",
                Monto = transaccionesSinTurno.Sum(t => t.Monto),
                Severidad = "Media"
            });
        }

        // Verificar diferencias significativas en caja
        var turnosConDiferencia = turnos
            .Where(t => t.MontoFinalCaja.HasValue && 
                       Math.Abs(t.MontoFinalCaja.Value - t.MontoInicialCaja) > 100)
            .ToList();

        foreach (var turno in turnosConDiferencia)
        {
            discrepancias.Add(new DiscrepanciaDto
            {
                Tipo = "Diferencia en caja",
                Descripcion = $"Turno {turno.Id}: diferencia de ${turno.MontoFinalCaja!.Value - turno.MontoInicialCaja:F2}",
                Monto = turno.MontoFinalCaja.Value - turno.MontoInicialCaja,
                Severidad = Math.Abs(turno.MontoFinalCaja.Value - turno.MontoInicialCaja) > 500 ? "Alta" : "Media"
            });
        }

        return discrepancias;
    }

    private static decimal CalcularDiferencias(IEnumerable<Transaccion> transacciones, IEnumerable<Turno> turnos)
    {
        var montoTransacciones = transacciones.Sum(t => t.Monto);
        var diferenciaTurnos = turnos.Where(t => t.MontoFinalCaja.HasValue)
            .Sum(t => t.MontoFinalCaja!.Value - t.MontoInicialCaja);
        
        return Math.Abs(montoTransacciones - diferenciaTurnos);
    }

    private static Dictionary<int, ResumenCarril> GenerarDetallesPorCarril(IEnumerable<Transaccion> transacciones)
    {
        return transacciones
            .GroupBy(t => t.CarrilId)
            .ToDictionary(g => g.Key, g => new ResumenCarril
            {
                Transacciones = g.Count(),
                Monto = g.Sum(x => x.Monto),
                PromedioTransaccion = g.Average(x => x.Monto),
                HoraPico = g.GroupBy(x => x.FechaTransaccion.Hour)
                    .OrderByDescending(h => h.Count())
                    .First().Key
            });
    }

    private static Dictionary<int, ResumenEmpleado> GenerarDetallesPorEmpleado(IEnumerable<Transaccion> transacciones)
    {
        return transacciones
            .Where(t => t.EmpleadoId.HasValue)
            .GroupBy(t => t.EmpleadoId!.Value)
            .ToDictionary(g => g.Key, g => new ResumenEmpleado
            {
                Transacciones = g.Count(),
                Monto = g.Sum(x => x.Monto),
                HoraInicio = g.Min(x => x.FechaTransaccion),
                HoraFin = g.Max(x => x.FechaTransaccion)
            });
    }
}

// Enums y DTOs
public enum TipoPreLiquidacion
{
    Turno,
    Cajero,
    Dia,
    Estacion
}

public class PreLiquidacionDto
{
    public Guid Id { get; set; }
    public string TipoLiquidacion { get; set; } = string.Empty;
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public int? EstacionId { get; set; }
    public int? EmpleadoId { get; set; }
    public DateTime FechaGeneracion { get; set; }
    
    public int TotalTransacciones { get; set; }
    public decimal MontoTotalRecaudado { get; set; }
    public Dictionary<string, ResumenTipoPago> TransaccionesPorTipoPago { get; set; } = new();
    
    public int TotalTurnos { get; set; }
    public int TurnosCerrados { get; set; }
    public decimal DiferenciaCaja { get; set; }
    
    public List<DiscrepanciaDto> Discrepancias { get; set; } = new();
    public string Estado { get; set; } = string.Empty;
    public bool RequiereAprobacion { get; set; }
    
    public Dictionary<int, ResumenCarril> DetallesPorCarril { get; set; } = new();
    public Dictionary<int, ResumenEmpleado> DetallesPorEmpleado { get; set; } = new();
}

public class ResumenTipoPago
{
    public int Cantidad { get; set; }
    public decimal Monto { get; set; }
}

public class DiscrepanciaDto
{
    public string Tipo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Monto { get; set; }
    public string Severidad { get; set; } = string.Empty;
}

public class ResumenCarril
{
    public int Transacciones { get; set; }
    public decimal Monto { get; set; }
    public decimal PromedioTransaccion { get; set; }
    public int HoraPico { get; set; }
}

public class ResumenEmpleado
{
    public int Transacciones { get; set; }
    public decimal Monto { get; set; }
    public DateTime HoraInicio { get; set; }
    public DateTime HoraFin { get; set; }
}
