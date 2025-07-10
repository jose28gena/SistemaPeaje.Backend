using AutoMapper;
using MediatR;
using SistemaPeaje.Application.Mappings;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;

namespace SistemaPeaje.Application.Features.Carriles;

public record UpdateCarrilCommand : IRequest<CarrilDto>
{
    public int Id { get; set; }
    public int EstacionId { get; set; }
    public string Numero { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
}

public class UpdateCarrilHandler : IRequestHandler<UpdateCarrilCommand, CarrilDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateCarrilHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<CarrilDto> Handle(UpdateCarrilCommand request, CancellationToken cancellationToken)
    {
        var carril = await _unitOfWork.Repository<Carril>().GetByIdAsync(request.Id);
        if (carril == null)
            throw new KeyNotFoundException($"Carril with ID {request.Id} not found");

        carril.EstacionId = request.EstacionId;
        carril.Numero = request.Numero;
        carril.Tipo = request.Tipo;
        carril.Estado = request.Estado;

        await _unitOfWork.Repository<Carril>().UpdateAsync(carril);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<CarrilDto>(carril);
    }
}
