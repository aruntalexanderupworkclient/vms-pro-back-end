## Specification Pattern Implementation Guide

This guide explains how to apply the Specification pattern to all your services.

### Quick Reference - Method Requirements

| Method | Needs Spec? | Service Implementation |
|--------|---|---|
| **GetAll/GetPaged** | ✅ YES | Use Specification with includes |
| **GetById** | ✅ YES | Use Specification with includes |
| **Find/GetByX** | ✅ YES | Use Specification with includes |
| **Create** | ❌ NO | Direct `AddAsync()` call |
| **Update** | ❌ NO | Direct `UpdateAsync()` call |
| **Delete** | ❌ NO | Direct `DeleteAsync()` call |
| **GetCount** | ❌ NO | Direct `GetCountAsync()` call |

---

### Pattern Template for Any Service

```csharp
// 1. Import the specification
using VMS.Infrastructure.Repositories.Specifications.YourEntity;

// 2. Methods that NEED specification (with includes)
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

public async Task<YourDto?> GetByIdAsync(Guid id)
{
    var spec = new GetYourEntityByIdSpecification(id);
    var entity = await _repository.GetByIdWithSpecificationAsync(id, spec);
    return entity == null ? null : _mapper.Map<YourDto>(entity);
}

// 3. CRUD methods (NO specification needed)
public async Task<YourDto> CreateAsync(CreateYourDto dto)
{
    var entity = _mapper.Map<YourEntity>(dto);
    var created = await _repository.AddAsync(entity);  // ← Direct call
    return _mapper.Map<YourDto>(created);
}

public async Task<YourDto?> UpdateAsync(Guid id, UpdateYourDto dto)
{
    var existing = await _repository.GetByIdAsync(id);  // ← Direct call
    if (existing == null) return null;
    
    _mapper.Map(dto, existing);
    var updated = await _repository.UpdateAsync(existing);  // ← Direct call
    return _mapper.Map<YourDto>(updated);
}

public async Task<bool> DeleteAsync(Guid id)
{
    var existing = await _repository.GetByIdAsync(id);  // ← Direct call
    if (existing == null) return false;
    await _repository.DeleteAsync(id);  // ← Direct call
    return true;
}
```

---

### Services Already Updated

✅ **UserService** - Uses specifications for GetAll, GetById, GetByEmail

### Services That Still Need Updates

**Follow the pattern above for these services:**

1. **VisitorService**
   - Use: `GetVisitorsPagedSpecification`
   - Includes: Host, Tokens

2. **TokenService** (VisitorToken)
   - Use: `GetTokensPagedSpecification`
   - Includes: Visitor, Visitor.Host

3. **HostService**
   - Use: `GetHostsPagedSpecification`
   - No special includes needed

4. **AppointmentService**
   - Use: `GetAppointmentsPagedSpecification`
   - Includes: Host

5. **EmployeeService**
   - Create specification file if needed
   - Check Entities to see what relations exist

6. **RoleService**
   - Create specification file if needed

7. **PermissionService**
   - Create specification file if needed

---

### Benefits of This Pattern

✅ **Scalable** - Easy to add new specifications
✅ **Reusable** - Create once, use everywhere
✅ **Maintainable** - Centralized query logic
✅ **InMemory Compatible** - Works with both implementations
✅ **Clean Separation** - Layer separation maintained
✅ **Performance** - Explicit include statements prevent N+1 queries

---

### File Structure Created

```
VMS.Infrastructure/Repositories/
├── Specifications/
│   ├── Specification.cs (Base class)
│   ├── Users/
│   │   └── UserSpecification.cs
│   ├── Visitors/
│   │   └── VisitorSpecification.cs
│   ├── Tokens/
│   │   └── TokenSpecification.cs
│   ├── Hosts/
│   │   └── HostSpecification.cs
│   └── Appointments/
│       └── AppointmentSpecification.cs
├── Interfaces/
│   └── IRepository.cs (Updated)
├── PostgreSQL/
│   └── PostgreSqlRepository.cs (Updated)
└── InMemory/
    └── InMemoryRepository.cs (Updated)
```

---

### Example: Updating VisitorService

```csharp
using AutoMapper;
using VMS.Application.DTOs;
using VMS.Application.DTOs.Common;
using VMS.Application.Interfaces;
using VMS.Domain.Entities;
using VMS.Infrastructure.Repositories.Interfaces;
using VMS.Infrastructure.Repositories.Specifications.Visitors;

namespace VMS.Application.Services;

public class VisitorService : IVisitorService
{
    private readonly IRepository<Visitor> _repository;
    private readonly IMapper _mapper;

    public VisitorService(IRepository<Visitor> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    // ✅ Uses specification with Host and Tokens includes
    public async Task<PagedResult<VisitorDto>> GetAllAsync(PaginationParams pagination)
    {
        var spec = new GetVisitorsPagedSpecification(pagination.Page, pagination.PageSize, pagination.Search);
        var items = await _repository.GetBySpecificationAsync(spec);
        var count = await _repository.GetCountAsync(pagination.Search);
        
        return new PagedResult<VisitorDto>
        {
            Items = _mapper.Map<IEnumerable<VisitorDto>>(items),
            TotalCount = count,
            Page = pagination.Page,
            PageSize = pagination.PageSize
        };
    }

    // ✅ Uses specification with includes
    public async Task<VisitorDto?> GetByIdAsync(Guid id)
    {
        var spec = new GetVisitorByIdSpecification(id);
        var entity = await _repository.GetByIdWithSpecificationAsync(id, spec);
        return entity == null ? null : _mapper.Map<VisitorDto>(entity);
    }

    // ❌ No specification - direct calls
    public async Task<VisitorDto> CreateAsync(CreateVisitorDto dto)
    {
        var entity = _mapper.Map<Visitor>(dto);
        var created = await _repository.AddAsync(entity);
        return _mapper.Map<VisitorDto>(created);
    }

    public async Task<VisitorDto?> UpdateAsync(Guid id, UpdateVisitorDto dto)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return null;

        _mapper.Map(dto, existing);
        var updated = await _repository.UpdateAsync(existing);
        return _mapper.Map<VisitorDto>(updated);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return false;
        await _repository.DeleteAsync(id);
        return true;
    }
}
```

---

### Next Steps

1. ✅ Specification classes are created
2. ✅ Repository interfaces and implementations updated
3. ✅ UserService updated
4. ⏳ Update remaining services (VisitorService, TokenService, etc.)

Apply the pattern template above to each service for consistency.

