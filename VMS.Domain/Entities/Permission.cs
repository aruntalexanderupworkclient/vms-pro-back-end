namespace VMS.Domain.Entities;

public class Permission : BaseEntity
{
    public string Module { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public Guid RoleId { get; set; }

    public Role? Role { get; set; }
}
