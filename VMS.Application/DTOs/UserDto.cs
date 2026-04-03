namespace VMS.Application.DTOs;

public class UserDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public Guid RoleId { get; set; }
    public string? RoleName { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid StatusId { get; set; }
    public string StatusLabel { get; set; } = string.Empty;
    public Guid? OrganisationId { get; set; }
    public string? OrganisationName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateUserDto
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public Guid RoleId { get; set; }
    public Guid StatusId { get; set; } 
    public Guid? OrganisationId { get; set; }
}

public class UpdateUserDto
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public Guid RoleId { get; set; }
    public Guid StatusId { get; set; }
    public Guid? OrganisationId { get; set; }
}
