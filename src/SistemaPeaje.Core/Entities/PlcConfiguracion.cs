using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaPeaje.Core.Entities
{
    /// <summary>
    /// Configuración de PLC para monitoreo dinámico
    /// </summary>
    public class PlcConfiguracion : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty; // Nombre descriptivo del PLC
        
        [Required]
        [StringLength(50)]
        public string Ip { get; set; } = string.Empty; // IP del PLC
        
        [Required]
        public int Puerto { get; set; } = 502; // Puerto Modbus TCP
        
        [Required]
        public byte UnitId { get; set; } = 1; // ID de unidad Modbus
        
        [Required]
        public ushort DireccionInicial { get; set; } = 1000; // Dirección inicial de coils
        
        [Required]
        public ushort CantidadCoils { get; set; } = 4; // Cantidad de coils a leer
        
        [Required]
        public int IntervaloMonitoreo { get; set; } = 2000; // Intervalo en milisegundos
        
        public bool HabilitarLoggingPeriodico { get; set; } = true;
        
        [Required]
        public int EstacionId { get; set; } // FK a Estacion
        
        [Required]
        public int CarrilId { get; set; } // FK a Carril
        
        public bool EstaConectado { get; set; } = false; // Estado actual de conexión
        
        public DateTime? UltimaConexion { get; set; } // Última vez que se conectó
        
        [StringLength(500)]
        public string? Observaciones { get; set; }
        
        // Navigation properties
        public virtual Estacion? Estacion { get; set; }
        public virtual Carril? Carril { get; set; }
        public virtual ICollection<PlcCoilConfiguracion> CoilsConfiguracion { get; set; } = new List<PlcCoilConfiguracion>();
    }
}
