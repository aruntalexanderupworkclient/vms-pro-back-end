using Microsoft.AspNetCore.Mvc;
using VMS.Application.DTOs;
using VMS.Application.DTOs.Common;
using VMS.Application.Interfaces;

namespace VMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MdmController : ControllerBase
{
    private readonly IMdmService _mdmService;
    private static readonly HashSet<string> ValidTypes = new() { "VisitStatus", "UserStatus", "TokenType", "OrganisationType" };

    public MdmController(IMdmService mdmService)
    {
        _mdmService = mdmService;
    }

    /// <summary>
    /// Get all MDM entries for a given type
    /// </summary>
    [HttpGet("{type}/GetAll")]
    public async Task<ActionResult<ApiResponse<List<MdmDto>>>> GetAll(string type)
    {
        if (!ValidTypes.Contains(type))
            return BadRequest(ApiResponse<List<MdmDto>>.FailResponse($"Invalid MDM type: {type}. Valid types: {string.Join(", ", ValidTypes)}"));

        var items = await _mdmService.GetAllAsync(type);
        return Ok(ApiResponse<List<MdmDto>>.SuccessResponse(items, $"{type} data retrieved successfully"));
    }

    /// <summary>
    /// Get all MDM types grouped
    /// </summary>
    [HttpGet("All")]
    public async Task<ActionResult<ApiResponse<Dictionary<string, List<MdmDto>>>>> GetAllGrouped()
    {
        var result = new Dictionary<string, List<MdmDto>>();
        foreach (var type in ValidTypes)
        {
            result[type] = await _mdmService.GetAllAsync(type);
        }
        return Ok(ApiResponse<Dictionary<string, List<MdmDto>>>.SuccessResponse(result, "All MDM data retrieved successfully"));
    }

    /// <summary>
    /// Get a single MDM entry by ID
    /// </summary>
    [HttpGet("{type}/GetById")]
    public async Task<ActionResult<ApiResponse<MdmDto>>> GetById(string type, [FromQuery] Guid id)
    {
        if (!ValidTypes.Contains(type))
            return BadRequest(ApiResponse<MdmDto>.FailResponse($"Invalid MDM type: {type}"));

        var item = await _mdmService.GetByIdAsync(type, id);
        if (item == null)
            return NotFound(ApiResponse<MdmDto>.FailResponse($"{type} entry not found"));

        return Ok(ApiResponse<MdmDto>.SuccessResponse(item, $"{type} entry retrieved successfully"));
    }

    /// <summary>
    /// Create a new MDM entry
    /// </summary>
    [HttpPost("{type}/Create")]
    public async Task<ActionResult<ApiResponse<MdmDto>>> Create(string type, [FromBody] CreateMdmDto dto)
    {
        if (!ValidTypes.Contains(type))
            return BadRequest(ApiResponse<MdmDto>.FailResponse($"Invalid MDM type: {type}"));

        var created = await _mdmService.CreateAsync(type, dto);
        return Ok(ApiResponse<MdmDto>.SuccessResponse(created, $"{type} entry created successfully"));
    }

    /// <summary>
    /// Update an existing MDM entry
    /// </summary>
    [HttpPut("{type}/Update")]
    public async Task<ActionResult<ApiResponse<MdmDto>>> Update(string type, [FromQuery] Guid id, [FromBody] UpdateMdmDto dto)
    {
        if (!ValidTypes.Contains(type))
            return BadRequest(ApiResponse<MdmDto>.FailResponse($"Invalid MDM type: {type}"));

        var updated = await _mdmService.UpdateAsync(type, id, dto);
        if (updated == null)
            return NotFound(ApiResponse<MdmDto>.FailResponse($"{type} entry not found"));

        return Ok(ApiResponse<MdmDto>.SuccessResponse(updated, $"{type} entry updated successfully"));
    }

    /// <summary>
    /// Delete an MDM entry (soft delete)
    /// </summary>
    [HttpDelete("{type}/Delete")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(string type, [FromQuery] Guid id)
    {
        if (!ValidTypes.Contains(type))
            return BadRequest(ApiResponse<bool>.FailResponse($"Invalid MDM type: {type}"));

        var result = await _mdmService.DeleteAsync(type, id);
        return Ok(ApiResponse<bool>.SuccessResponse(result, $"{type} entry deleted successfully"));
    }
}

