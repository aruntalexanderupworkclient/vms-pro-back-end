using AutoMapper;
using VMS.Application.DTOs;
using VMS.Application.DTOs.Common;
using VMS.Application.Interfaces;
using VMS.Domain.Entities;
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
            // Resolve MDM IDs first
            var checkedInStatus = (await uow.MdmVisitStatuses.FindAsync(s => s.Code == MdmCodes.VisitStatus.CheckedIn)).FirstOrDefault();
            var checkedOutStatus = (await uow.MdmVisitStatuses.FindAsync(s => s.Code == MdmCodes.VisitStatus.CheckedOut)).FirstOrDefault();
            var scheduledStatus = (await uow.MdmVisitStatuses.FindAsync(s => s.Code == MdmCodes.VisitStatus.Scheduled)).FirstOrDefault();

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
                CheckedInVisitors = visitors.Count(v => checkedInStatus != null && v.StatusId == checkedInStatus.Id),
                CheckedOutVisitors = visitors.Count(v => checkedOutStatus != null && v.StatusId == checkedOutStatus.Id),
                ScheduledVisitors = visitors.Count(v => scheduledStatus != null && v.StatusId == scheduledStatus.Id),
                ActiveTokens = tokens.Count(t => checkedInStatus != null && t.StatusId == checkedInStatus.Id),
                TotalAppointments = appointments.Count,
                UpcomingAppointments = appointments.Count(a => scheduledStatus != null && a.StatusId == scheduledStatus.Id && a.ScheduledAt > DateTime.UtcNow),
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
