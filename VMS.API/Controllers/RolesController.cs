using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VMS.Application.DTOs;
using VMS.Application.DTOs.Common;
using VMS.Application.Interfaces;

namespace VMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RolesController : ControllerBase
{
    private readonly IRoleService _service;

    public RolesController(IRoleService service)
    {
        _service = service;
    }

    [HttpGet("GetAll")]
    public async Task<ActionResult<ApiResponse<PagedResult<RoleDto>>>> GetAll([FromQuery] PaginationParams pagination)
    {
        var result = await _service.GetAllAsync(pagination);
        return Ok(ApiResponse<PagedResult<RoleDto>>.SuccessResponse(result));
    }

    [HttpGet("GetById")]
    public async Task<ActionResult<ApiResponse<RoleDto>>> GetById([FromQuery] Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null)
            return NotFound(ApiResponse<RoleDto>.FailResponse("Role not found."));
        return Ok(ApiResponse<RoleDto>.SuccessResponse(result));
    }

    [HttpPost("Create")]
    // [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<RoleDto>>> Create([FromBody] CreateRoleDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<RoleDto>.SuccessResponse(result, "Role created."));
    }

    [HttpPut("Update")]
    // [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<RoleDto>>> Update([FromQuery] Guid id, [FromBody] UpdateRoleDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        if (result == null)
            return NotFound(ApiResponse<RoleDto>.FailResponse("Role not found."));
        return Ok(ApiResponse<RoleDto>.SuccessResponse(result, "Role updated."));
    }

    [HttpDelete("Delete")]
    // [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id)
    {
        var success = await _service.DeleteAsync(id);
        if (!success)
            return NotFound(ApiResponse<object>.FailResponse("Role not found."));
        return Ok(ApiResponse<object>.SuccessResponse(null!, "Role deleted."));
    }
}
