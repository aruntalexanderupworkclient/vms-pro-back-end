namespace VMS.Domain.Entities;

public class Role : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public ICollection<Permission> Permissions { get; set; } = new List<Permission>();
    public ICollection<User> Users { get; set; } = new List<User>();
    
    // NOTE: CreatedBy field exists in BaseEntity but is NOT tracked via FK for Role table
    // This avoids bootstrap problem when creating the first role
    // UpdatedBy and DeletedBy ARE tracked via FK for audit trail on role modifications
}


