using VMS.Domain.Entities;
using VMS.Infrastructure.Repositories.Specifications;

namespace VMS.Infrastructure.Repositories.Specifications.Visitors;

/// <summary>
/// Specification for getting all visitors with related entities
/// </summary>
public class GetAllVisitorsSpecification : Specification<Visitor>
{
    public GetAllVisitorsSpecification()
    {
        Includes.Add("Host");
        Includes.Add("Tokens");
        Includes.Add("Status");
        Includes.Add("OrgType");
        OrderByDescending = v => v.CreatedAt;
    }
}

/// <summary>
/// Specification for getting paginated visitors with related entities
/// </summary>
public class GetVisitorsPagedSpecification : Specification<Visitor>
{
    public GetVisitorsPagedSpecification(int page, int pageSize, string? search = null)
    {
        Includes.Add("Host");
        Includes.Add("Tokens");
        Includes.Add("Status");
        Includes.Add("OrgType");

        if (!string.IsNullOrEmpty(search))
            Criteria = v => v.FullName.Contains(search);

        OrderByDescending = v => v.CreatedAt;
        Skip = (page - 1) * pageSize;
        Take = pageSize;
        IsPagingEnabled = true;
    }
}

/// <summary>
/// Specification for getting a single visitor by ID with related entities
/// </summary>
public class GetVisitorByIdSpecification : Specification<Visitor>
{
    public GetVisitorByIdSpecification(Guid id)
    {
        Includes.Add("Host");
        Includes.Add("Tokens");
        Includes.Add("Status");
        Includes.Add("OrgType");
        Criteria = v => v.Id == id;
    }
}
