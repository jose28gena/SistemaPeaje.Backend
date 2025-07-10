using MediatR;
using Microsoft.AspNetCore.Mvc;
using SistemaPeaje.Application.Features.TiposPago;
using SistemaPeaje.Application.Mappings;

namespace SistemaPeaje.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TiposPagoController : ControllerBase
{
    private readonly IMediator _mediator;

    public TiposPagoController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtiene todos los tipos de pago (Efectivo, Prepago, Residentes, etc.)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<TipoPagoDto>>> GetTiposPago()
    {
        var tiposPago = await _mediator.Send(new GetTiposPagoQuery());
        return Ok(tiposPago);
    }

    /// <summary>
    /// Crea un nuevo tipo de pago
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<TipoPagoDto>> CreateTipoPago(CreateTipoPagoCommand command)
    {
        var tipoPago = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetTiposPago), tipoPago);
    }

    /// <summary>
    /// Obtiene tipos de pago activos solamente
    /// </summary>
    [HttpGet("activos")]
    public async Task<ActionResult<List<TipoPagoDto>>> GetTiposPagoActivos()
    {
        var tiposPago = await _mediator.Send(new GetTiposPagoQuery());
        var tiposActivos = tiposPago.Where(t => t.EsActivo).ToList();
        return Ok(tiposActivos);
    }

    /// <summary>
    /// Procesa un pago según el tipo especificado
    /// </summary>
    [HttpPost("procesar")]
    public async Task<ActionResult<ProcesarPagoResponse>> ProcesarPago(ProcesarPagoRequest request)
    {
        var response = new ProcesarPagoResponse
        {
            TransaccionId = Guid.NewGuid().ToString(),
            TipoPagoId = request.TipoPagoId,
            Monto = request.Monto,
            FechaProceso = DateTime.UtcNow,
            Estado = "Procesado"
        };

        // Aquí se implementaría la lógica específica según el tipo de pago
        switch (request.TipoPago?.ToLower())
        {
            case "efectivo":
                response.Detalles = "Pago en efectivo procesado correctamente";
                break;
            case "prepago":
                response.Detalles = "Descuento aplicado de saldo prepago";
                response.SaldoRestante = request.SaldoDisponible - request.Monto;
                break;
            case "residentes":
                response.Detalles = "Descuento de residente aplicado";
                response.DescuentoAplicado = request.Monto * 0.20m; // 20% descuento
                break;
            case "credito":
                response.Detalles = "Pago a crédito autorizado";
                response.NumeroAutorizacion = $"AUTH-{DateTime.UtcNow:yyyyMMddHHmmss}";
                break;
            default:
                response.Detalles = "Tipo de pago procesado";
                break;
        }

        return Ok(response);
    }
}

// DTOs para procesamiento de pagos
public class ProcesarPagoRequest
{
    public int TipoPagoId { get; set; }
    public string? TipoPago { get; set; }
    public decimal Monto { get; set; }
    public decimal SaldoDisponible { get; set; }
    public string? NumeroTarjeta { get; set; }
    public string? ClienteId { get; set; }
}

public class ProcesarPagoResponse
{
    public string TransaccionId { get; set; } = string.Empty;
    public int TipoPagoId { get; set; }
    public decimal Monto { get; set; }
    public decimal? SaldoRestante { get; set; }
    public decimal? DescuentoAplicado { get; set; }
    public string? NumeroAutorizacion { get; set; }
    public DateTime FechaProceso { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string Detalles { get; set; } = string.Empty;
}
