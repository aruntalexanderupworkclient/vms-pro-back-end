# Specification Pattern Implementation Checklist

## ✅ Completed Tasks

### Core Infrastructure (100% Complete)

- [x] Create base `Specification<T>` class
  - Location: `VMS.Infrastructure/Repositories/Specifications/Specification.cs`
  - Purpose: Provides reusable template for query specifications
  
- [x] Update `IRepository<T>` interface
  - Location: `VMS.Infrastructure/Repositories/Interfaces/IRepository.cs`
  - Added: `GetBySpecificationAsync()`, `GetByIdWithSpecificationAsync()`
  - Preserved: All existing methods for backward compatibility
  
- [x] Update `PostgreSqlRepository<T>` implementation
  - Location: `VMS.Infrastructure/Repositories/PostgreSQL/PostgreSqlRepository.cs`
  - Added: `GetBySpecificationAsync()`, `GetByIdWithSpecificationAsync()`, `ApplySpecification()`
  - Uses: `.Include()` to load related entities
  
- [x] Update `InMemoryRepository<T>` implementation
  - Location: `VMS.Infrastructure/Repositories/InMemory/InMemoryRepository.cs`
  - Added: `GetBySpecificationAsync()`, `GetByIdWithSpecificationAsync()`
  - Compatible: Works without `.Include()` since all data is in memory

### Entity Specifications (100% Complete)

- [x] **User Specifications**
  - Location: `VMS.Infrastructure/Repositories/Specifications/Users/UserSpecification.cs`
  - Classes: GetAllUsersSpecification, GetUsersPagedSpecification, GetUserByIdSpecification, FindUserSpecification
  - Includes: Role, Organisation
  
- [x] **Visitor Specifications**
  - Location: `VMS.Infrastructure/Repositories/Specifications/Visitors/VisitorSpecification.cs`
  - Classes: GetAllVisitorsSpecification, GetVisitorsPagedSpecification, GetVisitorByIdSpecification
  - Includes: Host, Tokens
  
- [x] **Token Specifications**
  - Location: `VMS.Infrastructure/Repositories/Specifications/Tokens/TokenSpecification.cs`
  - Classes: GetAllTokensSpecification, GetTokensPagedSpecification, GetTokenByIdSpecification
  - Includes: Visitor, Visitor.Host
  
- [x] **Host Specifications**
  - Location: `VMS.Infrastructure/Repositories/Specifications/Hosts/HostSpecification.cs`
  - Classes: GetAllHostsSpecification, GetHostsPagedSpecification, GetHostByIdSpecification
  - Includes: None (Host is typically a root entity)
  
- [x] **Appointment Specifications**
  - Location: `VMS.Infrastructure/Repositories/Specifications/Appointments/AppointmentSpecification.cs`
  - Classes: GetAllAppointmentsSpecification, GetAppointmentsPagedSpecification, GetAppointmentByIdSpecification
  - Includes: Host

### Service Layer (Partial - 14%)

- [x] **UserService Updated**
  - Location: `VMS.Application/Services/UserService.cs`
  - Methods Updated:
    - GetAllAsync() → Uses GetUsersPagedSpecification
    - GetByIdAsync() → Uses GetUserByIdSpecification
    - GetByEmailAsync() → Uses FindUserSpecification
  - Methods Unchanged (as intended):
    - CreateAsync() → Direct AddAsync()
    - UpdateAsync() → Direct UpdateAsync()
    - DeleteAsync() → Direct DeleteAsync()

---

## ⏳ Remaining Tasks

### Service Updates (14% → 100% needed)

- [ ] **VisitorService**
  - Update: GetAllAsync()
  - Update: GetByIdAsync()
  - Use: GetVisitorsPagedSpecification, GetVisitorByIdSpecification
  
- [ ] **TokenService**
  - Update: GetAllAsync()
  - Update: GetByIdAsync()
  - Use: GetTokensPagedSpecification, GetTokenByIdSpecification
  
- [ ] **HostService**
  - Update: GetAllAsync()
  - Update: GetByIdAsync()
  - Use: GetHostsPagedSpecification, GetHostByIdSpecification
  
- [ ] **AppointmentService**
  - Update: GetAllAsync()
  - Update: GetByIdAsync()
  - Use: GetAppointmentsPagedSpecification, GetAppointmentByIdSpecification
  
- [ ] **EmployeeService**
  - Create: EmployeeSpecification class (if not exists)
  - Update: GetAllAsync()
  - Update: GetByIdAsync()
  
- [ ] **RoleService**
  - Create: RoleSpecification class (if not exists)
  - Update: GetAllAsync()
  - Update: GetByIdAsync()
  
- [ ] **PermissionService**
  - Create: PermissionSpecification class (if not exists)
  - Update: GetAllAsync()
  - Update: GetByIdAsync()
  
- [ ] **ReportService**
  - Evaluate: Whether this needs specifications
  - Update: As needed

---

## Service Update Template

Use this template to update each remaining service:

```csharp
// At the top of the file, add:
using VMS.Infrastructure.Repositories.Specifications.YourEntity;

// Update GetAll method (if exists):
public async Task<PagedResult<YourDto>> GetAllAsync(PaginationParams pagination)
{
    var spec = new GetYourEntityPagedSpecification(pagination.Page, pagination.PageSize, pagination.Search);
    var items = await _repository.GetBySpecificationAsync(spec);
    var count = await _repository.GetCountAsync(pagination.Search);
    
    return new PagedResult<YourDto>
    {
        Items = _mapper.Map<IEnumerable<YourDto>>(items),
        TotalCount = count,
        Page = pagination.Page,
        PageSize = pagination.PageSize
    };
}

// Update GetById method (if exists):
public async Task<YourDto?> GetByIdAsync(Guid id)
{
    var spec = new GetYourEntityByIdSpecification(id);
    var entity = await _repository.GetByIdWithSpecificationAsync(id, spec);
    return entity == null ? null : _mapper.Map<YourDto>(entity);
}

// Keep CRUD methods unchanged:
// - CreateAsync() → Keep as-is
// - UpdateAsync() → Keep as-is
// - DeleteAsync() → Keep as-is
```

---

## Before & After Comparison

### Before (Problem)
```csharp
// UserService.cs
public async Task<PagedResult<UserDto>> GetAllAsync(PaginationParams pagination)
{
    var items = await _repository.GetPagedAsync(...);
    // ❌ items.Role is NULL
    // ❌ items.Organisation is NULL
    // ❌ RoleName will be null in response
    // ❌ OrganisationName will be null in response
}

// Result in API Response:
{
    "roleName": null,           // ❌ PROBLEM!
    "organisationName": null    // ❌ PROBLEM!
}
```

### After (Solution)
```csharp
// UserService.cs
public async Task<PagedResult<UserDto>> GetAllAsync(PaginationParams pagination)
{
    var spec = new GetUsersPagedSpecification(...);
    var items = await _repository.GetBySpecificationAsync(spec);
    // ✅ items.Role is loaded
    // ✅ items.Organisation is loaded
    // ✅ RoleName populated from Role.Name
    // ✅ OrganisationName populated from Organisation.Name
}

// Result in API Response:
{
    "roleName": "Admin",              // ✅ WORKS!
    "organisationName": "Company Inc" // ✅ WORKS!
}
```

---

## Testing Checklist

After updating each service, test:

### For GetAllAsync / GetPagedAsync
- [ ] Test without search filter
- [ ] Test with search filter
- [ ] Test pagination (page 1, 2, etc.)
- [ ] Verify related entities are populated
- [ ] Verify DTO fields are not null

### For GetByIdAsync
- [ ] Test with valid ID
- [ ] Test with invalid ID (should return null)
- [ ] Verify related entities are populated
- [ ] Verify DTO fields are not null

### For CRUD Operations
- [ ] Create operation works
- [ ] Update operation works
- [ ] Delete operation works
- [ ] Data persists correctly

---

## File Locations Summary

### Core Infrastructure
```
VMS.Infrastructure/Repositories/
├── Specifications/
│   ├── Specification.cs ✅
│   ├── Users/
│   │   └── UserSpecification.cs ✅
│   ├── Visitors/
│   │   └── VisitorSpecification.cs ✅
│   ├── Tokens/
│   │   └── TokenSpecification.cs ✅
│   ├── Hosts/
│   │   └── HostSpecification.cs ✅
│   └── Appointments/
│       └── AppointmentSpecification.cs ✅
├── Interfaces/
│   └── IRepository.cs ✅
├── PostgreSQL/
│   └── PostgreSqlRepository.cs ✅
└── InMemory/
    └── InMemoryRepository.cs ✅
```

### Service Layer
```
VMS.Application/Services/
├── UserService.cs ✅
├── VisitorService.cs ⏳
├── TokenService.cs ⏳
├── HostService.cs ⏳
├── AppointmentService.cs ⏳
├── EmployeeService.cs ⏳
├── RoleService.cs ⏳
├── PermissionService.cs ⏳
└── ReportService.cs ⏳
```

---

## Key Points

### ✅ What Works Now
1. RoleId and RoleName are both populated in UserDto
2. OrganisationId and OrganisationName are both populated
3. Repository is database-agnostic (PostgreSQL and InMemory compatible)
4. Service layer is clean (no direct _context access)
5. Query logic is centralized and reusable

### ✅ What Needs Doing
1. Update 7 remaining services following the UserService pattern
2. Create specifications for EmployeeService if needed
3. Create specifications for RoleService if needed
4. Create specifications for PermissionService if needed
5. Test all services after updates

### ✅ Benefits
- Scalable to 100+ entities
- Easy to maintain
- InMemory compatible
- No N+1 queries
- Clean separation of concerns
- Reusable specifications

---

## Estimation

- **Time to complete remaining updates**: ~2-3 hours
- **Difficulty level**: Low (just following the UserService template)
- **Risk level**: Minimal (backward compatible)

---

## Quick Reference for Each Service

### Which Specification to Use?

| Service | GetAll | GetById | Related Entities |
|---------|--------|--------|---|
| User | GetUsersPagedSpecification | GetUserByIdSpecification | Role, Organisation |
| Visitor | GetVisitorsPagedSpecification | GetVisitorByIdSpecification | Host, Tokens |
| Token | GetTokensPagedSpecification | GetTokenByIdSpecification | Visitor, Visitor.Host |
| Host | GetHostsPagedSpecification | GetHostByIdSpecification | None |
| Appointment | GetAppointmentsPagedSpecification | GetAppointmentByIdSpecification | Host |
| Employee | TBD | TBD | TBD |
| Role | TBD | TBD | TBD |
| Permission | TBD | TBD | TBD |

---

## Support Documentation

- See: `SPECIFICATION_PATTERN_GUIDE.md` - Implementation guide for all services
- See: `SPECIFICATION_FLOW_DIAGRAM.md` - Visual flow and data transformation
- See: `IMPLEMENTATION_SUMMARY.md` - What was implemented and why

---

## Success Criteria

✅ All services implement Specification pattern for read operations
✅ RoleName and OrganisationName are populated in API responses
✅ No NULL values in related entity fields
✅ Both PostgreSQL and InMemory repositories work correctly
✅ All tests pass
✅ API responds with complete data

