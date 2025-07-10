using AutoMapper;
using FluentAssertions;
using Moq;
using SistemaPeaje.Application.Features.Transacciones.Handlers;
using SistemaPeaje.Application.Features.Transacciones.Queries;
using SistemaPeaje.Application.DTOs;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;
using Xunit;

namespace SistemaPeaje.Tests.Application.Handlers;

public class GetTransaccionByIdQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetTransaccionByIdQueryHandler _handler;

    public GetTransaccionByIdQueryHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetTransaccionByIdQueryHandler(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ValidId_ReturnsTransaccionDto()
    {
        // Arrange
        var query = new GetTransaccionByIdQuery { Id = 1 };

        var transaccion = new Transaccion
        {
            Id = 1,
            EstacionId = 1,
            CarrilId = 1,
            TipoVehiculoId = 1,
            TipoPagoId = 1,
            Monto = 25.50m,
            PlacaVehiculo = "ABC123",
            FechaTransaccion = DateTime.UtcNow
        };

        var transaccionDto = new TransaccionDto
        {
            Id = 1,
            EstacionId = 1,
            CarrilId = 1,
            TipoVehiculoId = 1,
            TipoPagoId = 1,
            Monto = 25.50m,
            PlacaVehiculo = "ABC123",
            FechaTransaccion = transaccion.FechaTransaccion
        };

        var repositoryMock = new Mock<IRepository<Transaccion>>();
        repositoryMock.Setup(r => r.GetByIdAsync(query.Id))
                     .ReturnsAsync(transaccion);

        _unitOfWorkMock.Setup(u => u.Repository<Transaccion>())
                       .Returns(repositoryMock.Object);

        _mapperMock.Setup(m => m.Map<TransaccionDto>(It.IsAny<Transaccion>()))
                   .Returns(transaccionDto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.EstacionId.Should().Be(1);
        result.PlacaVehiculo.Should().Be("ABC123");
        result.Monto.Should().Be(25.50m);

        repositoryMock.Verify(r => r.GetByIdAsync(query.Id), Times.Once);
        _mapperMock.Verify(m => m.Map<TransaccionDto>(It.IsAny<Transaccion>()), Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var query = new GetTransaccionByIdQuery { Id = 999 };

        var repositoryMock = new Mock<IRepository<Transaccion>>();
        repositoryMock.Setup(r => r.GetByIdAsync(query.Id))
                     .ReturnsAsync((Transaccion)null!);

        _unitOfWorkMock.Setup(u => u.Repository<Transaccion>())
                       .Returns(repositoryMock.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _handler.Handle(query, CancellationToken.None));

        exception.Message.Should().Contain("Transacción con ID 999 no encontrada");

        repositoryMock.Verify(r => r.GetByIdAsync(query.Id), Times.Once);
        _mapperMock.Verify(m => m.Map<TransaccionDto>(It.IsAny<Transaccion>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ZeroId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var query = new GetTransaccionByIdQuery { Id = 0 };

        var repositoryMock = new Mock<IRepository<Transaccion>>();
        repositoryMock.Setup(r => r.GetByIdAsync(query.Id))
                     .ReturnsAsync((Transaccion)null!);

        _unitOfWorkMock.Setup(u => u.Repository<Transaccion>())
                       .Returns(repositoryMock.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _handler.Handle(query, CancellationToken.None));

        exception.Message.Should().Contain("Transacción con ID 0 no encontrada");

        repositoryMock.Verify(r => r.GetByIdAsync(query.Id), Times.Once);
    }

    [Fact]
    public async Task Handle_NegativeId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var query = new GetTransaccionByIdQuery { Id = -1 };

        var repositoryMock = new Mock<IRepository<Transaccion>>();
        repositoryMock.Setup(r => r.GetByIdAsync(query.Id))
                     .ReturnsAsync((Transaccion)null!);

        _unitOfWorkMock.Setup(u => u.Repository<Transaccion>())
                       .Returns(repositoryMock.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _handler.Handle(query, CancellationToken.None));

        exception.Message.Should().Contain("Transacción con ID -1 no encontrada");

        repositoryMock.Verify(r => r.GetByIdAsync(query.Id), Times.Once);
    }
}
