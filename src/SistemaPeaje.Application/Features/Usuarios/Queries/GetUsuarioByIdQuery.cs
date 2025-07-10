using AutoMapper;
using MediatR;
using SistemaPeaje.Core.Interfaces;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Application.Mappings;

namespace SistemaPeaje.Application.Features.Usuarios.Queries;

public record GetUsuarioByIdQuery(int Id) : IRequest<UsuarioDto?>;

public class GetUsuarioByIdHandler : IRequestHandler<GetUsuarioByIdQuery, UsuarioDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetUsuarioByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<UsuarioDto?> Handle(GetUsuarioByIdQuery request, CancellationToken cancellationToken)
    {
        var usuario = await _unitOfWork.Repository<Usuario>().GetByIdAsync(request.Id);
        return usuario != null ? _mapper.Map<UsuarioDto>(usuario) : null;
    }
}
