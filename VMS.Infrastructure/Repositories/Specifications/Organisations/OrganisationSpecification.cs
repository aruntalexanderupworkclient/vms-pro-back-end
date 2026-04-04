using VMS.Domain.Entities;
using VMS.Infrastructure.Repositories.Specifications;

namespace VMS.Infrastructure.Repositories.Specifications.Organisations;

/// <summary>
/// Specification for getting paginated organisations with related entities (Type)
/// </summary>
public class GetOrganisationsPagedSpecification : Specification<Organisation>
{
    public GetOrganisationsPagedSpecification(int page, int pageSize, string? search = null)
    {
        Includes.Add("Type");
        Includes.Add("Users");

        if (!string.IsNullOrEmpty(search))
            Criteria = o => o.Name.Contains(search) || o.Address.Contains(search);

        OrderByDescending = o => o.CreatedAt;
        Skip = (page - 1) * pageSize;
        Take = pageSize;
        IsPagingEnabled = true;
    }
}

/// <summary>
/// Specification for getting a single organisation by ID with related entities (Type)
/// </summary>
public class GetOrganisationByIdSpecification : Specification<Organisation>
{
    public GetOrganisationByIdSpecification(Guid id)
    {
        Includes.Add("Type");
        Criteria = o => o.Id == id;
    }
}

