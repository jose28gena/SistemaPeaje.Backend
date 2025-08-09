using AutoMapper;
using MediatR;
using SistemaPeaje.Application.Mappings;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;

namespace SistemaPeaje.Application.Features.ConfiguracionTiposPago;

/// <summary>
/// Command para crear configuración de tipo de pago
/// </summary>
public record CreateConfiguracionTipoPagoCommand : IRequest<ConfiguracionTipoPagoDto>
{
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
/// Handler para crear configuración de tipo de pago
/// </summary>
public class CreateConfiguracionTipoPagoHandler : IRequestHandler<CreateConfiguracionTipoPagoCommand, ConfiguracionTipoPagoDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateConfiguracionTipoPagoHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ConfiguracionTipoPagoDto> Handle(CreateConfiguracionTipoPagoCommand request, CancellationToken cancellationToken)
    {
        // Verificar que el tipo de pago existe
        var tipoPago = await _unitOfWork.Repository<TipoPago>().GetByIdAsync(request.TipoPagoId);
        if (tipoPago == null)
            throw new ArgumentException($"Tipo de pago con ID {request.TipoPagoId} no encontrado");

        // Verificar que no existe ya una configuración para este tipo de pago
        var existingConfig = await _unitOfWork.Repository<ConfiguracionTipoPago>()
            .GetAsync(c => c.TipoPagoId == request.TipoPagoId);
        
        if (existingConfig.Any())
            throw new InvalidOperationException($"Ya existe una configuración para el tipo de pago '{tipoPago.Nombre}'");

        var configuracion = new ConfiguracionTipoPago
        {
            TipoPagoId = request.TipoPagoId,
            Moneda = request.Moneda,
            SimboloMoneda = request.SimboloMoneda,
            LimiteDiario = request.LimiteDiario,
            LimiteTransaccion = request.LimiteTransaccion,
            ComisionPorcentaje = request.ComisionPorcentaje,
            ComisionFija = request.ComisionFija,
            EstaActivo = request.EstaActivo,
            RequiereValidacionAdicional = request.RequiereValidacionAdicional,
            TiempoEsperaSegundos = request.TiempoEsperaSegundos,
            DescuentoPorDefecto = request.DescuentoPorDefecto,
            PermiteTransaccionesParcialeS = request.PermiteTransaccionesParcialeS,
            ConfiguracionEspecifica = request.ConfiguracionEspecifica,
            Observaciones = request.Observaciones
        };

        await _unitOfWork.Repository<ConfiguracionTipoPago>().AddAsync(configuracion);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ConfiguracionTipoPagoDto>(configuracion);
    }
}
