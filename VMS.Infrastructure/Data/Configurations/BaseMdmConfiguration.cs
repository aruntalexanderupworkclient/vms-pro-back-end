using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VMS.Domain.Entities;

namespace VMS.Infrastructure.Data.Configurations;

/// <summary>
/// Base EF configuration shared by all MDM entities.
/// Each subclass just sets the table name.
/// </summary>
public abstract class BaseMdmConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseMdmEntity
{
    private readonly string _tableName;

    protected BaseMdmConfiguration(string tableName)
    {
        _tableName = tableName;
    }

    public void Configure(EntityTypeBuilder<T> builder)
    {
        builder.ToTable(_tableName);
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Code).IsRequired().HasMaxLength(50);
        builder.HasIndex(e => e.Code).IsUnique();
        builder.Property(e => e.Value).IsRequired().HasMaxLength(200);
        builder.Property(e => e.SortOrder).HasDefaultValue(0);
        builder.Property(e => e.IsActive).HasDefaultValue(true);

        // Audit FKs
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

// ─── Concrete MDM Configurations (one-liners) ───

public class MdmVisitStatusConfiguration : BaseMdmConfiguration<MdmVisitStatus>
{
    public MdmVisitStatusConfiguration() : base("MdmVisitStatuses") { }
}

public class MdmUserStatusConfiguration : BaseMdmConfiguration<MdmUserStatus>
{
    public MdmUserStatusConfiguration() : base("MdmUserStatuses") { }
}

public class MdmTokenTypeConfiguration : BaseMdmConfiguration<MdmTokenType>
{
    public MdmTokenTypeConfiguration() : base("MdmTokenTypes") { }
}

public class MdmOrganisationTypeConfiguration : BaseMdmConfiguration<MdmOrganisationType>
{
    public MdmOrganisationTypeConfiguration() : base("MdmOrganisationTypes") { }
}

