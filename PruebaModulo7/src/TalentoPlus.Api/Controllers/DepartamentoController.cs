using Microsoft.AspNetCore.Mvc;
using TalentoPlus.Application.Interfaces;

namespace TalentoPlus.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartamentosController : ControllerBase
{
    private readonly IDepartamentoService _departamentoService;
    
    public DepartamentosController(IDepartamentoService departamentoService)
    {
        _departamentoService = departamentoService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var departamentos = await _departamentoService.GetAllDepartamentosAsync();
            return Ok(departamentos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al obtener departamentos", error = ex.Message });
        }
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var departamento = await _departamentoService.GetByIdAsync(id);
            
            if (departamento == null)
                return NotFound(new { message = $"Departamento con ID {id} no encontrado" });
            
            return Ok(departamento);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al obtener departamento", error = ex.Message });
        }
    }
}