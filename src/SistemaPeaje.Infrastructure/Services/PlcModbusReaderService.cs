using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using SistemaPeaje.Core.Interfaces;

namespace SistemaPeaje.Infrastructure.Services;

/// <summary>
/// Servicio para lectura continua de coils del PLC vía Modbus TCP
/// Especializado para operaciones de monitoreo en background
/// </summary>
public class PlcModbusReaderService
{
    private readonly ILogger<PlcModbusReaderService> _logger;
    private readonly string _ip;
    private readonly int _port;
    private readonly byte _unitId;

    public PlcModbusReaderService(
        ILogger<PlcModbusReaderService> logger,
        string ip, 
        int port = 502, 
        byte unitId = 1)
    {
        _logger = logger;
        _ip = ip;
        _port = port;
        _unitId = unitId;
    }

    /// <summary>
    /// Lee múltiples coils desde una dirección inicial
    /// </summary>
    /// <param name="startAddress">Dirección inicial del coil</param>
    /// <param name="count">Cantidad de coils a leer</param>
    /// <returns>Array de estados de los coils</returns>
    public async Task<bool[]> LeerCoilsAsync(ushort startAddress, ushort count)
    {
        try
        {
            _logger.LogDebug("Leyendo {Count} coils desde dirección {StartAddress} en {IP}:{Port}", 
                count, startAddress, _ip, _port);

            using var client = new TcpClient();
            client.ReceiveTimeout = 3000; // Timeout más corto para monitoreo
            client.SendTimeout = 3000;
            
            await client.ConnectAsync(_ip, _port);
            using var stream = client.GetStream();

            // Crear mensaje Modbus TCP para leer múltiples coils (función 01)
            var modbusTcpMessage = CreateReadMultipleCoilsMessage(startAddress, count, _unitId);
            
            // Enviar mensaje
            await stream.WriteAsync(modbusTcpMessage);
            
            // Leer respuesta
            var responseHeader = new byte[9]; // MBAP header + function code + byte count
            await stream.ReadAsync(responseHeader);
            
            if (responseHeader[7] == 0x01) // Función 01 (Read Coils)
            {
                var byteCount = responseHeader[8];
                var dataBytes = new byte[byteCount];
                await stream.ReadAsync(dataBytes);
                
                return ExtractCoilStates(dataBytes, count);
            }
            
            _logger.LogWarning("Respuesta inválida del PLC {IP}:{Port}", _ip, _port);
            return new bool[count]; // Retorna todo falso si falla
        }
        catch (SocketException ex)
        {
            _logger.LogError(ex, "Error de conexión al leer coils del PLC {IP}:{Port}", _ip, _port);
            return new bool[count];
        }
        catch (TimeoutException ex)
        {
            _logger.LogError(ex, "Timeout al leer coils del PLC {IP}:{Port}", _ip, _port);
            return new bool[count];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al leer coils del PLC {IP}:{Port}", _ip, _port);
            return new bool[count];
        }
    }

    /// <summary>
    /// Lee un coil individual para verificaciones rápidas
    /// </summary>
    public async Task<bool> LeerCoilIndividualAsync(ushort coilAddress)
    {
        var result = await LeerCoilsAsync(coilAddress, 1);
        return result.Length > 0 && result[0];
    }

    /// <summary>
    /// Verifica si el PLC está disponible
    /// </summary>
    public async Task<bool> VerificarConexionAsync()
    {
        try
        {
            using var client = new TcpClient();
            client.ReceiveTimeout = 2000;
            client.SendTimeout = 2000;
            
            await client.ConnectAsync(_ip, _port);
            return client.Connected;
        }
        catch
        {
            return false;
        }
    }

    private byte[] CreateReadMultipleCoilsMessage(ushort startAddress, ushort count, byte unitId)
    {
        // Mensaje Modbus TCP para leer múltiples coils (función 01)
        var message = new byte[12];
        
        // MBAP Header (7 bytes)
        message[0] = 0x00; // Transaction ID (high)
        message[1] = 0x01; // Transaction ID (low)
        message[2] = 0x00; // Protocol ID (high)
        message[3] = 0x00; // Protocol ID (low)
        message[4] = 0x00; // Length (high)
        message[5] = 0x06; // Length (low) - 6 bytes follow
        message[6] = unitId; // Unit ID
        
        // PDU (5 bytes)
        message[7] = 0x01; // Function code (Read Coils)
        message[8] = (byte)(startAddress >> 8); // Starting address (high)
        message[9] = (byte)(startAddress & 0xFF); // Starting address (low)
        message[10] = (byte)(count >> 8); // Quantity (high)
        message[11] = (byte)(count & 0xFF); // Quantity (low)
        
        return message;
    }

    private bool[] ExtractCoilStates(byte[] dataBytes, ushort count)
    {
        var coilStates = new bool[count];
        
        for (int i = 0; i < count; i++)
        {
            var byteIndex = i / 8;
            var bitIndex = i % 8;
            
            if (byteIndex < dataBytes.Length)
            {
                coilStates[i] = (dataBytes[byteIndex] & (1 << bitIndex)) != 0;
            }
        }
        
        return coilStates;
    }
}
