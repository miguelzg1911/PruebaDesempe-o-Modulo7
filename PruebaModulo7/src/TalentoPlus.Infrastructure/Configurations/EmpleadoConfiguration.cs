using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TalentoPlus.Domain.Entities;

namespace TalentoPlus.Infrastructure.Configurations;

public class EmpleadoConfiguration : IEntityTypeConfiguration<Empleado>
{
    public void Configure(EntityTypeBuilder<Empleado> builder)
    {
        builder.ToTable("Empleados");
        
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Documento)
            .IsRequired()
            .HasMaxLength(20);
            
        builder.Property(e => e.Nombres)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(e => e.Apellidos)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(e => e.Email)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(e => e.Cargo)
            .HasMaxLength(100);
            
        builder.Property(e => e.Salario)
            .HasPrecision(18, 2);
            
        builder.Property(e => e.Estado)
            .HasMaxLength(50);
            
        builder.Property(e => e.NivelEducativo)
            .HasMaxLength(50);
            
        builder.Property(e => e.PerfilProfesional)
            .HasMaxLength(500);
            
        // Indexes
        builder.HasIndex(e => e.Documento)
            .IsUnique();
            
        builder.HasIndex(e => e.Email)
            .IsUnique();
        
        builder.HasOne(e => e.Departamento)
            .WithMany(d => d.Empleados)
            .HasForeignKey(e => e.DepartamentoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}