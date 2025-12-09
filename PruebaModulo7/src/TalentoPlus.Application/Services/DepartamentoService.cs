using Microsoft.EntityFrameworkCore;
using TalentoPlus.Application.DTOs.Departamento;
using TalentoPlus.Application.Interfaces;
using TalentoPlus.Infrastructure.Data;

namespace TalentoPlus.Application.Services;

public class DepartamentoService : IDepartamentoService
{
    private readonly AppDbContext _context;

    public DepartamentoService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<DepartamentoDto>> GetAllDepartamentosAsync()
    {
        return await _context.Departamentos
            .Select(d => new DepartamentoDto
            {
                Id = d.Id,
                Nombre = d.Nombre,
                Codigo = d.Codigo,
                TotalEmpleados = d.Empleados.Count
            })
            .ToListAsync();
    }

    public async Task<DepartamentoDto?> GetByIdAsync(int id)
    {
        var departamento = await _context.Departamentos
            .Include(d => d.Empleados)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (departamento == null)
            return null;

        return new DepartamentoDto
        {
            Id = departamento.Id,
            Nombre = departamento.Nombre,
            Codigo = departamento.Codigo,
            TotalEmpleados = departamento.Empleados.Count
        };
    }
}