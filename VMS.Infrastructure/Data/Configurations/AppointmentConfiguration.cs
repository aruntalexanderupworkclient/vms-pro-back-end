using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VMS.Domain.Entities;

namespace VMS.Infrastructure.Data.Configurations;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("Appointments");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.VisitorName).IsRequired().HasMaxLength(200);
        builder.Property(a => a.Purpose).HasMaxLength(500);
        builder.Property(a => a.Notes).HasMaxLength(1000);
        builder.Property(a => a.Status).HasConversion<string>().HasMaxLength(20);
        builder.HasOne(a => a.Host).WithMany(h => h.Appointments).HasForeignKey(a => a.HostId);
        builder.HasQueryFilter(a => !a.IsDeleted);
    }
}
