using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VMS.Domain.Entities;

namespace VMS.Infrastructure.Data.Configurations;

public class VisitorTokenConfiguration : IEntityTypeConfiguration<VisitorToken>
{
    public void Configure(EntityTypeBuilder<VisitorToken> builder)
    {
        builder.ToTable("VisitorTokens");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.TokenNumber).IsRequired().HasMaxLength(50);
        builder.HasIndex(t => t.TokenNumber).IsUnique();
        builder.HasOne(t => t.Visitor).WithMany(v => v.Tokens).HasForeignKey(t => t.VisitorId);
        builder.HasOne(t => t.TokenType).WithMany().HasForeignKey(t => t.TokenTypeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(t => t.Status).WithMany().HasForeignKey(t => t.StatusId).OnDelete(DeleteBehavior.Restrict);
        
        // 🆕 AUDIT TRACKING FOREIGN KEYS
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(t => t.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);
        
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(t => t.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);
        
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(t => t.DeletedBy)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);
        
        builder.HasQueryFilter(t => !t.IsDeleted);
    }
}
