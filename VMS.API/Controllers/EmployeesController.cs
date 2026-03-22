using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VMS.Application.DTOs;
using VMS.Application.DTOs.Common;
using VMS.Application.Interfaces;

namespace VMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _service;

    public EmployeesController(IEmployeeService service)
    {
        _service = service;
    }

    [HttpGet("GetAll")]
    public async Task<ActionResult<ApiResponse<PagedResult<EmployeeDto>>>> GetAll(
        [FromQuery] PaginationParams pagination)
    {
        var result = await _service.GetAllAsync(pagination);
        return Ok(ApiResponse<PagedResult<EmployeeDto>>.SuccessResponse(result));
    }

    [HttpGet("GetById")]
    public async Task<ActionResult<ApiResponse<EmployeeDto>>> GetById([FromQuery] Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null)
            return NotFound(ApiResponse<EmployeeDto>.FailResponse("Employee not found."));
        return Ok(ApiResponse<EmployeeDto>.SuccessResponse(result));
    }

    [HttpPost("Create")]
    // [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<EmployeeDto>>> Create([FromBody] CreateEmployeeDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id },
            ApiResponse<EmployeeDto>.SuccessResponse(result, "Employee created."));
    }

    [HttpPut("Update")]
    // [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<EmployeeDto>>> Update([FromQuery] Guid id,
        [FromBody] UpdateEmployeeDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        if (result == null)
            return NotFound(ApiResponse<EmployeeDto>.FailResponse("Employee not found."));
        return Ok(ApiResponse<EmployeeDto>.SuccessResponse(result, "Employee updated."));
    }

    [HttpDelete("Delete")]
    // [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> Delete([FromQuery] Guid id)
    {
        var success = await _service.DeleteAsync(id);
        if (!success)
            return NotFound(ApiResponse<object>.FailResponse("Employee not found."));
        return Ok(ApiResponse<object>.SuccessResponse(null!, "Employee deleted."));
    }
}