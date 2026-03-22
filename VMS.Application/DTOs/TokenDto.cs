using VMS.Domain.Enums;

namespace VMS.Application.DTOs;

public class TokenDto
{
    public Guid Id { get; set; }
    public string TokenNo { get; set; } = string.Empty;
    public Guid VisitorId { get; set; }
    public string VisitorName { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    // Enum metadata for frontend MDM display
    public int TypeId { get; set; }
    public string TypeLabel { get; set; } = string.Empty;
    public string IssueTime { get; set; } = string.Empty;
    public DateTime Expiry { get; set; }
    public string Status { get; set; } = string.Empty;
    // Enum metadata for frontend MDM display
    public int StatusId { get; set; }
    public string StatusLabel { get; set; } = string.Empty;
    public string HostName { get; set; } = string.Empty;
}

public class CreateTokenDto
{
    public string TokenNo { get; set; } = string.Empty;
    public Guid VisitorId { get; set; }
    public string Type { get; set; } = string.Empty;
    public DateTime Expiry { get; set; }
    public DateTime IssueTime { get; set; }
}

public class UpdateTokenDto
{
    public string TokenNo { get; set; } = string.Empty;
    public Guid VisitorId { get; set; }
    public string Type { get; set; } = string.Empty;
    public DateTime Expiry { get; set; }
    public string Status { get; set; } = string.Empty;
}
