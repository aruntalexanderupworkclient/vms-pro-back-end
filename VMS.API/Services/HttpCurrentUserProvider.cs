using VMS.API.Extensions;
using VMS.Domain.Entities;

namespace VMS.API.Services;

/// <summary>
/// Provides the current logged-in user's ID from HttpContext claims.
/// Used by repositories for automatic audit tracking (CreatedBy, UpdatedBy, DeletedBy).
/// Registered as scoped - available throughout the request lifetime.
/// </summary>
public class HttpCurrentUserProvider : ICurrentUserProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpCurrentUserProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    public Guid? GetCurrentUserId()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated != true)
            return null;

        var userId = user.GetUserId();
        return userId != Guid.Empty ? userId : null;
    }
}

