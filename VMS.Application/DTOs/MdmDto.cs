namespace VMS.Application.DTOs;

public class MdmDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
}

public class CreateMdmDto
{
    public string Code { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public int SortOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;
}

public class UpdateMdmDto
{
    public string Code { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
}

