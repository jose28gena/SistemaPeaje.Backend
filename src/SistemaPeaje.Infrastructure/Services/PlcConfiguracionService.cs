using Microsoft.EntityFrameworkCore;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;
using SistemaPeaje.Infrastructure.Data;

namespace SistemaPeaje.Infrastructure.Services
{
    public class PlcConfiguracionService : IPlcConfiguracionService
    {
        private readonly ApplicationDbContext _context;

        public PlcConfiguracionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PlcConfiguracion>> ObtenerTodasLasConfiguracionesAsync()
        {
            return await _context.PlcConfiguraciones
                .Include(p => p.Estacion)
                .Include(p => p.Carril)
                .Include(p => p.CoilsConfiguracion)
                .ToListAsync();
        }

        public async Task<IEnumerable<PlcConfiguracion>> ObtenerConfiguracionesActivasAsync()
        {
            return await _context.PlcConfiguraciones
                .Include(p => p.Estacion)
                .Include(p => p.Carril)
                .Include(p => p.CoilsConfiguracion)
                .Where(p => p.Activo)
                .ToListAsync();
        }

        public async Task<PlcConfiguracion?> ObtenerConfiguracionPorIdAsync(int id)
        {
            return await _context.PlcConfiguraciones
                .Include(p => p.Estacion)
                .Include(p => p.Carril)
                .Include(p => p.CoilsConfiguracion)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<PlcConfiguracion?> ObtenerConfiguracionPorIpAsync(string ip, int puerto)
        {
            return await _context.PlcConfiguraciones
                .Include(p => p.Estacion)
                .Include(p => p.Carril)
                .Include(p => p.CoilsConfiguracion)
                .FirstOrDefaultAsync(p => p.Ip == ip && p.Puerto == puerto);
        }

        public async Task<PlcConfiguracion> CrearConfiguracionAsync(PlcConfiguracion configuracion)
        {
            // Verificar que no existe una configuración duplicada
            var existe = await ExisteConfiguracionAsync(configuracion.Ip, configuracion.Puerto);
            if (existe)
            {
                throw new InvalidOperationException($"Ya existe una configuración para el PLC {configuracion.Ip}:{configuracion.Puerto}");
            }

            configuracion.FechaCreacion = DateTime.UtcNow;
            configuracion.Activo = true;

            _context.PlcConfiguraciones.Add(configuracion);
            await _context.SaveChangesAsync();

            return configuracion;
        }

        public async Task<PlcConfiguracion> ActualizarConfiguracionAsync(PlcConfiguracion configuracion)
        {
            // Verificar que no existe una configuración duplicada (excluyendo la actual)
            var existe = await ExisteConfiguracionAsync(configuracion.Ip, configuracion.Puerto, configuracion.Id);
            if (existe)
            {
                throw new InvalidOperationException($"Ya existe otra configuración para el PLC {configuracion.Ip}:{configuracion.Puerto}");
            }

            configuracion.FechaActualizacion = DateTime.UtcNow;

            _context.PlcConfiguraciones.Update(configuracion);
            await _context.SaveChangesAsync();

            return configuracion;
        }

        public async Task<bool> EliminarConfiguracionAsync(int id)
        {
            var configuracion = await _context.PlcConfiguraciones.FindAsync(id);
            if (configuracion == null)
                return false;

            // Soft delete
            configuracion.Activo = false;
            configuracion.FechaActualizacion = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task ActualizarEstadoConexionAsync(int id, bool estaConectado)
        {
            var configuracion = await _context.PlcConfiguraciones.FindAsync(id);
            if (configuracion != null)
            {
                configuracion.EstaConectado = estaConectado;
                if (estaConectado)
                {
                    configuracion.UltimaConexion = DateTime.UtcNow;
                }
                configuracion.FechaActualizacion = DateTime.UtcNow;

                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExisteConfiguracionAsync(string ip, int puerto, int? excludeId = null)
        {
            var query = _context.PlcConfiguraciones
                .Where(p => p.Ip == ip && p.Puerto == puerto && p.Activo);

            if (excludeId.HasValue)
            {
                query = query.Where(p => p.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }
    }
}
