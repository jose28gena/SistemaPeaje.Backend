using MediatR;
using Microsoft.Extensions.Logging;
using SistemaPeaje.Application.DTOs.Comandos;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;
using System.Diagnostics;

namespace SistemaPeaje.Application.Features.ComandosPlc;

public class EjecutarTestConexionCommand : IRequest<ResultadoComandoDto>
{
    public string CasetaIp { get; set; } = string.Empty;
    public int Puerto { get; set; } = 502;
    public byte UnitId { get; set; } = 1;
    public int? UsuarioId { get; set; }
    public int? EmpleadoId { get; set; }
}

public class EjecutarTestConexionHandler : IRequestHandler<EjecutarTestConexionCommand, ResultadoComandoDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPlcModbusService _plcService;
    private readonly ILogger<EjecutarTestConexionHandler> _logger;

    public EjecutarTestConexionHandler(
        IUnitOfWork unitOfWork,
        IPlcModbusService plcService,
        ILogger<EjecutarTestConexionHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _plcService = plcService;
        _logger = logger;
    }

    public async Task<ResultadoComandoDto> Handle(EjecutarTestConexionCommand request, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var fechaEjecucion = DateTime.UtcNow;
        
        try
        {
            _logger.LogInformation("Ejecutando TEST_CONEXION para IP {IP}:{Puerto}", 
                request.CasetaIp, request.Puerto);

            // Ejecutar test de conexión
            var resultado = await _plcService.TestConexionAsync(
                request.CasetaIp, 
                request.Puerto, 
                request.UnitId);

            stopwatch.Stop();

            // Registrar comando en base de datos
            var comandoPlc = new ComandoPlc
            {
                TipoComando = "TEST_CONEXION",
                IpDestino = request.CasetaIp,
                Puerto = request.Puerto,
                UnitId = request.UnitId,
                CoilAddress = 0, // No aplica para test de conexión
                ValorEnviado = false,
                ComandoExitoso = resultado,
                Observaciones = "Test de conectividad Modbus TCP",
                UsuarioId = request.UsuarioId,
                EmpleadoId = request.EmpleadoId,
                FechaEjecucion = fechaEjecucion,
                TiempoRespuestaMs = (int)stopwatch.ElapsedMilliseconds,
                FechaCreacion = DateTime.UtcNow
            };

            await _unitOfWork.Repository<ComandoPlc>().AddAsync(comandoPlc);
            await _unitOfWork.SaveChangesAsync();

            if (resultado)
            {
                _logger.LogInformation("TEST_CONEXION exitoso en {TiempoMs}ms", stopwatch.ElapsedMilliseconds);
                
                return new ResultadoComandoDto
                {
                    Exitoso = true,
                    Mensaje = "✅ Conexión exitosa con el PLC",
                    FechaEjecucion = fechaEjecucion,
                    DatosAdicionales = new Dictionary<string, object>
                    {
                        { "TiempoRespuesta", $"{stopwatch.ElapsedMilliseconds}ms" },
                        { "IP", request.CasetaIp },
                        { "Puerto", request.Puerto },
                        { "UnitId", request.UnitId }
                    }
                };
            }
            else
            {
                return new ResultadoComandoDto
                {
                    Exitoso = false,
                    Mensaje = "❌ No se pudo conectar con el PLC",
                    FechaEjecucion = fechaEjecucion,
                    CodigoError = "CONNECTION_FAILED",
                    DatosAdicionales = new Dictionary<string, object>
                    {
                        { "TiempoRespuesta", $"{stopwatch.ElapsedMilliseconds}ms" },
                        { "IP", request.CasetaIp },
                        { "Puerto", request.Puerto }
                    }
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al ejecutar TEST_CONEXION");
            stopwatch.Stop();
            
            return await RegistrarComandoFallido(request, ex.Message, stopwatch.ElapsedMilliseconds, fechaEjecucion);
        }
    }

    private async Task<ResultadoComandoDto> RegistrarComandoFallido(
        EjecutarTestConexionCommand request, 
        string mensajeError, 
        long tiempoMs, 
        DateTime fechaEjecucion)
    {
        try
        {
            var comandoPlc = new ComandoPlc
            {
                TipoComando = "TEST_CONEXION",
                IpDestino = request.CasetaIp,
                Puerto = request.Puerto,
                UnitId = request.UnitId,
                CoilAddress = 0,
                ValorEnviado = false,
                ComandoExitoso = false,
                MensajeError = mensajeError,
                Observaciones = "Test de conectividad Modbus TCP",
                UsuarioId = request.UsuarioId,
                EmpleadoId = request.EmpleadoId,
                FechaEjecucion = fechaEjecucion,
                TiempoRespuestaMs = (int)tiempoMs,
                FechaCreacion = DateTime.UtcNow
            };

            await _unitOfWork.Repository<ComandoPlc>().AddAsync(comandoPlc);
            await _unitOfWork.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adicional al registrar comando fallido");
        }

        return new ResultadoComandoDto
        {
            Exitoso = false,
            Mensaje = $"❌ {mensajeError}",
            FechaEjecucion = fechaEjecucion,
            CodigoError = "COMMAND_FAILED"
        };
    }
}
