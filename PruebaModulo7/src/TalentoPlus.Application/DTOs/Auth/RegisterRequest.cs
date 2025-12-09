namespace TalentoPlus.Application.DTOs.Auth;

public class RegisterRequest
{
    public string Documento { get; set; }
    public string Nombres { get; set; }
    public string Apellidos { get; set; }  
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } 
    public string Telefono { get; set; }
    public int DepartamentoId { get; set; }
}