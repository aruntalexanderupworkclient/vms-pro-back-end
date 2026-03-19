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
        builder.Property(t => t.TokenType).HasConversion<string>().HasMaxLength(20);
        builder.Property(t => t.Status).HasConversion<string>().HasMaxLength(20);
        builder.HasOne(t => t.Visitor).WithMany(v => v.Tokens).HasForeignKey(t => t.VisitorId);
        builder.HasQueryFilter(t => !t.IsDeleted);
    }
}
