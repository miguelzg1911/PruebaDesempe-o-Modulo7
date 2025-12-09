using Microsoft.EntityFrameworkCore;
using TalentoPlus.Domain.Entities;
using TalentoPlus.Domain.Interfaces;
using TalentoPlus.Infrastructure.Data;

namespace TalentoPlus.Infrastructure.Repositories;

public class DepartamentoRepository : IDepartamentoRepository
{
    private readonly AppDbContext _context;

    public DepartamentoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Departamento>> GetAllAsync()
    {
        return await _context.Departamentos.ToListAsync();
    }

    public async Task<Departamento?> GetByIdAsync(int id)
    {
        return await _context.Departamentos.FindAsync(id);
    }

    public async Task AddAsync(Departamento departamento)
    {
        await _context.Departamentos.AddAsync(departamento);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Departamento departamento)
    {
        _context.Departamentos.Update(departamento);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var departamento = await GetByIdAsync(id);
        if (departamento == null) return false;
        
        _context.Departamentos.Remove(departamento);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}