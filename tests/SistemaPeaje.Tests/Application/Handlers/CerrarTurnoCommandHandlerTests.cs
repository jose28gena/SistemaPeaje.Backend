using AutoMapper;
using FluentAssertions;
using Moq;
using SistemaPeaje.Application.Features.Turnos;
using SistemaPeaje.Application.Mappings;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;
using Xunit;

namespace SistemaPeaje.Tests.Application.Handlers;

public class CerrarTurnoCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CerrarTurnoHandler _handler;

    public CerrarTurnoCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _handler = new CerrarTurnoHandler(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ValidOpenTurno_ClosesAndReturnsTurnoDto()
    {
        // Arrange
        var command = new CerrarTurnoCommand
        {
            TurnoId = 1,
            MontoFinalCaja = 750.50m
        };

        var turno = new Turno
        {
            Id = 1,
            EmpleadoId = 1,
            EstacionId = 1,
            FechaInicio = DateTime.UtcNow.AddHours(-8),
            MontoInicialCaja = 500.00m,
            Estado = "Abierto"
        };

        var turnoActualizado = new Turno
        {
            Id = 1,
            EmpleadoId = 1,
            EstacionId = 1,
            FechaInicio = turno.FechaInicio,
            FechaFin = DateTime.UtcNow,
            MontoInicialCaja = 500.00m,
            MontoFinalCaja = 750.50m,
            Estado = "Cerrado"
        };

        var turnoDto = new TurnoDto
        {
            Id = 1,
            EmpleadoId = 1,
            EstacionId = 1,
            FechaInicio = turno.FechaInicio,
            FechaFin = turnoActualizado.FechaFin,
            MontoInicialCaja = 500.00m,
            MontoFinalCaja = 750.50m,
            Estado = "Cerrado"
        };

        var repositoryMock = new Mock<IRepository<Turno>>();
        repositoryMock.Setup(r => r.GetByIdAsync(command.TurnoId))
                     .ReturnsAsync(turno);

        repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Turno>()))
                     .Returns(Task.CompletedTask);

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
        result.MontoFinalCaja.Should().Be(750.50m);
        result.Estado.Should().Be("Cerrado");
        result.FechaFin.Should().NotBeNull();

        // Verify that the turno was updated
        turno.Estado.Should().Be("Cerrado");
        turno.MontoFinalCaja.Should().Be(750.50m);
        turno.FechaFin.Should().NotBeNull();

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Turno>()), Times.Once);
        repositoryMock.Verify(r => r.GetByIdAsync(command.TurnoId), Times.Once);
        _mapperMock.Verify(m => m.Map<TurnoDto>(It.IsAny<Turno>()), Times.Once);
    }

    [Fact]
    public async Task Handle_TurnoNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var command = new CerrarTurnoCommand
        {
            TurnoId = 999,
            MontoFinalCaja = 750.50m
        };

        var repositoryMock = new Mock<IRepository<Turno>>();
        repositoryMock.Setup(r => r.GetByIdAsync(command.TurnoId))
                     .ReturnsAsync((Turno)null!);

        _unitOfWorkMock.Setup(u => u.Repository<Turno>())
                       .Returns(repositoryMock.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _handler.Handle(command, CancellationToken.None));

        exception.Message.Should().Contain("Turno with ID 999 not found");

        repositoryMock.Verify(r => r.GetByIdAsync(command.TurnoId), Times.Once);
        repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Turno>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_TurnoAlreadyClosed_ThrowsInvalidOperationException()
    {
        // Arrange
        var command = new CerrarTurnoCommand
        {
            TurnoId = 1,
            MontoFinalCaja = 750.50m
        };

        var turno = new Turno
        {
            Id = 1,
            EmpleadoId = 1,
            EstacionId = 1,
            FechaInicio = DateTime.UtcNow.AddHours(-10),
            FechaFin = DateTime.UtcNow.AddHours(-2),
            MontoInicialCaja = 500.00m,
            MontoFinalCaja = 600.00m,
            Estado = "Cerrado" // Ya está cerrado
        };

        var repositoryMock = new Mock<IRepository<Turno>>();
        repositoryMock.Setup(r => r.GetByIdAsync(command.TurnoId))
                     .ReturnsAsync(turno);

        _unitOfWorkMock.Setup(u => u.Repository<Turno>())
                       .Returns(repositoryMock.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, CancellationToken.None));

        exception.Message.Should().Be("El turno ya está cerrado");

        repositoryMock.Verify(r => r.GetByIdAsync(command.TurnoId), Times.Once);
        repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Turno>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_ValidTurnoWithZeroFinalAmount_AllowsZeroAmount()
    {
        // Arrange
        var command = new CerrarTurnoCommand
        {
            TurnoId = 2,
            MontoFinalCaja = 0.00m
        };

        var turno = new Turno
        {
            Id = 2,
            EmpleadoId = 2,
            EstacionId = 2,
            FechaInicio = DateTime.UtcNow.AddHours(-4),
            MontoInicialCaja = 100.00m,
            Estado = "Abierto"
        };

        var turnoDto = new TurnoDto
        {
            Id = 2,
            EmpleadoId = 2,
            EstacionId = 2,
            FechaInicio = turno.FechaInicio,
            FechaFin = DateTime.UtcNow,
            MontoInicialCaja = 100.00m,
            MontoFinalCaja = 0.00m,
            Estado = "Cerrado"
        };

        var repositoryMock = new Mock<IRepository<Turno>>();
        repositoryMock.Setup(r => r.GetByIdAsync(command.TurnoId))
                     .ReturnsAsync(turno);

        repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Turno>()))
                     .Returns(Task.CompletedTask);

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
        result.MontoFinalCaja.Should().Be(0.00m);
        result.Estado.Should().Be("Cerrado");

        turno.MontoFinalCaja.Should().Be(0.00m);
        turno.Estado.Should().Be("Cerrado");

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Turno>()), Times.Once);
    }
}
