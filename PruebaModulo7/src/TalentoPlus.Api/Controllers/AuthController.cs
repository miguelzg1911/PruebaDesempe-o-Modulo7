using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TalentoPlus.Application.DTOs.Auth;
using TalentoPlus.Application.Interfaces;

namespace TalentoPlus.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IEmailService _emailService;
    
    public AuthController(IAuthService authService, IEmailService emailService)
    {
        _authService = authService;
        _emailService = emailService;
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var result = await _authService.LoginAsync(request.Email, request.Password);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { 
                message = "Error interno del servidor", 
                error = ex.Message,
                innerError = ex.InnerException?.Message
            });
        }
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var result = await _authService.RegisterEmpleadoAsync(request);
            
            if (!result.Success)
                return BadRequest(new { 
                    message = result.Message,
                    details = "Verifique los datos enviados"
                });
            
            // Enviar email de bienvenida
            try
            {
                await _emailService.SendEmailAsync(
                    request.Email,
                    "Bienvenido a TalentoPlus",
                    $"<h1>¡Bienvenido {request.Nombres}!</h1>" +
                    "<p>Tu registro en TalentoPlus ha sido exitoso.</p>" +
                    "<p>Ya puedes iniciar sesión en nuestra plataforma con tus credenciales.</p>"
                );
            }
            catch (Exception emailEx)
            {
                Console.WriteLine($"Error enviando email: {emailEx.Message}");
            }
            
            return Ok(new { 
                message = result.Message, 
                userId = result.UserId,
                emailSent = true
            });
        }
        catch (Exception ex)
        {
            // CAPTURAR ERROR COMPLETO
            var errorMessage = ex.Message;
            var innerMessage = ex.InnerException?.Message ?? "No inner exception";
            var innerInnerMessage = ex.InnerException?.InnerException?.Message ?? "No inner inner exception";
            
            Console.WriteLine("=== ERROR DE REGISTRO ===");
            Console.WriteLine($"Error: {errorMessage}");
            Console.WriteLine($"Inner: {innerMessage}");
            Console.WriteLine($"Inner2: {innerInnerMessage}");
            Console.WriteLine($"StackTrace: {ex.StackTrace}");
            
            return BadRequest(new { 
                message = "Error en registro",
                error = errorMessage,
                innerError = innerMessage,
                details = innerInnerMessage
            });
        }
    }
    
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            var result = await _authService.RefreshTokenAsync(request.Token, request.RefreshToken);
            return Ok(result);
        }
        catch (SecurityTokenException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { 
                message = "Error interno", 
                error = ex.Message,
                innerError = ex.InnerException?.Message
            });
        }
    }
    
    // ENDPOINT DE DIAGNÓSTICO - TEMPORAL
    [HttpPost("test-db")]
    [AllowAnonymous]
    public async Task<IActionResult> TestDatabase()
    {
        try
        {
            // Este método necesita acceso a AppDbContext
            // Si no lo tienes inyectado, inyéctalo en el constructor:
            // private readonly AppDbContext _context;
            
            // Por ahora, prueba simple
            return Ok(new {
                status = "API funcionando",
                timestamp = DateTime.UtcNow,
                message = "El endpoint /api/Auth está activo"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new {
                status = "Error",
                error = ex.Message,
                innerError = ex.InnerException?.Message
            });
        }
    }
}

public class RefreshTokenRequest
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}