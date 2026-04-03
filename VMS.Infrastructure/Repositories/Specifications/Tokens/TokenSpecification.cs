using VMS.Domain.Entities;
using VMS.Infrastructure.Repositories.Specifications;

namespace VMS.Infrastructure.Repositories.Specifications.Tokens;

/// <summary>
/// Specification for getting all visitor tokens with related entities
/// </summary>
public class GetAllTokensSpecification : Specification<VisitorToken>
{
    public GetAllTokensSpecification()
    {
        Includes.Add("Visitor");
        Includes.Add("Visitor.Host");
        Includes.Add("TokenType");
        Includes.Add("Status");
        OrderByDescending = t => t.CreatedAt;
    }
}

/// <summary>
/// Specification for getting paginated tokens with related entities
/// </summary>
public class GetTokensPagedSpecification : Specification<VisitorToken>
{
    public GetTokensPagedSpecification(int page, int pageSize, string? search = null)
    {
        Includes.Add("Visitor");
        Includes.Add("Visitor.Host");
        Includes.Add("TokenType");
        Includes.Add("Status");

        if (!string.IsNullOrEmpty(search))
            Criteria = t => t.TokenNumber.Contains(search);

        OrderByDescending = t => t.CreatedAt;
        Skip = (page - 1) * pageSize;
        Take = pageSize;
        IsPagingEnabled = true;
    }
}

/// <summary>
/// Specification for getting a single token by ID with related entities
/// </summary>
public class GetTokenByIdSpecification : Specification<VisitorToken>
{
    public GetTokenByIdSpecification(Guid id)
    {
        Includes.Add("Visitor");
        Includes.Add("Visitor.Host");
        Includes.Add("TokenType");
        Includes.Add("Status");
        Criteria = t => t.Id == id;
    }
}
