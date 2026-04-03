namespace VMS.Domain.Entities;

public class VisitorToken : BaseEntity
{
    public string TokenNumber { get; set; } = string.Empty;
    public Guid VisitorId { get; set; }
    public Guid TokenTypeId { get; set; }
    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
    public Guid StatusId { get; set; }

    public Visitor? Visitor { get; set; }
    public MdmTokenType? TokenType { get; set; }
    public MdmVisitStatus? Status { get; set; }
}
