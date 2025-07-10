namespace SistemaPeaje.Core.Interfaces;

public interface IPlcModbusService
{
    Task<bool> EscribirCoilAsync(string ip, ushort coilAddress, bool value, int puerto = 502, byte unitId = 1);
    Task<bool> LeerCoilAsync(string ip, ushort coilAddress, int puerto = 502, byte unitId = 1);
    Task<bool> TestConexionAsync(string ip, int puerto = 502, byte unitId = 1);
}
