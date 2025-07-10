using MediatR;
using Microsoft.AspNetCore.Mvc;
using SistemaPeaje.Application.Features.Tarifas;
using SistemaPeaje.Application.Mappings;

namespace SistemaPeaje.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TarifasController : ControllerBase
{
    private readonly IMediator _mediator;

    public TarifasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtiene todas las tarifas
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<TarifaDto>>> GetTarifas()
    {
        var tarifas = await _mediator.Send(new GetTarifasQuery());
        return Ok(tarifas);
    }

    /// <summary>
    /// Obtiene las tarifas vigentes, con filtros opcionales
    /// </summary>
    [HttpGet("vigentes")]
    public async Task<ActionResult<List<TarifaDto>>> GetTarifasVigentes(
        [FromQuery] int? estacionId = null,
        [FromQuery] int? tipoVehiculoId = null)
    {
        var query = new GetTarifasVigentesQuery
        {
            EstacionId = estacionId,
            TipoVehiculoId = tipoVehiculoId
        };

        var tarifas = await _mediator.Send(query);
        return Ok(tarifas);
    }

    /// <summary>
    /// Crea una nueva tarifa
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<TarifaDto>> CreateTarifa(CreateTarifaCommand command)
    {
        var tarifa = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetTarifas), tarifa);
    }

    /// <summary>
    /// Obtiene la tarifa vigente para un tipo de vehículo y estación específicos
    /// </summary>
    [HttpGet("vigente")]
    public async Task<ActionResult<TarifaDto>> GetTarifaVigente(
        [FromQuery] int tipoVehiculoId,
        [FromQuery] int? estacionId = null)
    {
        var query = new GetTarifasVigentesQuery
        {
            EstacionId = estacionId,
            TipoVehiculoId = tipoVehiculoId
        };

        var tarifas = await _mediator.Send(query);
        var tarifa = tarifas.FirstOrDefault();

        if (tarifa == null)
            return NotFound($"No hay tarifa vigente para tipo de vehículo {tipoVehiculoId}" +
                           (estacionId.HasValue ? $" en estación {estacionId}" : ""));

        return Ok(tarifa);
    }

    /// <summary>
    /// Calcula el monto de peaje para múltiples conceptos
    /// Conceptos: Peaje Base, Descuento Residente, Descuento Prepago, etc.
    /// </summary>
    [HttpPost("calcular")]
    public async Task<ActionResult<CalculoTarifaResponse>> CalcularTarifa(CalculoTarifaRequest request)
    {
        var tarifaQuery = new GetTarifasVigentesQuery
        {
            EstacionId = request.EstacionId,
            TipoVehiculoId = request.TipoVehiculoId
        };

        var tarifas = await _mediator.Send(tarifaQuery);
        var tarifaBase = tarifas.FirstOrDefault();

        if (tarifaBase == null)
        {
            return NotFound($"No hay tarifa vigente para tipo de vehículo {request.TipoVehiculoId}" +
                           (request.EstacionId.HasValue ? $" en estación {request.EstacionId}" : ""));
        }

        var response = new CalculoTarifaResponse
        {
            TarifaBaseId = tarifaBase.Id,
            MontoBase = tarifaBase.Monto,
            TipoVehiculoId = request.TipoVehiculoId,
            EstacionId = request.EstacionId,
            Conceptos = new List<ConceptoTarifa>()
        };

        // Concepto 1: Tarifa Base
        response.Conceptos.Add(new ConceptoTarifa
        {
            Nombre = "Tarifa Base",
            Monto = tarifaBase.Monto,
            TipoConcepto = "Base"
        });

        // Concepto 2: Descuento por Residente (si aplica)
        if (request.EsResidente)
        {
            var descuentoResidente = tarifaBase.Monto * 0.20m; // 20% de descuento
            response.Conceptos.Add(new ConceptoTarifa
            {
                Nombre = "Descuento Residente",
                Monto = -descuentoResidente,
                TipoConcepto = "Descuento"
            });
        }

        // Concepto 3: Descuento por Prepago (si aplica)
        if (request.EsPrepago)
        {
            var descuentoPrepago = tarifaBase.Monto * 0.10m; // 10% de descuento
            response.Conceptos.Add(new ConceptoTarifa
            {
                Nombre = "Descuento Prepago",
                Monto = -descuentoPrepago,
                TipoConcepto = "Descuento"
            });
        }

        // Concepto 4: Recargo por Horario Pico (si aplica)
        if (request.EsHorarioPico)
        {
            var recargoHorarioPico = tarifaBase.Monto * 0.15m; // 15% de recargo
            response.Conceptos.Add(new ConceptoTarifa
            {
                Nombre = "Recargo Horario Pico",
                Monto = recargoHorarioPico,
                TipoConcepto = "Recargo"
            });
        }

        // Concepto 5: Descuento por Frecuencia (si aplica)
        if (request.DescuentoFrecuencia > 0)
        {
            var descuentoFrecuencia = tarifaBase.Monto * (request.DescuentoFrecuencia / 100m);
            response.Conceptos.Add(new ConceptoTarifa
            {
                Nombre = "Descuento por Frecuencia",
                Monto = -descuentoFrecuencia,
                TipoConcepto = "Descuento"
            });
        }

        // Concepto 6: IVA (si aplica)
        if (request.AplicaIVA)
        {
            var montoSinIVA = response.Conceptos.Sum(c => c.Monto);
            var iva = montoSinIVA * 0.19m; // 19% IVA
            response.Conceptos.Add(new ConceptoTarifa
            {
                Nombre = "IVA (19%)",
                Monto = iva,
                TipoConcepto = "Impuesto"
            });
        }

        // Concepto 7: Tasa Administrativa (si aplica)
        if (request.AplicaTasaAdministrativa)
        {
            response.Conceptos.Add(new ConceptoTarifa
            {
                Nombre = "Tasa Administrativa",
                Monto = 500m, // Monto fijo
                TipoConcepto = "Tasa"
            });
        }

        // Concepto 8: Otros Recargos (si aplica)
        if (request.OtrosRecargos > 0)
        {
            response.Conceptos.Add(new ConceptoTarifa
            {
                Nombre = "Otros Recargos",
                Monto = request.OtrosRecargos,
                TipoConcepto = "Recargo"
            });
        }

        response.MontoTotal = response.Conceptos.Sum(c => c.Monto);
        response.FechaCalculo = DateTime.UtcNow;

        return Ok(response);
    }
}

// DTOs para el cálculo de tarifas
public class CalculoTarifaRequest
{
    public int TipoVehiculoId { get; set; }
    public int? EstacionId { get; set; }
    public bool EsResidente { get; set; } = false;
    public bool EsPrepago { get; set; } = false;
    public bool EsHorarioPico { get; set; } = false;
    public decimal DescuentoFrecuencia { get; set; } = 0; // Porcentaje
    public bool AplicaIVA { get; set; } = false;
    public bool AplicaTasaAdministrativa { get; set; } = false;
    public decimal OtrosRecargos { get; set; } = 0;
}

public class CalculoTarifaResponse
{
    public int TarifaBaseId { get; set; }
    public decimal MontoBase { get; set; }
    public decimal MontoTotal { get; set; }
    public int TipoVehiculoId { get; set; }
    public int? EstacionId { get; set; }
    public DateTime FechaCalculo { get; set; }
    public List<ConceptoTarifa> Conceptos { get; set; } = new();
}

public class ConceptoTarifa
{
    public string Nombre { get; set; } = string.Empty;
    public decimal Monto { get; set; }
    public string TipoConcepto { get; set; } = string.Empty; // Base, Descuento, Recargo, Impuesto, Tasa
}
