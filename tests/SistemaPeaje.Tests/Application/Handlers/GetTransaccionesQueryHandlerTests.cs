using AutoMapper;
using FluentAssertions;
using Moq;
using SistemaPeaje.Application.Features.Transacciones.Handlers;
using SistemaPeaje.Application.Features.Transacciones.Queries;
using SistemaPeaje.Application.DTOs;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;
using System.Linq.Expressions;
using Xunit;

namespace SistemaPeaje.Tests.Application.Handlers;

public class GetTransaccionesQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetTransaccionesQueryHandler _handler;

    public GetTransaccionesQueryHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetTransaccionesQueryHandler(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ValidQuery_ReturnsTransaccionDtoList()
    {
        // Arrange
        var query = new GetTransaccionesQuery();

        var transacciones = new List<Transaccion>
        {
            new Transaccion 
            { 
                Id = 1, 
                EstacionId = 1, 
                CarrilId = 1,
                TipoVehiculoId = 1,
                TipoPagoId = 1,
                Monto = 25.50m,
                PlacaVehiculo = "ABC123",
                FechaTransaccion = DateTime.UtcNow.AddHours(-2)
            },
            new Transaccion 
            { 
                Id = 2, 
                EstacionId = 1, 
                CarrilId = 2,
                TipoVehiculoId = 2,
                TipoPagoId = 1,
                Monto = 35.75m,
                PlacaVehiculo = "XYZ789",
                FechaTransaccion = DateTime.UtcNow.AddHours(-1)
            }
        };

        var transaccionDtos = new List<TransaccionDto>
        {
            new TransaccionDto 
            { 
                Id = 1, 
                EstacionId = 1, 
                CarrilId = 1,
                TipoVehiculoId = 1,
                TipoPagoId = 1,
                Monto = 25.50m,
                PlacaVehiculo = "ABC123",
                FechaTransaccion = transacciones[0].FechaTransaccion
            },
            new TransaccionDto 
            { 
                Id = 2, 
                EstacionId = 1, 
                CarrilId = 2,
                TipoVehiculoId = 2,
                TipoPagoId = 1,
                Monto = 35.75m,
                PlacaVehiculo = "XYZ789",
                FechaTransaccion = transacciones[1].FechaTransaccion
            }
        };

        var repositoryMock = new Mock<IRepository<Transaccion>>();
        repositoryMock.Setup(r => r.GetAllAsync())
                     .ReturnsAsync(transacciones);

        _unitOfWorkMock.Setup(u => u.Repository<Transaccion>())
                       .Returns(repositoryMock.Object);

        _mapperMock.Setup(m => m.Map<IEnumerable<TransaccionDto>>(It.IsAny<IEnumerable<Transaccion>>()))
                   .Returns(transaccionDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        var resultList = result.ToList();
        resultList[0].Id.Should().Be(1);
        resultList[0].PlacaVehiculo.Should().Be("ABC123");
        resultList[1].Id.Should().Be(2);
        resultList[1].Monto.Should().Be(35.75m);

        repositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
        _mapperMock.Verify(m => m.Map<IEnumerable<TransaccionDto>>(It.IsAny<IEnumerable<Transaccion>>()), Times.Once);
    }

    [Fact]
    public async Task Handle_EmptyDatabase_ReturnsEmptyList()
    {
        // Arrange
        var query = new GetTransaccionesQuery();
        var emptyTransacciones = new List<Transaccion>();
        var emptyTransaccionDtos = new List<TransaccionDto>();

        var repositoryMock = new Mock<IRepository<Transaccion>>();
        repositoryMock.Setup(r => r.GetAllAsync())
                     .ReturnsAsync(emptyTransacciones);

        _unitOfWorkMock.Setup(u => u.Repository<Transaccion>())
                       .Returns(repositoryMock.Object);

        _mapperMock.Setup(m => m.Map<IEnumerable<TransaccionDto>>(It.IsAny<IEnumerable<Transaccion>>()))
                   .Returns(emptyTransaccionDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();

        repositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
        _mapperMock.Verify(m => m.Map<IEnumerable<TransaccionDto>>(It.IsAny<IEnumerable<Transaccion>>()), Times.Once);
    }
}
