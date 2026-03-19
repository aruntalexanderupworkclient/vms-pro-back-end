using VMS.Application.DTOs;
using VMS.Application.DTOs.Common;

namespace VMS.Application.Interfaces;

public interface IAppointmentService
{
    Task<PagedResult<AppointmentDto>> GetAllAsync(PaginationParams pagination);
    Task<AppointmentDto?> GetByIdAsync(Guid id);
    Task<AppointmentDto> CreateAsync(CreateAppointmentDto dto);
    Task<AppointmentDto?> UpdateAsync(Guid id, UpdateAppointmentDto dto);
    Task<bool> DeleteAsync(Guid id);
}
