using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace UsuarioApi.Models
{
    public class UsuarioArea
    {
        public string UsuarioEmail { get; set; }
        public int AreaId { get; set; }
        [JsonIgnore]
        public Usuario Usuario { get; set; }
    }
}
