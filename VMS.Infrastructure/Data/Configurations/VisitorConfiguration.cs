using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VMS.Domain.Entities;

namespace VMS.Infrastructure.Data.Configurations;

public class VisitorConfiguration : IEntityTypeConfiguration<Visitor>
{
    public void Configure(EntityTypeBuilder<Visitor> builder)
    {
        builder.ToTable("Visitors");
        builder.HasKey(v => v.Id);
        builder.Property(v => v.FullName).IsRequired().HasMaxLength(200);
        builder.Property(v => v.IdType).IsRequired().HasMaxLength(50);
        builder.Property(v => v.IdNumber).IsRequired().HasMaxLength(100);
        builder.Property(v => v.Phone).HasMaxLength(20);
        builder.Property(v => v.PhotoUrl).HasMaxLength(500);
        builder.Property(v => v.Purpose).HasMaxLength(500);
        builder.HasOne(v => v.Host).WithMany(h => h.Visitors).HasForeignKey(v => v.HostId);
        builder.HasOne(v => v.Status).WithMany().HasForeignKey(v => v.StatusId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(v => v.OrgType).WithMany().HasForeignKey(v => v.OrgTypeId).OnDelete(DeleteBehavior.Restrict).IsRequired(false);
        
        // 🆕 AUDIT TRACKING FOREIGN KEYS
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(v => v.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);
        
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(v => v.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);
        
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(v => v.DeletedBy)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);
        
        builder.HasQueryFilter(v => !v.IsDeleted);
    }
}
