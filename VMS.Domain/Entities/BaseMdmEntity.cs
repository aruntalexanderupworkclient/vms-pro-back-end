namespace VMS.Domain.Entities;

/// <summary>
/// Base class for all MDM (Master Data Management) entities.
/// Provides Code, Value, SortOrder, and IsActive on top of BaseEntity audit fields.
/// </summary>
public abstract class BaseMdmEntity : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public int SortOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;
}

