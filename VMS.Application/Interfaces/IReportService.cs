using VMS.Application.DTOs;
using VMS.Application.DTOs.Common;

namespace VMS.Application.Interfaces;

public interface IReportService
{
    Task<object> GetDashboardSummaryAsync();
    Task<PagedResult<VisitorDto>> GetVisitorReportAsync(PaginationParams pagination, DateTime? from, DateTime? to);
}
