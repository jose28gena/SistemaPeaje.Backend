using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;
using SistemaPeaje.Infrastructure.Data;

namespace SistemaPeaje.Infrastructure.Services
{
    public class TarjetaRfidService : ITarjetaRfidService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TarjetaRfidService> _logger;

        public TarjetaRfidService(
            ApplicationDbContext context,
            ILogger<TarjetaRfidService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<TarjetaRFID> GetByNumeroTagAsync(string numeroTag)
        {
            return await _context.TarjetasRFID
                .Include(t => t.Cliente)
                .FirstOrDefaultAsync(t => t.NumeroTag == numeroTag) ?? throw new KeyNotFoundException($"Tarjeta con tag {numeroTag} no encontrada");
        }

        public async Task<bool> ValidarSaldoSuficienteAsync(string numeroTag, decimal montoTransaccion)
        {
            var tarjeta = await GetByNumeroTagAsync(numeroTag);
            if (tarjeta == null)
            {
                _logger.LogWarning($"Tarjeta con tag {numeroTag} no encontrada");
                return false;
            }

            if (tarjeta.Estado != "Activa")
            {
                _logger.LogWarning($"Tarjeta con tag {numeroTag} no está activa. Estado actual: {tarjeta.Estado}");
                return false;
            }

            return tarjeta.Saldo >= montoTransaccion;
        }

        public async Task<bool> DebitarSaldoAsync(string numeroTag, decimal monto)
        {
            var tarjeta = await GetByNumeroTagAsync(numeroTag);
            if (tarjeta == null)
            {
                _logger.LogWarning($"Tarjeta con tag {numeroTag} no encontrada para debitar");
                return false;
            }

            if (tarjeta.Estado != "Activa")
            {
                _logger.LogWarning($"No se puede debitar tarjeta inactiva con tag {numeroTag}. Estado: {tarjeta.Estado}");
                return false;
            }

            if (tarjeta.Saldo < monto)
            {
                _logger.LogWarning($"Saldo insuficiente en tarjeta {numeroTag}. Saldo: {tarjeta.Saldo}, Monto: {monto}");
                return false;
            }

            tarjeta.Saldo -= monto;
            _context.TarjetasRFID.Update(tarjeta);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"Saldo debitado correctamente de tarjeta {numeroTag}. Saldo anterior: {tarjeta.Saldo + monto}, Saldo actual: {tarjeta.Saldo}");
            return true;
        }

        public async Task<decimal> ConsultarSaldoAsync(string numeroTag)
        {
            var tarjeta = await GetByNumeroTagAsync(numeroTag);
            if (tarjeta == null)
            {
                _logger.LogWarning($"Tarjeta con tag {numeroTag} no encontrada para consulta de saldo");
                return -1; // Código de error
            }

            return tarjeta.Saldo;
        }

        public async Task<TarjetaRFID> CrearTarjetaAsync(string numeroTag, int clienteId, decimal saldoInicial)
        {
            // Verificar que no exista ya una tarjeta con ese tag
            try
            {
                var tarjetaExistente = await GetByNumeroTagAsync(numeroTag);
                _logger.LogWarning($"Ya existe una tarjeta con el tag {numeroTag}");
                return new TarjetaRFID(); // Retorna un objeto vacío para indicar error
            }
            catch (KeyNotFoundException)
            {
                // No existe tarjeta, podemos continuar
            }

            // Verificar que exista el cliente
            var cliente = await _context.Clientes.FindAsync(clienteId);
            if (cliente == null)
            {
                _logger.LogWarning($"Cliente con ID {clienteId} no encontrado");
                return new TarjetaRFID(); // Retorna un objeto vacío para indicar error
            }

            var nuevaTarjeta = new TarjetaRFID
            {
                NumeroTag = numeroTag,
                ClienteId = clienteId,
                Saldo = saldoInicial,
                Estado = "Activa",
                FechaEmision = DateTime.Now,
                FechaVencimiento = DateTime.Now.AddYears(3) // Validez de 3 años por defecto
            };

            await _context.TarjetasRFID.AddAsync(nuevaTarjeta);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"Nueva tarjeta RFID creada con ID {nuevaTarjeta.Id} para cliente {clienteId}");
            return nuevaTarjeta;
        }

        public async Task<bool> BloquearTarjetaAsync(string numeroTag)
        {
            var tarjeta = await GetByNumeroTagAsync(numeroTag);
            if (tarjeta == null)
            {
                _logger.LogWarning($"Tarjeta con tag {numeroTag} no encontrada para bloqueo");
                return false;
            }

            tarjeta.Estado = "Bloqueada";
            _context.TarjetasRFID.Update(tarjeta);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"Tarjeta {numeroTag} bloqueada correctamente");
            return true;
        }

        public async Task<bool> DesbloquearTarjetaAsync(string numeroTag)
        {
            var tarjeta = await GetByNumeroTagAsync(numeroTag);
            if (tarjeta == null)
            {
                _logger.LogWarning($"Tarjeta con tag {numeroTag} no encontrada para desbloqueo");
                return false;
            }

            tarjeta.Estado = "Activa";
            _context.TarjetasRFID.Update(tarjeta);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"Tarjeta {numeroTag} desbloqueada correctamente");
            return true;
        }

        public async Task<bool> RecargarSaldoAsync(string numeroTag, decimal monto)
        {
            var tarjeta = await GetByNumeroTagAsync(numeroTag);
            if (tarjeta == null)
            {
                _logger.LogWarning($"Tarjeta con tag {numeroTag} no encontrada para recarga");
                return false;
            }

            if (tarjeta.Estado == "Bloqueada")
            {
                _logger.LogWarning($"No se puede recargar tarjeta bloqueada con tag {numeroTag}");
                return false;
            }

            tarjeta.Saldo += monto;
            _context.TarjetasRFID.Update(tarjeta);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"Saldo recargado correctamente en tarjeta {numeroTag}. Saldo anterior: {tarjeta.Saldo - monto}, Saldo actual: {tarjeta.Saldo}");
            return true;
        }

        public async Task<IEnumerable<TarjetaRFID>> GetTarjetasByClienteIdAsync(int clienteId)
        {
            return await _context.TarjetasRFID
                .Where(t => t.ClienteId == clienteId)
                .ToListAsync();
        }
    }
}
