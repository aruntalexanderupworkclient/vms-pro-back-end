namespace VMS.Application.DTOs;

public class PermissionDto
{
    public Guid Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Module { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
}

public class CreatePermissionDto
{
    public string Module { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public Guid RoleId { get; set; }
}

public class UpdatePermissionDto
{
    public string Module { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public Guid RoleId { get; set; }
}
