using AutoMapper;
using FluentAssertions;
using Moq;
using SistemaPeaje.Application.Features.Tarifas;
using SistemaPeaje.Application.Mappings;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;
using System.Linq.Expressions;
using Xunit;

namespace SistemaPeaje.Tests.Application.Handlers;

public class CreateTarifaCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreateTarifaHandler _handler;

    public CreateTarifaCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _handler = new CreateTarifaHandler(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommandWithoutOverlapping_ReturnsNuevaTarifa()
    {
        // Arrange
        var command = new CreateTarifaCommand
        {
            TipoVehiculoId = 1,
            EstacionId = 1,
            Monto = 25.50m,
            FechaVigenciaInicio = DateTime.UtcNow.AddDays(1),
            FechaVigenciaFin = DateTime.UtcNow.AddMonths(12)
        };

        var nuevaTarifa = new Tarifa
        {
            Id = 1,
            TipoVehiculoId = command.TipoVehiculoId,
            EstacionId = command.EstacionId,
            Monto = command.Monto,
            FechaVigenciaInicio = command.FechaVigenciaInicio,
            FechaVigenciaFin = command.FechaVigenciaFin,
            EsVigente = true
        };

        var tarifaDto = new TarifaDto
        {
            Id = 1,
            TipoVehiculoId = command.TipoVehiculoId,
            EstacionId = command.EstacionId,
            Monto = command.Monto,
            FechaVigenciaInicio = command.FechaVigenciaInicio,
            FechaVigenciaFin = command.FechaVigenciaFin,
            EsVigente = true
        };

        var repositoryMock = new Mock<IRepository<Tarifa>>();
        
        // No overlapping tariffs
        repositoryMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Tarifa, bool>>>()))
                     .ReturnsAsync(new List<Tarifa>());

        repositoryMock.Setup(r => r.AddAsync(It.IsAny<Tarifa>()))
                     .ReturnsAsync(nuevaTarifa);

        _unitOfWorkMock.Setup(u => u.Repository<Tarifa>())
                       .Returns(repositoryMock.Object);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                       .ReturnsAsync(1);

        _mapperMock.Setup(m => m.Map<TarifaDto>(It.IsAny<Tarifa>()))
                   .Returns(tarifaDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.TipoVehiculoId.Should().Be(1);
        result.EstacionId.Should().Be(1);
        result.Monto.Should().Be(25.50m);
        result.EsVigente.Should().BeTrue();

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        repositoryMock.Verify(r => r.AddAsync(It.IsAny<Tarifa>()), Times.Once);
        repositoryMock.Verify(r => r.GetAsync(It.IsAny<Expression<Func<Tarifa, bool>>>()), Times.Once);
        _mapperMock.Verify(m => m.Map<TarifaDto>(It.IsAny<Tarifa>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidCommandWithOverlappingTariffs_DeactivatesExistingAndCreatesNew()
    {
        // Arrange
        var command = new CreateTarifaCommand
        {
            TipoVehiculoId = 1,
            EstacionId = 1,
            Monto = 30.00m,
            FechaVigenciaInicio = DateTime.UtcNow.AddDays(1),
            FechaVigenciaFin = DateTime.UtcNow.AddMonths(12)
        };

        var existingTarifa = new Tarifa
        {
            Id = 1,
            TipoVehiculoId = 1,
            EstacionId = 1,
            Monto = 20.00m,
            FechaVigenciaInicio = DateTime.UtcNow.AddDays(-30),
            FechaVigenciaFin = DateTime.UtcNow.AddMonths(6),
            EsVigente = true
        };

        var nuevaTarifa = new Tarifa
        {
            Id = 2,
            TipoVehiculoId = command.TipoVehiculoId,
            EstacionId = command.EstacionId,
            Monto = command.Monto,
            FechaVigenciaInicio = command.FechaVigenciaInicio,
            FechaVigenciaFin = command.FechaVigenciaFin,
            EsVigente = true
        };

        var tarifaDto = new TarifaDto
        {
            Id = 2,
            TipoVehiculoId = command.TipoVehiculoId,
            EstacionId = command.EstacionId,
            Monto = command.Monto,
            FechaVigenciaInicio = command.FechaVigenciaInicio,
            FechaVigenciaFin = command.FechaVigenciaFin,
            EsVigente = true
        };

        var repositoryMock = new Mock<IRepository<Tarifa>>();
        
        // Existing overlapping tariff
        repositoryMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Tarifa, bool>>>()))
                     .ReturnsAsync(new List<Tarifa> { existingTarifa });

        repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Tarifa>()))
                     .Returns(Task.CompletedTask);

        repositoryMock.Setup(r => r.AddAsync(It.IsAny<Tarifa>()))
                     .ReturnsAsync(nuevaTarifa);

        _unitOfWorkMock.Setup(u => u.Repository<Tarifa>())
                       .Returns(repositoryMock.Object);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                       .ReturnsAsync(1);

        _mapperMock.Setup(m => m.Map<TarifaDto>(It.IsAny<Tarifa>()))
                   .Returns(tarifaDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(2);
        result.Monto.Should().Be(30.00m);
        result.EsVigente.Should().BeTrue();

        // Verify that existing tariff was deactivated
        existingTarifa.EsVigente.Should().BeFalse();
        existingTarifa.FechaVigenciaFin.Should().Be(command.FechaVigenciaInicio.AddDays(-1));

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Tarifa>()), Times.Once);
        repositoryMock.Verify(r => r.AddAsync(It.IsAny<Tarifa>()), Times.Once);
    }

    [Fact]
    public async Task Handle_TarifaWithoutEstacion_CreatesGlobalTarifa()
    {
        // Arrange
        var command = new CreateTarifaCommand
        {
            TipoVehiculoId = 2,
            EstacionId = null, // Tarifa global
            Monto = 15.75m,
            FechaVigenciaInicio = DateTime.UtcNow,
            FechaVigenciaFin = null // Sin fecha fin
        };

        var nuevaTarifa = new Tarifa
        {
            Id = 3,
            TipoVehiculoId = command.TipoVehiculoId,
            EstacionId = null,
            Monto = command.Monto,
            FechaVigenciaInicio = command.FechaVigenciaInicio,
            FechaVigenciaFin = null,
            EsVigente = true
        };

        var tarifaDto = new TarifaDto
        {
            Id = 3,
            TipoVehiculoId = command.TipoVehiculoId,
            EstacionId = null,
            Monto = command.Monto,
            FechaVigenciaInicio = command.FechaVigenciaInicio,
            FechaVigenciaFin = null,
            EsVigente = true
        };

        var repositoryMock = new Mock<IRepository<Tarifa>>();
        
        repositoryMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Tarifa, bool>>>()))
                     .ReturnsAsync(new List<Tarifa>());

        repositoryMock.Setup(r => r.AddAsync(It.IsAny<Tarifa>()))
                     .ReturnsAsync(nuevaTarifa);

        _unitOfWorkMock.Setup(u => u.Repository<Tarifa>())
                       .Returns(repositoryMock.Object);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                       .ReturnsAsync(1);

        _mapperMock.Setup(m => m.Map<TarifaDto>(It.IsAny<Tarifa>()))
                   .Returns(tarifaDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.EstacionId.Should().BeNull();
        result.FechaVigenciaFin.Should().BeNull();
        result.TipoVehiculoId.Should().Be(2);
        result.Monto.Should().Be(15.75m);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        repositoryMock.Verify(r => r.AddAsync(It.IsAny<Tarifa>()), Times.Once);
    }
}
