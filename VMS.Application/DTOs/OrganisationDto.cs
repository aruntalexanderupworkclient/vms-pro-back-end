namespace VMS.Application.DTOs;

public class OrganisationDto
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public Guid TypeId { get; set; }
    public string TypeLabel { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}

public class CreateOrganisationDto
{
    public string Type { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}

public class UpdateOrganisationDto
{
    public string Type { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}
