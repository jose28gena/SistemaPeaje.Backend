using System.Net.Sockets;
using SistemaPeaje.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace SistemaPeaje.API.Services
{
    /// <summary>
    /// Servicio de Modbus TCP para operaciones síncronas desde controladores
    /// Para monitoreo continuo usar PlcModbusReaderService del Infrastructure
    /// </summary>
    public class PlcModbusService : IPlcModbusService
    {
        private readonly ILogger<PlcModbusService> _logger;
        private readonly string _defaultPlcIp;
        private readonly int _defaultPlcPort;

        public PlcModbusService(ILogger<PlcModbusService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _defaultPlcIp = configuration["PlcMonitor:PlcIp"] ?? "192.168.1.103";
            _defaultPlcPort = configuration.GetValue<int>("PlcMonitor:PlcPort", 502);
        }

        // Implementación de la interfaz IPlcModbusService
        public async Task<bool> LeerCoilAsync(string ip, ushort coilAddress, int puerto = 502, byte unitId = 1)
        {
            var result = await LeerCoilsAsync(ip, puerto, coilAddress, 1);
            return result.Length > 0 && result[0];
        }

        public async Task<bool> EscribirCoilAsync(string ip, ushort coilAddress, bool value, int puerto = 502, byte unitId = 1)
        {
            return await EscribirCoilAsync(ip, puerto, coilAddress, value);
        }

        public async Task<bool> TestConexionAsync(string ip, int puerto = 502, byte unitId = 1)
        {
            try
            {
                using var client = new TcpClient();
                client.ReceiveTimeout = 3000;
                client.SendTimeout = 3000;
                await client.ConnectAsync(ip, puerto);
                return client.Connected;
            }
            catch
            {
                return false;
            }
        }

        // Métodos adicionales para operaciones múltiples
        public async Task<bool[]> LeerCoilsAsync(ushort startAddress, ushort count)
        {
            return await LeerCoilsAsync(_defaultPlcIp, _defaultPlcPort, startAddress, count);
        }

        public async Task<bool[]> LeerCoilsAsync(string plcIp, int plcPort, ushort startAddress, ushort count)
        {
            try
            {
                using var client = new TcpClient();
                await client.ConnectAsync(plcIp, plcPort);

                // Crear mensaje Modbus TCP para leer coils
                var message = CreateReadCoilsMessage(startAddress, count);
                
                using var stream = client.GetStream();
                await stream.WriteAsync(message);

                // Leer respuesta
                var response = new byte[1024];
                var bytesRead = await stream.ReadAsync(response);

                return ParseCoilsResponse(response, bytesRead, count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al leer coils del PLC {PlcIp}:{PlcPort}", plcIp, plcPort);
                return new bool[count]; // Retorna todo falso si falla
            }
        }

        public async Task<bool> EscribirCoilAsync(ushort coilAddress, bool value)
        {
            return await EscribirCoilAsync(_defaultPlcIp, _defaultPlcPort, coilAddress, value);
        }

        public async Task<bool> EscribirCoilAsync(string plcIp, int plcPort, ushort coilAddress, bool value)
        {
            try
            {
                using var client = new TcpClient();
                await client.ConnectAsync(plcIp, plcPort);

                // Crear mensaje Modbus TCP para escribir coil individual (función 05)
                var message = CreateWriteSingleCoilMessage(coilAddress, value);
                
                using var stream = client.GetStream();
                await stream.WriteAsync(message);

                // Leer respuesta
                var response = new byte[12];
                var bytesRead = await stream.ReadAsync(response);

                return bytesRead >= 12 && response[7] == 0x05; // Verificar función de respuesta
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al escribir coil {CoilAddress} en PLC {PlcIp}:{PlcPort}", 
                    coilAddress, plcIp, plcPort);
                return false;
            }
        }

        private byte[] CreateReadCoilsMessage(ushort startAddress, ushort count)
        {
            var message = new byte[12];
            
            // MBAP Header
            message[0] = 0x00; message[1] = 0x01; // Transaction ID
            message[2] = 0x00; message[3] = 0x00; // Protocol ID
            message[4] = 0x00; message[5] = 0x06; // Length
            message[6] = 0x01; // Unit ID
            
            // PDU
            message[7] = 0x01; // Function code (Read Coils)
            message[8] = (byte)(startAddress >> 8);
            message[9] = (byte)(startAddress & 0xFF);
            message[10] = (byte)(count >> 8);
            message[11] = (byte)(count & 0xFF);
            
            return message;
        }

        private byte[] CreateWriteSingleCoilMessage(ushort coilAddress, bool value)
        {
            var message = new byte[12];
            
            // MBAP Header
            message[0] = 0x00; message[1] = 0x01; // Transaction ID
            message[2] = 0x00; message[3] = 0x00; // Protocol ID
            message[4] = 0x00; message[5] = 0x06; // Length
            message[6] = 0x01; // Unit ID
            
            // PDU
            message[7] = 0x05; // Function code (Write Single Coil)
            message[8] = (byte)(coilAddress >> 8);
            message[9] = (byte)(coilAddress & 0xFF);
            message[10] = (byte)(value ? 0xFF : 0x00);
            message[11] = 0x00;
            
            return message;
        }

        private bool[] ParseCoilsResponse(byte[] response, int bytesRead, ushort count)
        {
            var coils = new bool[count];
            
            if (bytesRead < 9 || response[7] != 0x01)
                return coils;

            var byteCount = response[8];
            
            for (int i = 0; i < count && i / 8 < byteCount; i++)
            {
                var byteIndex = 9 + (i / 8);
                var bitIndex = i % 8;
                
                if (byteIndex < bytesRead)
                {
                    coils[i] = (response[byteIndex] & (1 << bitIndex)) != 0;
                }
            }
            
            return coils;
        }
    }
}