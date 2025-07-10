using AutoMapper;
using FluentAssertions;
using Moq;
using SistemaPeaje.Application.Features.Empleados;
using SistemaPeaje.Application.Mappings;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;
using Xunit;

namespace SistemaPeaje.Tests.Application.Handlers;

public class CreateEmpleadoCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreateEmpleadoHandler _handler;

    public CreateEmpleadoCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _handler = new CreateEmpleadoHandler(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsEmpleadoDto()
    {
        // Arrange
        var command = new CreateEmpleadoCommand
        {
            Nombres = "Juan Carlos",
            Apellidos = "Pérez García",
            NumeroDocumento = "12345678",
            Email = "juan.perez@sistemapeaje.com",
            Telefono = "555-1234",
            Cargo = "Operador",
            EstacionId = 1
        };

        var empleado = new Empleado
        {
            Id = 1,
            Nombres = command.Nombres,
            Apellidos = command.Apellidos,
            NumeroDocumento = command.NumeroDocumento,
            Email = command.Email,
            Telefono = command.Telefono,
            Cargo = command.Cargo,
            EstacionId = command.EstacionId,
            FechaContratacion = DateTime.UtcNow,
            EsActivo = true
        };

        var empleadoDto = new EmpleadoDto
        {
            Id = 1,
            Nombres = command.Nombres,
            Apellidos = command.Apellidos,
            NumeroDocumento = command.NumeroDocumento,
            Email = command.Email,
            Telefono = command.Telefono,
            Cargo = command.Cargo,
            EstacionId = command.EstacionId,
            FechaContratacion = empleado.FechaContratacion,
            EsActivo = true
        };

        var repositoryMock = new Mock<IRepository<Empleado>>();
        repositoryMock.Setup(r => r.AddAsync(It.IsAny<Empleado>()))
                     .ReturnsAsync(empleado);

        _unitOfWorkMock.Setup(u => u.Repository<Empleado>())
                       .Returns(repositoryMock.Object);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                       .ReturnsAsync(1);

        _mapperMock.Setup(m => m.Map<EmpleadoDto>(It.IsAny<Empleado>()))
                   .Returns(empleadoDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Nombres.Should().Be("Juan Carlos");
        result.Apellidos.Should().Be("Pérez García");
        result.NumeroDocumento.Should().Be("12345678");
        result.Email.Should().Be("juan.perez@sistemapeaje.com");
        result.Cargo.Should().Be("Operador");
        result.EsActivo.Should().BeTrue();

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        repositoryMock.Verify(r => r.AddAsync(It.IsAny<Empleado>()), Times.Once);
        _mapperMock.Verify(m => m.Map<EmpleadoDto>(It.IsAny<Empleado>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidCommandWithoutEstacion_ReturnsEmpleadoDto()
    {
        // Arrange
        var command = new CreateEmpleadoCommand
        {
            Nombres = "María",
            Apellidos = "González",
            NumeroDocumento = "87654321",
            Email = "maria.gonzalez@sistemapeaje.com",
            Cargo = "Supervisor",
            EstacionId = null // Sin estación asignada
        };

        var empleado = new Empleado
        {
            Id = 2,
            Nombres = command.Nombres,
            Apellidos = command.Apellidos,
            NumeroDocumento = command.NumeroDocumento,
            Email = command.Email,
            Cargo = command.Cargo,
            EstacionId = null,
            FechaContratacion = DateTime.UtcNow,
            EsActivo = true
        };

        var empleadoDto = new EmpleadoDto
        {
            Id = 2,
            Nombres = command.Nombres,
            Apellidos = command.Apellidos,
            NumeroDocumento = command.NumeroDocumento,
            Email = command.Email,
            Cargo = command.Cargo,
            EstacionId = null,
            FechaContratacion = empleado.FechaContratacion,
            EsActivo = true
        };

        var repositoryMock = new Mock<IRepository<Empleado>>();
        repositoryMock.Setup(r => r.AddAsync(It.IsAny<Empleado>()))
                     .ReturnsAsync(empleado);

        _unitOfWorkMock.Setup(u => u.Repository<Empleado>())
                       .Returns(repositoryMock.Object);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                       .ReturnsAsync(1);

        _mapperMock.Setup(m => m.Map<EmpleadoDto>(It.IsAny<Empleado>()))
                   .Returns(empleadoDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(2);
        result.EstacionId.Should().BeNull();
        result.Cargo.Should().Be("Supervisor");
        result.EsActivo.Should().BeTrue();

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        repositoryMock.Verify(r => r.AddAsync(It.IsAny<Empleado>()), Times.Once);
    }
}
