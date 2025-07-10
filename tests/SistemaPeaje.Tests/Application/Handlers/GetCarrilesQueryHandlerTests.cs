using AutoMapper;
using FluentAssertions;
using Moq;
using SistemaPeaje.Application.Features.Carriles;
using SistemaPeaje.Application.Mappings;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;
using Xunit;

namespace SistemaPeaje.Tests.Application.Handlers;

public class GetCarrilesQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetCarrilesHandler _handler;

    public GetCarrilesQueryHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetCarrilesHandler(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ValidQuery_ReturnsCarrilDtoList()
    {
        // Arrange
        var query = new GetCarrilesQuery();

        var carriles = new List<Carril>
        {
            new Carril { Id = 1, EstacionId = 1, Numero = "C01", Tipo = "Manual", Estado = "Activo" },
            new Carril { Id = 2, EstacionId = 1, Numero = "C02", Tipo = "Automático", Estado = "Activo" },
            new Carril { Id = 3, EstacionId = 2, Numero = "C01", Tipo = "Manual", Estado = "Inactivo" }
        };

        var carrilDtos = new List<CarrilDto>
        {
            new CarrilDto { Id = 1, EstacionId = 1, Numero = "C01", Tipo = "Manual", Estado = "Activo" },
            new CarrilDto { Id = 2, EstacionId = 1, Numero = "C02", Tipo = "Automático", Estado = "Activo" },
            new CarrilDto { Id = 3, EstacionId = 2, Numero = "C01", Tipo = "Manual", Estado = "Inactivo" }
        };

        var repositoryMock = new Mock<IRepository<Carril>>();
        repositoryMock.Setup(r => r.GetAllAsync())
                     .ReturnsAsync(carriles);

        _unitOfWorkMock.Setup(u => u.Repository<Carril>())
                       .Returns(repositoryMock.Object);

        _mapperMock.Setup(m => m.Map<List<CarrilDto>>(It.IsAny<List<Carril>>()))
                   .Returns(carrilDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result[0].Id.Should().Be(1);
        result[0].Numero.Should().Be("C01");
        result[1].Id.Should().Be(2);
        result[1].Tipo.Should().Be("Automático");
        result[2].Estado.Should().Be("Inactivo");

        repositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
        _mapperMock.Verify(m => m.Map<List<CarrilDto>>(It.IsAny<List<Carril>>()), Times.Once);
    }

    [Fact]
    public async Task Handle_EmptyDatabase_ReturnsEmptyList()
    {
        // Arrange
        var query = new GetCarrilesQuery();
        var emptyCarriles = new List<Carril>();
        var emptyCarrilDtos = new List<CarrilDto>();

        var repositoryMock = new Mock<IRepository<Carril>>();
        repositoryMock.Setup(r => r.GetAllAsync())
                     .ReturnsAsync(emptyCarriles);

        _unitOfWorkMock.Setup(u => u.Repository<Carril>())
                       .Returns(repositoryMock.Object);

        _mapperMock.Setup(m => m.Map<List<CarrilDto>>(It.IsAny<List<Carril>>()))
                   .Returns(emptyCarrilDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();

        repositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
        _mapperMock.Verify(m => m.Map<List<CarrilDto>>(It.IsAny<List<Carril>>()), Times.Once);
    }
}
