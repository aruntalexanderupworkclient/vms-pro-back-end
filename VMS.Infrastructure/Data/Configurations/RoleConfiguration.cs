using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VMS.Domain.Entities;

namespace VMS.Infrastructure.Data.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Name).IsRequired().HasMaxLength(100);
        builder.HasIndex(r => r.Name).IsUnique();
        builder.Property(r => r.Description).HasMaxLength(500);
        
        // 🆕 AUDIT TRACKING FOREIGN KEYS
        // CreatedBy: Nullable FK - allows bootstrap without error, but enforces FK when populated
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(r => r.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);  // Nullable FK
        
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(r => r.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);
        
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(r => r.DeletedBy)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);
        
        builder.HasQueryFilter(r => !r.IsDeleted);
    }
}
