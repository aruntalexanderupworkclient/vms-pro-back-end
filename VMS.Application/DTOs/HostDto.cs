namespace VMS.Application.DTOs;

public class HostDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public string Contact { get; set; } = string.Empty;
    public string OrgType { get; set; } = string.Empty;
    public Guid OrgTypeId { get; set; }
    public string OrgTypeLabel { get; set; } = string.Empty;
    public bool Status { get; set; }
}

public class CreateHostDto
{
    public string Name { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public string Contact { get; set; } = string.Empty;
    public string OrgType { get; set; } = string.Empty;
    
    public bool Status { get; set; } = true;
}

public class UpdateHostDto
{
    public string Name { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public string Contact { get; set; } = string.Empty;
    public string OrgType { get; set; } = string.Empty;
    public bool Status { get; set; }
}
