using AutoMapper;
using MediatR;
using SistemaPeaje.Application.Mappings;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;

namespace SistemaPeaje.Application.Features.Carriles;

public record GetCarrilByIdQuery(int Id) : IRequest<CarrilDto?>;

public class GetCarrilByIdHandler : IRequestHandler<GetCarrilByIdQuery, CarrilDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetCarrilByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<CarrilDto?> Handle(GetCarrilByIdQuery request, CancellationToken cancellationToken)
    {
        var carril = await _unitOfWork.Repository<Carril>().GetByIdAsync(request.Id);

        return carril == null ? null : _mapper.Map<CarrilDto>(carril);
    }
}
