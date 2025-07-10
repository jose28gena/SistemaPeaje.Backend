using AutoMapper;
using MediatR;
using SistemaPeaje.Application.Mappings;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;

namespace SistemaPeaje.Application.Features.TiposPago;

public record CreateTipoPagoCommand : IRequest<TipoPagoDto>
{
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public bool RequiereAutorizacion { get; set; } = false;
    public decimal? LimiteCredito { get; set; }
}

public class CreateTipoPagoHandler : IRequestHandler<CreateTipoPagoCommand, TipoPagoDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateTipoPagoHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<TipoPagoDto> Handle(CreateTipoPagoCommand request, CancellationToken cancellationToken)
    {
        var tipoPago = new TipoPago
        {
            Nombre = request.Nombre,
            Descripcion = request.Descripcion,
            RequiereAutorizacion = request.RequiereAutorizacion,
            LimiteCredito = request.LimiteCredito,
            EsActivo = true
        };

        await _unitOfWork.Repository<TipoPago>().AddAsync(tipoPago);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<TipoPagoDto>(tipoPago);
    }
}
