namespace VMS.Domain.Entities;

public class User : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public Guid RoleId { get; set; }
    public Guid StatusId { get; set; }
    public Guid? OrganisationId { get; set; }

    public Role? Role { get; set; }
    public MdmUserStatus? Status { get; set; }
    public Organisation? Organisation { get; set; }
    
    // NOTE: CreatedBy field exists in BaseEntity but is NOT tracked via FK for User table
    // This avoids bootstrap problem when creating the first user
    // UpdatedBy and DeletedBy ARE tracked via FK for audit trail on user modifications
}

