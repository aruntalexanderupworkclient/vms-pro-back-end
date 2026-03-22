using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VMS.Application.DTOs;
using VMS.Application.DTOs.Common;
using VMS.Application.Interfaces;

namespace VMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _service;

    public UsersController(IUserService service)
    {
        _service = service;
    }

    [HttpGet("GetAll")]
    public async Task<ActionResult<ApiResponse<PagedResult<UserDto>>>> GetAll([FromQuery] PaginationParams pagination)
    {
        var result = await _service.GetAllAsync(pagination);
        return Ok(ApiResponse<PagedResult<UserDto>>.SuccessResponse(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null)
            return NotFound(ApiResponse<UserDto>.FailResponse("User not found."));
        return Ok(ApiResponse<UserDto>.SuccessResponse(result));
    }

    [HttpPost("Create")]
    //[Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<UserDto>>> Create([FromBody] CreateUserDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id },
            ApiResponse<UserDto>.SuccessResponse(result, "User created."));
    }

    [HttpPut("Update")]
    //[Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<UserDto>>> Update([FromQuery] Guid id, [FromBody] UpdateUserDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        if (result == null)
            return NotFound(ApiResponse<UserDto>.FailResponse("User not found."));
        return Ok(ApiResponse<UserDto>.SuccessResponse(result, "User updated."));
    }

    [HttpDelete("Delete")]
    // [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> Delete([FromQuery] Guid id)
    {
        var success = await _service.DeleteAsync(id);
        if (!success)
            return NotFound(ApiResponse<object>.FailResponse("User not found."));
        return Ok(ApiResponse<object>.SuccessResponse(null!, "User deleted."));
    }
}