using AutoMapper;
using VMS.Application.DTOs;
using VMS.Application.DTOs.Common;
using VMS.Application.Interfaces;
using VMS.Domain.Entities;
using VMS.Infrastructure.Repositories.UnitOfWork;

namespace VMS.Application.Services;

public class PermissionService : IPermissionService
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IMapper _mapper;

    public PermissionService(IUnitOfWorkFactory unitOfWorkFactory, IMapper mapper)
    {
        _unitOfWorkFactory = unitOfWorkFactory ?? throw new ArgumentNullException(nameof(unitOfWorkFactory));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<PagedResult<PermissionDto>> GetAllAsync(PaginationParams pagination)
    {
        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            var items = await uow.Permissions.GetPagedAsync(pagination.Page, pagination.PageSize, pagination.Search);
            var count = await uow.Permissions.GetCountAsync(pagination.Search);
            return new PagedResult<PermissionDto>
            {
                Items = _mapper.Map<IEnumerable<PermissionDto>>(items),
                TotalCount = count,
                Page = pagination.Page,
                PageSize = pagination.PageSize
            };
        }
    }

    public async Task<PermissionDto?> GetByIdAsync(Guid id)
    {
        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            var entity = await uow.Permissions.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<PermissionDto>(entity);
        }
    }

    public async Task<PermissionDto> CreateAsync(CreatePermissionDto dto)
    {
        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            return await uow.ExecuteTransactionAsync(async () =>
            {
                var entity = _mapper.Map<Permission>(dto);
                var created = await uow.Permissions.AddAsync(entity);
                await uow.SaveChangesAsync();
                return _mapper.Map<PermissionDto>(created);
            });
        }
    }

    public async Task<PermissionDto?> UpdateAsync(Guid id, UpdatePermissionDto dto)
    {
        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            return await uow.ExecuteTransactionAsync(async () =>
            {
                var existing = await uow.Permissions.GetByIdAsync(id);
                if (existing == null) return null;

                _mapper.Map(dto, existing);
                var updated = await uow.Permissions.UpdateAsync(existing);
                await uow.SaveChangesAsync();
                return _mapper.Map<PermissionDto>(updated);
            });
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            return await uow.ExecuteTransactionAsync(async () =>
            {
                var existing = await uow.Permissions.GetByIdAsync(id);
                if (existing == null) return false;
                await uow.Permissions.DeleteAsync(id);
                await uow.SaveChangesAsync();
                return true;
            });
        }
    }

    public async Task<IEnumerable<PermissionDto>> GetByRoleIdAsync(Guid roleId)
    {
        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            var items = await uow.Permissions.FindAsync(p => p.RoleId == roleId);
            return _mapper.Map<IEnumerable<PermissionDto>>(items);
        }
    }
}
