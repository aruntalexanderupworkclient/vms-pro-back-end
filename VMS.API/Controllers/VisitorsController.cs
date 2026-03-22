using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VMS.Application.DTOs;
using VMS.Application.DTOs.Common;
using VMS.Application.Interfaces;

namespace VMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
//[AllowAnonymous]
public class VisitorsController : ControllerBase
{
    private readonly IVisitorService _service;

    public VisitorsController(IVisitorService service)
    {
        _service = service;
    }

    [HttpGet("GetAll")]
    public async Task<ActionResult<ApiResponse<PagedResult<VisitorDto>>>> GetAll([FromQuery] PaginationParams pagination)
    {
        var result = await _service.GetAllAsync(pagination);
        return Ok(ApiResponse<PagedResult<VisitorDto>>.SuccessResponse(result));
    }

    [HttpGet("GetById")]
    public async Task<ActionResult<ApiResponse<VisitorDto>>> GetById([FromQuery] Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null)
            return NotFound(ApiResponse<VisitorDto>.FailResponse("Visitor not found."));
        return Ok(ApiResponse<VisitorDto>.SuccessResponse(result));
    }

    [HttpPost("Create")]
    // [Authorize(Roles = "Admin,Manager,Receptionist")]
    public async Task<ActionResult<ApiResponse<VisitorDto>>> Create([FromBody] CreateVisitorDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<VisitorDto>.SuccessResponse(result, "Visitor created."));
    }

    [HttpPut("Update")]
    // [Authorize(Roles = "Admin,Manager,Receptionist")]
    public async Task<ActionResult<ApiResponse<VisitorDto>>> Update([FromQuery] Guid id, [FromBody] UpdateVisitorDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        if (result == null)
            return NotFound(ApiResponse<VisitorDto>.FailResponse("Visitor not found."));
        return Ok(ApiResponse<VisitorDto>.SuccessResponse(result, "Visitor updated."));
    }

    [HttpDelete("Delete")]
    // [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<object>>> Delete([FromQuery] Guid id)
    {
        var success = await _service.DeleteAsync(id);
        if (!success)
            return NotFound(ApiResponse<object>.FailResponse("Visitor not found."));
        return Ok(ApiResponse<object>.SuccessResponse(null!, "Visitor deleted."));
    }
}
