using VMS.Domain.Enums;

namespace VMS.Domain.Entities;

public class Visitor : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string IdType { get; set; } = string.Empty;
    public string IdNumber { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
    public Guid HostId { get; set; }
    public string Purpose { get; set; } = string.Empty;
    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public VisitStatus Status { get; set; } = VisitStatus.Scheduled;
    public string ExpectedDuration { get; set; } = string.Empty;
    public string OrgType { get; set; } = string.Empty;

    public Host? Host { get; set; }
    public ICollection<VisitorToken> Tokens { get; set; } = new List<VisitorToken>();
}
