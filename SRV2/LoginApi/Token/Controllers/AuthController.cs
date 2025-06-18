using Azure.Core;
using loginapi.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login()
    {
        try
        {
            var email = Request.Headers["email"].ToString();
            var password = Request.Headers["password"].ToString();
            var tipoUsuario = Request.Headers["tipo_usuario"].ToString();

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(tipoUsuario))
                return BadRequest(new { codigo = 401,mensaje = "Usuario y/o contraseña incorrectos" });

            if (!int.TryParse(tipoUsuario, out int tipoId))
                return BadRequest(new { codigo=401, mensaje = "Usuario y/o contraseña incorrectos" });

            var result = await _authService.LoginAsync(email, password, tipoId);
            if (!result.Success)
                return Unauthorized(new { mensaje = result.Message });

            return StatusCode(201, result.TokenData);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensaje = "Error interno en el servidor"/*, detalle = ex.Message */});
        }
    }//fin  login

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] string refreshToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return BadRequest(new { codigo = 400 ,mensaje = "Datos vacíos" });

            var result = await _authService.RefreshTokenAsync(refreshToken);
            if (!result.Success)
                return Unauthorized(new { codigo = 401, mensaje = "No autorizado" });

            return StatusCode(201, result.TokenData);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { codigo = 500, mensaje = "Error interno en el servidor"/*, detalle = ex.Message*/ });
        }
    }//fin refresh

    [HttpPost("validate")]
    public IActionResult Validate([FromBody] string token)
    {
        try
        {
            // Verificamos el Content-Type del request
            var contentType = Request.ContentType;
            if (string.IsNullOrWhiteSpace(contentType) || !contentType.Contains("application/json", StringComparison.OrdinalIgnoreCase))
            {
                return StatusCode(415, new
                {
                    codigo = 415,
                    mensaje = "Tipo de contenido no soportado."
                });
            }

            if (string.IsNullOrWhiteSpace(token))
            {
                return BadRequest(new { codigo = 400, mensaje = "Datos vacíos" });
            }

            var isValid = _authService.ValidarToken(token);
            if (!isValid)
            {
                return Unauthorized(new { codigo = 401, mensaje = "No autorizado" });
            }

            return Ok(new { codigo = 200, mensaje = "True" });
        }
        catch (Exception)
        {
            return StatusCode(500, new
            {
                codigo = 500,
                mensaje = "Error interno en el servidor"
            });
        }
    }//fin metodo validate

}//FinClase
