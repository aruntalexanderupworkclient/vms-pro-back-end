using VMS.Application.DTOs;
using VMS.Application.DTOs.Common;

namespace VMS.Application.Interfaces;

public interface IHostService
{
    Task<PagedResult<HostDto>> GetAllAsync(PaginationParams pagination);
    Task<HostDto?> GetByIdAsync(Guid id);
    Task<HostDto> CreateAsync(CreateHostDto dto);
    Task<HostDto?> UpdateAsync(Guid id, UpdateHostDto dto);
    Task<bool> DeleteAsync(Guid id);
}
