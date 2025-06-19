using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UsuarioApi.DataAccess;
using UsuarioApi.Models;

namespace UsuarioApi.Business
{
    public class UsuarioService : IUsuarioService
    {
        private readonly CarnetDbContext _context;

        public UsuarioService(CarnetDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Usuario>> ObtenerTodosAsync()
        {
            return await _context.Usuarios
                .Include(u => u.Telefonos)
                .Include(u => u.Carreras)
                .Include(u => u.Areas)
                .ToListAsync();
        }

        public async Task<Usuario?> ObtenerPorEmailAsync(string email)
        {
            return await _context.Usuarios
                .Include(u => u.Telefonos)
                .Include(u => u.Carreras)
                .Include(u => u.Areas)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<Usuario>> FiltrarAsync(string? identificacion, string? nombre, int? tipo)
        {
            var query = _context.Usuarios
                .Include(u => u.Telefonos)
                .Include(u => u.Carreras)
                .Include(u => u.Areas)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(identificacion))
                query = query.Where(u => u.Identificacion.Contains(identificacion));

            if (!string.IsNullOrWhiteSpace(nombre))
                query = query.Where(u => u.Nombre.Contains(nombre));

            if (tipo.HasValue)
                query = query.Where(u => u.TipoUsuario == tipo);

            return await query.ToListAsync();
        }

        public async Task<Usuario?> CrearUsuarioAsync(UsuarioDto dto)
        {
            if (!ValidarDto(dto)) return null;

            if (await _context.Usuarios.AnyAsync(u => u.Email == dto.Email))
                return null;

            var hasher = new PasswordHasher<string>();
            var usuario = new Usuario
            {
                Email = dto.Email,
                TipoIdentificacion = dto.TipoIdentificacion,
                Identificacion = dto.Identificacion,
                Nombre = dto.Nombre,
                PrimerApellido = dto.PrimerApellido,
                SegundoApellido = dto.SegundoApellido,
                Contrasena = hasher.HashPassword(null, dto.Contrasena),
                TipoUsuario = dto.TipoUsuario,
                Estado = 1
            };

            _context.Usuarios.Add(usuario);

            if (dto.Telefonos != null)
            {
                foreach (var numero in dto.Telefonos)
                {
                    _context.UsuarioTelefonos.Add(new UsuarioTelefono
                    {
                        UsuarioEmail = dto.Email,
                        Numero = numero
                    });
                }
            }

            if (dto.TipoUsuario == 2 && dto.Carreras != null)
            {
                foreach (var carreraId in dto.Carreras)
                {
                    _context.UsuarioCarreras.Add(new UsuarioCarrera
                    {
                        UsuarioEmail = dto.Email,
                        CarreraId = carreraId
                    });
                }
            }

            if ((dto.TipoUsuario == 1 || dto.TipoUsuario == 3) && dto.Areas != null)
            {
                foreach (var areaId in dto.Areas)
                {
                    _context.UsuarioAreas.Add(new UsuarioArea
                    {
                        UsuarioEmail = dto.Email,
                        AreaId = areaId
                    });
                }
            }

            await _context.SaveChangesAsync();
            return usuario;
        }

        public async Task<Usuario?> ModificarUsuarioAsync(string email, UsuarioDto dto)
        {
            if (!ValidarDto(dto)) return null;

            var usuario = await _context.Usuarios.FindAsync(email);
            if (usuario == null) return null;

            usuario.TipoIdentificacion = dto.TipoIdentificacion;
            usuario.Identificacion = dto.Identificacion;
            usuario.Nombre = dto.Nombre;
            usuario.PrimerApellido = dto.PrimerApellido;
            usuario.SegundoApellido = dto.SegundoApellido;
            usuario.TipoUsuario = dto.TipoUsuario;
            usuario.Contrasena = new PasswordHasher<string>().HashPassword(null, dto.Contrasena);

            // Eliminar relaciones actuales
            var telefonos = _context.UsuarioTelefonos.Where(t => t.UsuarioEmail == email);
            var carreras = _context.UsuarioCarreras.Where(c => c.UsuarioEmail == email);
            var areas = _context.UsuarioAreas.Where(a => a.UsuarioEmail == email);

            _context.UsuarioTelefonos.RemoveRange(telefonos);
            _context.UsuarioCarreras.RemoveRange(carreras);
            _context.UsuarioAreas.RemoveRange(areas);

            // Nuevas relaciones
            if (dto.Telefonos != null)
            {
                foreach (var numero in dto.Telefonos)
                {
                    _context.UsuarioTelefonos.Add(new UsuarioTelefono
                    {
                        UsuarioEmail = email,
                        Numero = numero
                    });
                }
            }

            if (dto.TipoUsuario == 2 && dto.Carreras != null)
            {
                foreach (var carreraId in dto.Carreras)
                {
                    _context.UsuarioCarreras.Add(new UsuarioCarrera
                    {
                        UsuarioEmail = email,
                        CarreraId = carreraId
                    });
                }
            }

            if ((dto.TipoUsuario == 1 || dto.TipoUsuario == 3) && dto.Areas != null)
            {
                foreach (var areaId in dto.Areas)
                {
                    _context.UsuarioAreas.Add(new UsuarioArea
                    {
                        UsuarioEmail = email,
                        AreaId = areaId
                    });
                }
            }

            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        public async Task<bool> EliminarUsuarioAsync(string email)
        {
            var usuario = await _context.Usuarios.FindAsync(email);
            if (usuario == null) return false;

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            return true;
        }

        private bool ValidarDto(UsuarioDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email)) return false;
            if (!dto.Email.Contains("@")) return false;

            bool esCuc = dto.Email.EndsWith("@cuc.cr", StringComparison.OrdinalIgnoreCase);
            bool esCucAc = dto.Email.EndsWith("@cuc.ac.cr", StringComparison.OrdinalIgnoreCase);

            if (!esCuc && !esCucAc) return false;
            if (esCuc && dto.TipoUsuario != 2) return false;
            if (esCucAc && dto.TipoUsuario == 2) return false;

            if (string.IsNullOrWhiteSpace(dto.Nombre) ||
                string.IsNullOrWhiteSpace(dto.PrimerApellido) ||
                string.IsNullOrWhiteSpace(dto.SegundoApellido)) return false;

            if (dto.TipoUsuario == 2 && (dto.Carreras == null || !dto.Carreras.Any())) return false;
            if ((dto.TipoUsuario == 1 || dto.TipoUsuario == 3) && (dto.Areas == null || !dto.Areas.Any())) return false;

            return true;
        }
    }
}
