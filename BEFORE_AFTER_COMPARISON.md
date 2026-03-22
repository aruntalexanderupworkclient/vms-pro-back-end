# Before & After: Getting User Details

## The Problem (Before)

You had no built-in way to access logged-in user details consistently. You had to:

### ❌ Option 1: Direct claims access (verbose)
```csharp
[HttpPost("Create")]
[Authorize]
public async Task<IActionResult> Create([FromBody] CreateEmployeeDto dto)
{
    // Verbose and repetitive
    var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
    var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
    var orgId = User.FindFirst("organisationId")?.Value;
    
    // Parsing required
    if (!Guid.TryParse(userId, out var parsedUserId)) return BadRequest();
    if (!Guid.TryParse(orgId, out var parsedOrgId)) return BadRequest();
    
    // Then use them...
}
```

### ❌ Option 2: No consistent way in services
```csharp
public class EmployeeService : IEmployeeService
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    
    // ❌ No way to know who's calling this!
    public async Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto)
    {
        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            var entity = _mapper.Map<Employee>(dto);
            // Who created this? Unknown!
            // What org? Unknown!
            // Can they do this? Unknown!
            await uow.Employees.AddAsync(entity);
            await uow.SaveChangesAsync();
            return _mapper.Map<EmployeeDto>(entity);
        }
    }
}
```

### ❌ Option 3: Pass everything as parameters
```csharp
[HttpPost("Create")]
public async Task<IActionResult> Create([FromBody] CreateEmployeeDto dto)
{
    var userId = User.FindFirst(...)?.Value;
    var role = User.FindFirst(...)?.Value;
    var orgId = User.FindFirst(...)?.Value;
    
    // Verbose parameter passing
    var result = await _service.CreateAsync(dto, userId, role, orgId);
}

public async Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto, string userId, string role, string orgId)
{
    // Parameters get lost in translation
}
```

---

## The Solution (After) ✨

### ✅ Option 1: Clean, type-safe extension methods
```csharp
using VMS.API.Extensions; // One import!

[HttpPost("Create")]
[Authorize]
public async Task<IActionResult> Create([FromBody] CreateEmployeeDto dto)
{
    // Clean and readable
    var userId = User.GetUserId();          // Guid
    var email = User.GetUserEmail();        // string?
    var role = User.GetUserRole();          // string?
    var orgId = User.GetOrganisationId();   // Guid
    
    // No parsing needed, no repetition
    var result = await _service.CreateAsync(dto);
}
```

### ✅ Option 2: Service knows about current user
```csharp
public class EmployeeService : IEmployeeService
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IUserContext _userContext;  // ← Inject this
    
    public EmployeeService(IUnitOfWorkFactory unitOfWorkFactory, IUserContext userContext)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _userContext = userContext;
    }
    
    public async Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto)
    {
        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            // Now we know!
            var entity = _mapper.Map<Employee>(dto);
            entity.CreatedBy = _userContext.UserId;  // ← Who created
            entity.OrganisationId = _userContext.OrganisationId;  // ← Which org
            
            // Can even check permissions
            if (!_userContext.IsAdmin)
                throw new ForbiddenException();
            
            await uow.Employees.AddAsync(entity);
            await uow.SaveChangesAsync();
            return _mapper.Map<EmployeeDto>(entity);
        }
    }
}
```

### ✅ Option 3: Best of both worlds - use IUserContext everywhere
```csharp
// In Controller
[HttpPost("Create")]
[Authorize]
public async Task<IActionResult> Create([FromBody] CreateEmployeeDto dto)
{
    var result = await _service.CreateAsync(dto);
    return Ok(result);
}

// In Service - no need to pass parameters!
public async Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto)
{
    var userId = _userContext.UserId;
    var role = _userContext.UserRole;
    var orgId = _userContext.OrganisationId;
    
    // Use automatically available user context
}
```

---

## Comparison Table

| Scenario | Before | After |
|----------|--------|-------|
| **Get User ID in Controller** | `User.FindFirst(ClaimTypes.NameIdentifier)?.Value` (need parsing) | `User.GetUserId()` |
| **Get User Role in Controller** | `User.FindFirst(ClaimTypes.Role)?.Value` | `User.GetUserRole()` |
| **Get User in Service** | Pass as parameters OR use HttpContextAccessor directly | Inject `IUserContext` |
| **Audit Trail** | Manual parameter passing or no tracking | `entity.CreatedBy = _userContext.UserId` |
| **Check Permission in Service** | Not possible cleanly | `if (!_userContext.IsAdmin)` |
| **Organization Filtering** | Manual logic in each service | `_userContext.OrganisationId` |
| **Lines of Code** | 5-10 per access | 1-2 per access |
| **Type Safety** | String parsing required | Full type safety |
| **Consistency** | Varies across project | Unified approach |
| **Testability** | Difficult (HttpContext) | Easy (Mock IUserContext) |

---

## Real-World Example: Before vs After

### Create Employee Endpoint

#### BEFORE (Messy)
```csharp
[HttpPost("Create")]
[Authorize(Roles = "Admin,Manager")]
public async Task<ActionResult<ApiResponse<EmployeeDto>>> Create([FromBody] CreateEmployeeDto dto)
{
    // Extract user info (verbose)
    var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    var userRoleStr = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
    var userNameStr = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
    var orgIdStr = User.FindFirst("organisationId")?.Value;
    
    // Parse strings to guids (error-prone)
    if (!Guid.TryParse(userIdStr, out var userId)) 
        return BadRequest("Invalid user ID");
    if (!Guid.TryParse(orgIdStr, out var orgId)) 
        return BadRequest("Invalid organization ID");
    
    // Pass everything to service
    var result = await _service.CreateAsync(dto, userId, userRoleStr, userNameStr, orgId);
    
    return CreatedAtAction(nameof(GetById), new { id = result.Id }, 
        ApiResponse<EmployeeDto>.SuccessResponse(result, "Employee created."));
}

public async Task<EmployeeDto> CreateAsync(
    CreateEmployeeDto dto, 
    Guid userId, 
    string userRole, 
    string userName,
    Guid orgId)
{
    // Parameters scattered throughout
    var entity = _mapper.Map<Employee>(dto);
    entity.CreatedBy = userId;
    entity.OrganisationId = orgId;
    
    // Authorization logic in service
    if (userRole != "Admin" && userRole != "Manager")
        throw new UnauthorizedAccessException();
    
    // Logging
    Console.WriteLine($"Employee created by {userName}");
    
    // ...rest of implementation
}
```

**Issues:** Verbose, error-prone parsing, parameters passed around, mixing concerns

#### AFTER (Clean & Professional)
```csharp
[HttpPost("Create")]
[Authorize(Roles = "Admin,Manager")]
public async Task<ActionResult<ApiResponse<EmployeeDto>>> Create([FromBody] CreateEmployeeDto dto)
{
    var result = await _service.CreateAsync(dto);
    
    return CreatedAtAction(nameof(GetById), new { id = result.Id }, 
        ApiResponse<EmployeeDto>.SuccessResponse(result, "Employee created."));
}

public async Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto)
{
    // User context automatically available
    if (!_userContext.IsAdmin && _userContext.UserRole != "Manager")
        throw new UnauthorizedAccessException();
    
    var entity = _mapper.Map<Employee>(dto);
    entity.CreatedBy = _userContext.UserId;
    entity.OrganisationId = _userContext.OrganisationId;
    
    _logger.LogInformation("Employee created by {User}", _userContext.UserName);
    
    // ...rest of implementation
}
```

**Benefits:** Clean, type-safe, no parsing, concerns separated, highly readable

---

## What You Get Now

### Extension Methods (Controllers)
```csharp
User.GetUserId()           // Guid
User.GetUserEmail()        // string?
User.GetUserName()         // string?
User.GetUserRole()         // string?
User.GetOrganisationId()   // Guid
User.HasRole("Admin")      // bool
User.HasAnyRole(...)       // bool
User.HasAllRoles(...)      // bool
```

### IUserContext Properties (Services)
```csharp
_userContext.UserId           // Guid
_userContext.UserEmail        // string?
_userContext.UserName         // string?
_userContext.UserRole         // string?
_userContext.OrganisationId   // Guid
_userContext.IsAuthenticated  // bool
_userContext.IsAdmin          // bool
_userContext.AllClaims        // Dictionary<string, string>
```

---

## Migration Path

You can migrate gradually:

### Step 1: New code uses new approach
```csharp
// New endpoints use IUserContext
var userId = _userContext.UserId;
```

### Step 2: Update existing code incrementally
```csharp
// Old: User.FindFirst(ClaimTypes.NameIdentifier)?.Value
// New: User.GetUserId()
```

### Step 3: Full cleanup
All code uses consistent, clean approach.

---

## Summary

| Aspect | Before | After |
|--------|--------|-------|
| **Simplicity** | 😞 Complex | 😊 Simple |
| **Consistency** | 😞 Scattered | 😊 Unified |
| **Type Safety** | 😞 Strings & Parsing | 😊 Strong Types |
| **Testability** | 😞 Hard to Mock | 😊 Easy with DI |
| **Maintainability** | 😞 Verbose | 😊 Clean |
| **Developer Experience** | 😞 Repetitive | 😊 Enjoyable |

🎉 **You can now write cleaner, safer, more maintainable code!**

