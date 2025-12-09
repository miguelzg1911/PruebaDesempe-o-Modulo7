using Microsoft.EntityFrameworkCore;
using TalentoPlus.Application.Interfaces;
using TalentoPlus.Infrastructure.Data;

namespace TalentoPlus.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly AppDbContext _context;

    public DashboardService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardStats> GetDashboardStatsAsync()
    {
        var stats = new DashboardStats
        {
            TotalEmpleados = await _context.Empleados.CountAsync(),
            EmpleadosActivos = await _context.Empleados.CountAsync(e => e.Estado == "Activo"),
            EmpleadosPorDepartamento = await _context.Empleados
                .Include(e => e.Departamento)
                .GroupBy(e => e.Departamento.Nombre)
                .Select(g => new { Departamento = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Departamento, x => x.Count)
        };

        return stats;
    }

    public async Task<string> ProcesarConsultaNaturalAsync(string query)
    {
        query = query.ToLower();

        if (query.Contains("total empleados") || query.Contains("cuántos empleados"))
        {
            var total = await _context.Empleados.CountAsync();
            return $"Actualmente hay {total} empleados en la empresa.";
        }
    
        if (query.Contains("activos") || query.Contains("empleados activos"))
        {
            var activos = await _context.Empleados.CountAsync(e => e.Estado == "Activo");
            return $"Hay {activos} empleados activos.";
        }
    
        if (query.Contains("inactivos") || query.Contains("empleados inactivos"))
        {
            var inactivos = await _context.Empleados.CountAsync(e => e.Estado == "Inactivo");
            return $"Hay {inactivos} empleados inactivos.";
        }
    
        if (query.Contains("salario promedio") || query.Contains("salario medio"))
        {
            var promedio = await _context.Empleados.AverageAsync(e => e.Salario);
            return $"El salario promedio es de {promedio:C}.";
        }

        return "Lo siento, no pude procesar tu consulta. Intenta preguntar sobre 'total de empleados', 'empleados activos' o 'salario promedio'.";
    }
}