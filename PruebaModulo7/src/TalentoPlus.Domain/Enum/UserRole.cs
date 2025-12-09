namespace TalentoPlus.Domain.Enums;

public enum UserRole
{
    Admin = 1,      // Acceso a web admin
    Empleado = 2    // Acceso a API
}

// Extensión para convertir a string
public static class UserRoleExtensions
{
    public static string ToString(this UserRole role)
    {
        return role switch
        {
            UserRole.Admin => "Admin",
            UserRole.Empleado => "Empleado",
            _ => "Unknown"
        };
    }
    
    public static UserRole FromString(string role)
    {
        return role switch
        {
            "Admin" => UserRole.Admin,
            "Empleado" => UserRole.Empleado,
            _ => throw new ArgumentException($"Rol no válido: {role}")
        };
    }
}