using VMS.Domain.Entities;
using VMS.Infrastructure.Repositories.Specifications;

namespace VMS.Infrastructure.Repositories.Specifications.Hosts;

/// <summary>
/// Specification for getting all hosts with related entities
/// </summary>
public class GetAllHostsSpecification : Specification<Host>
{
    public GetAllHostsSpecification()
    {
        Includes.Add("Visitors");
        Includes.Add("Appointments");
        OrderByDescending = h => h.CreatedAt;
    }
}

/// <summary>
/// Specification for getting paginated hosts with related entities
/// </summary>
public class GetHostsPagedSpecification : Specification<Host>
{
    public GetHostsPagedSpecification(int page, int pageSize, string? search = null)
    {
        Includes.Add("Visitors");
        Includes.Add("Appointments");

        if (!string.IsNullOrEmpty(search))
            Criteria = h => h.Name.Contains(search);

        OrderByDescending = h => h.CreatedAt;
        Skip = (page - 1) * pageSize;
        Take = pageSize;
        IsPagingEnabled = true;
    }
}

/// <summary>
/// Specification for getting a single host by ID with related entities
/// </summary>
public class GetHostByIdSpecification : Specification<Host>
{
    public GetHostByIdSpecification(Guid id)
    {
        Includes.Add("Visitors");
        Includes.Add("Appointments");
        Criteria = h => h.Id == id;
    }
}

