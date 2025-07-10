using FluentAssertions;
using Moq;
using SistemaPeaje.Application.Features.Transacciones.Commands;
using SistemaPeaje.Application.Features.Transacciones.Handlers;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;
using Xunit;

namespace SistemaPeaje.Tests.Application.Handlers;

public class UpdateTransaccionCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly UpdateTransaccionCommandHandler _handler;

    public UpdateTransaccionCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new UpdateTransaccionCommandHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_UpdatesTransaccion()
    {
        // Arrange
        var command = new UpdateTransaccionCommand
        {
            Id = 1,
            EstacionId = 2,
            CarrilId = 3,
            TipoVehiculoId = 2,
            TipoPagoId = 1,
            Monto = 35.75m,
            PlacaVehiculo = "XYZ789",
            TagRFID = "RF123456",
            ClienteId = 1,
            EmpleadoId = 2,
            Observaciones = "Transacción actualizada"
        };

        var existingTransaccion = new Transaccion
        {
            Id = 1,
            EstacionId = 1,
            CarrilId = 1,
            TipoVehiculoId = 1,
            TipoPagoId = 1,
            Monto = 25.50m,
            PlacaVehiculo = "ABC123",
            TagRFID = null,
            ClienteId = null,
            EmpleadoId = 1,
            Observaciones = "Transacción original",
            FechaTransaccion = DateTime.UtcNow.AddHours(-2),
            FechaActualizacion = null
        };

        var repositoryMock = new Mock<IRepository<Transaccion>>();
        repositoryMock.Setup(r => r.GetByIdAsync(command.Id))
                     .ReturnsAsync(existingTransaccion);

        repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Transaccion>()))
                     .Returns(Task.CompletedTask);

        _unitOfWorkMock.Setup(u => u.Repository<Transaccion>())
                       .Returns(repositoryMock.Object);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                       .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        // Verificar que la transacción fue actualizada
        existingTransaccion.EstacionId.Should().Be(command.EstacionId);
        existingTransaccion.CarrilId.Should().Be(command.CarrilId);
        existingTransaccion.TipoVehiculoId.Should().Be(command.TipoVehiculoId);
        existingTransaccion.TipoPagoId.Should().Be(command.TipoPagoId);
        existingTransaccion.Monto.Should().Be(command.Monto);
        existingTransaccion.PlacaVehiculo.Should().Be(command.PlacaVehiculo);
        existingTransaccion.TagRFID.Should().Be(command.TagRFID);
        existingTransaccion.ClienteId.Should().Be(command.ClienteId);
        existingTransaccion.EmpleadoId.Should().Be(command.EmpleadoId);
        existingTransaccion.Observaciones.Should().Be(command.Observaciones);
        existingTransaccion.FechaActualizacion.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        repositoryMock.Verify(r => r.GetByIdAsync(command.Id), Times.Once);
        repositoryMock.Verify(r => r.UpdateAsync(existingTransaccion), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_TransaccionNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var command = new UpdateTransaccionCommand
        {
            Id = 999,
            EstacionId = 1,
            CarrilId = 1,
            TipoVehiculoId = 1,
            TipoPagoId = 1,
            Monto = 25.50m,
            PlacaVehiculo = "ABC123"
        };

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
        repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Transaccion>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_ValidCommandWithNullOptionalFields_UpdatesSuccessfully()
    {
        // Arrange
        var command = new UpdateTransaccionCommand
        {
            Id = 1,
            EstacionId = 1,
            CarrilId = 1,
            TipoVehiculoId = 1,
            TipoPagoId = 1,
            Monto = 25.50m,
            PlacaVehiculo = "ABC123",
            TagRFID = null, // Campo opcional nulo
            ClienteId = null, // Campo opcional nulo
            EmpleadoId = 1,
            Observaciones = null // Campo opcional nulo
        };

        var existingTransaccion = new Transaccion
        {
            Id = 1,
            EstacionId = 2,
            CarrilId = 2,
            TipoVehiculoId = 2,
            TipoPagoId = 2,
            Monto = 30.00m,
            PlacaVehiculo = "OLD123",
            TagRFID = "OLD_RFID",
            ClienteId = 5,
            EmpleadoId = 2,
            Observaciones = "Old observations",
            FechaTransaccion = DateTime.UtcNow.AddHours(-1)
        };

        var repositoryMock = new Mock<IRepository<Transaccion>>();
        repositoryMock.Setup(r => r.GetByIdAsync(command.Id))
                     .ReturnsAsync(existingTransaccion);

        repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Transaccion>()))
                     .Returns(Task.CompletedTask);

        _unitOfWorkMock.Setup(u => u.Repository<Transaccion>())
                       .Returns(repositoryMock.Object);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                       .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        existingTransaccion.EstacionId.Should().Be(command.EstacionId);
        existingTransaccion.PlacaVehiculo.Should().Be(command.PlacaVehiculo);
        existingTransaccion.TagRFID.Should().BeNull();
        existingTransaccion.ClienteId.Should().BeNull();
        existingTransaccion.Observaciones.Should().BeNull();

        repositoryMock.Verify(r => r.UpdateAsync(existingTransaccion), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ZeroMonto_UpdatesSuccessfully()
    {
        // Arrange
        var command = new UpdateTransaccionCommand
        {
            Id = 1,
            EstacionId = 1,
            CarrilId = 1,
            TipoVehiculoId = 1,
            TipoPagoId = 1,
            Monto = 0.00m, // Monto cero
            PlacaVehiculo = "ABC123",
            EmpleadoId = 1
        };

        var existingTransaccion = new Transaccion
        {
            Id = 1,
            EstacionId = 1,
            CarrilId = 1,
            TipoVehiculoId = 1,
            TipoPagoId = 1,
            Monto = 25.50m,
            PlacaVehiculo = "ABC123",
            EmpleadoId = 1,
            FechaTransaccion = DateTime.UtcNow.AddHours(-1)
        };

        var repositoryMock = new Mock<IRepository<Transaccion>>();
        repositoryMock.Setup(r => r.GetByIdAsync(command.Id))
                     .ReturnsAsync(existingTransaccion);

        repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Transaccion>()))
                     .Returns(Task.CompletedTask);

        _unitOfWorkMock.Setup(u => u.Repository<Transaccion>())
                       .Returns(repositoryMock.Object);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                       .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        existingTransaccion.Monto.Should().Be(0.00m);

        repositoryMock.Verify(r => r.UpdateAsync(existingTransaccion), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
}
