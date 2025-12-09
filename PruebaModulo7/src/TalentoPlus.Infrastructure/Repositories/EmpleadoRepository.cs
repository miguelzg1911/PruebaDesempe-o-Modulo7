using Microsoft.EntityFrameworkCore;
using TalentoPlus.Domain.Entities;
using TalentoPlus.Domain.Interfaces;
using TalentoPlus.Infrastructure.Data;

namespace TalentoPlus.Infrastructure.Repositories;

public class EmpleadoRepository : IEmpleadoRepository
{
    private readonly AppDbContext _context;

    public EmpleadoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Empleado>> GetAllAsync()
    {
        return await _context.Empleados
            .Include(e => e.Departamento)
            .ToListAsync();
    }

    public async Task<Empleado?> GetByIdAsync(int id)
    {
        return await _context.Empleados
            .Include(e => e.Departamento)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Empleado?> GetByDocumentoAsync(string documento)
    {
        return await _context.Empleados
            .FirstOrDefaultAsync(e => e.Documento == documento);
    }

    public async Task<Empleado?> GetByEmailAsync(string email)
    {
        return await _context.Empleados
            .FirstOrDefaultAsync(e => e.Email == email);
    }

    public async Task AddAsync(Empleado empleado)
    {
        await _context.Empleados.AddAsync(empleado);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Empleado empleado)
    {
        _context.Empleados.Update(empleado);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var empleado = await GetByIdAsync(id);
        if (empleado != null)
        {
            _context.Empleados.Remove(empleado);
            await _context.SaveChangesAsync();
        }
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}