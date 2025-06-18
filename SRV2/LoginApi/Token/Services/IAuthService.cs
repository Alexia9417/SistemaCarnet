namespace loginapi.Services
{
    public interface IAuthService
    {
        Task<(bool Success, string Message, object TokenData)> LoginAsync(string email, string password, int tipoUsuarioId);
        Task<(bool Success, string Message, object TokenData)> RefreshTokenAsync(string refreshToken);
        bool ValidarToken(string token);
    }
}