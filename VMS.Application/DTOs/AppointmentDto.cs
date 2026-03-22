using VMS.Domain.Enums;

namespace VMS.Application.DTOs;

public class AppointmentDto
{
    public Guid Id { get; set; }
    public string VisitorName { get; set; } = string.Empty;
    public Guid HostId { get; set; }
    public string HostName { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public string Time { get; set; } = string.Empty;
    public string Purpose { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public string Status { get; set; } = string.Empty;
    // Enum metadata for frontend MDM display
    public int StatusId { get; set; }
    public string StatusLabel { get; set; } = string.Empty;
}

public class CreateAppointmentDto
{
    public string VisitorName { get; set; } = string.Empty;
    public Guid HostId { get; set; }
    public string Date { get; set; } = string.Empty;
    public string Time { get; set; } = string.Empty;
    public string Purpose { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

public class UpdateAppointmentDto
{
    public string VisitorName { get; set; } = string.Empty;
    public Guid HostId { get; set; }
    public string Date { get; set; } = string.Empty;
    public string Time { get; set; } = string.Empty;
    public string Purpose { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public string Status { get; set; } = string.Empty;
}
