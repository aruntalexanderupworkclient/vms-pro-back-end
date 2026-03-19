using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VMS.Domain.Entities;

namespace VMS.Infrastructure.Data.Configurations;

public class HostConfiguration : IEntityTypeConfiguration<Host>
{
    public void Configure(EntityTypeBuilder<Host> builder)
    {
        builder.ToTable("Hosts");
        builder.HasKey(h => h.Id);
        builder.Property(h => h.Name).IsRequired().HasMaxLength(200);
        builder.Property(h => h.Department).HasMaxLength(100);
        builder.Property(h => h.ContactNumber).HasMaxLength(20);
        builder.Property(h => h.Email).HasMaxLength(256);
        builder.Property(h => h.OrganisationType).HasConversion<string>().HasMaxLength(20);
        builder.Property(h => h.Status).HasConversion<string>().HasMaxLength(20);
        builder.HasQueryFilter(h => !h.IsDeleted);
    }
}
