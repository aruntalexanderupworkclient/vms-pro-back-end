using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VMS.Application.DTOs;
using VMS.Application.DTOs.Common;
using VMS.Application.Interfaces;

namespace VMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TokensController : ControllerBase
{
    private readonly ITokenService _service;

    public TokensController(ITokenService service)
    {
        _service = service;
    }

    [HttpGet("GetAll")]
    public async Task<ActionResult<ApiResponse<PagedResult<TokenDto>>>> GetAll([FromQuery] PaginationParams pagination)
    {
        var result = await _service.GetAllAsync(pagination);
        return Ok(ApiResponse<PagedResult<TokenDto>>.SuccessResponse(result));
    }

    [HttpGet("GetById")]
    public async Task<ActionResult<ApiResponse<TokenDto>>> GetById([FromQuery] Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null)
            return NotFound(ApiResponse<TokenDto>.FailResponse("Token not found."));
        return Ok(ApiResponse<TokenDto>.SuccessResponse(result));
    }

    [HttpPost("Create")]
    // [Authorize(Roles = "Admin,Manager,Receptionist")]
    public async Task<ActionResult<ApiResponse<TokenDto>>> Create([FromBody] CreateTokenDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<TokenDto>.SuccessResponse(result, "Token created."));
    }

    [HttpPut("Update")]
    // [Authorize(Roles = "Admin,Manager,Receptionist")]
    public async Task<ActionResult<ApiResponse<TokenDto>>> Update([FromQuery] Guid id, [FromBody] UpdateTokenDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        if (result == null)
            return NotFound(ApiResponse<TokenDto>.FailResponse("Token not found."));
        return Ok(ApiResponse<TokenDto>.SuccessResponse(result, "Token updated."));
    }

    [HttpDelete("Delete")]
    // [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<object>>> Delete([FromQuery] Guid id)
    {
        var success = await _service.DeleteAsync(id);
        if (!success)
            return NotFound(ApiResponse<object>.FailResponse("Token not found."));
        return Ok(ApiResponse<object>.SuccessResponse(null!, "Token deleted."));
    }
}
