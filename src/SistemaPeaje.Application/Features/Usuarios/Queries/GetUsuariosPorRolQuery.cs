using AutoMapper;
using MediatR;
using SistemaPeaje.Core.Interfaces;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Application.Mappings;

namespace SistemaPeaje.Application.Features.Usuarios.Queries;

public record GetUsuariosPorRolQuery(string Rol) : IRequest<List<UsuarioDto>>;

public class GetUsuariosPorRolHandler : IRequestHandler<GetUsuariosPorRolQuery, List<UsuarioDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetUsuariosPorRolHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<UsuarioDto>> Handle(GetUsuariosPorRolQuery request, CancellationToken cancellationToken)
    {
        var usuarios = await _unitOfWork.Repository<Usuario>()
            .GetAsync(u => u.Rol == request.Rol && u.EsActivo);
        
        return _mapper.Map<List<UsuarioDto>>(usuarios);
    }
}
