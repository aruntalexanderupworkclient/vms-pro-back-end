using VMS.Application.DTOs;
using VMS.Application.DTOs.Common;

namespace VMS.Application.Interfaces;

public interface ITokenService
{
    Task<PagedResult<TokenDto>> GetAllAsync(PaginationParams pagination);
    Task<TokenDto?> GetByIdAsync(Guid id);
    Task<TokenDto> CreateAsync(CreateTokenDto dto);
    Task<TokenDto?> UpdateAsync(Guid id, UpdateTokenDto dto);
    Task<bool> DeleteAsync(Guid id);
}
