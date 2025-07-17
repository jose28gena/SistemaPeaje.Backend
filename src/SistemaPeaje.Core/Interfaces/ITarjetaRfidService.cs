using SistemaPeaje.Core.Entities;

namespace SistemaPeaje.Core.Interfaces
{
    public interface ITarjetaRfidService
    {
        Task<TarjetaRFID> GetByNumeroTagAsync(string numeroTag);
        Task<bool> ValidarSaldoSuficienteAsync(string numeroTag, decimal montoTransaccion);
        Task<bool> DebitarSaldoAsync(string numeroTag, decimal monto);
        Task<decimal> ConsultarSaldoAsync(string numeroTag);
        Task<TarjetaRFID> CrearTarjetaAsync(string numeroTag, int clienteId, decimal saldoInicial);
        Task<bool> BloquearTarjetaAsync(string numeroTag);
        Task<bool> DesbloquearTarjetaAsync(string numeroTag);
        Task<bool> RecargarSaldoAsync(string numeroTag, decimal monto);
        Task<IEnumerable<TarjetaRFID>> GetTarjetasByClienteIdAsync(int clienteId);
    }
}
