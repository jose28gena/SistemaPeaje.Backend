using System.ComponentModel.DataAnnotations;

namespace SistemaPeaje.Core.Entities
{
    /// <summary>
    /// Configuración de coils individuales del PLC
    /// </summary>
    public class PlcCoilConfiguracion : BaseEntity
    {
        [Required]
        public int PlcConfiguracionId { get; set; } // FK a PlcConfiguracion
        
        [Required]
        public int Indice { get; set; } // Índice del coil (0, 1, 2, 3...)
        
        [Required]
        public ushort Direccion { get; set; } // Dirección absoluta del coil
        
        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty; // Nombre descriptivo (Presencia, BarreraAbierta, etc.)
        
        [StringLength(500)]
        public string? Descripcion { get; set; } // Descripción detallada
        
        [StringLength(50)]
        public string? TipoEvento { get; set; } // Tipo de evento que genera (VEHICULO_DETECTADO, BARRERA_ABIERTA, etc.)
        
        public bool GenerarEvento { get; set; } = true; // Si debe generar eventos en BD al cambiar
        
        public bool EsAlarma { get; set; } = false; // Si es un coil de alarma crítica
        
        public bool EstadoActual { get; set; } = false; // Último estado leído
        
        public DateTime? UltimaActualizacion { get; set; } // Última vez que cambió
        
        [StringLength(200)]
        public string? AccionEspecial { get; set; } // Acción especial a ejecutar al cambiar
        
        // Navigation property
        public virtual PlcConfiguracion? PlcConfiguracion { get; set; }
    }
}
