using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace loginapi.Models
{
    public class Token
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("usuario_email")]
        public string UsuarioEmail { get; set; }

        [Column("token")]
        public string TokenValor { get; set; }

        [Column("tipo")]
        public string Tipo { get; set; }

        [Column("creado_en")]
        public DateTime CreadoEn { get; set; }

        [Column("expiracion")]
        public DateTime? Expiracion { get; set; }

        [Column("estado")]
        public bool Estado { get; set; }

        public Usuario Usuario { get; set; }
    }
}
