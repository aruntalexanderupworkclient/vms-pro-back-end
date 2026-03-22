using VMS.Domain.Entities;
using VMS.Infrastructure.Repositories.Specifications;

namespace VMS.Infrastructure.Repositories.Specifications.Appointments;

/// <summary>
/// Specification for getting all appointments with related entities
/// </summary>
public class GetAllAppointmentsSpecification : Specification<Appointment>
{
    public GetAllAppointmentsSpecification()
    {
        Includes.Add("Host");
        OrderByDescending = a => a.CreatedAt;
    }
}

/// <summary>
/// Specification for getting paginated appointments with related entities
/// </summary>
public class GetAppointmentsPagedSpecification : Specification<Appointment>
{
    public GetAppointmentsPagedSpecification(int page, int pageSize, string? search = null)
    {
        Includes.Add("Host");

        if (!string.IsNullOrEmpty(search))
            Criteria = a => a.VisitorName.Contains(search);

        OrderByDescending = a => a.CreatedAt;
        Skip = (page - 1) * pageSize;
        Take = pageSize;
        IsPagingEnabled = true;
    }
}

/// <summary>
/// Specification for getting a single appointment by ID with related entities
/// </summary>
public class GetAppointmentByIdSpecification : Specification<Appointment>
{
    public GetAppointmentByIdSpecification(Guid id)
    {
        Includes.Add("Host");
        Criteria = a => a.Id == id;
    }
}

