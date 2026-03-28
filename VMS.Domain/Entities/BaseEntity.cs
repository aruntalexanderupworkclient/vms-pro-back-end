namespace VMS.Domain.Entities;

public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    // ═══════════════════════════════════════════════════════════
    // CREATION AUDIT TRAIL
    // ═══════════════════════════════════════════════════════════
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid? CreatedBy { get; set; }  // Foreign Key to Users.Id (Nullable - for bootstrap scenarios)
    
    // ═══════════════════════════════════════════════════════════
    // UPDATE AUDIT TRAIL
    // ═══════════════════════════════════════════════════════════
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }  // Foreign Key to Users.Id (Nullable)
    
    // ═══════════════════════════════════════════════════════════
    // SOFT DELETE AUDIT TRAIL
    // ═══════════════════════════════════════════════════════════
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }  // Nullable - set only on soft delete
    public Guid? DeletedBy { get; set; }  // Foreign Key to Users.Id (Nullable)
}


