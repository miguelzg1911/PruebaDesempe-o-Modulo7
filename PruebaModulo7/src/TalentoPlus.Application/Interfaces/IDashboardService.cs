namespace TalentoPlus.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardStats> GetDashboardStatsAsync();
    Task<string> ProcesarConsultaNaturalAsync(string query);
}

public class DashboardStats
{
    public int TotalEmpleados { get; set; }
    public int EmpleadosActivos { get; set; }
    public Dictionary<string, int> EmpleadosPorDepartamento { get; set; } = new();
}
