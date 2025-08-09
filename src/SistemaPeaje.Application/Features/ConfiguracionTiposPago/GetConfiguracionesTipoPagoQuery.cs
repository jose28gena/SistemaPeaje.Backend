using AutoMapper;
using MediatR;
using SistemaPeaje.Application.Mappings;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;
using System.Linq;

namespace SistemaPeaje.Application.Features.ConfiguracionTiposPago;

/// <summary>
/// Query para obtener todas las configuraciones de tipos de pago
/// </summary>
public record GetConfiguracionesTipoPagoQuery : IRequest<List<ConfiguracionTipoPagoDto>>;

/// <summary>
/// Handler para obtener todas las configuraciones de tipos de pago
/// </summary>
public class GetConfiguracionesTipoPagoHandler : IRequestHandler<GetConfiguracionesTipoPagoQuery, List<ConfiguracionTipoPagoDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetConfiguracionesTipoPagoHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<ConfiguracionTipoPagoDto>> Handle(GetConfiguracionesTipoPagoQuery request, CancellationToken cancellationToken)
    {
        var configuraciones = await _unitOfWork.Repository<ConfiguracionTipoPago>()
            .GetAllAsync();
        
        return _mapper.Map<List<ConfiguracionTipoPagoDto>>(configuraciones);
    }
}

/// <summary>
/// Query para obtener configuración por tipo de pago
/// </summary>
public record GetConfiguracionByTipoPagoQuery(int TipoPagoId) : IRequest<ConfiguracionTipoPagoDto?>;

/// <summary>
/// Handler para obtener configuración por tipo de pago
/// </summary>
public class GetConfiguracionByTipoPagoHandler : IRequestHandler<GetConfiguracionByTipoPagoQuery, ConfiguracionTipoPagoDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetConfiguracionByTipoPagoHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ConfiguracionTipoPagoDto?> Handle(GetConfiguracionByTipoPagoQuery request, CancellationToken cancellationToken)
    {
        var configuraciones = await _unitOfWork.Repository<ConfiguracionTipoPago>()
            .GetAsync(c => c.TipoPagoId == request.TipoPagoId);
        
        var configuracion = configuraciones.FirstOrDefault();
        
        return configuracion != null ? _mapper.Map<ConfiguracionTipoPagoDto>(configuracion) : null;
    }
}

/// <summary>
/// Query para obtener tipos de pago activos con su configuración
/// </summary>
public record GetTiposPagoActivosConConfiguracionQuery : IRequest<List<TipoPagoConConfiguracionDto>>;

/// <summary>
/// Handler para obtener tipos de pago activos con su configuración
/// </summary>
public class GetTiposPagoActivosConConfiguracionHandler : IRequestHandler<GetTiposPagoActivosConConfiguracionQuery, List<TipoPagoConConfiguracionDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetTiposPagoActivosConConfiguracionHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<TipoPagoConConfiguracionDto>> Handle(GetTiposPagoActivosConConfiguracionQuery request, CancellationToken cancellationToken)
    {
        var tiposPago = await _unitOfWork.Repository<TipoPago>()
            .GetAsync(t => t.EsActivo);
        
        return _mapper.Map<List<TipoPagoConConfiguracionDto>>(tiposPago);
    }
}
