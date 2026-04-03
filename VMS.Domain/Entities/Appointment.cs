namespace VMS.Domain.Entities;

public class Appointment : BaseEntity
{
    public string VisitorName { get; set; } = string.Empty;
    public Guid HostId { get; set; }
    public DateTime ScheduledAt { get; set; }
    public int Duration { get; set; }
    public string Purpose { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public Guid StatusId { get; set; }

    public Host? Host { get; set; }
    public MdmVisitStatus? Status { get; set; }
}
