using loginapi.Data;
using loginapi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace loginapi.Services
{
    public class AuthService : IAuthService
    {
        private readonly CarnetDbContext _context;
        private readonly IConfiguration _configuration;
        public AuthService(CarnetDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        public async Task<(bool Success, string Message, object TokenData)> LoginAsync(string email, string password, int tipoUsuarioId)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == email && u.tipo_usuario == tipoUsuarioId);

            if (usuario == null)
                return (false, "Usuario y/o contraseña incorrectos", null);

            var hasher = new PasswordHasher<string>();
            var result = hasher.VerifyHashedPassword(null, usuario.password, password);
            if (result == PasswordVerificationResult.Failed)
                return (false, "Usuario y/o contraseña incorrectos", null);

            int accessMinutes = int.Parse(_configuration["Jwt:AccessTokenMinutes"]);
            int refreshMinutes = int.Parse(_configuration["Jwt:RefreshTokenMinutes"]);

            var accessToken = GenerarToken(usuario.Email, tipoUsuarioId.ToString(), accessMinutes, "access");
            var refreshToken = GenerarToken(usuario.Email, tipoUsuarioId.ToString(), refreshMinutes, "refresh");


            var refreshTokenEntity = new Token
            {
                UsuarioEmail = usuario.Email,
                TokenValor = refreshToken,
                Tipo = "refresh",
                CreadoEn = DateTime.UtcNow,
                Expiracion = DateTime.UtcNow.AddMinutes(refreshMinutes),
                Estado = true
            };
            _context.Tokens.Add(refreshTokenEntity);
            await _context.SaveChangesAsync();

            return (true, "Autenticación exitosa", new
            {
                expires_in = DateTime.Now.AddMinutes(accessMinutes).ToString("HH:mm:ss"),
                access_token = accessToken,
                refresh_token = refreshToken,
                usuarioID = usuario.Email
            });
        }
        public async Task<(bool Success, string Message, object TokenData)> RefreshTokenAsync(string refreshToken)
        {
            var tokenValido = await _context.Tokens
                .FirstOrDefaultAsync(t => t.TokenValor == refreshToken && t.Estado && t.Expiracion > DateTime.UtcNow);

            if (tokenValido == null)
                return (false, "No autorizado", null);

            var principal = DecodeToken(refreshToken);
            if (principal == null)
                return (false, "No autorizado", null);

            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var tipo = principal.FindFirstValue(ClaimTypes.Role);

            int accessMinutes = int.Parse(_configuration["Jwt:AccessTokenMinutes"]);
            int refreshMinutes = int.Parse(_configuration["Jwt:RefreshTokenMinutes"]);

            var newAccessToken = GenerarToken(userId, tipo, accessMinutes, "access");
            var newRefreshToken = GenerarToken(userId, tipo, refreshMinutes, "refresh");

            var nuevo = new Token
            {
                UsuarioEmail = tokenValido.UsuarioEmail,
                TokenValor = newRefreshToken,
                Tipo = "refresh",
                CreadoEn = DateTime.UtcNow,
                Expiracion = DateTime.UtcNow.AddMinutes(refreshMinutes),
                Estado = true
            };
            tokenValido.Estado = false;
            _context.Tokens.Update(tokenValido);
            _context.Tokens.Add(nuevo);
            await _context.SaveChangesAsync();

            return (true, "Renovación exitosa", new
            {
                expires_in = DateTime.Now.AddMinutes(accessMinutes).ToString("HH:mm:ss"),
                access_token = newAccessToken,
                refresh_token = newRefreshToken
            });
        }
        public bool ValidarToken(string token)
        {
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero, 
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                }, out SecurityToken validatedToken);

                var jwtToken = validatedToken as JwtSecurityToken;
                if (jwtToken == null)
                    return false;

                var tokenType = jwtToken.Claims.FirstOrDefault(c => c.Type == "token_type")?.Value;

                if (string.IsNullOrEmpty(tokenType))
                    return false;

                return tokenType == "access";
            }
            catch
            {
                return false;
            }
        }
        private ClaimsPrincipal DecodeToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
                return tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                }, out SecurityToken validatedToken);
            }
            catch
            {
                return null;
            }
        }
        private string GenerarToken(string userId, string tipoUsuario, int expireMinutes, string tokenType)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, tipoUsuario),
                new Claim("token_type", tokenType)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expireMinutes),
                signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }//finClass
}//finNamespace
