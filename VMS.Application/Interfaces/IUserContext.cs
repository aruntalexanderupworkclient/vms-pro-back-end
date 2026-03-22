namespace VMS.Application.Interfaces;

/// <summary>
/// Interface for accessing current logged-in user information in service layer
/// </summary>
public interface IUserContext
{
    Guid UserId { get; }
    string? UserEmail { get; }
    string? UserName { get; }
    string? UserRole { get; }
    Guid OrganisationId { get; }
    bool IsAuthenticated { get; }
    bool IsAdmin { get; }
    Dictionary<string, string> AllClaims { get; }
}

