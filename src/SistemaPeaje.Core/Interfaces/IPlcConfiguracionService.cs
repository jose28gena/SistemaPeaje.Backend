using SistemaPeaje.Core.Entities;

namespace SistemaPeaje.Core.Interfaces
{
    public interface IPlcConfiguracionService
    {
        Task<IEnumerable<PlcConfiguracion>> ObtenerTodasLasConfiguracionesAsync();
        Task<IEnumerable<PlcConfiguracion>> ObtenerConfiguracionesActivasAsync();
        Task<PlcConfiguracion?> ObtenerConfiguracionPorIdAsync(int id);
        Task<PlcConfiguracion?> ObtenerConfiguracionPorIpAsync(string ip, int puerto);
        Task<PlcConfiguracion> CrearConfiguracionAsync(PlcConfiguracion configuracion);
        Task<PlcConfiguracion> ActualizarConfiguracionAsync(PlcConfiguracion configuracion);
        Task<bool> EliminarConfiguracionAsync(int id);
        Task ActualizarEstadoConexionAsync(int id, bool estaConectado);
        Task<bool> ExisteConfiguracionAsync(string ip, int puerto, int? excludeId = null);
    }
}
