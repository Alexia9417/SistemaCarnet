namespace UsuarioApi.Models
{
    public class UsuarioDto
    {
        public string Email { get; set; } = string.Empty;
        public int TipoIdentificacion { get; set; }
        public string Identificacion { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string PrimerApellido { get; set; } = string.Empty;
        public string SegundoApellido { get; set; } = string.Empty;
        public string Contrasena { get; set; } = string.Empty;
        public int TipoUsuario { get; set; }


        public List<long>? Telefonos { get; set; }

        public List<int>? Carreras { get; set; } 
        public List<int>? Areas { get; set; }    
    }
}
