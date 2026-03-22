# Quick Reference: Getting Logged User Details

## 🎯 TL;DR Quick Start

### In Controller:
```csharp
using VMS.API.Extensions;

[HttpPost("Create")]
[Authorize]
public async Task<ActionResult> Create([FromBody] CreateDto dto)
{
    var userId = User.GetUserId();
    var role = User.GetUserRole();
    var email = User.GetUserEmail();
    var orgId = User.GetOrganisationId();
    
    // Use these values...
}
```

### In Service:
```csharp
public class MyService
{
    private readonly IUserContext _userContext;
    
    public MyService(IUserContext userContext)
    {
        _userContext = userContext;
    }
    
    public async Task DoSomethingAsync()
    {
        var userId = _userContext.UserId;
        var role = _userContext.UserRole;
        var email = _userContext.UserEmail;
        var orgId = _userContext.OrganisationId;
        var isAdmin = _userContext.IsAdmin;
        var isAuth = _userContext.IsAuthenticated;
        
        // Use these values...
    }
}
```

---

## 📋 Extension Methods (Use in Controllers)

```csharp
User.GetUserId()              // → Guid
User.GetUserEmail()           // → string?
User.GetUserName()            // → string?
User.GetUserRole()            // → string?
User.GetOrganisationId()      // → Guid
User.GetAllUserClaims()       // → Dictionary<string, string>
User.HasRole("Admin")         // → bool
User.HasAnyRole("Admin", "Manager") // → bool
User.HasAllRoles("Admin", "Manager") // → bool
User.IsInRole("Admin")        // → bool (built-in)
```

---

## 🔌 IUserContext Properties (Use in Services)

```csharp
_userContext.UserId           // → Guid
_userContext.UserEmail        // → string?
_userContext.UserName         // → string?
_userContext.UserRole         // → string?
_userContext.OrganisationId   // → Guid
_userContext.IsAuthenticated  // → bool
_userContext.IsAdmin          // → bool
_userContext.AllClaims        // → Dictionary<string, string>
```

---

## 🔐 Files Created/Modified

✅ **Created:**
- `VMS.API/Extensions/ClaimsPrincipalExtensions.cs` - Extension methods
- `VMS.Application/Interfaces/IUserContext.cs` - Interface for services
- `VMS.API/Services/UserContext.cs` - Implementation
- `LOGGED_USER_ACCESS_GUIDE.md` - Full documentation

✅ **Modified:**
- `VMS.API/Extensions/ServiceExtensions.cs` - Added DI registration

---

## 📝 Common Usage Patterns

### Get current user ID and verify action
```csharp
var userId = User.GetUserId();
if (userId == entity.CreatedBy || User.IsInRole("Admin"))
{
    // Proceed
}
```

### Restrict to organization
```csharp
var userOrgId = User.GetOrganisationId();
if (entity.OrganisationId != userOrgId && !User.IsInRole("Admin"))
{
    return Forbid();
}
```

### Audit trail
```csharp
entity.CreatedBy = User.GetUserId();
entity.CreatedAt = DateTime.UtcNow;
_logger.LogInformation($"Created by {User.GetUserName()}");
```

### Role-based logic
```csharp
if (User.HasRole("Admin"))
{
    // Admin only
}
else if (User.HasRole("Manager"))
{
    // Manager only
}
```

---

## ❓ FAQ

**Q: Where do I get the user info?**
A: From JWT token claims automatically extracted by ASP.NET Core middleware

**Q: What's the difference between `User` (extension methods) and `IUserContext`?**
A: `User` is for controllers (direct HTTP context), `IUserContext` is for services (dependency injection)

**Q: Can I use both approaches?**
A: Yes! Use extensions in controllers, and pass `IUserContext` to services via dependency injection.

**Q: What happens if user is not authenticated?**
A: 
- `User.GetUserId()` returns `Guid.Empty`
- `_userContext.IsAuthenticated` returns `false`
- API returns 401 Unauthorized if endpoint has `[Authorize]`

**Q: Where is user role stored?**
A: In JWT token as `ClaimTypes.Role` claim, added during login in `AuthController.GenerateJwtToken()`

**Q: Can I add more claims?**
A: Yes! Edit `AuthController.cs` GenerateJwtToken() method to add more claims to the JWT

---

## 🚀 Getting Started Now

1. **In any controller:**
   ```csharp
   using VMS.API.Extensions;
   
   var userId = User.GetUserId();
   ```

2. **In any service:**
   ```csharp
   public MyService(IUserContext userContext) => _userContext = userContext;
   
   var userId = _userContext.UserId;
   ```

Done! 🎉

