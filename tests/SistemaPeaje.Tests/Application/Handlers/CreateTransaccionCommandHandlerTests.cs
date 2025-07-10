using AutoMapper;
using FluentAssertions;
using Moq;
using SistemaPeaje.Application.DTOs;
using SistemaPeaje.Application.Features.Transacciones.Commands;
using SistemaPeaje.Application.Features.Transacciones.Handlers;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;
using Xunit;

namespace SistemaPeaje.Tests.Application.Handlers;

public class CreateTransaccionCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreateTransaccionCommandHandler _handler;

    public CreateTransaccionCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _handler = new CreateTransaccionCommandHandler(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsTransaccionDto()
    {
        // Arrange
        var command = new CreateTransaccionCommand
        {
            EstacionId = 1,
            CarrilId = 1,
            TipoVehiculoId = 1,
            TipoPagoId = 1,
            Monto = 100.50m,
            PlacaVehiculo = "ABC123"
        };

        var transaccion = new Transaccion
        {
            Id = 1,
            EstacionId = command.EstacionId,
            CarrilId = command.CarrilId,
            TipoVehiculoId = command.TipoVehiculoId,
            TipoPagoId = command.TipoPagoId,
            Monto = command.Monto,
            PlacaVehiculo = command.PlacaVehiculo
        };

        var transaccionDto = new TransaccionDto
        {
            Id = 1,
            EstacionId = command.EstacionId,
            CarrilId = command.CarrilId,
            TipoVehiculoId = command.TipoVehiculoId,
            TipoPagoId = command.TipoPagoId,
            Monto = command.Monto,
            PlacaVehiculo = command.PlacaVehiculo
        };

        var repositoryMock = new Mock<IRepository<Transaccion>>();
        repositoryMock.Setup(r => r.AddAsync(It.IsAny<Transaccion>()))
                     .ReturnsAsync(transaccion);

        _unitOfWorkMock.Setup(u => u.Repository<Transaccion>())
                       .Returns(repositoryMock.Object);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                       .ReturnsAsync(1);

        _mapperMock.Setup(m => m.Map<TransaccionDto>(It.IsAny<Transaccion>()))
                   .Returns(transaccionDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Monto.Should().Be(100.50m);
        result.PlacaVehiculo.Should().Be("ABC123");

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        repositoryMock.Verify(r => r.AddAsync(It.IsAny<Transaccion>()), Times.Once);
        _mapperMock.Verify(m => m.Map<TransaccionDto>(It.IsAny<Transaccion>()), Times.Once);
    }
}
