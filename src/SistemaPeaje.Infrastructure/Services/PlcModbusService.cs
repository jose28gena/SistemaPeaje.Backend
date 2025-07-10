using System.Net.Sockets;
using SistemaPeaje.Core.Interfaces;
using Microsoft.Extensions.Logging;

// NOTA: Esta implementación usa Modbus TCP nativo para .NET 8 compatibility
// Para usar NModbus4 específicamente como fue solicitado, agregue:
// dotnet add package NModbus4
// Y reemplace la implementación con la librería externa

namespace SistemaPeaje.Infrastructure.Services;

public class PlcModbusService : IPlcModbusService
{
    private readonly ILogger<PlcModbusService> _logger;

    public PlcModbusService(ILogger<PlcModbusService> logger)
    {
        _logger = logger;
    }

    public async Task<bool> EscribirCoilAsync(string ip, ushort coilAddress, bool value, int puerto = 502, byte unitId = 1)
    {
        try
        {
            _logger.LogInformation("Escribiendo coil {CoilAddress} con valor {Value} en {IP}:{Puerto}", 
                coilAddress, value, ip, puerto);

            using var client = new TcpClient();
            await client.ConnectAsync(ip, puerto);
            using var stream = client.GetStream();

            // Crear mensaje Modbus TCP para escribir coil (función 05)
            var modbusTcpMessage = CreateWriteCoilMessage(coilAddress, value, unitId);
            
            // Enviar mensaje
            await stream.WriteAsync(modbusTcpMessage);
            
            // Leer respuesta
            var response = new byte[12];
            var bytesRead = await stream.ReadAsync(response);
            
            if (bytesRead >= 12)
            {
                // Verificar que la respuesta sea correcta
                var isSuccess = response[7] == 0x05 && // Función 05 (Write Single Coil)
                               response[8] == (byte)(coilAddress >> 8) && // Coil address high
                               response[9] == (byte)(coilAddress & 0xFF); // Coil address low
                
                _logger.LogInformation("Comando ejecutado exitosamente: {Success}", isSuccess);
                return isSuccess;
            }
            
            _logger.LogWarning("Respuesta incompleta del PLC");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al escribir coil {CoilAddress} en {IP}:{Puerto}", 
                coilAddress, ip, puerto);
            return false;
        }
    }

    public async Task<bool> LeerCoilAsync(string ip, ushort coilAddress, int puerto = 502, byte unitId = 1)
    {
        try
        {
            _logger.LogInformation("Leyendo coil {CoilAddress} en {IP}:{Puerto}", 
                coilAddress, ip, puerto);

            using var client = new TcpClient();
            await client.ConnectAsync(ip, puerto);
            using var stream = client.GetStream();

            // Crear mensaje Modbus TCP para leer coil (función 01)
            var modbusTcpMessage = CreateReadCoilMessage(coilAddress, unitId);
            
            // Enviar mensaje
            await stream.WriteAsync(modbusTcpMessage);
            
            // Leer respuesta
            var response = new byte[10];
            var bytesRead = await stream.ReadAsync(response);
            
            if (bytesRead >= 10)
            {
                // Verificar que la respuesta sea correcta y extraer valor
                var isValidResponse = response[7] == 0x01; // Función 01 (Read Coils)
                if (isValidResponse && response[8] == 1) // Byte count
                {
                    var coilValue = (response[9] & 0x01) != 0; // Primer bit
                    _logger.LogInformation("Coil {CoilAddress} valor: {Value}", coilAddress, coilValue);
                    return coilValue;
                }
            }
            
            _logger.LogWarning("Respuesta inválida del PLC");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al leer coil {CoilAddress} en {IP}:{Puerto}", 
                coilAddress, ip, puerto);
            return false;
        }
    }

    public async Task<bool> TestConexionAsync(string ip, int puerto = 502, byte unitId = 1)
    {
        try
        {
            _logger.LogInformation("Probando conexión con {IP}:{Puerto}", ip, puerto);

            using var client = new TcpClient();
            client.ReceiveTimeout = 5000; // 5 segundos timeout
            client.SendTimeout = 5000;
            
            await client.ConnectAsync(ip, puerto);
            
            if (client.Connected)
            {
                _logger.LogInformation("Conexión exitosa con {IP}:{Puerto}", ip, puerto);
                return true;
            }
            
            _logger.LogWarning("No se pudo establecer conexión con {IP}:{Puerto}", ip, puerto);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al probar conexión con {IP}:{Puerto}", ip, puerto);
            return false;
        }
    }

    private byte[] CreateWriteCoilMessage(ushort coilAddress, bool value, byte unitId)
    {
        // Mensaje Modbus TCP para escribir coil individual (función 05)
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
        message[7] = 0x05; // Function code (Write Single Coil)
        message[8] = (byte)(coilAddress >> 8); // Coil address (high)
        message[9] = (byte)(coilAddress & 0xFF); // Coil address (low)
        message[10] = (byte)(value ? 0xFF : 0x00); // Coil value (high)
        message[11] = 0x00; // Coil value (low)
        
        return message;
    }

    private byte[] CreateReadCoilMessage(ushort coilAddress, byte unitId)
    {
        // Mensaje Modbus TCP para leer coil (función 01)
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
        message[8] = (byte)(coilAddress >> 8); // Starting address (high)
        message[9] = (byte)(coilAddress & 0xFF); // Starting address (low)
        message[10] = 0x00; // Quantity (high)
        message[11] = 0x01; // Quantity (low) - 1 coil
        
        return message;
    }
}
