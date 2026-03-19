using VMS.Domain.Enums;

namespace VMS.Domain.Entities;

public class User : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public Guid RoleId { get; set; }
    public UserStatus Status { get; set; } = UserStatus.Active;
    public Guid? OrganisationId { get; set; }

    public Role? Role { get; set; }
    public Organisation? Organisation { get; set; }
}
