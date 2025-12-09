using TalentoPlus.Application.DTOs.Departamento;

namespace TalentoPlus.Application.Interfaces;

public interface IDepartamentoService
{
    Task<List<DepartamentoDto>> GetAllDepartamentosAsync();
    Task<DepartamentoDto?> GetByIdAsync(int id);
}