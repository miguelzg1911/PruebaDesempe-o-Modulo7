using TalentoPlus.Domain.Entities;

namespace TalentoPlus.Domain.Interfaces;

public interface IDepartamentoRepository
{
    Task<IEnumerable<Departamento>> GetAllAsync();
    Task<Departamento?> GetByIdAsync(int id);
    Task AddAsync(Departamento departamento);
    Task UpdateAsync(Departamento departamento);
    Task<bool> DeleteAsync(int id);
    Task SaveChangesAsync();
}