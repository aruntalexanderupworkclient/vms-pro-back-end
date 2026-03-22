# Enum Implementation - Quick Reference Guide

## 📌 Quick Links

- **Enum Helper**: `VMS.Application/Utilities/EnumHelper.cs`
- **Enum DTOs**: `VMS.Application/DTOs/Common/EnumOptionDto.cs`
- **API Endpoint**: `VMS.API/Controllers/EnumsController.cs`
- **Mappings**: `VMS.Application/Mappings/MappingProfile.cs`
- **Full Documentation**: `ENUM_IMPLEMENTATION_COMPLETE.md`

---

## 🚀 Quick Start for Developers

### **Call Enum Endpoint from Frontend**

```javascript
// Get specific enum type
fetch('/api/enums/UserStatuses')
  .then(r => r.json())
  .then(data => console.log(data.data)); // Array of {id, value, label}

// Or get ALL enums at once
fetch('/api/enums/All')
  .then(r => r.json())
  .then(data => {
    const { UserStatuses, OrganisationTypes, TokenTypes, VisitStatuses } = data.data;
    // Use for dropdowns
  });
```

### **Use Enum Metadata in Response**

```json
// User API Response Example
{
  "id": "guid",
  "fullName": "John Doe",
  "status": "Active",               // ← String value (backward compatible)
  "statusId": 0,                    // ← Numeric ID (new)
  "statusLabel": "User is Active"   // ← Display label (new)
}
```

### **Display in React Component**

```jsx
function UserStatus({ statusId, statusLabel }) {
  return <span className="badge">{statusLabel}</span>;
  // Displays: "User is Active"
}
```

---

## 🔍 Current Enum Mappings

### **UserStatus** → `UserStatus` enum
| ID | Value | Label |
|----|-------|-------|
| 0 | Active | User is Active |
| 1 | Inactive | User is Inactive |
| 2 | Suspended | User is Suspended |
| 3 | Deleted | User is Deleted |

### **OrganisationType** → `OrganisationType` enum
| ID | Value | Label |
|----|-------|-------|
| 0 | Hospital | Hospital Building |
| 1 | Residential | Residential Complex |
| 2 | Corporate | Corporate Office |
| 3 | Factory | Manufacturing Facility |

### **TokenType** → `TokenType` enum
| ID | Value | Label |
|----|-------|-------|
| 0 | Visitor | Visitor Pass |
| 1 | Contractor | Contractor Pass |
| 2 | Delivery | Delivery Person |
| 3 | Temporary | Temporary Pass |
| 4 | VIP | VIP Pass |

### **VisitStatus** → `VisitStatus` enum
| ID | Value | Label |
|----|-------|-------|
| 0 | Scheduled | Visit is Scheduled |
| 1 | CheckedIn | Visitor Checked In |
| 2 | CheckedOut | Visitor Checked Out |
| 3 | Cancelled | Visit Cancelled |
| 4 | Expired | Visit Expired |

---

## 🛠️ How to Add a New Enum

### **Step 1**: Define Enum in `VMS.Domain/Enums/`
```csharp
public enum MyStatus
{
    Pending = 0,
    Active = 1,
    Completed = 2
}
```

### **Step 2**: Add to `EnumHelper.cs`
```csharp
private static readonly Dictionary<int, (string value, string label)> MyStatusMap = new()
{
    { 0, ("Pending", "Request is Pending") },
    { 1, ("Active", "Status is Active") },
    { 2, ("Completed", "Work Completed") }
};

public static List<EnumOption> GetMyStatusOptions()
    => ConvertToOptions(MyStatusMap);

public static (string value, string label) GetMyStatusInfo(MyStatus status)
    => MyStatusMap.TryGetValue((int)status, out var info) 
        ? info 
        : (status.ToString(), status.ToString());
```

### **Step 3**: Add DTO Field
```csharp
public class MyEntityDto
{
    public MyStatus Status { get; set; }
    // New fields:
    public int StatusId { get; set; }
    public string StatusLabel { get; set; } = string.Empty;
}
```

### **Step 4**: Add AutoMapper Mapping
```csharp
CreateMap<MyEntity, MyEntityDto>()
    .ForMember(d => d.StatusId, opt => opt.MapFrom(s => (int)s.Status))
    .ForMember(d => d.StatusLabel, opt => opt.MapFrom(s => 
        EnumHelper.GetMyStatusInfo(s.Status).label));
```

### **Step 5**: Add Endpoint to `EnumsController.cs`
```csharp
[HttpGet("MyStatuses")]
public ActionResult<ApiResponse<List<EnumOptionDto>>> GetMyStatuses()
{
    var options = EnumHelper.GetMyStatusOptions()
        .Select(x => new EnumOptionDto { Id = x.Id, Value = x.Value, Label = x.Label })
        .ToList();
    return Ok(ApiResponse<List<EnumOptionDto>>.SuccessResponse(options));
}
```

---

## 📡 API Response Examples

### **Single Entity (e.g., Get User by ID)**
```json
GET /api/users/GetById?id=550e8400-e29b-41d4-a716-446655440101

{
  "statusCode": 200,
  "data": {
    "id": "550e8400-e29b-41d4-a716-446655440101",
    "fullName": "John Doe",
    "email": "john@example.com",
    "status": "Active",
    "statusId": 0,
    "statusLabel": "User is Active",
    "createdAt": "2024-01-15T10:30:00Z"
  },
  "message": "User retrieved successfully"
}
```

### **Enum Options List**
```json
GET /api/enums/UserStatuses

{
  "statusCode": 200,
  "data": [
    {
      "id": 0,
      "value": "Active",
      "label": "User is Active"
    },
    {
      "id": 1,
      "value": "Inactive",
      "label": "User is Inactive"
    },
    {
      "id": 2,
      "value": "Suspended",
      "label": "User is Suspended"
    },
    {
      "id": 3,
      "value": "Deleted",
      "label": "User is Deleted"
    }
  ],
  "message": "User statuses retrieved successfully"
}
```

### **All Enums (Grouped)**
```json
GET /api/enums/All

{
  "statusCode": 200,
  "data": {
    "OrganisationTypes": [
      { "id": 0, "value": "Hospital", "label": "Hospital Building" },
      ...
    ],
    "UserStatuses": [
      { "id": 0, "value": "Active", "label": "User is Active" },
      ...
    ],
    "TokenTypes": [
      { "id": 0, "value": "Visitor", "label": "Visitor Pass" },
      ...
    ],
    "VisitStatuses": [
      { "id": 0, "value": "Scheduled", "label": "Visit is Scheduled" },
      ...
    ]
  },
  "message": "All enums retrieved successfully"
}
```

---

## 💾 Database Impact

✅ **NO DATABASE CHANGES REQUIRED**
- All metadata stored in `EnumHelper.cs`
- Enum values stored as integers (unchanged)
- No migrations needed
- No schema modifications

---

## 🔄 Backward Compatibility

✅ **Fully Backward Compatible**
- Existing `Status` field still present and works
- New `StatusId` and `StatusLabel` fields added
- Old API clients work without modification
- New clients can use label fields for better UX

---

## ⚡ Performance Notes

- **Metadata access**: O(1) Dictionary lookup
- **Zero database queries** for enum options
- **In-memory operations** - Very fast
- **No N+1 problems** - All enum data cached

---

## 🐛 Troubleshooting

### **Problem**: Enum label not showing
**Solution**: Check `EnumHelper.cs` mapping exists for that enum value

### **Problem**: API returns 404 for enum endpoint
**Solution**: Make sure `EnumsController.cs` is in correct namespace and endpoint route is correct

### **Problem**: DTO field not populated
**Solution**: Verify AutoMapper mapping includes the new field in `MappingProfile.cs`

---

## 📝 Code Snippets

### **Get All Enums (JavaScript)**
```javascript
const getAllEnums = async () => {
  const response = await fetch('http://localhost:5000/api/enums/All');
  return (await response.json()).data;
};

const enums = await getAllEnums();
const userStatuses = enums.UserStatuses; // Use for dropdowns
```

### **Create User with Status (TypeScript)**
```typescript
interface UserStatus {
  id: number;
  value: string;
  label: string;
}

interface CreateUserRequest {
  fullName: string;
  email: string;
  statusId: number;
}

const createUser = async (data: CreateUserRequest) => {
  const response = await fetch('/api/users/Create', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(data)
  });
  return response.json();
};

// Usage
createUser({
  fullName: 'John Doe',
  email: 'john@example.com',
  statusId: 0 // Active
});
```

### **React Hook for Enums**
```typescript
import { useEffect, useState } from 'react';

const useEnums = () => {
  const [enums, setEnums] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetch('/api/enums/All')
      .then(r => r.json())
      .then(data => {
        setEnums(data.data);
        setLoading(false);
      });
  }, []);

  return { enums, loading };
};

// Usage in component
const { enums, loading } = useEnums();
if (!loading) {
  enums.UserStatuses.map(status => (
    <option key={status.id} value={status.id}>
      {status.label}
    </option>
  ))
}
```

---

## 🎯 Testing

### **Test Enum Endpoint**
```bash
curl http://localhost:5000/api/enums/UserStatuses
curl http://localhost:5000/api/enums/All
```

### **Test Entity with Enum Metadata**
```bash
curl http://localhost:5000/api/users/GetById?id=<user-guid>
# Should see statusId and statusLabel in response
```

---

## 📚 References

- Full Implementation: `ENUM_IMPLEMENTATION_COMPLETE.md`
- Enum Helper: `VMS.Application/Utilities/EnumHelper.cs`
- Enum Controller: `VMS.API/Controllers/EnumsController.cs`
- Mapping Config: `VMS.Application/Mappings/MappingProfile.cs`

---

## ✅ Summary

| Aspect | Status |
|--------|--------|
| Enum String Values | ✅ Implemented |
| Display Labels | ✅ Implemented |
| API Endpoints | ✅ Implemented |
| DTO Updates | ✅ Complete |
| AutoMapper Mappings | ✅ Updated |
| Frontend Integration | ✅ Ready |
| Database Changes | ✅ None Required |
| Backward Compatibility | ✅ Maintained |
| Documentation | ✅ Complete |

---

**Need help?** Check `ENUM_IMPLEMENTATION_COMPLETE.md` for detailed information!

