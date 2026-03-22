using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VMS.Application.DTOs;
using VMS.Application.DTOs.Common;
using VMS.Application.Interfaces;

namespace VMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class HostsController : ControllerBase
{
    private readonly IHostService _service;

    public HostsController(IHostService service)
    {
        _service = service;
    }

    [HttpGet("GetAll")]
    public async Task<ActionResult<ApiResponse<PagedResult<HostDto>>>> GetAll([FromQuery] PaginationParams pagination)
    {
        var result = await _service.GetAllAsync(pagination);
        return Ok(ApiResponse<PagedResult<HostDto>>.SuccessResponse(result));
    }

    [HttpGet("GetById")]
    public async Task<ActionResult<ApiResponse<HostDto>>> GetById([FromQuery] Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null)
            return NotFound(ApiResponse<HostDto>.FailResponse("Host not found."));
        return Ok(ApiResponse<HostDto>.SuccessResponse(result));
    }

    [HttpPost("Create")]
    // [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<HostDto>>> Create([FromBody] CreateHostDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id },
            ApiResponse<HostDto>.SuccessResponse(result, "Host created."));
    }

    [HttpPut("Update")]
    // [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<HostDto>>> Update([FromQuery] Guid id, [FromBody] UpdateHostDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        if (result == null)
            return NotFound(ApiResponse<HostDto>.FailResponse("Host not found."));
        return Ok(ApiResponse<HostDto>.SuccessResponse(result, "Host updated."));
    }

    [HttpDelete("Delete")]
    // [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> Delete([FromQuery] Guid id)
    {
        var success = await _service.DeleteAsync(id);
        if (!success)
            return NotFound(ApiResponse<object>.FailResponse("Host not found."));
        return Ok(ApiResponse<object>.SuccessResponse(null!, "Host deleted."));
    }
}