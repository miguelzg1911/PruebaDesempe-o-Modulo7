using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TalentoPlus.Application.DTOs.Empleado;
using TalentoPlus.Application.Interfaces;
using TalentoPlus.Infrastructure.Data;

namespace TalentoPlus.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmpleadosController : ControllerBase
{
    private readonly IEmpleadoService _empleadoService;
    private readonly AppDbContext _context;
    
    public EmpleadosController(IEmpleadoService empleadoService, AppDbContext context)
    {
        _empleadoService = empleadoService;
        _context = context;
    }
    
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var empleados = await _empleadoService.GetAllEmpleadosAsync();
            return Ok(empleados);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al obtener empleados", error = ex.Message });
        }
    }
    
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")] 
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var empleado = await _empleadoService.GetEmpleadoByIdAsync(id);
            return Ok(empleado);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al obtener empleado", error = ex.Message });
        }
    }
    
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] EmpleadoCreateDto dto)
    {
        try
        {
            var id = await _empleadoService.CreateEmpleadoAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id }, new { id, message = "Empleado creado exitosamente" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Error al crear empleado", error = ex.Message });
        }
    }
    
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateEmpleadoDto dto)
    {
        try
        {
            await _empleadoService.UpdateEmpleadoAsync(id, dto);
            return Ok(new { message = "Empleado actualizado exitosamente" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Error al actualizar empleado", error = ex.Message });
        }
    }
    
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _empleadoService.DeleteEmpleadoAsync(id);
            return Ok(new { message = "Empleado eliminado exitosamente" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Error al eliminar empleado", error = ex.Message });
        }
    }
    
    [HttpPost("import-excel")]
    [Authorize(Roles = "Admin")] 
    public async Task<IActionResult> ImportExcel(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No se ha subido ningún archivo" });
            
            if (!file.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
                return BadRequest(new { message = "Solo se permiten archivos Excel (.xlsx)" });
            
            using var stream = file.OpenReadStream();
            var result = await _empleadoService.ImportarDesdeExcelAsync(stream);
            
            return Ok(new
            {
                message = "Importación completada",
                added = result.Added,
                updated = result.Updated,
                errors = result.Errors,
                errorMessages = result.ErrorMessages
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al importar Excel", error = ex.Message });
        }
    }
    
    [HttpGet("{id}/pdf")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DownloadPdf(int id)
    {
        try
        {
            var pdfBytes = await _empleadoService.GenerarPdfAsync(id);
            return File(pdfBytes, "application/pdf", $"hoja-vida-{id}.pdf");
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al generar PDF", error = ex.Message });
        }
    }
    
    // ENDPOINTS PARA EMPLEADOS (solo su info) 
    
    [HttpGet("me")]
    public async Task<IActionResult> GetMyInfo() 
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Usuario no autenticado" });
            
            // Buscar el usuario para obtener su EmpleadoId
            var user = await _context.Users.FindAsync(int.Parse(userId));
            
            if (user == null || user.EmpleadoId == null)
                return NotFound(new { message = "No se encontró información de empleado" });
            
            // Obtener SU información
            var empleado = await _empleadoService.GetEmpleadoByIdAsync(user.EmpleadoId.Value);
            
            return Ok(empleado);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al obtener información", error = ex.Message });
        }
    }
    
    [HttpGet("me/pdf")]
    public async Task<IActionResult> DownloadMyPdf()
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Usuario no autenticado" });
            
            var user = await _context.Users.FindAsync(int.Parse(userId));
            
            if (user == null || user.EmpleadoId == null)
                return NotFound(new { message = "No se encontró información de empleado" });
            
            // Generar SU PDF
            var pdfBytes = await _empleadoService.GenerarPdfAsync(user.EmpleadoId.Value);
            
            return File(pdfBytes, "application/pdf", "mi-hoja-vida.pdf");
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al generar PDF", error = ex.Message });
        }
    }
}