using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace UsuarioApi.Models
{
    public class UsuarioCarrera
    {
        public string UsuarioEmail { get; set; }
        public int CarreraId { get; set; }
        [JsonIgnore]
        public Usuario Usuario { get; set; }
    }
}
