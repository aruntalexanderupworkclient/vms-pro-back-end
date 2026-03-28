using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VMS.Domain.Entities;

namespace VMS.Infrastructure.Data.Configurations;

public class OrganisationConfiguration : IEntityTypeConfiguration<Organisation>
{
    public void Configure(EntityTypeBuilder<Organisation> builder)
    {
        builder.ToTable("Organisations");
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Name).IsRequired().HasMaxLength(300);
        builder.Property(o => o.Type).HasConversion<string>().HasMaxLength(20);
        builder.Property(o => o.LogoUrl).HasMaxLength(500);
        
        // 🆕 AUDIT TRACKING FOREIGN KEYS
        // CreatedBy: Nullable FK - allows bootstrap without error, but enforces FK when populated
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(o => o.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);  // Nullable FK
        
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(o => o.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);
        
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(o => o.DeletedBy)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);
        
        builder.HasQueryFilter(o => !o.IsDeleted);
    }
}
