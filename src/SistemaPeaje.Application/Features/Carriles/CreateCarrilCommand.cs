using AutoMapper;
using MediatR;
using SistemaPeaje.Application.Mappings;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;

namespace SistemaPeaje.Application.Features.Carriles;

public record CreateCarrilCommand : IRequest<CarrilDto>
{
    public int EstacionId { get; set; }
    public string Numero { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string Estado { get; set; } = "Activo";
}

public class CreateCarrilHandler : IRequestHandler<CreateCarrilCommand, CarrilDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateCarrilHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<CarrilDto> Handle(CreateCarrilCommand request, CancellationToken cancellationToken)
    {
        var carril = new Carril
        {
            EstacionId = request.EstacionId,
            Numero = request.Numero,
            Tipo = request.Tipo,
            Estado = request.Estado
        };

        await _unitOfWork.Repository<Carril>().AddAsync(carril);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<CarrilDto>(carril);
    }
}
