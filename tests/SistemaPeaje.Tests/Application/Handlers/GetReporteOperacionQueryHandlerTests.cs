using FluentAssertions;
using Moq;
using SistemaPeaje.Application.Features.Reportes;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;
using System.Linq.Expressions;
using Xunit;

namespace SistemaPeaje.Tests.Application.Handlers;

public class GetReporteOperacionQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly GetReporteOperacionHandler _handler;

    public GetReporteOperacionQueryHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new GetReporteOperacionHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ValidQueryWithData_ReturnsCompleteReport()
    {
        // Arrange
        var fechaInicio = DateTime.Today.AddDays(-1);
        var fechaFin = DateTime.Today;
        var query = new GetReporteOperacionQuery
        {
            FechaInicio = fechaInicio,
            FechaFin = fechaFin,
            EstacionId = 1
        };

        var tipoPagoEfectivo = new TipoPago { Id = 1, Nombre = "Efectivo" };
        var tipoPagoTarjeta = new TipoPago { Id = 2, Nombre = "Tarjeta" };
        var tipoVehiculoAuto = new TipoVehiculo { Id = 1, Nombre = "Automóvil" };
        var tipoVehiculoCamion = new TipoVehiculo { Id = 2, Nombre = "Camión" };

        var transacciones = new List<Transaccion>
        {
            new Transaccion 
            { 
                Id = 1, 
                EstacionId = 1,
                Monto = 25.00m,
                FechaTransaccion = fechaInicio.AddHours(10),
                TipoPago = tipoPagoEfectivo,
                TipoVehiculo = tipoVehiculoAuto
            },
            new Transaccion 
            { 
                Id = 2, 
                EstacionId = 1,
                Monto = 35.00m,
                FechaTransaccion = fechaInicio.AddHours(14),
                TipoPago = tipoPagoTarjeta,
                TipoVehiculo = tipoVehiculoCamion
            },
            new Transaccion 
            { 
                Id = 3, 
                EstacionId = 1,
                Monto = 25.00m,
                FechaTransaccion = fechaInicio.AddHours(16),
                TipoPago = tipoPagoEfectivo,
                TipoVehiculo = tipoVehiculoAuto
            }
        };

        var turnos = new List<Turno>
        {
            new Turno 
            { 
                Id = 1, 
                EstacionId = 1,
                FechaInicio = fechaInicio.AddHours(8),
                Estado = "Cerrado"
            },
            new Turno 
            { 
                Id = 2, 
                EstacionId = 1,
                FechaInicio = fechaInicio.AddHours(16),
                Estado = "Abierto"
            }
        };

        var transaccionRepositoryMock = new Mock<IRepository<Transaccion>>();
        transaccionRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Transaccion, bool>>>()))
                                 .ReturnsAsync(transacciones);

        var turnoRepositoryMock = new Mock<IRepository<Turno>>();
        turnoRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Turno, bool>>>()))
                          .ReturnsAsync(turnos);

        _unitOfWorkMock.Setup(u => u.Repository<Transaccion>())
                       .Returns(transaccionRepositoryMock.Object);

        _unitOfWorkMock.Setup(u => u.Repository<Turno>())
                       .Returns(turnoRepositoryMock.Object);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.FechaInicio.Should().Be(fechaInicio);
        result.FechaFin.Should().Be(fechaFin);
        result.EstacionId.Should().Be(1);
        result.TotalTransacciones.Should().Be(3);
        result.MontoTotalRecaudado.Should().Be(85.00m);
        result.TurnosRegistrados.Should().Be(2);
        result.TurnosCerrados.Should().Be(1);

        // Verificar agrupaciones por tipo de pago
        result.TransaccionesPorTipoPago.Should().HaveCount(2);
        result.TransaccionesPorTipoPago["Efectivo"].Should().Be(2);
        result.TransaccionesPorTipoPago["Tarjeta"].Should().Be(1);

        result.MontosPorTipoPago["Efectivo"].Should().Be(50.00m);
        result.MontosPorTipoPago["Tarjeta"].Should().Be(35.00m);

        // Verificar agrupaciones por tipo de vehículo
        result.TransaccionesPorVehiculo.Should().HaveCount(2);
        result.TransaccionesPorVehiculo["Automóvil"].Should().Be(2);
        result.TransaccionesPorVehiculo["Camión"].Should().Be(1);

        result.PromedioTransaccionesPorHora.Should().BeGreaterThan(0);
        result.FechaGeneracion.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));

        transaccionRepositoryMock.Verify(r => r.GetAsync(It.IsAny<Expression<Func<Transaccion, bool>>>()), Times.Once);
        turnoRepositoryMock.Verify(r => r.GetAsync(It.IsAny<Expression<Func<Turno, bool>>>()), Times.Once);
    }

    [Fact]
    public async Task Handle_QueryWithoutEstacion_ReturnsGlobalReport()
    {
        // Arrange
        var fechaInicio = DateTime.Today.AddDays(-1);
        var fechaFin = DateTime.Today;
        var query = new GetReporteOperacionQuery
        {
            FechaInicio = fechaInicio,
            FechaFin = fechaFin,
            EstacionId = null // Sin filtro de estación
        };

        var transacciones = new List<Transaccion>
        {
            new Transaccion 
            { 
                Id = 1, 
                EstacionId = 1,
                Monto = 25.00m,
                FechaTransaccion = fechaInicio.AddHours(10),
                TipoPago = new TipoPago { Nombre = "Efectivo" },
                TipoVehiculo = new TipoVehiculo { Nombre = "Automóvil" }
            },
            new Transaccion 
            { 
                Id = 2, 
                EstacionId = 2,
                Monto = 30.00m,
                FechaTransaccion = fechaInicio.AddHours(12),
                TipoPago = new TipoPago { Nombre = "Tarjeta" },
                TipoVehiculo = new TipoVehiculo { Nombre = "Motocicleta" }
            }
        };

        var turnos = new List<Turno>
        {
            new Turno { Id = 1, EstacionId = 1, FechaInicio = fechaInicio.AddHours(8), Estado = "Cerrado" },
            new Turno { Id = 2, EstacionId = 2, FechaInicio = fechaInicio.AddHours(8), Estado = "Cerrado" }
        };

        var transaccionRepositoryMock = new Mock<IRepository<Transaccion>>();
        transaccionRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Transaccion, bool>>>()))
                                 .ReturnsAsync(transacciones);

        var turnoRepositoryMock = new Mock<IRepository<Turno>>();
        turnoRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Turno, bool>>>()))
                          .ReturnsAsync(turnos);

        _unitOfWorkMock.Setup(u => u.Repository<Transaccion>())
                       .Returns(transaccionRepositoryMock.Object);

        _unitOfWorkMock.Setup(u => u.Repository<Turno>())
                       .Returns(turnoRepositoryMock.Object);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.EstacionId.Should().BeNull();
        result.TotalTransacciones.Should().Be(2);
        result.MontoTotalRecaudado.Should().Be(55.00m);
        result.TurnosRegistrados.Should().Be(2);
        result.TurnosCerrados.Should().Be(2);
    }

    [Fact]
    public async Task Handle_EmptyData_ReturnsEmptyReport()
    {
        // Arrange
        var fechaInicio = DateTime.Today.AddDays(-1);
        var fechaFin = DateTime.Today;
        var query = new GetReporteOperacionQuery
        {
            FechaInicio = fechaInicio,
            FechaFin = fechaFin,
            EstacionId = 1
        };

        var emptyTransacciones = new List<Transaccion>();
        var emptyTurnos = new List<Turno>();

        var transaccionRepositoryMock = new Mock<IRepository<Transaccion>>();
        transaccionRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Transaccion, bool>>>()))
                                 .ReturnsAsync(emptyTransacciones);

        var turnoRepositoryMock = new Mock<IRepository<Turno>>();
        turnoRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Turno, bool>>>()))
                          .ReturnsAsync(emptyTurnos);

        _unitOfWorkMock.Setup(u => u.Repository<Transaccion>())
                       .Returns(transaccionRepositoryMock.Object);

        _unitOfWorkMock.Setup(u => u.Repository<Turno>())
                       .Returns(turnoRepositoryMock.Object);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.TotalTransacciones.Should().Be(0);
        result.MontoTotalRecaudado.Should().Be(0);
        result.TurnosRegistrados.Should().Be(0);
        result.TurnosCerrados.Should().Be(0);
        result.PromedioTransaccionesPorHora.Should().Be(0);
        result.TransaccionesPorTipoPago.Should().BeEmpty();
        result.MontosPorTipoPago.Should().BeEmpty();
        result.TransaccionesPorVehiculo.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_TransaccionesWithNullReferences_HandlesGracefully()
    {
        // Arrange
        var fechaInicio = DateTime.Today.AddDays(-1);
        var fechaFin = DateTime.Today;
        var query = new GetReporteOperacionQuery
        {
            FechaInicio = fechaInicio,
            FechaFin = fechaFin,
            EstacionId = 1
        };

        var transacciones = new List<Transaccion>
        {
            new Transaccion 
            { 
                Id = 1, 
                EstacionId = 1,
                Monto = 25.00m,
                FechaTransaccion = fechaInicio.AddHours(10),
                TipoPago = null, // Null reference
                TipoVehiculo = null // Null reference
            }
        };

        var turnos = new List<Turno>();

        var transaccionRepositoryMock = new Mock<IRepository<Transaccion>>();
        transaccionRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Transaccion, bool>>>()))
                                 .ReturnsAsync(transacciones);

        var turnoRepositoryMock = new Mock<IRepository<Turno>>();
        turnoRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Turno, bool>>>()))
                          .ReturnsAsync(turnos);

        _unitOfWorkMock.Setup(u => u.Repository<Transaccion>())
                       .Returns(transaccionRepositoryMock.Object);

        _unitOfWorkMock.Setup(u => u.Repository<Turno>())
                       .Returns(turnoRepositoryMock.Object);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.TotalTransacciones.Should().Be(1);
        result.MontoTotalRecaudado.Should().Be(25.00m);
        
        // Debe manejar referencias nulas con valores por defecto
        result.TransaccionesPorTipoPago.Should().ContainKey("Sin Especificar");
        result.TransaccionesPorTipoPago["Sin Especificar"].Should().Be(1);
        result.TransaccionesPorVehiculo.Should().ContainKey("Sin Especificar");
        result.TransaccionesPorVehiculo["Sin Especificar"].Should().Be(1);
    }
}
