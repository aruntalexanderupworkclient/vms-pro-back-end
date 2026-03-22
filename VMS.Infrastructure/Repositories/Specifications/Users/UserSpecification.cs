using VMS.Domain.Entities;
using VMS.Infrastructure.Repositories.Specifications;

namespace VMS.Infrastructure.Repositories.Specifications.Users;

/// <summary>
/// Specification for getting all users with related entities
/// </summary>
public class GetAllUsersSpecification : Specification<User>
{
    public GetAllUsersSpecification()
    {
        Includes.Add("Role");
        Includes.Add("Organisation");
        OrderByDescending = u => u.CreatedAt;
    }
}

/// <summary>
/// Specification for getting paginated users with related entities
/// </summary>
public class GetUsersPagedSpecification : Specification<User>
{
    public GetUsersPagedSpecification(int page, int pageSize, string? search = null)
    {
        Includes.Add("Role");
        Includes.Add("Organisation");

        if (!string.IsNullOrEmpty(search))
            Criteria = u => u.FullName.Contains(search) || u.Email.Contains(search);

        OrderByDescending = u => u.CreatedAt;
        Skip = (page - 1) * pageSize;
        Take = pageSize;
        IsPagingEnabled = true;
    }
}

/// <summary>
/// Specification for getting a single user by ID with related entities
/// </summary>
public class GetUserByIdSpecification : Specification<User>
{
    public GetUserByIdSpecification(Guid id)
    {
        Includes.Add("Role");
        Includes.Add("Organisation");
        Criteria = u => u.Id == id;
    }
}

/// <summary>
/// Specification for finding users by email with related entities
/// </summary>
public class FindUserSpecification : Specification<User>
{
    public FindUserSpecification(string email)
    {
        Includes.Add("Role");
        Includes.Add("Organisation");
        Criteria = u => u.Email.ToLower() == email.ToLower();
    }
}

