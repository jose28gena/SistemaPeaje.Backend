using MediatR;
using Microsoft.Extensions.Logging;
using SistemaPeaje.Application.DTOs.Comandos;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;
using System.Diagnostics;

namespace SistemaPeaje.Application.Features.ComandosPlc;

public class EjecutarComandoAbrirBarreraCommand : IRequest<ResultadoComandoDto>
{
    public string CasetaIp { get; set; } = string.Empty;
    public ushort CoilAddress { get; set; }
    public byte UnitId { get; set; } = 1;
    public int? CarrilId { get; set; }
    public string? Observaciones { get; set; }
    public int? UsuarioId { get; set; }
    public int? EmpleadoId { get; set; }
}

public class EjecutarComandoAbrirBarreraHandler : IRequestHandler<EjecutarComandoAbrirBarreraCommand, ResultadoComandoDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPlcModbusService _plcService;
    private readonly ILogger<EjecutarComandoAbrirBarreraHandler> _logger;

    public EjecutarComandoAbrirBarreraHandler(
        IUnitOfWork unitOfWork,
        IPlcModbusService plcService,
        ILogger<EjecutarComandoAbrirBarreraHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _plcService = plcService;
        _logger = logger;
    }

    public async Task<ResultadoComandoDto> Handle(EjecutarComandoAbrirBarreraCommand request, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var fechaEjecucion = DateTime.UtcNow;
        
        try
        {
            _logger.LogInformation("Ejecutando comando ABRIR_BARRERA para IP {IP}, Coil {Coil}", 
                request.CasetaIp, request.CoilAddress);

            // Validar que la IP esté en lista blanca (implementar según requisitos)
            if (!await ValidarIpPermitida(request.CasetaIp))
            {
                return await RegistrarComandoFallido(request, "IP no autorizada", stopwatch.ElapsedMilliseconds, fechaEjecucion);
            }

            // Ejecutar comando
            var resultado = await _plcService.EscribirCoilAsync(
                request.CasetaIp, 
                request.CoilAddress, 
                true, 
                502, 
                request.UnitId);

            stopwatch.Stop();

            // Registrar comando en base de datos
            var comandoPlc = new ComandoPlc
            {
                TipoComando = "ABRIR_BARRERA",
                IpDestino = request.CasetaIp,
                Puerto = 502,
                UnitId = request.UnitId,
                CoilAddress = request.CoilAddress,
                ValorEnviado = true,
                ComandoExitoso = resultado,
                Observaciones = request.Observaciones,
                CarrilId = request.CarrilId,
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
                _logger.LogInformation("Comando ABRIR_BARRERA ejecutado exitosamente en {TiempoMs}ms", 
                    stopwatch.ElapsedMilliseconds);
                
                return new ResultadoComandoDto
                {
                    Exitoso = true,
                    Mensaje = "✅ Barrera abierta correctamente",
                    FechaEjecucion = fechaEjecucion,
                    DatosAdicionales = new Dictionary<string, object>
                    {
                        { "TiempoRespuesta", $"{stopwatch.ElapsedMilliseconds}ms" },
                        { "CoilAddress", request.CoilAddress },
                        { "IP", request.CasetaIp }
                    }
                };
            }
            else
            {
                return new ResultadoComandoDto
                {
                    Exitoso = false,
                    Mensaje = "❌ Error al enviar comando al PLC",
                    FechaEjecucion = fechaEjecucion,
                    CodigoError = "PLC_COMMUNICATION_ERROR"
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al ejecutar comando ABRIR_BARRERA");
            stopwatch.Stop();
            
            return await RegistrarComandoFallido(request, ex.Message, stopwatch.ElapsedMilliseconds, fechaEjecucion);
        }
    }

    private Task<bool> ValidarIpPermitida(string ip)
    {
        // TODO: Implementar validación contra lista blanca en base de datos
        // Por ahora permitir IPs de rango privado
        var esPermitida = ip.StartsWith("192.168.") || ip.StartsWith("10.") || ip.StartsWith("172.");
        return Task.FromResult(esPermitida);
    }

    private async Task<ResultadoComandoDto> RegistrarComandoFallido(
        EjecutarComandoAbrirBarreraCommand request, 
        string mensajeError, 
        long tiempoMs, 
        DateTime fechaEjecucion)
    {
        try
        {
            var comandoPlc = new ComandoPlc
            {
                TipoComando = "ABRIR_BARRERA",
                IpDestino = request.CasetaIp,
                Puerto = 502,
                UnitId = request.UnitId,
                CoilAddress = request.CoilAddress,
                ValorEnviado = true,
                ComandoExitoso = false,
                MensajeError = mensajeError,
                Observaciones = request.Observaciones,
                CarrilId = request.CarrilId,
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
