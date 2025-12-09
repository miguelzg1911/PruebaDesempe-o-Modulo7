using TalentoPlus.Domain.Entities;

namespace TalentoPlus.Domain.Interfaces;

public interface IEmpleadoRepository
{
    Task<IEnumerable<Empleado>> GetAllAsync();
    Task<Empleado?> GetByIdAsync(int id);
    Task<Empleado?> GetByDocumentoAsync(string documento);
    Task<Empleado?> GetByEmailAsync(string email);
    Task AddAsync(Empleado empleado);
    Task UpdateAsync(Empleado empleado);
    Task DeleteAsync(int id);
    Task SaveChangesAsync();
}