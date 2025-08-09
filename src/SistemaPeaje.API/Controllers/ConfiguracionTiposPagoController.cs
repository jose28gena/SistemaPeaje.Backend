using MediatR;
using Microsoft.AspNetCore.Mvc;
using SistemaPeaje.Application.Features.ConfiguracionTiposPago;
using SistemaPeaje.Application.Mappings;

namespace SistemaPeaje.API.Controllers;

/// <summary>
/// Controlador para gestión de configuraciones de tipos de pago
/// Maneja parámetros como moneda, límites diarios, comisiones, etc.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ConfiguracionTiposPagoController : ControllerBase
{
    private readonly IMediator _mediator;

    public ConfiguracionTiposPagoController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtiene todas las configuraciones de tipos de pago
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<ConfiguracionTipoPagoDto>>> GetConfiguraciones()
    {
        var configuraciones = await _mediator.Send(new GetConfiguracionesTipoPagoQuery());
        return Ok(configuraciones);
    }

    /// <summary>
    /// Obtiene la configuración de un tipo de pago específico
    /// </summary>
    [HttpGet("tipo-pago/{tipoPagoId}")]
    public async Task<ActionResult<ConfiguracionTipoPagoDto>> GetConfiguracionByTipoPago(int tipoPagoId)
    {
        var configuracion = await _mediator.Send(new GetConfiguracionByTipoPagoQuery(tipoPagoId));
        
        if (configuracion == null)
            return NotFound($"No se encontró configuración para el tipo de pago ID {tipoPagoId}");
        
        return Ok(configuracion);
    }

    /// <summary>
    /// Obtiene todos los tipos de pago activos con su configuración
    /// Útil para interfaces de operador y procesamiento de pagos
    /// </summary>
    [HttpGet("activos-con-configuracion")]
    public async Task<ActionResult<List<TipoPagoConConfiguracionDto>>> GetTiposPagoActivosConConfiguracion()
    {
        var tiposPago = await _mediator.Send(new GetTiposPagoActivosConConfiguracionQuery());
        return Ok(tiposPago);
    }

    /// <summary>
    /// Crea una nueva configuración para un tipo de pago
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ConfiguracionTipoPagoDto>> CreateConfiguracion(CreateConfiguracionTipoPagoCommand command)
    {
        try
        {
            var configuracion = await _mediator.Send(command);
            return CreatedAtAction(
                nameof(GetConfiguracionByTipoPago), 
                new { tipoPagoId = configuracion.TipoPagoId }, 
                configuracion);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Actualiza una configuración existente de tipo de pago
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ConfiguracionTipoPagoDto>> UpdateConfiguracion(int id, UpdateConfiguracionTipoPagoCommand command)
    {
        if (id != command.Id)
            return BadRequest("El ID en la URL no coincide con el ID en el comando");
        
        try
        {
            var configuracion = await _mediator.Send(command);
            return Ok(configuracion);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Inicializa configuraciones por defecto para los tipos de pago principales
    /// Útil para setup inicial del sistema
    /// </summary>
    [HttpPost("inicializar-configuraciones-defecto")]
    public async Task<ActionResult<List<ConfiguracionTipoPagoDto>>> InicializarConfiguracionesDefecto()
    {
        var configuracionesCreadas = new List<ConfiguracionTipoPagoDto>();

        // Configuración por defecto para Efectivo
        try
        {
            var configEfectivo = await _mediator.Send(new CreateConfiguracionTipoPagoCommand
            {
                TipoPagoId = 1, // Asumiendo que Efectivo tiene ID 1
                Moneda = "MXN",
                SimboloMoneda = "$",
                LimiteDiario = 50000, // $50,000 pesos por día
                LimiteTransaccion = 5000, // $5,000 pesos por transacción
                ComisionPorcentaje = 0, // Sin comisión para efectivo
                ComisionFija = 0,
                EstaActivo = true,
                RequiereValidacionAdicional = false,
                TiempoEsperaSegundos = 15,
                DescuentoPorDefecto = 0,
                PermiteTransaccionesParcialeS = false,
                Observaciones = "Configuración por defecto para pagos en efectivo"
            });
            configuracionesCreadas.Add(configEfectivo);
        }
        catch (Exception) { /* Ignorar si ya existe */ }

        // Configuración por defecto para Prepago
        try
        {
            var configPrepago = await _mediator.Send(new CreateConfiguracionTipoPagoCommand
            {
                TipoPagoId = 2, // Asumiendo que Prepago tiene ID 2
                Moneda = "MXN",
                SimboloMoneda = "$",
                LimiteDiario = 100000, // $100,000 pesos por día para prepago
                LimiteTransaccion = 10000, // $10,000 pesos por transacción
                ComisionPorcentaje = 1.5m, // 1.5% de comisión
                ComisionFija = 0,
                EstaActivo = true,
                RequiereValidacionAdicional = true,
                TiempoEsperaSegundos = 30,
                DescuentoPorDefecto = 5, // 5% de descuento por defecto
                PermiteTransaccionesParcialeS = true,
                ConfiguracionEspecifica = "{\"requierePin\": true, \"validarSaldo\": true}",
                Observaciones = "Configuración por defecto para pagos prepago con descuento"
            });
            configuracionesCreadas.Add(configPrepago);
        }
        catch (Exception) { /* Ignorar si ya existe */ }

        // Configuración por defecto para Residentes
        try
        {
            var configResidentes = await _mediator.Send(new CreateConfiguracionTipoPagoCommand
            {
                TipoPagoId = 3, // Asumiendo que Residentes tiene ID 3
                Moneda = "MXN",
                SimboloMoneda = "$",
                LimiteDiario = 200000, // $200,000 pesos por día para residentes
                LimiteTransaccion = 15000, // $15,000 pesos por transacción
                ComisionPorcentaje = 0, // Sin comisión para residentes
                ComisionFija = 0,
                EstaActivo = true,
                RequiereValidacionAdicional = true,
                TiempoEsperaSegundos = 45,
                DescuentoPorDefecto = 20, // 20% de descuento para residentes
                PermiteTransaccionesParcialeS = false,
                ConfiguracionEspecifica = "{\"requiereDocumentacion\": true, \"validarResidencia\": true}",
                Observaciones = "Configuración por defecto para residentes con descuento especial"
            });
            configuracionesCreadas.Add(configResidentes);
        }
        catch (Exception) { /* Ignorar si ya existe */ }

        return Ok(new
        {
            message = $"Se inicializaron {configuracionesCreadas.Count} configuraciones por defecto",
            configuraciones = configuracionesCreadas
        });
    }

    /// <summary>
    /// Calcula el monto final aplicando comisiones y descuentos
    /// según la configuración del tipo de pago
    /// </summary>
    [HttpPost("calcular-monto")]
    public async Task<ActionResult<CalculoMontoResponse>> CalcularMonto(CalculoMontoRequest request)
    {
        var configuracion = await _mediator.Send(new GetConfiguracionByTipoPagoQuery(request.TipoPagoId));
        
        if (configuracion == null)
            return BadRequest($"No se encontró configuración para el tipo de pago ID {request.TipoPagoId}");

        if (!configuracion.EstaActivo)
            return BadRequest("El tipo de pago no está activo");

        var montoBase = request.MontoBase;
        var response = new CalculoMontoResponse
        {
            TipoPagoId = request.TipoPagoId,
            TipoPagoNombre = configuracion.TipoPagoNombre,
            MontoBase = montoBase,
            Moneda = configuracion.Moneda,
            SimboloMoneda = configuracion.SimboloMoneda
        };

        // Aplicar descuento por defecto
        var descuento = montoBase * (configuracion.DescuentoPorDefecto / 100);
        response.DescuentoAplicado = descuento;
        var montoConDescuento = montoBase - descuento;

        // Aplicar comisión porcentual
        var comisionPorcentual = montoConDescuento * (configuracion.ComisionPorcentaje / 100);
        response.ComisionPorcentual = comisionPorcentual;

        // Aplicar comisión fija
        response.ComisionFija = configuracion.ComisionFija;

        // Calcular monto final
        response.MontoFinal = montoConDescuento + comisionPorcentual + configuracion.ComisionFija;

        // Validar límites
        if (configuracion.LimiteTransaccion.HasValue && response.MontoFinal > configuracion.LimiteTransaccion.Value)
        {
            response.ExcedeLimiteTransaccion = true;
            response.LimiteTransaccion = configuracion.LimiteTransaccion.Value;
        }

        return Ok(response);
    }
}

// DTOs para cálculo de montos
public class CalculoMontoRequest
{
    public int TipoPagoId { get; set; }
    public decimal MontoBase { get; set; }
}

public class CalculoMontoResponse
{
    public int TipoPagoId { get; set; }
    public string TipoPagoNombre { get; set; } = string.Empty;
    public decimal MontoBase { get; set; }
    public decimal DescuentoAplicado { get; set; }
    public decimal ComisionPorcentual { get; set; }
    public decimal ComisionFija { get; set; }
    public decimal MontoFinal { get; set; }
    public string Moneda { get; set; } = string.Empty;
    public string SimboloMoneda { get; set; } = string.Empty;
    public bool ExcedeLimiteTransaccion { get; set; }
    public decimal? LimiteTransaccion { get; set; }
}
