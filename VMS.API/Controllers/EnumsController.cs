using Microsoft.AspNetCore.Mvc;
using VMS.Application.DTOs.Common;
using VMS.Application.Utilities;

namespace VMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnumsController : ControllerBase
{
    /// <summary>
    /// Get all OrganisationType enum options for dropdowns
    /// </summary>
    [HttpGet("OrganisationTypes")]
    public ActionResult<ApiResponse<List<EnumOptionDto>>> GetOrganisationTypes()
    {
        var options = EnumHelper.GetOrganisationTypeOptions()
            .Select(x => new EnumOptionDto { Id = x.Id, Value = x.Value, Label = x.Label })
            .ToList();
        return Ok(ApiResponse<List<EnumOptionDto>>.SuccessResponse(options, "Organisation types retrieved successfully"));
    }

    /// <summary>
    /// Get all UserStatus enum options for dropdowns
    /// </summary>
    [HttpGet("UserStatuses")]
    public ActionResult<ApiResponse<List<EnumOptionDto>>> GetUserStatuses()
    {
        var options = EnumHelper.GetUserStatusOptions()
            .Select(x => new EnumOptionDto { Id = x.Id, Value = x.Value, Label = x.Label })
            .ToList();
        return Ok(ApiResponse<List<EnumOptionDto>>.SuccessResponse(options, "User statuses retrieved successfully"));
    }

    /// <summary>
    /// Get all TokenType enum options for dropdowns
    /// </summary>
    [HttpGet("TokenTypes")]
    public ActionResult<ApiResponse<List<EnumOptionDto>>> GetTokenTypes()
    {
        var options = EnumHelper.GetTokenTypeOptions()
            .Select(x => new EnumOptionDto { Id = x.Id, Value = x.Value, Label = x.Label })
            .ToList();
        return Ok(ApiResponse<List<EnumOptionDto>>.SuccessResponse(options, "Token types retrieved successfully"));
    }

    /// <summary>
    /// Get all VisitStatus enum options for dropdowns
    /// </summary>
    [HttpGet("VisitStatuses")]
    public ActionResult<ApiResponse<List<EnumOptionDto>>> GetVisitStatuses()
    {
        var options = EnumHelper.GetVisitStatusOptions()
            .Select(x => new EnumOptionDto { Id = x.Id, Value = x.Value, Label = x.Label })
            .ToList();
        return Ok(ApiResponse<List<EnumOptionDto>>.SuccessResponse(options, "Visit statuses retrieved successfully"));
    }

    /// <summary>
    /// Get all enum options grouped by type
    /// </summary>
    [HttpGet("All")]
    public ActionResult<ApiResponse<Dictionary<string, List<EnumOptionDto>>>> GetAllEnums()
    {
        var allEnums = new Dictionary<string, List<EnumOptionDto>>
        {
            {
                "OrganisationTypes", EnumHelper.GetOrganisationTypeOptions()
                    .Select(x => new EnumOptionDto { Id = x.Id, Value = x.Value, Label = x.Label })
                    .ToList()
            },
            {
                "UserStatuses", EnumHelper.GetUserStatusOptions()
                    .Select(x => new EnumOptionDto { Id = x.Id, Value = x.Value, Label = x.Label })
                    .ToList()
            },
            {
                "TokenTypes", EnumHelper.GetTokenTypeOptions()
                    .Select(x => new EnumOptionDto { Id = x.Id, Value = x.Value, Label = x.Label })
                    .ToList()
            },
            {
                "VisitStatuses", EnumHelper.GetVisitStatusOptions()
                    .Select(x => new EnumOptionDto { Id = x.Id, Value = x.Value, Label = x.Label })
                    .ToList()
            }
        };

        return Ok(ApiResponse<Dictionary<string, List<EnumOptionDto>>>.SuccessResponse(allEnums, "All enums retrieved successfully"));
    }
}

