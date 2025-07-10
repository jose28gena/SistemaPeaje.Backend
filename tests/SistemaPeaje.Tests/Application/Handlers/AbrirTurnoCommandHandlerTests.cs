using AutoMapper;
using FluentAssertions;
using Moq;
using SistemaPeaje.Application.Features.Turnos;
using SistemaPeaje.Application.Mappings;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;
using System.Linq.Expressions;
using Xunit;

namespace SistemaPeaje.Tests.Application.Handlers;

public class AbrirTurnoCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly AbrirTurnoHandler _handler;

    public AbrirTurnoCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _handler = new AbrirTurnoHandler(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommandNoOpenTurns_ReturnsTurnoDto()
    {
        // Arrange
        var command = new AbrirTurnoCommand
        {
            EmpleadoId = 1,
            EstacionId = 1,
            MontoInicialCaja = 500.00m
        };

        var turno = new Turno
        {
            Id = 1,
            EmpleadoId = command.EmpleadoId,
            EstacionId = command.EstacionId,
            FechaInicio = DateTime.UtcNow,
            MontoInicialCaja = command.MontoInicialCaja,
            Estado = "Abierto"
        };

        var turnoDto = new TurnoDto
        {
            Id = 1,
            EmpleadoId = command.EmpleadoId,
            EstacionId = command.EstacionId,
            FechaInicio = turno.FechaInicio,
            MontoInicialCaja = command.MontoInicialCaja,
            Estado = "Abierto"
        };

        var repositoryMock = new Mock<IRepository<Turno>>();
        
        // No hay turnos abiertos
        repositoryMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Turno, bool>>>()))
                     .ReturnsAsync(new List<Turno>());

        repositoryMock.Setup(r => r.AddAsync(It.IsAny<Turno>()))
                     .ReturnsAsync(turno);

        _unitOfWorkMock.Setup(u => u.Repository<Turno>())
                       .Returns(repositoryMock.Object);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                       .ReturnsAsync(1);

        _mapperMock.Setup(m => m.Map<TurnoDto>(It.IsAny<Turno>()))
                   .Returns(turnoDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.EmpleadoId.Should().Be(1);
        result.EstacionId.Should().Be(1);
        result.MontoInicialCaja.Should().Be(500.00m);
        result.Estado.Should().Be("Abierto");

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        repositoryMock.Verify(r => r.AddAsync(It.IsAny<Turno>()), Times.Once);
        repositoryMock.Verify(r => r.GetAsync(It.IsAny<Expression<Func<Turno, bool>>>()), Times.Once);
        _mapperMock.Verify(m => m.Map<TurnoDto>(It.IsAny<Turno>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ExistingOpenTurnForEmployee_ThrowsInvalidOperationException()
    {
        // Arrange
        var command = new AbrirTurnoCommand
        {
            EmpleadoId = 1,
            EstacionId = 2,
            MontoInicialCaja = 500.00m
        };

        var existingTurno = new Turno
        {
            Id = 1,
            EmpleadoId = 1,
            EstacionId = 1,
            FechaInicio = DateTime.UtcNow.AddHours(-2),
            MontoInicialCaja = 300.00m,
            Estado = "Abierto"
        };

        var repositoryMock = new Mock<IRepository<Turno>>();
        
        // Hay un turno abierto para el mismo empleado
        repositoryMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Turno, bool>>>()))
                     .ReturnsAsync(new List<Turno> { existingTurno });

        _unitOfWorkMock.Setup(u => u.Repository<Turno>())
                       .Returns(repositoryMock.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, CancellationToken.None));

        exception.Message.Should().Contain("Ya existe un turno abierto");
        exception.Message.Should().Contain("empleado 1");

        repositoryMock.Verify(r => r.GetAsync(It.IsAny<Expression<Func<Turno, bool>>>()), Times.Once);
        repositoryMock.Verify(r => r.AddAsync(It.IsAny<Turno>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_ExistingOpenTurnForStation_ThrowsInvalidOperationException()
    {
        // Arrange
        var command = new AbrirTurnoCommand
        {
            EmpleadoId = 2,
            EstacionId = 1,
            MontoInicialCaja = 500.00m
        };

        var existingTurno = new Turno
        {
            Id = 1,
            EmpleadoId = 1,
            EstacionId = 1,
            FechaInicio = DateTime.UtcNow.AddHours(-2),
            MontoInicialCaja = 300.00m,
            Estado = "Abierto"
        };

        var repositoryMock = new Mock<IRepository<Turno>>();
        
        // Hay un turno abierto para la misma estación
        repositoryMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Turno, bool>>>()))
                     .ReturnsAsync(new List<Turno> { existingTurno });

        _unitOfWorkMock.Setup(u => u.Repository<Turno>())
                       .Returns(repositoryMock.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, CancellationToken.None));

        exception.Message.Should().Contain("Ya existe un turno abierto");
        exception.Message.Should().Contain("estación 1");

        repositoryMock.Verify(r => r.GetAsync(It.IsAny<Expression<Func<Turno, bool>>>()), Times.Once);
        repositoryMock.Verify(r => r.AddAsync(It.IsAny<Turno>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_ValidCommandZeroInitialAmount_AllowsZeroAmount()
    {
        // Arrange
        var command = new AbrirTurnoCommand
        {
            EmpleadoId = 3,
            EstacionId = 3,
            MontoInicialCaja = 0.00m
        };

        var turno = new Turno
        {
            Id = 2,
            EmpleadoId = command.EmpleadoId,
            EstacionId = command.EstacionId,
            FechaInicio = DateTime.UtcNow,
            MontoInicialCaja = 0.00m,
            Estado = "Abierto"
        };

        var turnoDto = new TurnoDto
        {
            Id = 2,
            EmpleadoId = command.EmpleadoId,
            EstacionId = command.EstacionId,
            FechaInicio = turno.FechaInicio,
            MontoInicialCaja = 0.00m,
            Estado = "Abierto"
        };

        var repositoryMock = new Mock<IRepository<Turno>>();
        
        repositoryMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Turno, bool>>>()))
                     .ReturnsAsync(new List<Turno>());

        repositoryMock.Setup(r => r.AddAsync(It.IsAny<Turno>()))
                     .ReturnsAsync(turno);

        _unitOfWorkMock.Setup(u => u.Repository<Turno>())
                       .Returns(repositoryMock.Object);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                       .ReturnsAsync(1);

        _mapperMock.Setup(m => m.Map<TurnoDto>(It.IsAny<Turno>()))
                   .Returns(turnoDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.MontoInicialCaja.Should().Be(0.00m);
        result.Estado.Should().Be("Abierto");

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        repositoryMock.Verify(r => r.AddAsync(It.IsAny<Turno>()), Times.Once);
    }
}
