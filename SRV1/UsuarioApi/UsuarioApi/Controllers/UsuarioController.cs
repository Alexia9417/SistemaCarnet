using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UsuarioApi.Models;
using UsuarioApi.Business;
using UsuarioApi.Filter;

namespace UsuarioApi.Controllers
{
    [ServiceFilter(typeof(Validate))]
    [ApiController]
    [Route("usuario")]
  
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _service;

        public UsuarioController(IUsuarioService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> CrearUsuario([FromBody] UsuarioDto usuario)
        {
            try
            {
                var creado = await _service.CrearUsuarioAsync(usuario);

                if (creado == null)
                {
                    var yaExiste = await _service.ObtenerPorEmailAsync(usuario.Email);
                    if (yaExiste != null)
                    {
                        return Conflict(new
                        {
                            status = 409,
                            mensaje = "Ya existe un usuario con este correo electrónico."
                        });
                    }

                    return BadRequest(new
                    {
                        status = 400,
                        mensaje = "Error al crear el usuario. Verifique los datos."
                    });
                }

                return CreatedAtAction(nameof(ObtenerPorEmail), new { email = creado.Email }, new
                {
                    status = 201,
                    mensaje = "Usuario creado correctamente.",
                    data = new
                    {
                        creado.Email,
                        creado.TipoIdentificacion,
                        creado.Identificacion,
                        creado.Nombre,
                        creado.PrimerApellido,
                        creado.SegundoApellido,
                        creado.TipoUsuario,
                        creado.Estado,
                        Telefonos = creado.Telefonos?.Select(t => t.Numero).ToList(),
                        Carreras = creado.Carreras?.Select(c => c.CarreraId).ToList(),
                        Areas = creado.Areas?.Select(a => a.AreaId).ToList()
                    }
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new
                {
                    status = 401,
                    mensaje = "No autorizado."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    mensaje = "Error interno del servidor.",
                    error = ex.Message
                });
            }
        }

        //Modificar usuario
        [HttpPut("{email}")]
        public async Task<IActionResult> ModificarUsuario(string email, [FromBody] UsuarioDto usuario)
        {
            try
            {
                var actualizado = await _service.ModificarUsuarioAsync(email, usuario);

                if (actualizado == null)

                    {
                        return BadRequest(new
                    {
                        status = 400,
                        mensaje = "Error al modificar el usuario. Verifique que los datos sean correctos."
                    });
                }

                return Ok(new
                {
                    status = 200,
                    mensaje = "Ok",
                    data = usuario
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new
                {
                    status = 401,
                    mensaje = "No autorizado."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    mensaje = "Error interno del servidor.",
                    error = ex.Message
                });
            }
        }


        //Eliminar usuario
        [HttpDelete("{email}")]
        public async Task<IActionResult> EliminarUsuario(string email)
        {
            try
            {
                var eliminado = await _service.EliminarUsuarioAsync(email);

                if (!eliminado)
                {
                    return NotFound(new
                    {
                        status = 404,
                        mensaje = "Usuario no encontrado. No se puede eliminar."
                    });
                }

                return Ok(new
                {
                    status = 200,
                    mensaje = "Ok."
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new
                {
                    status = 401,
                    mensaje = "No autorizado."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    mensaje = "Error interno del servidor.",
                    error = ex.Message
                });
            }
        }


        [HttpGet("all")]
        public async Task<IActionResult> ObtenerTodos()
        {
            try
            {
                var usuarios = await _service.ObtenerTodosAsync();

                if (!usuarios.Any())
                {
                    return NotFound(new
                    {
                        status = 404,
                        mensaje = "No se encontraron usuarios registrados."
                    });
                }

                return Ok(new
                {
                    status = 200,
                    mensaje = "OK",
                    data = usuarios
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    mensaje = "Error interno al obtener los usuarios.",
                    error = ex.Message
                });
            }
        }


        [HttpGet("{email}")]
        public async Task<IActionResult> ObtenerPorEmail(string email)
        {
            try
            {
                var resultado = await _service.ObtenerPorEmailAsync(email);

                if (resultado == null)
                {
                    return NotFound(new
                    {
                        status = 404,
                        mensaje = "Usuario no encontrado."
                    });
                }

                return Ok(new
                {
                    status = 200,
                    mensaje = "Ok",
                    data = resultado
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    mensaje = "Error interno del servidor.",
                    error = ex.Message
                });
            }
        }


        [HttpGet("filtrar")]
        public async Task<IActionResult> FiltrarUsuarios(
            [FromQuery] string? identificacion,
            [FromQuery] string? nombre,
            [FromQuery] int? tipo)
        {
            try
            {
                var usuarios = await _service.FiltrarAsync(identificacion, nombre, tipo);

                if (!usuarios.Any())
                    return NotFound(new { status = 404, mensaje = "No se encontraron usuarios con datos proporcionados." });

                return Ok(new
                {
                    status = 200,
                    mensaje = "Ok.",
                    data = usuarios
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = 500, mensaje = "Error interno del servidor.", error = ex.Message });
            }
        }
    }
}
