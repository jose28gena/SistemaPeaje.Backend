using AutoMapper;
using MediatR;
using SistemaPeaje.Core.Interfaces;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Application.Mappings;

namespace SistemaPeaje.Application.Features.Usuarios.Commands;

public record UpdateUsuarioCommand : IRequest<UsuarioDto>
{
    public int Id { get; init; }
    public string NombreUsuario { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Nombres { get; init; } = string.Empty;
    public string Apellidos { get; init; } = string.Empty;
    public string Rol { get; init; } = string.Empty;
    public string? Telefono { get; init; }
    public bool EsActivo { get; init; }
    public int? EmpleadoId { get; init; }
    public int? EstacionId { get; init; }
    public string? Permisos { get; init; }
}

public class UpdateUsuarioHandler : IRequestHandler<UpdateUsuarioCommand, UsuarioDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateUsuarioHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<UsuarioDto> Handle(UpdateUsuarioCommand request, CancellationToken cancellationToken)
    {
        var usuario = await _unitOfWork.Repository<Usuario>().GetByIdAsync(request.Id);
        if (usuario == null)
            throw new ArgumentException("Usuario no encontrado", nameof(request.Id));

        usuario.NombreUsuario = request.NombreUsuario;
        usuario.Email = request.Email;
        usuario.Nombres = request.Nombres;
        usuario.Apellidos = request.Apellidos;
        usuario.Rol = request.Rol;
        usuario.Telefono = request.Telefono;
        usuario.EsActivo = request.EsActivo;
        usuario.EmpleadoId = request.EmpleadoId;
        usuario.EstacionId = request.EstacionId;
        usuario.Permisos = request.Permisos;

        await _unitOfWork.Repository<Usuario>().UpdateAsync(usuario);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<UsuarioDto>(usuario);
    }
}
