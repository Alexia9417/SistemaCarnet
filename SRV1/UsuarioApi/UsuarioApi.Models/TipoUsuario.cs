using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace UsuarioApi.Models
{
    

    [Table("tipos_usuario")]
    public class TipoUsuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }

}
