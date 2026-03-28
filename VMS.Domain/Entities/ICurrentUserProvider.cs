namespace VMS.Domain.Entities;

/// <summary>
/// Simple interface to provide the current logged-in user's ID for audit tracking.
/// Implemented at the API layer (using HttpContext claims).
/// Used by repositories to automatically set CreatedBy, UpdatedBy, DeletedBy.
/// </summary>
public interface ICurrentUserProvider
{
    /// <summary>
    /// Returns the current user's ID, or null if no user is authenticated.
    /// </summary>
    Guid? GetCurrentUserId();
}

