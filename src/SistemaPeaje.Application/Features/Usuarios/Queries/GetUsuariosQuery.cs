using AutoMapper;
using MediatR;
using SistemaPeaje.Core.Interfaces;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Application.Mappings;

namespace SistemaPeaje.Application.Features.Usuarios.Queries;

public record GetUsuariosQuery : IRequest<List<UsuarioDto>>;

public class GetUsuariosHandler : IRequestHandler<GetUsuariosQuery, List<UsuarioDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetUsuariosHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<UsuarioDto>> Handle(GetUsuariosQuery request, CancellationToken cancellationToken)
    {
        var usuarios = await _unitOfWork.Repository<Usuario>().GetAllAsync();
        return _mapper.Map<List<UsuarioDto>>(usuarios);
    }
}
