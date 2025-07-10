using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaPeaje.Core.Entities
{
    public class EventoTransito : BaseEntity
    {
        [Required]
        public int EstacionId { get; set; }
        
        [Required]
        public int CarrilId { get; set; }
        
        [Required]
        public DateTime FechaEvento { get; set; }
        
        [Required]
        [StringLength(50)]
        public string TipoEvento { get; set; } = string.Empty; // "VehiculoDetectado", "PagoRealizado", "Error", etc.
        
        [StringLength(500)]
        public string? Descripcion { get; set; }
        
        [StringLength(1000)]
        public string? DatosAdicionales { get; set; }
        
        [StringLength(20)]
        public string? PlacaVehiculo { get; set; }
        
        public int? TipoVehiculoId { get; set; }
        
        [StringLength(100)]
        public string? CodigoRfid { get; set; }
        
        [StringLength(500)]
        public string? Observaciones { get; set; }
        
        public bool Procesado { get; set; } = false;
        
        public int? TransaccionId { get; set; }
        
        [StringLength(200)]
        public string? RutaImagen { get; set; }
        
        [StringLength(50)]
        public string? EstadoEvento { get; set; } = "PENDIENTE"; // "PENDIENTE", "PROCESADO", "ERROR"
        
        [Column(TypeName = "decimal(10,2)")]
        public decimal? VelocidadVehiculo { get; set; }
        
        [StringLength(100)]
        public string? SensorId { get; set; }

        // Navigation properties
        public virtual Estacion? Estacion { get; set; }
        public virtual Carril? Carril { get; set; }
        public virtual TipoVehiculo? TipoVehiculo { get; set; }
        public virtual Transaccion? Transaccion { get; set; }
    }
}
