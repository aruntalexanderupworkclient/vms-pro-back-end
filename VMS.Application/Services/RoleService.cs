using AutoMapper;
using VMS.Application.DTOs;
using VMS.Application.DTOs.Common;
using VMS.Application.Interfaces;
using VMS.Domain.Entities;
using VMS.Infrastructure.Repositories.Specifications.Roles;
using VMS.Infrastructure.Repositories.UnitOfWork;

namespace VMS.Application.Services;

public class RoleService : IRoleService
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IMapper _mapper;

    public RoleService(IUnitOfWorkFactory unitOfWorkFactory, IMapper mapper)
    {
        _unitOfWorkFactory = unitOfWorkFactory ?? throw new ArgumentNullException(nameof(unitOfWorkFactory));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<PagedResult<RoleDto>> GetAllAsync(PaginationParams pagination)
    {
        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            var spec = new GetRolesPagedSpecification(pagination.Page, pagination.PageSize, pagination.Search);
            var items = await uow.Roles.GetBySpecificationAsync(spec);
            var count = await uow.Roles.GetCountAsync(pagination.Search);
            return new PagedResult<RoleDto>
            {
                Items = _mapper.Map<IEnumerable<RoleDto>>(items),
                TotalCount = count,
                Page = pagination.Page,
                PageSize = pagination.PageSize
            };
        }
    }

    public async Task<RoleDto?> GetByIdAsync(Guid id)
    {
        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            var spec = new GetRoleByIdSpecification(id);
            var entity = await uow.Roles.GetByIdWithSpecificationAsync(id, spec);
            return entity == null ? null : _mapper.Map<RoleDto>(entity);
        }
    }

    public async Task<RoleDto> CreateAsync(CreateRoleDto dto)
    {
        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            return await uow.ExecuteTransactionAsync(async () =>
            {
                var entity = _mapper.Map<Role>(dto);
                var created = await uow.Roles.AddAsync(entity);
                await uow.SaveChangesAsync();
                return _mapper.Map<RoleDto>(created);
            });
        }
    }

    public async Task<RoleDto?> UpdateAsync(Guid id, UpdateRoleDto dto)
    {
        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            return await uow.ExecuteTransactionAsync(async () =>
            {
                var existing = await uow.Roles.GetByIdAsync(id);
                if (existing == null) return null;

                // Update role fields
                _mapper.Map(dto, existing);

                // Handle permissions update (insert/update/delete)
                if (dto.Permissions != null)
                {
                    await UpdateRolePermissionsAsync(uow, id, dto.Permissions);
                }

                // Save role
                var updated = await uow.Roles.UpdateAsync(existing);
                await uow.SaveChangesAsync();
                return _mapper.Map<RoleDto>(updated);
            });
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            return await uow.ExecuteTransactionAsync(async () =>
            {
                var existing = await uow.Roles.GetByIdAsync(id);
                if (existing == null) return false;

                // Soft delete the role
                await uow.Roles.DeleteAsync(id);

                // Soft delete associated permissions
                var existingPermissions = await uow.Permissions.FindAsync(p => p.RoleId == id && !p.IsDeleted);
                foreach (var perm in existingPermissions)
                {
                    await uow.Permissions.DeleteAsync(perm.Id);
                }

                await uow.SaveChangesAsync();
                return true;
            });
        }
    }

    /// <summary>
    /// Handle permission updates with insert/update/delete logic
    /// Matches existing permissions by Module+Action
    /// </summary>
    private async Task UpdateRolePermissionsAsync(
        IUnitOfWork uow,
        Guid roleId,
        Dictionary<string, ModulePermissions> newPermissions)
    {
        // Get existing permissions for this role
        var existingPermissions = await uow.Permissions.FindAsync(
            p => p.RoleId == roleId && !p.IsDeleted);

        var existingDict = existingPermissions.ToDictionary(
            p => $"{p.Module}.{p.Action}",
            p => p);

        var newPermissionKeys = new HashSet<string>(newPermissions.Keys);

        var properties = typeof(ModulePermissions).GetProperties();
        // Process each new permission (insert or update)
        foreach (var (module, value) in newPermissions)
        {
            foreach (var prop in properties)
            {
                var isAllowed = (bool)prop.GetValue(value)!;

                if (!isAllowed) continue; // skip false

                //var parts = key.Split('.');
                // if (parts.Length != 2) continue;

                //var module = parts[0];
                var action = prop.Name;

                if (existingDict.ContainsKey(module))
                {
                    // UPDATE: Permission exists, keep it (Module+Action match)
                    // The permission already exists, no action needed for now
                    // If we need to update other properties, do it here
                    var existingPerm = existingDict[module];
                    // You can add more properties to Permission entity if needed
                }
                else
                {
                    // INSERT: New permission
                    var newPerm = new Permission
                    {
                        Id = Guid.NewGuid(),
                        RoleId = roleId,
                        Module = module,
                        Action = action,
                        CreatedAt = DateTime.UtcNow
                    };
                    await uow.Permissions.AddAsync(newPerm);
                }
            }
        }

        // DELETE: Remove permissions not in new list
        foreach (var existingPerm in existingPermissions)
        {
            var key = $"{existingPerm.Module}.{existingPerm.Action}";
            if (!newPermissionKeys.Contains(key))
            {
                // Soft delete
                await uow.Permissions.DeleteAsync(existingPerm.Id);
            }
        }
    }
}
