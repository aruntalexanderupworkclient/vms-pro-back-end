using AutoMapper;
using VMS.Application.DTOs;
using VMS.Application.DTOs.Common;
using VMS.Application.Interfaces;
using VMS.Domain.Entities;
using VMS.Domain.Enums;
using VMS.Infrastructure.Repositories.Interfaces;

namespace VMS.Application.Services;

public class ReportService : IReportService
{
    private readonly IRepository<Visitor> _visitorRepository;
    private readonly IRepository<VisitorToken> _tokenRepository;
    private readonly IRepository<Appointment> _appointmentRepository;
    private readonly IRepository<Employee> _employeeRepository;
    private readonly IRepository<Host> _hostRepository;
    private readonly IMapper _mapper;

    public ReportService(
        IRepository<Visitor> visitorRepository,
        IRepository<VisitorToken> tokenRepository,
        IRepository<Appointment> appointmentRepository,
        IRepository<Employee> employeeRepository,
        IRepository<Host> hostRepository,
        IMapper mapper)
    {
        _visitorRepository = visitorRepository;
        _tokenRepository = tokenRepository;
        _appointmentRepository = appointmentRepository;
        _employeeRepository = employeeRepository;
        _hostRepository = hostRepository;
        _mapper = mapper;
    }

    public async Task<object> GetDashboardSummaryAsync()
    {
        var allVisitors = await _visitorRepository.GetAllAsync();
        var allTokens = await _tokenRepository.GetAllAsync();
        var allAppointments = await _appointmentRepository.GetAllAsync();
        var allEmployees = await _employeeRepository.GetAllAsync();
        var allHosts = await _hostRepository.GetAllAsync();

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

    public async Task<PagedResult<VisitorDto>> GetVisitorReportAsync(PaginationParams pagination, DateTime? from, DateTime? to)
    {
        System.Linq.Expressions.Expression<Func<Visitor, bool>>? filter = null;

        if (from.HasValue || to.HasValue)
        {
            filter = v =>
                (!from.HasValue || v.CreatedAt >= from.Value) &&
                (!to.HasValue || v.CreatedAt <= to.Value);
        }

        var items = await _visitorRepository.GetPagedAsync(pagination.Page, pagination.PageSize, pagination.Search, filter);
        var count = await _visitorRepository.GetCountAsync(pagination.Search, filter);

        return new PagedResult<VisitorDto>
        {
            Items = _mapper.Map<IEnumerable<VisitorDto>>(items),
            TotalCount = count,
            Page = pagination.Page,
            PageSize = pagination.PageSize
        };
    }
}
