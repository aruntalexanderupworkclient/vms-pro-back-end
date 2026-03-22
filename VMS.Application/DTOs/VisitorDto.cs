namespace VMS.Application.DTOs;

public class VisitorDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string IdType { get; set; } = string.Empty;
    public string IdNumber { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Purpose { get; set; } = string.Empty;
    public Guid HostId { get; set; }
    public string HostName { get; set; } = string.Empty;
    public string? Photo { get; set; }
    public string? CheckIn { get; set; }
    public string? CheckOut { get; set; }
    public string Status { get; set; } = string.Empty;
    // Enum metadata for frontend MDM display
    public int StatusId { get; set; }
    public string StatusLabel { get; set; } = string.Empty;
    public Guid? TokenId { get; set; }
    public string ExpectedDuration { get; set; } = string.Empty;
    public string OrgType { get; set; } = string.Empty;
    // Enum metadata for frontend MDM display
    public int OrgTypeId { get; set; }
    public string OrgTypeLabel { get; set; } = string.Empty;
}

public class CreateVisitorDto
{
    public string Name { get; set; } = string.Empty;
    public string IdType { get; set; } = string.Empty;
    public string IdNumber { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Photo { get; set; }
    public Guid HostId { get; set; }
    public string Purpose { get; set; } = string.Empty;
    public string ExpectedDuration { get; set; } = string.Empty;
    public string OrgType { get; set; } = string.Empty;
}

public class UpdateVisitorDto
{
    public string Name { get; set; } = string.Empty;
    public string IdType { get; set; } = string.Empty;
    public string IdNumber { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Photo { get; set; }
    public Guid HostId { get; set; }
    public string Purpose { get; set; } = string.Empty;
    public string? CheckIn { get; set; }
    public string? CheckOut { get; set; }
    public string Status { get; set; } = string.Empty;
    public string ExpectedDuration { get; set; } = string.Empty;
    public string OrgType { get; set; } = string.Empty;
}
