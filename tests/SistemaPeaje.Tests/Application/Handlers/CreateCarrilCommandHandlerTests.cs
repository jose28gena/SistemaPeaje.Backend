using AutoMapper;
using FluentAssertions;
using Moq;
using SistemaPeaje.Application.Features.Carriles;
using SistemaPeaje.Application.Mappings;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;
using Xunit;

namespace SistemaPeaje.Tests.Application.Handlers;

public class CreateCarrilCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreateCarrilHandler _handler;

    public CreateCarrilCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _handler = new CreateCarrilHandler(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsCarrilDto()
    {
        // Arrange
        var command = new CreateCarrilCommand
        {
            EstacionId = 1,
            Numero = "C01",
            Tipo = "Manual",
            Estado = "Activo"
        };

        var carril = new Carril
        {
            Id = 1,
            EstacionId = command.EstacionId,
            Numero = command.Numero,
            Tipo = command.Tipo,
            Estado = command.Estado
        };

        var carrilDto = new CarrilDto
        {
            Id = 1,
            EstacionId = command.EstacionId,
            Numero = command.Numero,
            Tipo = command.Tipo,
            Estado = command.Estado
        };

        var repositoryMock = new Mock<IRepository<Carril>>();
        repositoryMock.Setup(r => r.AddAsync(It.IsAny<Carril>()))
                     .ReturnsAsync(carril);

        _unitOfWorkMock.Setup(u => u.Repository<Carril>())
                       .Returns(repositoryMock.Object);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                       .ReturnsAsync(1);

        _mapperMock.Setup(m => m.Map<CarrilDto>(It.IsAny<Carril>()))
                   .Returns(carrilDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.EstacionId.Should().Be(1);
        result.Numero.Should().Be("C01");
        result.Tipo.Should().Be("Manual");
        result.Estado.Should().Be("Activo");

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        repositoryMock.Verify(r => r.AddAsync(It.IsAny<Carril>()), Times.Once);
        _mapperMock.Verify(m => m.Map<CarrilDto>(It.IsAny<Carril>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidCommand_DefaultEstadoActive()
    {
        // Arrange
        var command = new CreateCarrilCommand
        {
            EstacionId = 1,
            Numero = "C01",
            Tipo = "Manual"
            // Estado not set, should default to "Activo"
        };

        var carril = new Carril
        {
            Id = 1,
            EstacionId = command.EstacionId,
            Numero = command.Numero,
            Tipo = command.Tipo,
            Estado = "Activo"
        };

        var carrilDto = new CarrilDto
        {
            Id = 1,
            EstacionId = command.EstacionId,
            Numero = command.Numero,
            Tipo = command.Tipo,
            Estado = "Activo"
        };

        var repositoryMock = new Mock<IRepository<Carril>>();
        repositoryMock.Setup(r => r.AddAsync(It.IsAny<Carril>()))
                     .ReturnsAsync(carril);

        _unitOfWorkMock.Setup(u => u.Repository<Carril>())
                       .Returns(repositoryMock.Object);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                       .ReturnsAsync(1);

        _mapperMock.Setup(m => m.Map<CarrilDto>(It.IsAny<Carril>()))
                   .Returns(carrilDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Estado.Should().Be("Activo");

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        repositoryMock.Verify(r => r.AddAsync(It.IsAny<Carril>()), Times.Once);
    }
}
