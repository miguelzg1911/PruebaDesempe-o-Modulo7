namespace TalentoPlus.Application.DTOs.Departamento;

public class DepartamentoDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Codigo { get; set; } = string.Empty;
    public int TotalEmpleados { get; set; }
}