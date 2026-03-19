using VMS.Application.DTOs;
using VMS.Application.DTOs.Common;

namespace VMS.Application.Interfaces;

public interface IVisitorService
{
    Task<PagedResult<VisitorDto>> GetAllAsync(PaginationParams pagination);
    Task<VisitorDto?> GetByIdAsync(Guid id);
    Task<VisitorDto> CreateAsync(CreateVisitorDto dto);
    Task<VisitorDto?> UpdateAsync(Guid id, UpdateVisitorDto dto);
    Task<bool> DeleteAsync(Guid id);
}
