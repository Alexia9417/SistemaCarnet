using UsuarioApi.Models;

namespace UsuarioApi.Business
{
    public interface IUsuarioService
    {
        Task<IEnumerable<Usuario>> ObtenerTodosAsync();
        Task<Usuario?> ObtenerPorEmailAsync(string email);
        Task<IEnumerable<Usuario>> FiltrarAsync(string? identificacion, string? nombre, int? tipo);

       
        Task<(Usuario? usuario, string? error)> CrearUsuarioAsync(UsuarioDto dto);
        Task<(Usuario? usuario, string? error)> ModificarUsuarioAsync(string email, UsuarioDto dto);

        Task<bool> EliminarUsuarioAsync(string email);
    }
}
