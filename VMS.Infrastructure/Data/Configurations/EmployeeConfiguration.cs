using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VMS.Domain.Entities;

namespace VMS.Infrastructure.Data.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.FullName).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Department).HasMaxLength(100);
        builder.Property(e => e.Designation).HasMaxLength(100);
        builder.Property(e => e.EmployeeCode).IsRequired().HasMaxLength(20);
        builder.HasIndex(e => e.EmployeeCode).IsUnique();
        builder.Property(e => e.Phone).HasMaxLength(20);
        builder.Property(e => e.Email).HasMaxLength(256);
        builder.Property(e => e.IsActive).HasDefaultValue(true);
        
        // 🆕 AUDIT TRACKING FOREIGN KEYS
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(e => e.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);
        
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(e => e.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);
        
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(e => e.DeletedBy)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);
        
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
