using FluentAssertions;
using Moq;
using SistemaPeaje.Application.Features.MonitorEventos;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;
using System.Linq.Expressions;
using Xunit;

namespace SistemaPeaje.Tests.Application.Handlers;

public class GetEventosEnTiempoRealQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly GetEventosEnTiempoRealHandler _handler;

    public GetEventosEnTiempoRealQueryHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new GetEventosEnTiempoRealHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ValidQueryWithRecentEvents_ReturnsEventosDto()
    {
        // Arrange
        var query = new GetEventosEnTiempoRealQuery
        {
            EstacionId = 1,
            CarrilId = 2,
            FechaDesde = DateTime.UtcNow.AddMinutes(-15),
            LimitEventos = 10
        };

        var eventos = new List<EventoTransito>
        {
            new EventoTransito
            {
                Id = 1,
                EstacionId = 1,
                CarrilId = 2,
                TipoEvento = "VehiculoDetectado",
                Descripcion = "Vehículo detectado en carril 2",
                FechaEvento = DateTime.UtcNow.AddSeconds(-20), // Hace 20 segundos (< 30, debería ser "Procesando")
                DatosAdicionales = "Placa: ABC123"
            },
            new EventoTransito
            {
                Id = 2,
                EstacionId = 1,
                CarrilId = 2,
                TipoEvento = "PagoRealizado",
                Descripcion = "Pago completado exitosamente",
                FechaEvento = DateTime.UtcNow.AddMinutes(-2), // Hace 2 minutos (medio)
                DatosAdicionales = "Monto: $25.50"
            },
            new EventoTransito
            {
                Id = 3,
                EstacionId = 1,
                CarrilId = 2,
                TipoEvento = "ErrorLectura",
                Descripcion = "Error al leer tarjeta RFID",
                FechaEvento = DateTime.UtcNow.AddSeconds(-10), // Hace 10 segundos (más reciente)
                DatosAdicionales = "Código Error: E001"
            }
        };

        var repositoryMock = new Mock<IRepository<EventoTransito>>();
        repositoryMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<EventoTransito, bool>>>()))
                     .ReturnsAsync(eventos);

        _unitOfWorkMock.Setup(u => u.Repository<EventoTransito>())
                       .Returns(repositoryMock.Object);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        
        // Verificar orden descendente por fecha (más reciente primero)
        // Como el sistema ordena por fecha descendente, el orden esperado es: 3, 1, 2
        result[0].Id.Should().Be(3); // Más reciente (AddSeconds(-10))
        result[1].Id.Should().Be(1); // Medio (AddSeconds(-20))  
        result[2].Id.Should().Be(2); // Más antiguo (AddMinutes(-2))

        // Verificar estados calculados
        result.First(e => e.TipoEvento == "VehiculoDetectado").Estado.Should().Be("Procesando"); // 20 segundos < 30 segundos
        result.First(e => e.TipoEvento == "PagoRealizado").Estado.Should().Be("Completado");
        result.First(e => e.TipoEvento == "ErrorLectura").Estado.Should().Be("Error");

        // Verificar que el tiempo transcurrido se calcula
        result.All(e => e.TiempoTranscurrido >= TimeSpan.Zero).Should().BeTrue();

        repositoryMock.Verify(r => r.GetAsync(It.IsAny<Expression<Func<EventoTransito, bool>>>()), Times.Once);
    }

    [Fact]
    public async Task Handle_QueryWithoutFilters_UsesDefaultTimeFilter()
    {
        // Arrange
        var query = new GetEventosEnTiempoRealQuery
        {
            // Sin filtros específicos
        };

        var eventos = new List<EventoTransito>
        {
            new EventoTransito
            {
                Id = 1,
                EstacionId = 1,
                CarrilId = 1,
                TipoEvento = "VehiculoDetectado",
                Descripcion = "Evento reciente",
                FechaEvento = DateTime.UtcNow.AddMinutes(-15), // Dentro de los últimos 30 minutos
                DatosAdicionales = "Test"
            }
        };

        var repositoryMock = new Mock<IRepository<EventoTransito>>();
        repositoryMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<EventoTransito, bool>>>()))
                     .ReturnsAsync(eventos);

        _unitOfWorkMock.Setup(u => u.Repository<EventoTransito>())
                       .Returns(repositoryMock.Object);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result[0].Id.Should().Be(1);

        // Verificar que usa el límite por defecto
        repositoryMock.Verify(r => r.GetAsync(It.IsAny<Expression<Func<EventoTransito, bool>>>()), Times.Once);
    }

    [Fact]
    public async Task Handle_VehiculoDetectadoTimeout_ReturnsTimeoutState()
    {
        // Arrange
        var query = new GetEventosEnTiempoRealQuery
        {
            EstacionId = 1,
            LimitEventos = 5
        };

        var eventos = new List<EventoTransito>
        {
            new EventoTransito
            {
                Id = 1,
                EstacionId = 1,
                CarrilId = 1,
                TipoEvento = "VehiculoDetectado",
                Descripcion = "Vehículo detectado - sin completar",
                FechaEvento = DateTime.UtcNow.AddSeconds(-45), // Más de 30 segundos
                DatosAdicionales = "Timeout case"
            }
        };

        var repositoryMock = new Mock<IRepository<EventoTransito>>();
        repositoryMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<EventoTransito, bool>>>()))
                     .ReturnsAsync(eventos);

        _unitOfWorkMock.Setup(u => u.Repository<EventoTransito>())
                       .Returns(repositoryMock.Object);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result[0].Estado.Should().Be("Timeout");
        result[0].TiempoTranscurrido.Should().BeGreaterThan(TimeSpan.FromSeconds(30));
    }

    [Fact]
    public async Task Handle_LimitEventos_ReturnsLimitedResults()
    {
        // Arrange
        var query = new GetEventosEnTiempoRealQuery
        {
            LimitEventos = 2 // Límite específico
        };

        var eventos = new List<EventoTransito>();
        for (int i = 1; i <= 5; i++)
        {
            eventos.Add(new EventoTransito
            {
                Id = i,
                EstacionId = 1,
                CarrilId = 1,
                TipoEvento = "VehiculoDetectado",
                Descripcion = $"Evento {i}",
                FechaEvento = DateTime.UtcNow.AddMinutes(-i),
                DatosAdicionales = $"Data {i}"
            });
        }

        var repositoryMock = new Mock<IRepository<EventoTransito>>();
        repositoryMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<EventoTransito, bool>>>()))
                     .ReturnsAsync(eventos);

        _unitOfWorkMock.Setup(u => u.Repository<EventoTransito>())
                       .Returns(repositoryMock.Object);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2); // Respeta el límite
        result[0].Id.Should().Be(1); // Más reciente
        result[1].Id.Should().Be(2);
    }

    [Fact]
    public async Task Handle_EmptyEventos_ReturnsEmptyList()
    {
        // Arrange
        var query = new GetEventosEnTiempoRealQuery
        {
            EstacionId = 999
        };

        var emptyEventos = new List<EventoTransito>();

        var repositoryMock = new Mock<IRepository<EventoTransito>>();
        repositoryMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<EventoTransito, bool>>>()))
                     .ReturnsAsync(emptyEventos);

        _unitOfWorkMock.Setup(u => u.Repository<EventoTransito>())
                       .Returns(repositoryMock.Object);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();

        repositoryMock.Verify(r => r.GetAsync(It.IsAny<Expression<Func<EventoTransito, bool>>>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UnknownEventType_ReturnsPendienteState()
    {
        // Arrange
        var query = new GetEventosEnTiempoRealQuery();

        var eventos = new List<EventoTransito>
        {
            new EventoTransito
            {
                Id = 1,
                EstacionId = 1,
                CarrilId = 1,
                TipoEvento = "EventoDesconocido", // Tipo no manejado
                Descripcion = "Evento de tipo desconocido",
                FechaEvento = DateTime.UtcNow.AddMinutes(-5),
                DatosAdicionales = "Unknown type"
            }
        };

        var repositoryMock = new Mock<IRepository<EventoTransito>>();
        repositoryMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<EventoTransito, bool>>>()))
                     .ReturnsAsync(eventos);

        _unitOfWorkMock.Setup(u => u.Repository<EventoTransito>())
                       .Returns(repositoryMock.Object);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result[0].Estado.Should().Be("Pendiente"); // Estado por defecto
    }
}
