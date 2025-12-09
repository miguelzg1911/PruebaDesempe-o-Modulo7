using TalentoPlus.Domain.Entities;
using TalentoPlus.Domain.Enums;

public class User
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string PasswordHash { get; set; }  = string.Empty;
    public string Email { get; set; }  = string.Empty;
    public UserRole? Role { get; set; }

    public string? RefreshToken { get; set; } = string.Empty;
    public DateTime? RefreshTokenExpiry { get; set; }
    
    public int? EmpleadoId { get; set; }
    public Empleado? Empleado { get; set; }
}