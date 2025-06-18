using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UsuarioApi.Models
{
    [Table("usuario")]
    public class Usuario
    {
        [Key]
        [Column("email")]
        [Required]
        [EmailAddress]
        [MaxLength(70)]
        public string Email { get; set; }

        [Column("tipo_identificacion")]
        [Required]
        public int TipoIdentificacion { get; set; }

        [Column("identificacion")]
        [Required]
        [MaxLength(20)]
        public string Identificacion { get; set; }

        [Column("nombre")]
        [Required]
        [MaxLength(50)]
        public string Nombre { get; set; }

        [Column("primer_apellido")]
        [Required]
        [MaxLength(60)]
        public string PrimerApellido { get; set; }

        [Column("segundo_apellido")]
        [Required]
        [MaxLength(60)]
        public string SegundoApellido { get; set; }

        [Column("password")]
        [Required]
        [MaxLength(255)]
        public string Contrasena { get; set; }

        [Column("tipo_usuario")]
        [Required]
        public int TipoUsuario { get; set; }

        [Column("estado")]
        [Required]
        public int Estado { get; set; }

        // Relaciones
        public ICollection<UsuarioTelefono>? Telefonos { get; set; }
        public ICollection<UsuarioCarrera>? Carreras { get; set; }
        public ICollection<UsuarioArea>? Areas { get; set; }
    }
}
