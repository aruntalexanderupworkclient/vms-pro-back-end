using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VMS.Domain.Entities;

namespace VMS.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.FullName).IsRequired().HasMaxLength(200);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(256);
        builder.HasIndex(u => u.Email).IsUnique();
        builder.Property(u => u.Phone).HasMaxLength(20);
        builder.Property(u => u.PasswordHash).IsRequired();
        builder.HasOne(u => u.Role).WithMany(r => r.Users).HasForeignKey(u => u.RoleId);
        builder.HasOne(u => u.Status).WithMany().HasForeignKey(u => u.StatusId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(u => u.Organisation).WithMany(o => o.Users).HasForeignKey(u => u.OrganisationId);
        
        // 🆕 AUDIT TRACKING FOREIGN KEYS
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(u => u.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);
        
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(u => u.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);
        
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(u => u.DeletedBy)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);
        
        builder.HasQueryFilter(u => !u.IsDeleted);
    }
}
