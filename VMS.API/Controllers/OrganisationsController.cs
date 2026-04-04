using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VMS.Application.DTOs;
using VMS.Application.DTOs.Common;
using VMS.Application.Interfaces;

namespace VMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrganisationsController : ControllerBase
{
    private readonly IOrganisationService _service;

    public OrganisationsController(IOrganisationService service)
    {
        _service = service;
    }

    [HttpGet("GetAll")]
    public async Task<ActionResult<ApiResponse<PagedResult<OrganisationDto>>>> GetAll([FromQuery] PaginationParams pagination)
    {
        var result = await _service.GetAllAsync(pagination);
        return Ok(ApiResponse<PagedResult<OrganisationDto>>.SuccessResponse(result));
    }

    [HttpGet("GetById")]
    public async Task<ActionResult<ApiResponse<OrganisationDto>>> GetById([FromQuery] Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null)
            return NotFound(ApiResponse<OrganisationDto>.FailResponse("Organisation not found."));
        return Ok(ApiResponse<OrganisationDto>.SuccessResponse(result));
    }

    [HttpPost("Create")]
    // [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<OrganisationDto>>> Create([FromBody] CreateOrganisationDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<OrganisationDto>.SuccessResponse(result, "Organisation created."));
    }

    [HttpPut("Update")]
    // [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<OrganisationDto>>> Update([FromQuery] Guid id, [FromBody] UpdateOrganisationDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        if (result == null)
            return NotFound(ApiResponse<OrganisationDto>.FailResponse("Organisation not found."));
        return Ok(ApiResponse<OrganisationDto>.SuccessResponse(result, "Organisation updated."));
    }

    [HttpDelete("Delete")]
    // [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id)
    {
        var success = await _service.DeleteAsync(id);
        if (!success)
            return NotFound(ApiResponse<object>.FailResponse("Organisation not found."));
        return Ok(ApiResponse<object>.SuccessResponse(null!, "Organisation deleted."));
    }
}

