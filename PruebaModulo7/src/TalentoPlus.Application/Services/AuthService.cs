using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TalentoPlus.Application.DTOs.Auth;
using TalentoPlus.Application.Interfaces;
using TalentoPlus.Domain.Entities;
using TalentoPlus.Infrastructure.Data;

namespace TalentoPlus.Application.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<LoginResult> LoginAsync(string email, string password)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            throw new UnauthorizedAccessException("Credenciales inválidas");

        var token = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        await _context.SaveChangesAsync();

        return new LoginResult
        {
            Token = token,
            RefreshToken = refreshToken,
            Email = user.Email,
            Role = user.Role?.ToString() ?? "User",
            ExpiresIn = 3600
        };
    }

    public async Task<LoginResult> RefreshTokenAsync(string token, string refreshToken)
    {
        var principal = GetPrincipalFromExpiredToken(token);
        var userId = int.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        
        var user = await _context.Users.FindAsync(userId);
        
        if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiry <= DateTime.UtcNow)
            throw new SecurityTokenException("Token de refresco inválido");
        
        var newToken = GenerateJwtToken(user);
        var newRefreshToken = GenerateRefreshToken();
        
        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        await _context.SaveChangesAsync();
        
        return new LoginResult
        {
            Token = newToken,
            RefreshToken = newRefreshToken,
            Email = user.Email,
            Role = user.Role?.ToString() ?? "User",
            ExpiresIn = 3600
        };
    }

    private string GenerateJwtToken(User user)
    {
        var jwtKey = _configuration["Jwt:Key"] ?? "DefaultKeyMinimum32CharactersLong123456";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role?.ToString() ?? "User"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "TalentoPlus",
            audience: _configuration["Jwt:Audience"] ?? "TalentoPlusUsers",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var jwtKey = _configuration["Jwt:Key"] ?? "DefaultKeyMinimum32CharactersLong123456";
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
        
        if (securityToken is not JwtSecurityToken jwtSecurityToken || 
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Token inválido");
        
        return principal;
    }
    
    public async Task<RegisterResult> RegisterEmpleadoAsync(RegisterRequest request)
    {
        try
        {
            Console.WriteLine("=== INICIANDO REGISTRO ===");
            Console.WriteLine($"Email: {request.Email}, DeptId: {request.DepartamentoId}");
            
            // 1. Verificar si email ya existe
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                Console.WriteLine("✗ Email ya existe");
                return new RegisterResult
                {
                    Success = false,
                    Message = "El email ya está registrado"
                };
            }
            
            // 2. Verificar si departamento existe
            var departamento = await _context.Departamentos.FindAsync(request.DepartamentoId);
            if (departamento == null)
            {
                Console.WriteLine($"✗ Departamento {request.DepartamentoId} no existe");
                return new RegisterResult
                {
                    Success = false,
                    Message = $"El departamento con ID {request.DepartamentoId} no existe"
                };
            }
            Console.WriteLine($"✓ Departamento encontrado: {departamento.Nombre}");
            
            // 3. Crear empleado (VERSIÓN SIMPLIFICADA - solo campos obligatorios)
            Console.WriteLine("Creando empleado...");
            var empleado = new Empleado
            {
                Documento = request.Documento,
                Nombres = request.Nombres,
                Apellidos = request.Apellidos,
                Email = request.Email,
                Telefono = request.Telefono,
                DepartamentoId = request.DepartamentoId,
                Estado = "Activo",
                FechaIngreso = DateTime.UtcNow,
                FechaNacimiento = new DateTime(1990, 1, 1), // Fecha por defecto
                Direccion = "Por definir",
                Cargo = "Empleado",
                Salario = 0,
                NivelEducativo = "Por definir",
                PerfilProfesional = "Por definir"
            };
            
            _context.Empleados.Add(empleado);
            await _context.SaveChangesAsync();
            Console.WriteLine($"✓ Empleado creado con ID: {empleado.Id}");
            
            // 4. Crear usuario
            Console.WriteLine("Creando usuario...");
            var user = new User
            {
                UserName = request.Email,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                // Si es el primer usuario, hacerlo Admin
                Role = await _context.Users.AnyAsync() ? 
                    TalentoPlus.Domain.Enums.UserRole.Empleado : 
                    TalentoPlus.Domain.Enums.UserRole.Admin,
                EmpleadoId = empleado.Id
            };
            
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            Console.WriteLine($"✓ Usuario creado con ID: {user.Id}, Role: {user.Role}");
            
            Console.WriteLine("=== REGISTRO EXITOSO ===");
            
            return new RegisterResult
            {
                Success = true,
                Message = "Empleado registrado exitosamente",
                UserId = user.Id
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine("=== ERROR EN REGISTRO ===");
            Console.WriteLine($"Mensaje: {ex.Message}");
            Console.WriteLine($"Inner: {ex.InnerException?.Message}");
            Console.WriteLine($"Inner2: {ex.InnerException?.InnerException?.Message}");
            Console.WriteLine($"StackTrace: {ex.StackTrace}");
            
            return new RegisterResult
            {
                Success = false,
                Message = $"Error: {ex.Message} | Detalles: {ex.InnerException?.Message}"
            };
        }
    }

    public Task<bool> SendWelcomeEmailAsync(string email, string nombre)
    {
        Console.WriteLine($"[SIMULACIÓN] Email enviado a {email} para {nombre}");
        return Task.FromResult(true);
    }
}