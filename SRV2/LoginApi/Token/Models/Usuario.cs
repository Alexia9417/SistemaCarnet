using System.Collections.Generic;

namespace loginapi.Models
{
    public class Usuario
    {
        public string Email { get; set; }

        public string password { get; set; }

        public int tipo_usuario { get; set; }

        public int estado { get; set; }

        public ICollection<Token> Tokens { get; set; }
    }
}
