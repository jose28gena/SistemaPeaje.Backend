using AutoMapper;
using FluentAssertions;
using Moq;
using SistemaPeaje.Application.Features.Usuarios.Commands;
using SistemaPeaje.Application.Mappings;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;
using Xunit;

namespace SistemaPeaje.Tests.Application.Handlers;

public class CreateUsuarioCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreateUsuarioHandler _handler;

    public CreateUsuarioCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _handler = new CreateUsuarioHandler(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsUsuarioDto()
    {
        // Arrange
        var command = new CreateUsuarioCommand
        {
            NombreUsuario = "usuario.test",
            Email = "test@ejemplo.com",
            Password = "Password123!",
            Nombres = "Juan",
            Apellidos = "Pérez",
            Rol = "OPERADOR",
            Telefono = "1234567890",
            EstacionId = 1
        };

        var usuario = new Usuario
        {
            Id = 1,
            NombreUsuario = command.NombreUsuario,
            Email = command.Email,
            Nombres = command.Nombres,
            Apellidos = command.Apellidos,
            Rol = command.Rol,
            Telefono = command.Telefono,
            EstacionId = command.EstacionId,
            EsActivo = true
        };

        var usuarioDto = new UsuarioDto
        {
            Id = 1,
            NombreUsuario = command.NombreUsuario,
            Email = command.Email,
            Nombres = command.Nombres,
            Apellidos = command.Apellidos,
            Rol = command.Rol,
            Telefono = command.Telefono,
            EstacionId = command.EstacionId,
            EsActivo = true
        };

        var repositoryMock = new Mock<IRepository<Usuario>>();
        repositoryMock.Setup(r => r.AddAsync(It.IsAny<Usuario>()))
                     .ReturnsAsync(usuario);

        _unitOfWorkMock.Setup(u => u.Repository<Usuario>())
                       .Returns(repositoryMock.Object);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                       .ReturnsAsync(1);

        _mapperMock.Setup(m => m.Map<UsuarioDto>(It.IsAny<Usuario>()))
                   .Returns(usuarioDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.NombreUsuario.Should().Be("usuario.test");
        result.Email.Should().Be("test@ejemplo.com");
        result.Rol.Should().Be("OPERADOR");
        result.EsActivo.Should().BeTrue();

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        repositoryMock.Verify(r => r.AddAsync(It.IsAny<Usuario>()), Times.Once);
        _mapperMock.Verify(m => m.Map<UsuarioDto>(It.IsAny<Usuario>()), Times.Once);
    }

    [Theory]
    [InlineData("ADMIN")]
    [InlineData("OPERADOR")]
    [InlineData("SUPERVISOR")]
    public async Task Handle_ValidRoles_CreatesUserWithCorrectRole(string rol)
    {
        // Arrange
        var command = new CreateUsuarioCommand
        {
            NombreUsuario = "usuario.test",
            Email = "test@ejemplo.com",
            Password = "Password123!",
            Nombres = "Juan",
            Apellidos = "Pérez",
            Rol = rol
        };

        var repositoryMock = new Mock<IRepository<Usuario>>();
        _unitOfWorkMock.Setup(u => u.Repository<Usuario>()).Returns(repositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var usuarioDto = new UsuarioDto { Rol = rol };
        _mapperMock.Setup(m => m.Map<UsuarioDto>(It.IsAny<Usuario>())).Returns(usuarioDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Rol.Should().Be(rol);
        repositoryMock.Verify(r => r.AddAsync(It.Is<Usuario>(u => u.Rol == rol)), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidCommand_HashesPassword()
    {
        // Arrange
        var command = new CreateUsuarioCommand
        {
            NombreUsuario = "usuario.test",
            Email = "test@ejemplo.com",
            Password = "Password123!",
            Nombres = "Juan",
            Apellidos = "Pérez",
            Rol = "OPERADOR"
        };

        var repositoryMock = new Mock<IRepository<Usuario>>();
        _unitOfWorkMock.Setup(u => u.Repository<Usuario>()).Returns(repositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
        _mapperMock.Setup(m => m.Map<UsuarioDto>(It.IsAny<Usuario>())).Returns(new UsuarioDto());

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        repositoryMock.Verify(r => r.AddAsync(It.Is<Usuario>(u => 
            u.PasswordHash != command.Password && 
            !string.IsNullOrEmpty(u.PasswordHash))), Times.Once);
    }
}
