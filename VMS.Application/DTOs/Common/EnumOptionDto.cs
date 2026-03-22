namespace VMS.Application.DTOs.Common;

public class EnumOptionDto
{
    public int Id { get; set; }
    public string Value { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
}

public class EnumGroupDto
{
    public string Name { get; set; } = string.Empty;
    public List<EnumOptionDto> Options { get; set; } = new();
}

