using VMS.Domain.Enums;

namespace VMS.Application.DTOs;

public class HostDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public string Contact { get; set; } = string.Empty;
    public string OrgType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public class CreateHostDto
{
    public string Name { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public string Contact { get; set; } = string.Empty;
    public string OrgType { get; set; } = string.Empty;
}

public class UpdateHostDto
{
    public string Name { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public string Contact { get; set; } = string.Empty;
    public string OrgType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}
