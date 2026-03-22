# 🚀 Quick Start Checklist

## ✅ What's Already Done For You

- [x] Created `ClaimsPrincipalExtensions.cs` with all extension methods
- [x] Created `IUserContext.cs` interface
- [x] Created `UserContext.cs` implementation
- [x] Updated `ServiceExtensions.cs` with DI registration
- [x] All files compiled with zero errors
- [x] Created comprehensive documentation

---

## 📋 Use Cases & Code Templates

### 1️⃣ Get User ID in Controller

**Use this:**
```csharp
using VMS.API.Extensions;

var userId = User.GetUserId();
```

**Instead of this:**
```csharp
var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
if (!Guid.TryParse(id, out var userId)) { /*error*/ }
```

---

### 2️⃣ Get User Role in Controller

**Use this:**
```csharp
var role = User.GetUserRole();
```

**Instead of:**
```csharp
var role = User.FindFirst(ClaimTypes.Role)?.Value;
```

---

### 3️⃣ Check User Permission in Controller

**Use this:**
```csharp
if (!User.HasRole("Admin"))
    return Forbid();
```

**Instead of:**
```csharp
var role = User.FindFirst(ClaimTypes.Role)?.Value;
if (role != "Admin")
    return Forbid();
```

---

### 4️⃣ Get User in Service

**Use this:**
```csharp
public class MyService
{
    private readonly IUserContext _userContext;
    
    public MyService(IUserContext userContext)
    {
        _userContext = userContext;
    }
    
    public async Task DoAsync()
    {
        var userId = _userContext.UserId;
        var role = _userContext.UserRole;
    }
}
```

**Instead of:**
```csharp
// ❌ No way to know user in service!
// ❌ Or pass everything as parameters
```

---

### 5️⃣ Audit Trail - Track Who Did What

```csharp
// In Service
entity.CreatedBy = _userContext.UserId;
entity.CreatedAt = DateTime.UtcNow;
entity.UpdatedBy = _userContext.UserId;
entity.UpdatedAt = DateTime.UtcNow;

// Log it
_logger.LogInformation(
    "Record {Id} created by {User} at {Time}",
    entity.Id,
    _userContext.UserName,
    DateTime.UtcNow);
```

---

### 6️⃣ Organization Isolation

```csharp
// Only show records from user's organization (unless admin)
var items = _userContext.IsAdmin
    ? await GetAllAsync()
    : await GetByOrganizationAsync(_userContext.OrganisationId);
```

---

### 7️⃣ Permission Check in Service

```csharp
// Verify user is authenticated
if (!_userContext.IsAuthenticated)
    throw new UnauthorizedAccessException("Must be logged in");

// Verify user is admin
if (!_userContext.IsAdmin)
    throw new ForbiddenException("Admin access required");

// Verify user is owner or admin
if (_userContext.UserId != entity.CreatedBy && !_userContext.IsAdmin)
    throw new ForbiddenException("You can only modify your own records");
```

---

### 8️⃣ Role-Based Logic

```csharp
if (_userContext.UserRole == "Admin")
{
    // Admin-only logic
}
else if (_userContext.UserRole == "Manager")
{
    // Manager-only logic
}
else if (_userContext.UserRole == "Employee")
{
    // Employee-only logic
}
```

---

### 9️⃣ Debug: View All User Claims

```csharp
[HttpGet("debug/me")]
[Authorize]
public IActionResult GetMyInfo()
{
    return Ok(new
    {
        userId = User.GetUserId(),
        email = User.GetUserEmail(),
        name = User.GetUserName(),
        role = User.GetUserRole(),
        organisation = User.GetOrganisationId(),
        allClaims = User.GetAllUserClaims()
    });
}
```

---

### 🔟 Complete Example: Create Employee

```csharp
[HttpPost("Create")]
[Authorize(Roles = "Admin,Manager")]
public async Task<ActionResult<ApiResponse<EmployeeDto>>> Create(
    [FromBody] CreateEmployeeDto dto)
{
    var result = await _service.CreateAsync(dto);
    return CreatedAtAction(nameof(GetById), new { id = result.Id },
        ApiResponse<EmployeeDto>.SuccessResponse(result));
}
```

```csharp
public class EmployeeService : IEmployeeService
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IUserContext _userContext;
    private readonly ILogger<EmployeeService> _logger;

    public EmployeeService(
        IUnitOfWorkFactory unitOfWorkFactory,
        IUserContext userContext,
        ILogger<EmployeeService> logger)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _userContext = userContext;
        _logger = logger;
    }

    public async Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto)
    {
        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            return await uow.ExecuteTransactionAsync(async () =>
            {
                // Check authorization
                if (!_userContext.IsAdmin && _userContext.UserRole != "Manager")
                    throw new ForbiddenException("Insufficient permissions");

                // Map and set audit fields
                var entity = _mapper.Map<Employee>(dto);
                entity.CreatedBy = _userContext.UserId;
                entity.CreatedAt = DateTime.UtcNow;
                entity.OrganisationId = _userContext.OrganisationId;

                // Save
                var created = await uow.Employees.AddAsync(entity);
                await uow.SaveChangesAsync();

                // Log
                _logger.LogInformation(
                    "Employee {Id} created by {User} ({Role})",
                    created.Id,
                    _userContext.UserName,
                    _userContext.UserRole);

                return _mapper.Map<EmployeeDto>(created);
            });
        }
    }
}
```

---

## 🎯 Implementation Guide

### Step 1: Import Extensions in Controllers
```csharp
using VMS.API.Extensions;  // Add this line
```

### Step 2: Use in Controllers
```csharp
var userId = User.GetUserId();
var role = User.GetUserRole();
// etc.
```

### Step 3: Inject in Services
```csharp
public MyService(IUserContext userContext)
{
    _userContext = userContext;
}
```

### Step 4: Use in Services
```csharp
var userId = _userContext.UserId;
var isAdmin = _userContext.IsAdmin;
// etc.
```

---

## 🔗 Quick Reference

### Extension Methods (Controllers)
| Method | Returns |
|--------|---------|
| `User.GetUserId()` | `Guid` |
| `User.GetUserEmail()` | `string?` |
| `User.GetUserName()` | `string?` |
| `User.GetUserRole()` | `string?` |
| `User.GetOrganisationId()` | `Guid` |
| `User.GetAllUserClaims()` | `Dict<string, string>` |
| `User.HasRole("Admin")` | `bool` |
| `User.HasAnyRole("Admin", "Mgr")` | `bool` |
| `User.HasAllRoles("Admin", "Mgr")` | `bool` |

### IUserContext Properties (Services)
| Property | Type |
|----------|------|
| `UserId` | `Guid` |
| `UserEmail` | `string?` |
| `UserName` | `string?` |
| `UserRole` | `string?` |
| `OrganisationId` | `Guid` |
| `IsAuthenticated` | `bool` |
| `IsAdmin` | `bool` |
| `AllClaims` | `Dict<string, string>` |

---

## 📚 Documentation Files

- **QUICK_REFERENCE_USER_ACCESS.md** - This quick reference
- **LOGGED_USER_ACCESS_GUIDE.md** - Full comprehensive guide
- **BEFORE_AFTER_COMPARISON.md** - Before/after comparison
- **EXAMPLE_IMPLEMENTATION.cs** - Full code examples
- **IMPLEMENTATION_COMPLETE_USER_ACCESS.md** - Implementation details

---

## ✨ You're Ready!

Just follow the pattern from the examples above and you'll have:
- ✅ Clean code
- ✅ Type-safe access
- ✅ Easy auditing
- ✅ Better authorization
- ✅ More maintainable codebase

---

## 🆘 Troubleshooting

| Issue | Solution |
|-------|----------|
| "GetUserId method not found" | Add `using VMS.API.Extensions;` |
| "Cannot resolve IUserContext" | Ensure Program.cs calls `AddApplicationServices()` |
| "UserId returns Guid.Empty" | Check if endpoint has `[Authorize]` attribute |
| "IUserContext is null" | Make sure it's injected in constructor |

---

Happy coding! 🚀

