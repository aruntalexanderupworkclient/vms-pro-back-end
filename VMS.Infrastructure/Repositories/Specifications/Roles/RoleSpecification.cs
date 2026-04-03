using VMS.Domain.Entities;
using VMS.Infrastructure.Repositories.Specifications;

namespace VMS.Infrastructure.Repositories.Specifications.Roles;

/// <summary>
/// Specification for getting all roles with related entities
/// </summary>
public class GetAllRolesSpecification : Specification<Role>
{
    public GetAllRolesSpecification()
    {
        Includes.Add("Permissions");
        OrderByDescending = r => r.CreatedAt;
    }
}

/// <summary>
/// Specification for getting paginated roles with related entities (Permissions)
/// </summary>
public class GetRolesPagedSpecification : Specification<Role>
{
    public GetRolesPagedSpecification(int page, int pageSize, string? search = null)
    {
        Includes.Add("Permissions");
        Includes.Add("Users");

        if (!string.IsNullOrEmpty(search))
            Criteria = r => r.Name.Contains(search) || r.Description.Contains(search);

        OrderByDescending = r => r.CreatedAt;
        Skip = (page - 1) * pageSize;
        Take = pageSize;
        IsPagingEnabled = true;
    }
}

/// <summary>
/// Specification for getting a single role by ID with related entities (Permissions)
/// </summary>
public class GetRoleByIdSpecification : Specification<Role>
{
    public GetRoleByIdSpecification(Guid id)
    {
        Includes.Add("Permissions");
        Criteria = r => r.Id == id;
    }
}

