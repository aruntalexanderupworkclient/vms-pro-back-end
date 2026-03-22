using Microsoft.AspNetCore.Http;
using VMS.API.Extensions;
using VMS.Application.Interfaces;

namespace VMS.API.Services;

/// <summary>
/// Implementation of IUserContext for accessing current logged-in user information
/// This is registered as scoped, so it's available throughout the request lifetime
/// </summary>
public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    public Guid UserId => _httpContextAccessor.HttpContext?.User.GetUserId() ?? Guid.Empty;
    public string? UserEmail => _httpContextAccessor.HttpContext?.User.GetUserEmail();
    public string? UserName => _httpContextAccessor.HttpContext?.User.GetUserName();
    public string? UserRole => _httpContextAccessor.HttpContext?.User.GetUserRole();
    public Guid OrganisationId => _httpContextAccessor.HttpContext?.User.GetOrganisationId() ?? Guid.Empty;
    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
    public bool IsAdmin => _httpContextAccessor.HttpContext?.User.IsInRole("Admin") ?? false;
    public Dictionary<string, string> AllClaims => _httpContextAccessor.HttpContext?.User.GetAllUserClaims() ?? new Dictionary<string, string>();
}


