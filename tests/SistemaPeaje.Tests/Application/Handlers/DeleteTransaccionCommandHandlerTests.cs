using FluentAssertions;
using Moq;
using SistemaPeaje.Application.Features.Transacciones.Commands;
using SistemaPeaje.Application.Features.Transacciones.Handlers;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;
using Xunit;

namespace SistemaPeaje.Tests.Application.Handlers;

public class DeleteTransaccionCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly DeleteTransaccionCommandHandler _handler;

    public DeleteTransaccionCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new DeleteTransaccionCommandHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ValidId_DeletesTransaccion()
    {
        // Arrange
        var command = new DeleteTransaccionCommand { Id = 1 };

        var existingTransaccion = new Transaccion
        {
            Id = 1,
            EstacionId = 1,
            CarrilId = 1,
            TipoVehiculoId = 1,
            TipoPagoId = 1,
            Monto = 25.50m,
            PlacaVehiculo = "ABC123",
            FechaTransaccion = DateTime.UtcNow.AddHours(-2)
        };

        var repositoryMock = new Mock<IRepository<Transaccion>>();
        repositoryMock.Setup(r => r.GetByIdAsync(command.Id))
                     .ReturnsAsync(existingTransaccion);

        repositoryMock.Setup(r => r.DeleteAsync(It.IsAny<Transaccion>()))
                     .Returns(Task.CompletedTask);

        _unitOfWorkMock.Setup(u => u.Repository<Transaccion>())
                       .Returns(repositoryMock.Object);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                       .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        repositoryMock.Verify(r => r.GetByIdAsync(command.Id), Times.Once);
        repositoryMock.Verify(r => r.DeleteAsync(existingTransaccion), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_TransaccionNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var command = new DeleteTransaccionCommand { Id = 999 };

        var repositoryMock = new Mock<IRepository<Transaccion>>();
        repositoryMock.Setup(r => r.GetByIdAsync(command.Id))
                     .ReturnsAsync((Transaccion)null!);

        _unitOfWorkMock.Setup(u => u.Repository<Transaccion>())
                       .Returns(repositoryMock.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _handler.Handle(command, CancellationToken.None));

        exception.Message.Should().Contain("Transacción con ID 999 no encontrada");

        repositoryMock.Verify(r => r.GetByIdAsync(command.Id), Times.Once);
        repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Transaccion>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_ZeroId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var command = new DeleteTransaccionCommand { Id = 0 };

        var repositoryMock = new Mock<IRepository<Transaccion>>();
        repositoryMock.Setup(r => r.GetByIdAsync(command.Id))
                     .ReturnsAsync((Transaccion)null!);

        _unitOfWorkMock.Setup(u => u.Repository<Transaccion>())
                       .Returns(repositoryMock.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _handler.Handle(command, CancellationToken.None));

        exception.Message.Should().Contain("Transacción con ID 0 no encontrada");

        repositoryMock.Verify(r => r.GetByIdAsync(command.Id), Times.Once);
        repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Transaccion>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_NegativeId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var command = new DeleteTransaccionCommand { Id = -1 };

        var repositoryMock = new Mock<IRepository<Transaccion>>();
        repositoryMock.Setup(r => r.GetByIdAsync(command.Id))
                     .ReturnsAsync((Transaccion)null!);

        _unitOfWorkMock.Setup(u => u.Repository<Transaccion>())
                       .Returns(repositoryMock.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _handler.Handle(command, CancellationToken.None));

        exception.Message.Should().Contain("Transacción con ID -1 no encontrada");

        repositoryMock.Verify(r => r.GetByIdAsync(command.Id), Times.Once);
        repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Transaccion>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_ValidTransaccionWithComplexData_DeletesSuccessfully()
    {
        // Arrange
        var command = new DeleteTransaccionCommand { Id = 5 };

        var complexTransaccion = new Transaccion
        {
            Id = 5,
            EstacionId = 2,
            CarrilId = 3,
            TipoVehiculoId = 2,
            TipoPagoId = 2,
            Monto = 125.75m,
            PlacaVehiculo = "XYZ999",
            TagRFID = "RF987654",
            ClienteId = 10,
            EmpleadoId = 5,
            Observaciones = "Transacción compleja con todos los campos",
            FechaTransaccion = DateTime.UtcNow.AddDays(-1),
            FechaActualizacion = DateTime.UtcNow.AddHours(-2)
        };

        var repositoryMock = new Mock<IRepository<Transaccion>>();
        repositoryMock.Setup(r => r.GetByIdAsync(command.Id))
                     .ReturnsAsync(complexTransaccion);

        repositoryMock.Setup(r => r.DeleteAsync(It.IsAny<Transaccion>()))
                     .Returns(Task.CompletedTask);

        _unitOfWorkMock.Setup(u => u.Repository<Transaccion>())
                       .Returns(repositoryMock.Object);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                       .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        repositoryMock.Verify(r => r.GetByIdAsync(command.Id), Times.Once);
        repositoryMock.Verify(r => r.DeleteAsync(complexTransaccion), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
}
