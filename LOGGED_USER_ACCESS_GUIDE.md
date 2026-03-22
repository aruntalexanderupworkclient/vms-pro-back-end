# Getting Logged-In User Details in Controller & Service Layers

## Overview
Your VMS project now has built-in extensions and interfaces to access logged-in user information across all layers. Here's the complete guide:

---

## 1️⃣ IN CONTROLLER LAYER

### Option A: Direct Access via User Object (Simplest)

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VMS.Application.DTOs;
using VMS.Application.Interfaces;
using VMS.API.Extensions; // ← Required for extensions

namespace VMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _service;

    public EmployeesController(IEmployeeService service)
    {
        _service = service;
    }

    [HttpPost("Create")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<EmployeeDto>>> Create([FromBody] CreateEmployeeDto dto)
    {
        // Get user details using extension methods
        var userId = User.GetUserId();           // Returns: Guid
        var userEmail = User.GetUserEmail();     // Returns: string?
        var userName = User.GetUserName();       // Returns: string?
        var userRole = User.GetUserRole();       // Returns: string?
        var organisationId = User.GetOrganisationId(); // Returns: Guid

        // Log for auditing
        Console.WriteLine($"Employee created by: {userName} ({userEmail}) - Role: {userRole}");

        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), 
            new { id = result.Id }, 
            ApiResponse<EmployeeDto>.SuccessResponse(result, "Employee created."));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<EmployeeDto>>> Update(Guid id, [FromBody] UpdateEmployeeDto dto)
    {
        // Check user role before proceeding
        if (!User.HasRole("Admin") && !User.HasRole("Manager"))
        {
            return Forbid("You don't have permission to update employees.");
        }

        var result = await _service.UpdateAsync(id, dto);
        if (result == null)
            return NotFound(ApiResponse<EmployeeDto>.FailResponse("Employee not found."));
        
        return Ok(ApiResponse<EmployeeDto>.SuccessResponse(result, "Employee updated."));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id)
    {
        // Get all user claims at once
        var allClaims = User.GetAllUserClaims();
        
        var success = await _service.DeleteAsync(id);
        if (!success)
            return NotFound(ApiResponse<object>.FailResponse("Employee not found."));
        
        return Ok(ApiResponse<object>.SuccessResponse(null!, "Employee deleted."));
    }
}
```

### Available Extension Methods

| Method | Returns | Usage |
|--------|---------|-------|
| `User.GetUserId()` | `Guid` | Get current user's ID |
| `User.GetUserEmail()` | `string?` | Get current user's email |
| `User.GetUserName()` | `string?` | Get current user's full name |
| `User.GetUserRole()` | `string?` | Get current user's role |
| `User.GetOrganisationId()` | `Guid` | Get current user's organisation |
| `User.GetAllUserClaims()` | `Dictionary<string, string>` | Get all claims |
| `User.HasRole(role)` | `bool` | Check if user has specific role |
| `User.HasAnyRole(roles)` | `bool` | Check if user has any of roles |
| `User.HasAllRoles(roles)` | `bool` | Check if user has all roles |
| `User.IsInRole(role)` | `bool` | Built-in role check |

---

## 2️⃣ IN SERVICE LAYER

### Option A: Use IUserContext (Recommended for Services)

First, inject `IUserContext` into your service:

```csharp
using VMS.Application.Interfaces;
using VMS.Application.DTOs;
using VMS.Domain.Entities;
using VMS.Infrastructure.Repositories.UnitOfWork;
using AutoMapper;

namespace VMS.Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;  // ← Inject IUserContext

    public EmployeeService(IUnitOfWorkFactory unitOfWorkFactory, IMapper mapper, IUserContext userContext)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _mapper = mapper;
        _userContext = userContext;
    }

    public async Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto)
    {
        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            return await uow.ExecuteTransactionAsync(async () =>
            {
                // Access current user information
                var userId = _userContext.UserId;
                var userRole = _userContext.UserRole;
                var organisationId = _userContext.OrganisationId;
                var isAdmin = _userContext.IsAdmin;

                // Validate user is authenticated
                if (!_userContext.IsAuthenticated)
                {
                    throw new UnauthorizedAccessException("User must be authenticated.");
                }

                // Set the creating user
                var entity = _mapper.Map<Employee>(dto);
                entity.CreatedBy = userId;  // Track who created it
                entity.CreatedAt = DateTime.UtcNow;
                
                // Ensure employee belongs to user's organisation
                entity.OrganisationId = organisationId;

                var created = await uow.Employees.AddAsync(entity);
                await uow.SaveChangesAsync();

                // Log the action (for audit trail)
                Console.WriteLine($"Employee created by {_userContext.UserName} ({_userContext.UserRole})");

                return _mapper.Map<EmployeeDto>(created);
            });
        }
    }

    public async Task<EmployeeDto?> UpdateAsync(Guid id, UpdateEmployeeDto dto)
    {
        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            return await uow.ExecuteTransactionAsync(async () =>
            {
                var existing = await uow.Employees.GetByIdAsync(id);
                if (existing == null) return null;

                // Check if user is allowed to update (could be owner or admin)
                if (!_userContext.IsAdmin && existing.CreatedBy != _userContext.UserId)
                {
                    throw new UnauthorizedAccessException(
                        "You can only update employees you created.");
                }

                _mapper.Map(dto, existing);
                existing.UpdatedBy = _userContext.UserId;  // Track who updated it
                existing.UpdatedAt = DateTime.UtcNow;

                var updated = await uow.Employees.UpdateAsync(existing);
                await uow.SaveChangesAsync();

                return _mapper.Map<EmployeeDto>(updated);
            });
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            return await uow.ExecuteTransactionAsync(async () =>
            {
                // Only admins can delete
                if (!_userContext.IsAdmin)
                {
                    throw new UnauthorizedAccessException(
                        "Only administrators can delete employees.");
                }

                var existing = await uow.Employees.GetByIdAsync(id);
                if (existing == null) return false;

                await uop.Employees.DeleteAsync(id);
                await uow.SaveChangesAsync();

                return true;
            });
        }
    }
}
```

### IUserContext Properties

| Property | Type | Description |
|----------|------|-------------|
| `UserId` | `Guid` | Current user's ID |
| `UserEmail` | `string?` | Current user's email |
| `UserName` | `string?` | Current user's full name |
| `UserRole` | `string?` | Current user's role |
| `OrganisationId` | `Guid` | Current user's organisation |
| `IsAuthenticated` | `bool` | Is user authenticated |
| `IsAdmin` | `bool` | Is user an admin |
| `AllClaims` | `Dictionary<string, string>` | All JWT claims |

---

## 3️⃣ PASS USER CONTEXT TO SERVICE (Alternative Approach)

If you prefer not to use `IUserContext` interface, you can pass user information directly:

### In Controller:
```csharp
[HttpPost("Create")]
[Authorize(Roles = "Admin")]
public async Task<ActionResult<ApiResponse<EmployeeDto>>> Create([FromBody] CreateEmployeeDto dto)
{
    var userId = User.GetUserId();
    var userRole = User.GetUserRole();
    
    var result = await _service.CreateAsync(dto, userId, userRole);
    return CreatedAtAction(nameof(GetById), new { id = result.Id },
        ApiResponse<EmployeeDto>.SuccessResponse(result, "Employee created."));
}
```

### In Service:
```csharp
public async Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto, Guid userId, string? userRole)
{
    using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
    {
        return await uow.ExecuteTransactionAsync(async () =>
        {
            // Use userId and userRole parameters
            var entity = _mapper.Map<Employee>(dto);
            entity.CreatedBy = userId;
            entity.CreatedAt = DateTime.UtcNow;
            
            var created = await uow.Employees.AddAsync(entity);
            await uow.SaveChangesAsync();
            
            return _mapper.Map<EmployeeDto>(created);
        });
    }
}
```

---

## 4️⃣ PRACTICAL EXAMPLES

### Example 1: Audit Trail Logging

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
                var entity = _mapper.Map<Employee>(dto);
                entity.CreatedBy = _userContext.UserId;
                entity.CreatedAt = DateTime.UtcNow;

                var created = await uow.Employees.AddAsync(entity);
                await uow.SaveChangesAsync();

                // Log audit trail
                _logger.LogInformation(
                    "Employee created: {EmployeeId} by {UserName} ({UserRole}) from {Organisation}",
                    created.Id, 
                    _userContext.UserName, 
                    _userContext.UserRole, 
                    _userContext.OrganisationId);

                return _mapper.Map<EmployeeDto>(created);
            });
        }
    }
}
```

### Example 2: Organization-Based Filtering

```csharp
public async Task<PagedResult<EmployeeDto>> GetAllAsync(PaginationParams pagination)
{
    using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
    {
        // If not admin, filter by user's organization
        var spec = _userContext.IsAdmin
            ? new GetEmployeesPagedSpecification(pagination.Page, pagination.PageSize)
            : new GetEmployeesPagedSpecification(
                pagination.Page, 
                pagination.PageSize, 
                _userContext.OrganisationId);  // Filter by org

        var items = await uow.Employees.GetBySpecificationAsync(spec);
        var count = await uow.Employees.GetCountAsync(
            _userContext.IsAdmin ? null : _userContext.OrganisationId);

        return new PagedResult<EmployeeDto>
        {
            Items = _mapper.Map<IEnumerable<EmployeeDto>>(items),
            TotalCount = count,
            Page = pagination.Page,
            PageSize = pagination.PageSize
        };
    }
}
```

### Example 3: Role-Based Authorization in Service

```csharp
public async Task<bool> DeleteAsync(Guid id)
{
    using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
    {
        return await uow.ExecuteTransactionAsync(async () =>
        {
            // Ensure only admin or manager can delete
            if (!_userContext.IsAdmin && _userContext.UserRole != "Manager")
            {
                throw new UnauthorizedAccessException(
                    $"Role '{_userContext.UserRole}' is not authorized to delete employees.");
            }

            var existing = await uow.Employees.GetByIdAsync(id);
            if (existing == null) 
                return false;

            await uow.Employees.DeleteAsync(id);
            await uow.SaveChangesAsync();

            _logger.LogWarning(
                "Employee {Id} deleted by {User}",
                id,
                _userContext.UserName);

            return true;
        });
    }
}
```

---

## 5️⃣ SETUP CHECKLIST

✅ **Already Done for You:**
- Created `ClaimsPrincipalExtensions.cs` - Extension methods for User claims
- Created `IUserContext.cs` interface - Service layer abstraction
- Created `UserContext.cs` - Implementation of IUserContext
- Updated `ServiceExtensions.cs` - DI registration

✅ **To Use in Your Project:**

1. **In Controllers:** Use `User.GetUserId()`, `User.GetUserRole()`, etc.
   ```csharp
   using VMS.API.Extensions; // Add this using statement
   ```

2. **In Services:** Inject `IUserContext`
   ```csharp
   public class YourService(IUserContext userContext)
   {
       // Use _userContext.UserId, _userContext.UserRole, etc.
   }
   ```

---

## 6️⃣ COMMON PATTERNS

### Pattern 1: Owner-Only Update
```csharp
var isOwner = _userContext.UserId == entity.CreatedBy;
var isAdmin = _userContext.IsAdmin;

if (!isOwner && !isAdmin)
    throw new UnauthorizedAccessException("You can only update your own records.");
```

### Pattern 2: Organization Isolation
```csharp
var isOwnOrg = _userContext.OrganisationId == entity.OrganisationId;
var isAdmin = _userContext.IsAdmin;

if (!isOwnOrg && !isAdmin)
    throw new UnauthorizedAccessException("You can only access resources in your organization.");
```

### Pattern 3: Multi-Role Authorization
```csharp
var allowedRoles = new[] { "Admin", "Manager", "Supervisor" };
if (!allowedRoles.Contains(_userContext.UserRole))
    throw new UnauthorizedAccessException($"This operation requires one of these roles: {string.Join(", ", allowedRoles)}");
```

### Pattern 4: Audit Trail
```csharp
entity.CreatedBy = _userContext.UserId;
entity.CreatedAt = DateTime.UtcNow;
entity.UpdatedBy = _userContext.UserId;
entity.UpdatedAt = DateTime.UtcNow;

_logger.LogInformation(
    "Record {RecordId} {Action} by {User} from org {Org}",
    entity.Id,
    "created",
    _userContext.UserName,
    _userContext.OrganisationId);
```

---

## 7️⃣ TROUBLESHOOTING

### Issue: `User.GetUserId()` returns Guid.Empty
**Cause:** User is not authenticated or token doesn't contain `NameIdentifier` claim
**Solution:** Ensure `[Authorize]` is on the action and token is valid

### Issue: `IUserContext` injection fails
**Cause:** Not registered in DI or not using scoped lifetime
**Solution:** Already registered in `ServiceExtensions.cs`. Ensure it's called in `Program.cs`:
```csharp
builder.Services.AddApplicationServices();
```

### Issue: `ClaimsPrincipalExtensions` methods not found
**Cause:** Missing `using VMS.API.Extensions;`
**Solution:** Add the using statement at the top of your controller

---

## Summary

| Where | How | Best For |
|-------|-----|----------|
| **Controller** | `User.GetUserId()`, `User.GetUserRole()` | Direct HTTP context access, simple checks |
| **Service** | `IUserContext` interface injection | Service logic, complex authorization |
| **Both** | Pass parameters from Controller to Service | Stateless design, testing |

