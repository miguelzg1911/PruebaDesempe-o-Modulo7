using TalentoPlus.Application.DTOs.Auth;

namespace TalentoPlus.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResult> LoginAsync(string email, string password);
    Task<LoginResult> RefreshTokenAsync(string token, string refreshToken);
    Task<RegisterResult> RegisterEmpleadoAsync(RegisterRequest request);
    Task<bool> SendWelcomeEmailAsync(string email, string nombre);
}

public class LoginResult
{
    public string Token { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public int ExpiresIn { get; set; }
}

public class RegisterResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int? UserId { get; set; }
}