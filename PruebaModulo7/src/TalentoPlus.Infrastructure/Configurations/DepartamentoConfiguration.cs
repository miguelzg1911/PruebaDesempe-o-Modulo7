using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TalentoPlus.Domain.Entities;

namespace TalentoPlus.Infrastructure.Configurations;

public class DepartamentoConfiguration : IEntityTypeConfiguration<Departamento>
{
    public void Configure(EntityTypeBuilder<Departamento> builder)
    {
        builder.ToTable("Departamentos");
        
        builder.HasKey(d => d.Id);
        
        builder.Property(d => d.Nombre)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(d => d.Codigo)
            .IsRequired()
            .HasMaxLength(10);
            
        builder.HasIndex(d => d.Nombre)
            .IsUnique();
            
        builder.HasIndex(d => d.Codigo)
            .IsUnique();
    }
}