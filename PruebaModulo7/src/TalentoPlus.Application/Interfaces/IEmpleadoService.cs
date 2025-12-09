using TalentoPlus.Application.DTOs.Empleado;

namespace TalentoPlus.Application.Interfaces;

public interface IEmpleadoService
{
    Task<List<EmpleadoDto>> GetAllEmpleadosAsync();
    Task<EmpleadoDto> GetEmpleadoByIdAsync(int id);
    Task<int> CreateEmpleadoAsync(EmpleadoCreateDto dto);
    Task UpdateEmpleadoAsync(int id, UpdateEmpleadoDto dto);
    Task DeleteEmpleadoAsync(int id);
    
    Task<byte[]> GenerarPdfAsync(int empleadoId);
    Task<ImportResult> ImportarDesdeExcelAsync(Stream excelStream);
}

public class ImportResult
{
    public int Added { get; set; }
    public int Updated { get; set; }
    public int Errors { get; set; }
    public List<string> ErrorMessages { get; set; } = new();
}