using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using TalentoPlus.Application.DTOs.Empleado;
using TalentoPlus.Application.Interfaces;
using TalentoPlus.Domain.Entities;
using TalentoPlus.Infrastructure.Data;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace TalentoPlus.Application.Services;

public class EmpleadoService : IEmpleadoService
{
    private readonly AppDbContext _context;

    public EmpleadoService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<EmpleadoDto>> GetAllEmpleadosAsync()
    {
        return await _context.Empleados
            .Include(e => e.Departamento)
            .Select(e => new EmpleadoDto
            {
                Id = e.Id,
                Documento = e.Documento,
                Nombres = e.Nombres,
                Apellidos = e.Apellidos,
                Email = e.Email,
                Telefono = e.Telefono,
                Cargo = e.Cargo,
                Salario = e.Salario,
                FechaIngreso = e.FechaIngreso,
                Estado = e.Estado,
                DepartamentoNombre = e.Departamento.Nombre
            })
            .ToListAsync();
    }

    public async Task<EmpleadoDto> GetEmpleadoByIdAsync(int id)
    {
        var empleado = await _context.Empleados
            .Include(e => e.Departamento)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (empleado == null)
            throw new KeyNotFoundException($"Empleado con ID {id} no encontrado");

        return new EmpleadoDto
        {
            Id = empleado.Id,
            Documento = empleado.Documento,
            Nombres = empleado.Nombres,
            Apellidos = empleado.Apellidos,
            Email = empleado.Email,
            Telefono = empleado.Telefono,
            Cargo = empleado.Cargo,
            Salario = empleado.Salario,
            FechaIngreso = empleado.FechaIngreso,
            Estado = empleado.Estado,
            DepartamentoNombre = empleado.Departamento.Nombre
        };
    }

    public async Task<int> CreateEmpleadoAsync(EmpleadoCreateDto dto)
    {
        var empleado = new Empleado
        {
            Documento = dto.Documento,
            Nombres = dto.Nombres,
            Apellidos = dto.Apellidos,
            FechaNacimiento = dto.FechaNacimiento,
            Direccion = dto.Direccion,
            Telefono = dto.Telefono,
            Email = dto.Email,
            Cargo = dto.Cargo,
            Salario = dto.Salario,
            FechaIngreso = dto.FechaIngreso,
            Estado = dto.Estado,
            NivelEducativo = dto.NivelEducativo,
            PerfilProfesional = dto.PerfilProfesional,
            DepartamentoId = dto.DepartamentoId
        };

        _context.Empleados.Add(empleado);
        await _context.SaveChangesAsync();

        return empleado.Id;
    }

    public async Task UpdateEmpleadoAsync(int id, UpdateEmpleadoDto dto)
    {
        var empleado = await _context.Empleados.FindAsync(id);
        if (empleado == null)
            throw new KeyNotFoundException($"Empleado con ID {id} no encontrado");

        // Actualizar solo las propiedades que vienen en el DTO
        if (!string.IsNullOrEmpty(dto.Nombres)) empleado.Nombres = dto.Nombres;
        if (!string.IsNullOrEmpty(dto.Apellidos)) empleado.Apellidos = dto.Apellidos;
        if (!string.IsNullOrEmpty(dto.Telefono)) empleado.Telefono = dto.Telefono;
        if (!string.IsNullOrEmpty(dto.Cargo)) empleado.Cargo = dto.Cargo;
        if (dto.Salario.HasValue) empleado.Salario = dto.Salario.Value;
        if (!string.IsNullOrEmpty(dto.Estado)) empleado.Estado = dto.Estado;
        if (dto.DepartamentoId.HasValue) empleado.DepartamentoId = dto.DepartamentoId.Value;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteEmpleadoAsync(int id)
    {
        var empleado = await _context.Empleados.FindAsync(id);
        if (empleado == null)
            throw new KeyNotFoundException($"Empleado con ID {id} no encontrado");

        _context.Empleados.Remove(empleado);
        await _context.SaveChangesAsync();
    }
    
     public async Task<byte[]> GenerarPdfAsync(int empleadoId)
    { 
        QuestPDF.Settings.License = LicenseType.Community;
        
        var empleado = await _context.Empleados
            .Include(e => e.Departamento)
            .FirstOrDefaultAsync(e => e.Id == empleadoId);
        
        if (empleado == null)
            throw new KeyNotFoundException($"Empleado con ID {empleadoId} no encontrado");

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(11));
                
                page.Header()
                    .Text("Hoja de Vida")
                    .SemiBold().FontSize(24).FontColor(Colors.Blue.Medium);
                
                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(col =>
                    {
                        col.Spacing(10);
                        
                        // Datos Personales
                        col.Item().Text("DATOS PERSONALES").Bold().FontSize(14);
                        col.Item().Text($"Nombre: {empleado.Nombres} {empleado.Apellidos}");
                        col.Item().Text($"Documento: {empleado.Documento}");
                        col.Item().Text($"Fecha Nacimiento: {empleado.FechaNacimiento:dd/MM/yyyy}");
                        col.Item().Text($"Dirección: {empleado.Direccion}");
                        col.Item().Text($"Teléfono: {empleado.Telefono}");
                        col.Item().Text($"Email: {empleado.Email}");
                        
                        // Información Laboral
                        col.Item().Text("\nINFORMACIÓN LABORAL").Bold().FontSize(14);
                        col.Item().Text($"Cargo: {empleado.Cargo}");
                        col.Item().Text($"Salario: {empleado.Salario:C}");
                        col.Item().Text($"Fecha Ingreso: {empleado.FechaIngreso:dd/MM/yyyy}");
                        col.Item().Text($"Estado: {empleado.Estado}");
                        col.Item().Text($"Departamento: {empleado.Departamento?.Nombre ?? "No asignado"}");
                        
                        // Educación
                        col.Item().Text("\nNIVEL EDUCATIVO").Bold().FontSize(14);
                        col.Item().Text($"{empleado.NivelEducativo}");
                        
                        // Perfil Profesional
                        col.Item().Text("\nPERFIL PROFESIONAL").Bold().FontSize(14);
                        col.Item().Text($"{empleado.PerfilProfesional}");
                    });
                
                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Generado el ");
                        x.Span($"{DateTime.Now:dd/MM/yyyy HH:mm}"); 
                    });
            });
        });
        return document.GeneratePdf();
    }

    public async Task<ImportResult> ImportarDesdeExcelAsync(Stream excelStream)
    {
        var result = new ImportResult();
        
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        
        using var package = new ExcelPackage(excelStream);
        var worksheet = package.Workbook.Worksheets[0];
        
        var rowCount = worksheet.Dimension?.Rows ?? 0;
        
        // Empezar desde fila 2 (fila 1 es header)
        for (int row = 2; row <= rowCount; row++)
        {
            try
            {
                var documento = worksheet.Cells[row, 1]?.Text?.Trim(); // Columna 1: Documento
                if (string.IsNullOrEmpty(documento))
                    continue;
                
                var empleado = await _context.Empleados
                    .FirstOrDefaultAsync(e => e.Documento == documento);
                
                if (empleado == null)
                {
                    // Crear nuevo empleado con TODAS las columnas
                    empleado = new Empleado
                    {
                        Documento = documento,
                        Nombres = worksheet.Cells[row, 2]?.Text?.Trim() ?? "", // Col 2: Nombres
                        Apellidos = worksheet.Cells[row, 3]?.Text?.Trim() ?? "", // Col 3: Apellidos
                        
                        // Parsear fechas
                        FechaNacimiento = ParseDate(worksheet.Cells[row, 4]?.Text), // Col 4: FechaNacimiento
                        Direccion = worksheet.Cells[row, 5]?.Text?.Trim() ?? "", // Col 5: Direccion
                        Telefono = worksheet.Cells[row, 6]?.Text?.Trim() ?? "", // Col 6: Telefono
                        Email = worksheet.Cells[row, 7]?.Text?.Trim() ?? "", // Col 7: Email
                        Cargo = worksheet.Cells[row, 8]?.Text?.Trim() ?? "", // Col 8: Cargo
                        Salario = decimal.TryParse(worksheet.Cells[row, 9]?.Text, out var salario) ? salario : 0, // Col 9: Salario
                        FechaIngreso = ParseDate(worksheet.Cells[row, 10]?.Text) ?? DateTime.UtcNow, // Col 10: FechaIngreso
                        Estado = worksheet.Cells[row, 11]?.Text?.Trim() ?? "Activo", // Col 11: Estado
                        NivelEducativo = worksheet.Cells[row, 12]?.Text?.Trim() ?? "", // Col 12: NivelEducativo
                        PerfilProfesional = worksheet.Cells[row, 13]?.Text?.Trim() ?? "", // Col 13: PerfilProfesional
                        
                        // Buscar departamento por nombre (columna 14)
                        DepartamentoId = await GetDepartamentoIdAsync(worksheet.Cells[row, 14]?.Text?.Trim())
                    };
                    
                    _context.Empleados.Add(empleado);
                    result.Added++;
                }
                else
                {
                    // Actualizar empleado existente
                    empleado.Nombres = worksheet.Cells[row, 2]?.Text?.Trim() ?? empleado.Nombres;
                    empleado.Apellidos = worksheet.Cells[row, 3]?.Text?.Trim() ?? empleado.Apellidos;
                    empleado.FechaNacimiento = ParseDate(worksheet.Cells[row, 4]?.Text) ?? empleado.FechaNacimiento;
                    empleado.Direccion = worksheet.Cells[row, 5]?.Text?.Trim() ?? empleado.Direccion;
                    empleado.Telefono = worksheet.Cells[row, 6]?.Text?.Trim() ?? empleado.Telefono;
                    empleado.Email = worksheet.Cells[row, 7]?.Text?.Trim() ?? empleado.Email;
                    empleado.Cargo = worksheet.Cells[row, 8]?.Text?.Trim() ?? empleado.Cargo;
                    
                    if (decimal.TryParse(worksheet.Cells[row, 9]?.Text, out var salario))
                        empleado.Salario = salario;
                        
                    empleado.FechaIngreso = ParseDate(worksheet.Cells[row, 10]?.Text) ?? empleado.FechaIngreso;
                    empleado.Estado = worksheet.Cells[row, 11]?.Text?.Trim() ?? empleado.Estado;
                    empleado.NivelEducativo = worksheet.Cells[row, 12]?.Text?.Trim() ?? empleado.NivelEducativo;
                    empleado.PerfilProfesional = worksheet.Cells[row, 13]?.Text?.Trim() ?? empleado.PerfilProfesional;
                    
                    var deptoId = await GetDepartamentoIdAsync(worksheet.Cells[row, 14]?.Text?.Trim());
                    if (deptoId > 0)
                        empleado.DepartamentoId = deptoId;
                    
                    result.Updated++;
                }
            }
            catch (Exception ex)
            {
                result.Errors++;
                result.ErrorMessages.Add($"Fila {row}: {ex.Message}");
            }
        }
        
        await _context.SaveChangesAsync();
        return result;
    }

// Método auxiliar para parsear fechas
    private DateTime? ParseDate(string dateStr)
    {
        if (string.IsNullOrEmpty(dateStr))
            return null;
        
        if (DateTime.TryParse(dateStr, out var date))
            return date;
        
        return null;
    }

// Método auxiliar para obtener ID de departamento por nombre
    private async Task<int> GetDepartamentoIdAsync(string departamentoNombre)
    {
        if (string.IsNullOrEmpty(departamentoNombre))
            return 1; // Default a Tecnología
        
        var departamento = await _context.Departamentos
            .FirstOrDefaultAsync(d => d.Nombre.Contains(departamentoNombre) || 
                                     d.Codigo.Contains(departamentoNombre));
        
        return departamento?.Id ?? 1; // Default a Tecnología si no se encuentra
    }
}