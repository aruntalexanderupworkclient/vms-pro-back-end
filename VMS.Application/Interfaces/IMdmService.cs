using VMS.Application.DTOs;
using VMS.Application.DTOs.Common;

namespace VMS.Application.Interfaces;

public interface IMdmService
{
    Task<List<MdmDto>> GetAllAsync(string mdmType);
    Task<MdmDto?> GetByIdAsync(string mdmType, Guid id);
    Task<MdmDto> CreateAsync(string mdmType, CreateMdmDto dto);
    Task<MdmDto?> UpdateAsync(string mdmType, Guid id, UpdateMdmDto dto);
    Task<bool> DeleteAsync(string mdmType, Guid id);
}

