using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VMS.API.Extensions; // ← Add this to use extension methods
using VMS.Application.DTOs;
using VMS.Application.DTOs.Common;
using VMS.Application.Interfaces;

namespace VMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmployeesController_Example : ControllerBase
{
    private readonly IEmployeeService _service;

    public EmployeesController_Example(IEmployeeService service)
    {
        _service = service;
    }

    [HttpGet("GetAll")]
    public async Task<ActionResult<ApiResponse<PagedResult<EmployeeDto>>>> GetAll([FromQuery] PaginationParams pagination)
    {
        // Example: Get current user info for logging/auditing
        var userId = User.GetUserId();
        var userEmail = User.GetUserEmail();
        var userName = User.GetUserName();
        var userRole = User.GetUserRole();
        
        Console.WriteLine($"GetAll called by: {userName} ({userEmail}) - Role: {userRole}");

        var result = await _service.GetAllAsync(pagination);
        return Ok(ApiResponse<PagedResult<EmployeeDto>>.SuccessResponse(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<EmployeeDto>>> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null)
            return NotFound(ApiResponse<EmployeeDto>.FailResponse("Employee not found."));
        return Ok(ApiResponse<EmployeeDto>.SuccessResponse(result));
    }

    [HttpPost("Create")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<EmployeeDto>>> Create([FromBody] CreateEmployeeDto dto)
    {
        // Example 1: Get basic user info
        var userId = User.GetUserId();
        var userRole = User.GetUserRole();
        
        // Example 2: Check if user is admin
        if (User.IsInRole("Admin"))
        {
            // Admin-specific logic
            Console.WriteLine("Admin creating employee");
        }
        else if (User.IsInRole("Manager"))
        {
            // Manager-specific logic
            Console.WriteLine("Manager creating employee");
        }

        // Example 3: Get all user details
        var organizationId = User.GetOrganisationId();
        var userName = User.GetUserName();
        var userEmail = User.GetUserEmail();

        Console.WriteLine($"Employee being created by {userName} in org {organizationId}");

        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), 
            new { id = result.Id }, 
            ApiResponse<EmployeeDto>.SuccessResponse(result, "Employee created."));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<EmployeeDto>>> Update(Guid id, [FromBody] UpdateEmployeeDto dto)
    {
        // Example: Role-based authorization
        if (!User.HasRole("Admin") && !User.HasRole("Manager"))
        {
            return Forbid("You don't have permission to update employees.");
        }

        // Example: Get user info for audit
        var userId = User.GetUserId();
        var userName = User.GetUserName();
        Console.WriteLine($"Employee {id} being updated by {userName} ({userId})");

        var result = await _service.UpdateAsync(id, dto);
        if (result == null)
            return NotFound(ApiResponse<EmployeeDto>.FailResponse("Employee not found."));
        
        return Ok(ApiResponse<EmployeeDto>.SuccessResponse(result, "Employee updated."));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id)
    {
        // Example: Verify admin role
        var userRole = User.GetUserRole();
        if (userRole != "Admin")
        {
            return Forbid($"Only admins can delete. Your role: {userRole}");
        }

        // Example: Check any role from a list
        if (!User.HasAnyRole("Admin", "SuperAdmin"))
        {
            return Forbid("Insufficient permissions");
        }

        // Example: Get all claims
        var allClaims = User.GetAllUserClaims();
        Console.WriteLine($"Deletion initiated. User claims: {string.Join(", ", allClaims)}");

        var success = await _service.DeleteAsync(id);
        if (!success)
            return NotFound(ApiResponse<object>.FailResponse("Employee not found."));
        
        return Ok(ApiResponse<object>.SuccessResponse(null!, "Employee deleted."));
    }

    /// <summary>
    /// Example endpoint showing all available extension methods
    /// </summary>
    [HttpGet("debug/user-info")]
    [Authorize]
    public ActionResult<object> GetCurrentUserInfo()
    {
        return Ok(new
        {
            userId = User.GetUserId(),
            email = User.GetUserEmail(),
            name = User.GetUserName(),
            role = User.GetUserRole(),
            organisationId = User.GetOrganisationId(),
            isAuthenticated = User.Identity?.IsAuthenticated ?? false,
            hasAdminRole = User.IsInRole("Admin"),
            hasManagerRole = User.IsInRole("Manager"),
            allClaims = User.GetAllUserClaims()
        });
    }
}

/// <summary>
/// Example Service showing IUserContext usage
/// This would be in VMS.Application/Services/EmployeeService.cs
/// </summary>
/*
using VMS.Application.Interfaces;
using VMS.Domain.Entities;
using VMS.Infrastructure.Repositories.UnitOfWork;
using AutoMapper;

namespace VMS.Application.Services;

public class EmployeeService_Example : IEmployeeService
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;  // ← Injected here

    public EmployeeService_Example(
        IUnitOfWorkFactory unitOfWorkFactory, 
        IMapper mapper, 
        IUserContext userContext)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _mapper = mapper;
        _userContext = userContext;
    }

    public async Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto)
    {
        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            return await uow.ExecuteTransactionAsync(async () =>
            {
                // Example 1: Access user details from service
                var userId = _userContext.UserId;
                var userRole = _userContext.UserRole;
                var organizationId = _userContext.OrganisationId;
                var isAdmin = _userContext.IsAdmin;

                // Example 2: Validate authorization
                if (!_userContext.IsAuthenticated)
                {
                    throw new UnauthorizedAccessException("User must be authenticated.");
                }

                // Example 3: Role-based authorization
                if (!_userContext.IsAdmin && userRole != "Manager")
                {
                    throw new UnauthorizedAccessException(
                        $"Role '{userRole}' cannot create employees.");
                }

                // Example 4: Set audit fields
                var entity = _mapper.Map<Employee>(dto);
                entity.CreatedBy = userId;
                entity.CreatedAt = DateTime.UtcNow;
                entity.OrganisationId = organizationId;

                var created = await uow.Employees.AddAsync(entity);
                await uow.SaveChangesAsync();

                // Example 5: Log action
                Console.WriteLine(
                    $"Employee created by {_userContext.UserName} " +
                    $"({_userContext.UserRole}) from org {organizationId}");

                return _mapper.Map<EmployeeDto>(created);
            });
        }
    }

    public async Task<PagedResult<EmployeeDto>> GetAllAsync(PaginationParams pagination)
    {
        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            // Example: Filter by organization if not admin
            var spec = _userContext.IsAdmin
                ? new GetEmployeesPagedSpecification(pagination.Page, pagination.PageSize)
                : new GetEmployeesPagedSpecification(
                    pagination.Page, 
                    pagination.PageSize, 
                    _userContext.OrganisationId);  // Only show user's org employees

            var items = await uow.Employees.GetBySpecificationAsync(spec);
            var count = await uow.Employees.GetCountAsync(
                _userContext.IsAdmin ? null : _userContext.OrganisationId);

            return new PagedResult<EmployeeDto>
            {
                Items = _mapper.Map<IEnumerable<EmployeeDto>>(items),
                TotalCount = count,
                Page = pagination.Page,
                PageSize = pagination.PageSize
            };
        }
    }
}
*/

