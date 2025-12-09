using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TalentoPlus.Domain.Entities;
using TalentoPlus.Domain.Enums;

namespace TalentoPlus.Infrastructure.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        
        builder.HasKey(u => u.Id);
        
        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(500);
            
        builder.Property(u => u.UserName)
            .HasMaxLength(100);
            
        builder.Property(u => u.Role)
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(UserRole.Empleado);
        
        builder.HasOne(u => u.Empleado)
            .WithOne() 
            .HasForeignKey<User>(u => u.EmpleadoId)
            .OnDelete(DeleteBehavior.Restrict);
            
        // Indexes
        builder.HasIndex(u => u.Email)
            .IsUnique();
    }
}