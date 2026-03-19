using AutoMapper;
using VMS.Application.DTOs;
using VMS.Application.DTOs.Common;
using VMS.Application.Interfaces;
using VMS.Domain.Entities;
using VMS.Infrastructure.Repositories.Interfaces;

namespace VMS.Application.Services;

public class PermissionService : IPermissionService
{
    private readonly IRepository<Permission> _repository;
    private readonly IMapper _mapper;

    public PermissionService(IRepository<Permission> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PagedResult<PermissionDto>> GetAllAsync(PaginationParams pagination)
    {
        var items = await _repository.GetPagedAsync(pagination.Page, pagination.PageSize, pagination.Search);
        var count = await _repository.GetCountAsync(pagination.Search);
        return new PagedResult<PermissionDto>
        {
            Items = _mapper.Map<IEnumerable<PermissionDto>>(items),
            TotalCount = count,
            Page = pagination.Page,
            PageSize = pagination.PageSize
        };
    }

    public async Task<PermissionDto?> GetByIdAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity == null ? null : _mapper.Map<PermissionDto>(entity);
    }

    public async Task<PermissionDto> CreateAsync(CreatePermissionDto dto)
    {
        var entity = _mapper.Map<Permission>(dto);
        var created = await _repository.AddAsync(entity);
        return _mapper.Map<PermissionDto>(created);
    }

    public async Task<PermissionDto?> UpdateAsync(Guid id, UpdatePermissionDto dto)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return null;

        _mapper.Map(dto, existing);
        var updated = await _repository.UpdateAsync(existing);
        return _mapper.Map<PermissionDto>(updated);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return false;
        await _repository.DeleteAsync(id);
        return true;
    }

    public async Task<IEnumerable<PermissionDto>> GetByRoleIdAsync(Guid roleId)
    {
        var items = await _repository.FindAsync(p => p.RoleId == roleId);
        return _mapper.Map<IEnumerable<PermissionDto>>(items);
    }
}
