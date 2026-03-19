using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VMS.Application.DTOs;
using VMS.Application.DTOs.Common;
using VMS.Application.Interfaces;

namespace VMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
//[Authorize]
public class PermissionsController : ControllerBase
{
    private readonly IPermissionService _service;

    public PermissionsController(IPermissionService service)
    {
        _service = service;
    }

    [HttpGet("GetAll")]
    public async Task<ActionResult<ApiResponse<PagedResult<PermissionDto>>>> GetAll([FromQuery] PaginationParams pagination)
    {
        var result = await _service.GetAllAsync(pagination);
        return Ok(ApiResponse<PagedResult<PermissionDto>>.SuccessResponse(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<PermissionDto>>> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null)
            return NotFound(ApiResponse<PermissionDto>.FailResponse("Permission not found."));
        return Ok(ApiResponse<PermissionDto>.SuccessResponse(result));
    }

    [HttpGet("role/{roleId:guid}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<PermissionDto>>>> GetByRoleId(Guid roleId)
    {
        var result = await _service.GetByRoleIdAsync(roleId);
        return Ok(ApiResponse<IEnumerable<PermissionDto>>.SuccessResponse(result));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<PermissionDto>>> Create([FromBody] CreatePermissionDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<PermissionDto>.SuccessResponse(result, "Permission created."));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<PermissionDto>>> Update(Guid id, [FromBody] UpdatePermissionDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        if (result == null)
            return NotFound(ApiResponse<PermissionDto>.FailResponse("Permission not found."));
        return Ok(ApiResponse<PermissionDto>.SuccessResponse(result, "Permission updated."));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id)
    {
        var success = await _service.DeleteAsync(id);
        if (!success)
            return NotFound(ApiResponse<object>.FailResponse("Permission not found."));
        return Ok(ApiResponse<object>.SuccessResponse(null!, "Permission deleted."));
    }
}
