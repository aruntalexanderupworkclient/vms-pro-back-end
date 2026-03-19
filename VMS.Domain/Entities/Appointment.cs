using VMS.Domain.Enums;

namespace VMS.Domain.Entities;

public class Appointment : BaseEntity
{
    public string VisitorName { get; set; } = string.Empty;
    public Guid HostId { get; set; }
    public DateTime ScheduledAt { get; set; }
    public int Duration { get; set; }
    public string Purpose { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public VisitStatus Status { get; set; } = VisitStatus.Scheduled;

    public Host? Host { get; set; }
}
