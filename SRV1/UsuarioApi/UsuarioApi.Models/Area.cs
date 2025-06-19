using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace UsuarioApi.Models
{
 

    [Table("areas")]
    public class Area
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }

}
