using System;
using TalentoPlus.Domain.Enums;

namespace TalentoPlus.Domain.Entities;

public class Empleado
{
    public int Id { get; set; }
    public string Documento { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public DateTime? FechaNacimiento { get; set; }
    public string Direccion { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Cargo { get; set; } = string.Empty;
    public decimal Salario { get; set; } 
    public DateTime FechaIngreso { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string NivelEducativo { get; set; } = string.Empty;
    public string PerfilProfesional { get; set; } = string.Empty;
    
    public int DepartamentoId { get; set; }
    public Departamento Departamento { get; set; } 
}   