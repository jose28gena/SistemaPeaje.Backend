using AutoMapper;
using MediatR;
using SistemaPeaje.Application.Mappings;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;

namespace SistemaPeaje.Application.Features.ConfiguracionTiposPago;

/// <summary>
/// Command para actualizar configuración de tipo de pago
/// </summary>
public record UpdateConfiguracionTipoPagoCommand : IRequest<ConfiguracionTipoPagoDto>
{
    public int Id { get; set; }
    public int TipoPagoId { get; set; }
    public string Moneda { get; set; } = "MXN";
    public string SimboloMoneda { get; set; } = "$";
    public decimal? LimiteDiario { get; set; }
    public decimal? LimiteTransaccion { get; set; }
    public decimal ComisionPorcentaje { get; set; } = 0;
    public decimal ComisionFija { get; set; } = 0;
    public bool EstaActivo { get; set; } = true;
    public bool RequiereValidacionAdicional { get; set; } = false;
    public int TiempoEsperaSegundos { get; set; } = 30;
    public decimal DescuentoPorDefecto { get; set; } = 0;
    public bool PermiteTransaccionesParcialeS { get; set; } = false;
    public string? ConfiguracionEspecifica { get; set; }
    public string? Observaciones { get; set; }
}

/// <summary>
/// Handler para actualizar configuración de tipo de pago
/// </summary>
public class UpdateConfiguracionTipoPagoHandler : IRequestHandler<UpdateConfiguracionTipoPagoCommand, ConfiguracionTipoPagoDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateConfiguracionTipoPagoHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ConfiguracionTipoPagoDto> Handle(UpdateConfiguracionTipoPagoCommand request, CancellationToken cancellationToken)
    {
        var configuracion = await _unitOfWork.Repository<ConfiguracionTipoPago>()
            .GetByIdAsync(request.Id);
        
        if (configuracion == null)
            throw new ArgumentException($"Configuración con ID {request.Id} no encontrada");

        // Verificar si el tipo de pago cambió y existe
        if (configuracion.TipoPagoId != request.TipoPagoId)
        {
            var tipoPago = await _unitOfWork.Repository<TipoPago>().GetByIdAsync(request.TipoPagoId);
            if (tipoPago == null)
                throw new ArgumentException($"Tipo de pago con ID {request.TipoPagoId} no encontrado");
        }

        // Actualizar campos
        configuracion.TipoPagoId = request.TipoPagoId;
        configuracion.Moneda = request.Moneda;
        configuracion.SimboloMoneda = request.SimboloMoneda;
        configuracion.LimiteDiario = request.LimiteDiario;
        configuracion.LimiteTransaccion = request.LimiteTransaccion;
        configuracion.ComisionPorcentaje = request.ComisionPorcentaje;
        configuracion.ComisionFija = request.ComisionFija;
        configuracion.EstaActivo = request.EstaActivo;
        configuracion.RequiereValidacionAdicional = request.RequiereValidacionAdicional;
        configuracion.TiempoEsperaSegundos = request.TiempoEsperaSegundos;
        configuracion.DescuentoPorDefecto = request.DescuentoPorDefecto;
        configuracion.PermiteTransaccionesParcialeS = request.PermiteTransaccionesParcialeS;
        configuracion.ConfiguracionEspecifica = request.ConfiguracionEspecifica;
        configuracion.Observaciones = request.Observaciones;

        await _unitOfWork.Repository<ConfiguracionTipoPago>().UpdateAsync(configuracion);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ConfiguracionTipoPagoDto>(configuracion);
    }
}
