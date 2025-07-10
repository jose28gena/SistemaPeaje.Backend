using AutoMapper;
using MediatR;
using SistemaPeaje.Application.Mappings;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;

namespace SistemaPeaje.Application.Features.TiposVehiculo;

public record CreateTipoVehiculoCommand : IRequest<TipoVehiculoDto>
{
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public int NumeroEjes { get; set; }
    public decimal TarifaBase { get; set; }
    public string? Categoria { get; set; }
}

public class CreateTipoVehiculoHandler : IRequestHandler<CreateTipoVehiculoCommand, TipoVehiculoDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateTipoVehiculoHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<TipoVehiculoDto> Handle(CreateTipoVehiculoCommand request, CancellationToken cancellationToken)
    {
        var tipoVehiculo = new TipoVehiculo
        {
            Nombre = request.Nombre,
            Descripcion = request.Descripcion,
            NumeroEjes = request.NumeroEjes,
            TarifaBase = request.TarifaBase,
            Categoria = request.Categoria,
            EsActivo = true
        };

        await _unitOfWork.Repository<TipoVehiculo>().AddAsync(tipoVehiculo);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<TipoVehiculoDto>(tipoVehiculo);
    }
}
