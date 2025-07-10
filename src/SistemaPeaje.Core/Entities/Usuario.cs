using System.ComponentModel.DataAnnotations;

namespace SistemaPeaje.Core.Entities
{
    public class Usuario : BaseEntity
    {
        [Required]
        [StringLength(50)]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(256)]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Nombres { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Apellidos { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Rol { get; set; } = string.Empty; // "ADMIN", "OPERADOR", "SUPERVISOR"

        [StringLength(20)]
        public string? Telefono { get; set; }

        public bool EsActivo { get; set; } = true;

        public DateTime? UltimoAcceso { get; set; }

        public int? EmpleadoId { get; set; }

        public int? EstacionId { get; set; }

        [StringLength(500)]
        public string? Permisos { get; set; } // JSON string con permisos específicos

        // Navigation properties
        public virtual Empleado? Empleado { get; set; }
        public virtual Estacion? Estacion { get; set; }
    }
}
