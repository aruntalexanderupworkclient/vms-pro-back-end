using AutoMapper;
using VMS.Application.DTOs;
using VMS.Application.DTOs.Common;
using VMS.Application.Interfaces;
using VMS.Domain.Entities;
using VMS.Domain.Enums;
using VMS.Infrastructure.Repositories.UnitOfWork;

namespace VMS.Application.Services;

public class ReportService : IReportService
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IMapper _mapper;

    public ReportService(IUnitOfWorkFactory unitOfWorkFactory, IMapper mapper)
    {
        _unitOfWorkFactory = unitOfWorkFactory ?? throw new ArgumentNullException(nameof(unitOfWorkFactory));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<object> GetDashboardSummaryAsync()
    {
        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            var allVisitors = await uow.Visitors.GetAllAsync();
            var allTokens = await uow.Tokens.GetAllAsync();
            var allAppointments = await uow.Appointments.GetAllAsync();
            var allEmployees = await uow.Employees.GetAllAsync();
            var allHosts = await uow.Hosts.GetAllAsync();

            var visitors = allVisitors.ToList();
            var tokens = allTokens.ToList();
            var appointments = allAppointments.ToList();

            return new
            {
                TotalVisitors = visitors.Count,
                CheckedInVisitors = visitors.Count(v => v.Status == VisitStatus.CheckedIn),
                CheckedOutVisitors = visitors.Count(v => v.Status == VisitStatus.CheckedOut),
                ScheduledVisitors = visitors.Count(v => v.Status == VisitStatus.Scheduled),
                ActiveTokens = tokens.Count(t => t.Status == VisitStatus.CheckedIn),
                TotalAppointments = appointments.Count,
                UpcomingAppointments = appointments.Count(a => a.Status == VisitStatus.Scheduled && a.ScheduledAt > DateTime.UtcNow),
                TotalEmployees = allEmployees.Count(),
                TotalHosts = allHosts.Count()
            };
        }
    }

    public async Task<PagedResult<VisitorDto>> GetVisitorReportAsync(PaginationParams pagination, DateTime? from, DateTime? to)
    {
        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            System.Linq.Expressions.Expression<Func<Visitor, bool>>? filter = null;

            if (from.HasValue || to.HasValue)
            {
                filter = v =>
                    (!from.HasValue || v.CreatedAt >= from.Value) &&
                    (!to.HasValue || v.CreatedAt <= to.Value);
            }

            var items = await uow.Visitors.GetPagedAsync(pagination.Page, pagination.PageSize, pagination.Search, filter);
            var count = await uow.Visitors.GetCountAsync(pagination.Search, filter);

            return new PagedResult<VisitorDto>
            {
                Items = _mapper.Map<IEnumerable<VisitorDto>>(items),
                TotalCount = count,
                Page = pagination.Page,
                PageSize = pagination.PageSize
            };
        }
    }
}
