using VMS.Application.DTOs;
using VMS.Application.DTOs.Common;

namespace VMS.Application.Interfaces;

public interface IPermissionService
{
    Task<PagedResult<PermissionDto>> GetAllAsync(PaginationParams pagination);
    Task<PermissionDto?> GetByIdAsync(Guid id);
    Task<PermissionDto> CreateAsync(CreatePermissionDto dto);
    Task<PermissionDto?> UpdateAsync(Guid id, UpdatePermissionDto dto);
    Task<bool> DeleteAsync(Guid id);
    Task<IEnumerable<PermissionDto>> GetByRoleIdAsync(Guid roleId);
}
