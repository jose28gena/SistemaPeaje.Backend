using AutoMapper;
using MediatR;
using SistemaPeaje.Core.Interfaces;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Application.Mappings;

namespace SistemaPeaje.Application.Features.Usuarios.Commands;

public record CreateUsuarioCommand : IRequest<UsuarioDto>
{
    public string NombreUsuario { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string Nombres { get; init; } = string.Empty;
    public string Apellidos { get; init; } = string.Empty;
    public string Rol { get; init; } = string.Empty;
    public string? Telefono { get; init; }
    public int? EmpleadoId { get; init; }
    public int? EstacionId { get; init; }
    public string? Permisos { get; init; }
}

public class CreateUsuarioHandler : IRequestHandler<CreateUsuarioCommand, UsuarioDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateUsuarioHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<UsuarioDto> Handle(CreateUsuarioCommand request, CancellationToken cancellationToken)
    {
        var usuario = new Usuario
        {
            NombreUsuario = request.NombreUsuario,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Nombres = request.Nombres,
            Apellidos = request.Apellidos,
            Rol = request.Rol,
            Telefono = request.Telefono,
            EmpleadoId = request.EmpleadoId,
            EstacionId = request.EstacionId,
            Permisos = request.Permisos,
            EsActivo = true
        };

        await _unitOfWork.Repository<Usuario>().AddAsync(usuario);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<UsuarioDto>(usuario);
    }
}
