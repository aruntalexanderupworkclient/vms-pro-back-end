# Specification Pattern Flow Diagram

## Architecture Overview

```
┌─────────────────────────────────────────────────────────────────┐
│                        API Controller                            │
│                    (UsersController.cs)                          │
└────────────────────────┬────────────────────────────────────────┘
                         │ Call GetAllAsync(pagination)
                         ▼
┌─────────────────────────────────────────────────────────────────┐
│                       Service Layer                             │
│                    (UserService.cs)                             │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │ Create specification with includes                       │  │
│  │ var spec = new GetUsersPagedSpecification(...)          │  │
│  │   - Includes: ["Role", "Organisation"]                  │  │
│  │   - Criteria: search filter                             │  │
│  │   - Pagination: page, size                              │  │
│  └──────────────────────────────────────────────────────────┘  │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │ Call repository with specification                       │  │
│  │ await _repository.GetBySpecificationAsync(spec)         │  │
│  └──────────────────────────────────────────────────────────┘  │
└────────────────────────┬────────────────────────────────────────┘
                         │ Pass Specification<User>
                         ▼
┌─────────────────────────────────────────────────────────────────┐
│                   Repository Interface                          │
│                    (IRepository<T>)                             │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │ GetBySpecificationAsync(Specification<T> spec)          │  │
│  │ GetByIdWithSpecificationAsync(Guid id, spec)            │  │
│  └──────────────────────────────────────────────────────────┘  │
└────────────────────────┬────────────────────────────────────────┘
                         │ Routes to appropriate implementation
                         ├─────────────────┬────────────────────┐
                         ▼                 ▼                    ▼
        ┌───────────────────┐  ┌──────────────────────┐
        │  PostgreSQL       │  │    InMemory         │
        │  Repository       │  │    Repository       │
        │                   │  │                     │
        │ 1. Apply includes │  │ (Includes ignored   │
        │ 2. Apply criteria │  │  - all data loaded) │
        │ 3. Apply ordering │  │                     │
        │ 4. Apply paging   │  │ ApplySpecification()│
        │                   │  │ Query().Where().    │
        │ ApplySpecification│  │ OrderBy().Skip()    │
        │ () method         │  │ .Take()             │
        └─────────┬─────────┘  └──────────┬──────────┘
                  │                       │
      ┌───────────┴───────────────────────┴─────────────────┐
      │ Return: IEnumerable<T> with related entities loaded │
      └───────────┬──────────────────────────────────────────┘
                  │
                  ▼
        ┌──────────────────────┐
        │    Service Layer     │
        │  (UserService)       │
        │                      │
        │ _mapper.Map() to DTO │
        │ AutoMapper does:     │
        │  - User → UserDto    │
        │  - Role → RoleName   │
        │  - Org → OrgName     │
        └──────────┬───────────┘
                  │
                  ▼
        ┌──────────────────────┐
        │  PagedResult<UserDto>│
        │  with full data:     │
        │  - Id                │
        │  - FullName          │
        │  - Email             │
        │  - RoleId ✅         │
        │  - RoleName ✅       │
        │  - OrganisationId ✅ │
        │  - OrganisationName ✅
        └──────────┬───────────┘
                  │
                  ▼
        ┌──────────────────────┐
        │   API Response       │
        │  to Client           │
        └──────────────────────┘
```

## Method Decision Tree

```
Do I need related entities?
│
├─ YES → Does the method return multiple items?
│        │
│        ├─ YES → Use GetBySpecificationAsync()
│        │        Example: GetAll(), GetPaged()
│        │
│        └─ NO → Use GetByIdWithSpecificationAsync()
│                Example: GetById(), GetByEmail()
│
└─ NO → Is it a CRUD operation?
        │
        ├─ ADD → Use AddAsync()
        ├─ UPDATE → Use UpdateAsync()
        └─ DELETE → Use DeleteAsync()
```

## Specification Usage Example

### Creating a Specification

```csharp
public class GetUsersPagedSpecification : Specification<User>
{
    public GetUsersPagedSpecification(int page, int pageSize, string? search = null)
    {
        // ✅ Define what to load
        Includes.Add("Role");              // Load Role relationship
        Includes.Add("Organisation");      // Load Organisation relationship

        // ✅ Define filter
        if (!string.IsNullOrEmpty(search))
            Criteria = u => u.FullName.Contains(search);

        // ✅ Define ordering
        OrderByDescending = u => u.CreatedAt;

        // ✅ Define pagination
        Skip = (page - 1) * pageSize;
        Take = pageSize;
        IsPagingEnabled = true;
    }
}
```

### Using the Specification in Service

```csharp
public async Task<PagedResult<UserDto>> GetAllAsync(PaginationParams pagination)
{
    // ✅ Create specification with all parameters
    var spec = new GetUsersPagedSpecification(
        pagination.Page, 
        pagination.PageSize, 
        pagination.Search
    );

    // ✅ Pass to repository
    var items = await _repository.GetBySpecificationAsync(spec);
    
    // At this point:
    // - items.Role is loaded ✅
    // - items.Organisation is loaded ✅
    
    // ✅ Get count for pagination info
    var count = await _repository.GetCountAsync(pagination.Search);
    
    // ✅ Map to DTO
    return new PagedResult<UserDto>
    {
        Items = _mapper.Map<IEnumerable<UserDto>>(items),
        TotalCount = count,
        Page = pagination.Page,
        PageSize = pagination.PageSize
    };
}
```

### In Repository - PostgreSQL

```csharp
public async Task<IEnumerable<T>> GetBySpecificationAsync(Specification<T> spec)
{
    var query = ApplySpecification(spec);
    return await query.ToListAsync();
}

private IQueryable<T> ApplySpecification(Specification<T> spec)
{
    var query = _dbSet.AsQueryable();

    // Step 1: Apply includes (eager loading)
    foreach (var include in spec.Includes)
        query = query.Include(include);
    // Result: SELECT * FROM Users INNER JOIN Roles ON ...

    // Step 2: Apply filter
    if (spec.Criteria != null)
        query = query.Where(spec.Criteria);
    // Result: ... WHERE FullName LIKE '%search%'

    // Step 3: Apply ordering
    if (spec.OrderByDescending != null)
        query = query.OrderByDescending(spec.OrderByDescending);
    // Result: ... ORDER BY CreatedAt DESC

    // Step 4: Apply paging
    if (spec.IsPagingEnabled)
        query = query.Skip(spec.Skip).Take(spec.Take);
    // Result: ... SKIP 0 TAKE 10

    return query;
}
```

## Data Flow Example

### Request
```json
GET /api/users?page=1&pageSize=10&search=john
```

### Service Creates Specification
```csharp
var spec = new GetUsersPagedSpecification(1, 10, "john");
// spec.Includes = ["Role", "Organisation"]
// spec.Criteria = u => u.FullName.Contains("john")
// spec.OrderByDescending = u => u.CreatedAt
// spec.Skip = 0
// spec.Take = 10
```

### Repository Builds Query
```sql
SELECT u.*, r.*, o.*
FROM "Users" u
INNER JOIN "Roles" r ON u."RoleId" = r."Id"
INNER JOIN "Organisations" o ON u."OrganisationId" = o."Id"
WHERE u."FullName" ILIKE '%john%'
ORDER BY u."CreatedAt" DESC
LIMIT 10
OFFSET 0
```

### Data Returned
```
User {
    Id: guid,
    FullName: "John Doe",
    Email: "john@example.com",
    RoleId: guid,
    Role: {
        Id: guid,
        Name: "Admin"  ✅ Loaded from specification
    },
    OrganisationId: guid,
    Organisation: {
        Id: guid,
        Name: "Company Inc"  ✅ Loaded from specification
    }
}
```

### AutoMapper Converts to DTO
```
UserDto {
    Id: guid,
    FullName: "John Doe",
    Email: "john@example.com",
    RoleId: guid,
    RoleName: "Admin"  ✅ From Role.Name
    OrganisationId: guid,
    OrganisationName: "Company Inc"  ✅ From Organisation.Name
}
```

### Response Sent to Client
```json
{
    "items": [
        {
            "id": "guid",
            "fullName": "John Doe",
            "email": "john@example.com",
            "roleId": "guid",
            "roleName": "Admin",
            "organisationId": "guid",
            "organisationName": "Company Inc"
        }
    ],
    "totalCount": 1,
    "page": 1,
    "pageSize": 10
}
```

## InMemory vs PostgreSQL Comparison

```
┌────────────────────────────────────────────────────────────────┐
│                    ApplySpecification()                        │
├─────────────────────────────┬─────────────────────────────────┤
│      PostgreSQL              │         InMemory                │
├─────────────────────────────┼─────────────────────────────────┤
│ Include(string propName)    │ Not used - all data in memory   │
│ → Uses EF Core reflection    │ Already available              │
│                              │                                 │
│ Where(criteria)             │ Where(criteria)                │
│ → SQL WHERE clause           │ LINQ to Objects                │
│                              │                                 │
│ OrderBy/OrderByDescending   │ OrderBy/OrderByDescending      │
│ → SQL ORDER BY              │ LINQ ordering                  │
│                              │                                 │
│ Skip/Take                    │ Skip/Take                      │
│ → SQL OFFSET/LIMIT          │ LINQ Skip/Take                 │
├─────────────────────────────┼─────────────────────────────────┤
│ Result: SQL Query            │ Result: In-memory filtered     │
│ Sent to database             │ Returned from memory           │
└─────────────────────────────┴─────────────────────────────────┘
```

## Benefits Summary

```
BEFORE (Without Specification Pattern)
├─ RoleName is NULL ❌
├─ OrganisationName is NULL ❌
├─ Need to use context directly ❌
└─ Hard to maintain ❌

AFTER (With Specification Pattern)
├─ RoleName is populated ✅
├─ OrganisationName is populated ✅
├─ Clean service layer ✅
├─ Reusable specifications ✅
├─ Works with InMemory too ✅
└─ Easy to extend ✅
```

