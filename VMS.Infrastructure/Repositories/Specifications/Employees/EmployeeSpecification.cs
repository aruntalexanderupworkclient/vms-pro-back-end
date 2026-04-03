using VMS.Domain.Entities;
using VMS.Infrastructure.Repositories.Specifications;

namespace VMS.Infrastructure.Repositories.Specifications.Employees;

public class GetEmployeesPagedSpecification : Specification<Employee>
{
    public GetEmployeesPagedSpecification(int page, int pageSize, string? search = null)
    {

        if (!string.IsNullOrEmpty(search))
            Criteria = e => e.FullName.Contains(search);

        OrderByDescending = e => e.CreatedAt;
        Skip = (page - 1) * pageSize;
        Take = pageSize;
        IsPagingEnabled = true;
    }
}

public class GetEmployeeByIdSpecification : Specification<Employee>
{
    public GetEmployeeByIdSpecification(Guid id)
    {
        Criteria = e => e.Id == id;
    }
}

