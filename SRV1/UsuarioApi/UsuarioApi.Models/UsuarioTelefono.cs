using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace UsuarioApi.Models
{
    public class UsuarioTelefono
    {
        public string UsuarioEmail { get; set; }
        public long Numero { get; set; }

        [JsonIgnore]
        public Usuario Usuario { get; set; }
    }
}
