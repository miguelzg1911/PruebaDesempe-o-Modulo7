namespace TalentoPlus.Application.DTOs.Empleado;

public class UpdateEmpleadoDto
{
    public string? Nombres { get; set; }
    public string? Apellidos { get; set; }
    public DateTime? FechaNacimiento { get; set; }
    public string? Direccion { get; set; }
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public string? Cargo { get; set; }
    public decimal? Salario { get; set; }
    public DateTime? FechaIngreso { get; set; }
    public string? Estado { get; set; }
    public string? NivelEducativo { get; set; }
    public string? PerfilProfesional { get; set; }
    public int? DepartamentoId { get; set; }
}