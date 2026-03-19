using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VMS.Application.DTOs;
using VMS.Application.DTOs.Common;
using VMS.Application.Interfaces;

namespace VMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly IReportService _service;

    public ReportsController(IReportService service)
    {
        _service = service;
    }

    [HttpGet("dashboard")]
    public async Task<ActionResult<ApiResponse<object>>> GetDashboard()
    {
        var result = await _service.GetDashboardSummaryAsync();
        return Ok(ApiResponse<object>.SuccessResponse(result));
    }

    [HttpGet("visitors")]
    public async Task<ActionResult<ApiResponse<PagedResult<VisitorDto>>>> GetVisitorReport(
        [FromQuery] PaginationParams pagination,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to)
    {
        var result = await _service.GetVisitorReportAsync(pagination, from, to);
        return Ok(ApiResponse<PagedResult<VisitorDto>>.SuccessResponse(result));
    }
}
