using VMS.Application.DTOs;
using VMS.Application.DTOs.Common;

namespace VMS.Application.Interfaces;

public interface IOrganisationService
{
    Task<PagedResult<OrganisationDto>> GetAllAsync(PaginationParams pagination);
    Task<OrganisationDto?> GetByIdAsync(Guid id);
    Task<OrganisationDto> CreateAsync(CreateOrganisationDto dto);
    Task<OrganisationDto?> UpdateAsync(Guid id, UpdateOrganisationDto dto);
    Task<bool> DeleteAsync(Guid id);
}

