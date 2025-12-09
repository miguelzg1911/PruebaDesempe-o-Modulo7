using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentoPlus.Application.Interfaces;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;
    
    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }
    
    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        try
        {
            var stats = await _dashboardService.GetDashboardStatsAsync();
            return Ok(stats);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al obtener estadísticas", error = ex.Message });
        }
    }
    
    [HttpPost("ask")]
    public async Task<IActionResult> AskQuestion([FromBody] AskQuestionRequest request)
    {
        try
        {
            var answer = await _dashboardService.ProcesarConsultaNaturalAsync(request.Question);
            return Ok(new { question = request.Question, answer });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al procesar la pregunta", error = ex.Message });
        }
    }
}

public class AskQuestionRequest
{
    public string Question { get; set; } = string.Empty;
}