using System.Security.Claims;

namespace VMS.API.Extensions;

/// <summary>
/// Extension methods for ClaimsPrincipal to extract logged-in user details from JWT claims
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Gets the user ID from claims
    /// </summary>
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        if (user == null) return Guid.Empty;
        var id = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(id, out var userId) ? userId : Guid.Empty;
    }

    /// <summary>
    /// Gets the user's email from claims
    /// </summary>
    public static string? GetUserEmail(this ClaimsPrincipal user)
    {
        if (user == null) return null;
        return user.FindFirst(ClaimTypes.Email)?.Value;
    }

    /// <summary>
    /// Gets the user's full name from claims
    /// </summary>
    public static string? GetUserName(this ClaimsPrincipal user)
    {
        if (user == null) return null;
        return user.FindFirst(ClaimTypes.Name)?.Value;
    }

    /// <summary>
    /// Gets the user's role from claims
    /// </summary>
    public static string? GetUserRole(this ClaimsPrincipal user)
    {
        if (user == null) return null;
        return user.FindFirst(ClaimTypes.Role)?.Value;
    }

    /// <summary>
    /// Gets the organisation ID from claims
    /// </summary>
    public static Guid GetOrganisationId(this ClaimsPrincipal user)
    {
        if (user == null) return Guid.Empty;
        var orgId = user.FindFirst("organisationId")?.Value;
        return Guid.TryParse(orgId, out var organisationId) ? organisationId : Guid.Empty;
    }

    /// <summary>
    /// Gets all available user claims as a dictionary
    /// </summary>
    public static Dictionary<string, string> GetAllUserClaims(this ClaimsPrincipal user)
    {
        var claims = new Dictionary<string, string>();
        
        if (user != null && user.Claims != null)
        {
            foreach (var claim in user.Claims)
            {
                claims[claim.Type] = claim.Value;
            }
        }

        return claims;
    }

    /// <summary>
    /// Checks if user has a specific role
    /// </summary>
    public static bool HasRole(this ClaimsPrincipal user, string role)
    {
        if (user == null) return false;
        return user.IsInRole(role);
    }

    /// <summary>
    /// Checks if user has any of the specified roles
    /// </summary>
    public static bool HasAnyRole(this ClaimsPrincipal user, params string[] roles)
    {
        if (user == null) return false;
        return roles.Any(role => user.IsInRole(role));
    }

    /// <summary>
    /// Checks if user has all of the specified roles
    /// </summary>
    public static bool HasAllRoles(this ClaimsPrincipal user, params string[] roles)
    {
        if (user == null) return false;
        return roles.All(role => user.IsInRole(role));
    }
}



