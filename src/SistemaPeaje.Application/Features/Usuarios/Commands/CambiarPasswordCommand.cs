using MediatR;
using SistemaPeaje.Core.Interfaces;
using SistemaPeaje.Core.Entities;

namespace SistemaPeaje.Application.Features.Usuarios.Commands;

public record CambiarPasswordCommand : IRequest
{
    public int UsuarioId { get; init; }
    public string PasswordActual { get; init; } = string.Empty;
    public string PasswordNuevo { get; init; } = string.Empty;
}

public class CambiarPasswordHandler : IRequestHandler<CambiarPasswordCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public CambiarPasswordHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(CambiarPasswordCommand request, CancellationToken cancellationToken)
    {
        var usuario = await _unitOfWork.Repository<Usuario>().GetByIdAsync(request.UsuarioId);
        if (usuario == null)
            throw new ArgumentException("Usuario no encontrado", nameof(request.UsuarioId));

        if (!BCrypt.Net.BCrypt.Verify(request.PasswordActual, usuario.PasswordHash))
            throw new UnauthorizedAccessException("Password actual incorrecto");

        usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.PasswordNuevo);

        await _unitOfWork.Repository<Usuario>().UpdateAsync(usuario);
        await _unitOfWork.SaveChangesAsync();
    }
}
