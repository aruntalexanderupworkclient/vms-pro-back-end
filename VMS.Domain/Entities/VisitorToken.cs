using VMS.Domain.Enums;

namespace VMS.Domain.Entities;

public class VisitorToken : BaseEntity
{
    public string TokenNumber { get; set; } = string.Empty;
    public Guid VisitorId { get; set; }
    public TokenType TokenType { get; set; } = TokenType.Visitor;
    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
    public VisitStatus Status { get; set; } = VisitStatus.CheckedIn;

    public Visitor? Visitor { get; set; }
}
