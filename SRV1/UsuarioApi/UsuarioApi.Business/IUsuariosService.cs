using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsuarioApi.Models;

namespace UsuarioApi.Business
{
    public interface IUsuarioService
    {
        Task<IEnumerable<Usuario>> ObtenerTodosAsync();
        Task<Usuario?> ObtenerPorEmailAsync(string email);
        Task<IEnumerable<Usuario>> FiltrarAsync(string? identificacion, string? nombre, int? tipo);
        Task<Usuario?> CrearUsuarioAsync(UsuarioDto dto);
        Task<Usuario?> ModificarUsuarioAsync(string email, UsuarioDto dto);

        Task<bool> EliminarUsuarioAsync(string email);
    }
}
