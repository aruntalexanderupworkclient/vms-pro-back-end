using AutoMapper;
using VMS.Application.DTOs;
using VMS.Application.DTOs.Common;
using VMS.Application.Interfaces;
using VMS.Domain.Entities;
using VMS.Infrastructure.Repositories.Specifications.Users;
using VMS.Infrastructure.Repositories.UnitOfWork;

namespace VMS.Application.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IMapper _mapper;

    public UserService(IUnitOfWorkFactory unitOfWorkFactory, IMapper mapper)
    {
        _unitOfWorkFactory = unitOfWorkFactory ?? throw new ArgumentNullException(nameof(unitOfWorkFactory));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    // ✅ GetPagedAsync with includes - uses Specification
    public async Task<PagedResult<UserDto>> GetAllAsync(PaginationParams pagination)
    {
        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            var spec = new GetUsersPagedSpecification(pagination.Page, pagination.PageSize, pagination.Search);
            var items = await uow.Users.GetBySpecificationAsync(spec);
            var count = await uow.Users.GetCountAsync(pagination.Search);
            
            return new PagedResult<UserDto>
            {
                Items = _mapper.Map<IEnumerable<UserDto>>(items),
                TotalCount = count,
                Page = pagination.Page,
                PageSize = pagination.PageSize
            };
        }
    }

    // ✅ GetByIdAsync with includes - uses Specification
    public async Task<UserDto?> GetByIdAsync(Guid id)
    {
        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            var spec = new GetUserByIdSpecification(id);
            var entity = await uow.Users.GetByIdWithSpecificationAsync(id, spec);
            return entity == null ? null : _mapper.Map<UserDto>(entity);
        }
    }

    // ✅ GetByEmailAsync with includes - uses Specification
    public async Task<UserDto?> GetByEmailAsync(string email)
    {
        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            Guid activeStatusId = (await uow.MdmUserStatuses.FindAsync(s => s.Code == "Active")).Select(x=>x.Id).FirstOrDefault();

            var spec = new FindUserSpecification(email,activeStatusId);
            var users = await uow.Users.GetBySpecificationAsync(spec);
            var user = users.FirstOrDefault();
            return user == null ? null : _mapper.Map<UserDto>(user);
        }
    }

    // ✅ CRUD methods - Now with UoW for atomic transactions
    public async Task<UserDto> CreateAsync(CreateUserDto dto)
    {
        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            return await uow.ExecuteTransactionAsync(async () =>
            {
                try
                {
                    var entity = _mapper.Map<User>(dto);
                    entity.PasswordHash = BCryptHash(dto.Password);
                    
                    if (dto.StatusId != Guid.Empty)
                    {
                        var status = (await uow.MdmUserStatuses.FindAsync(s => s.Id == dto.StatusId)).FirstOrDefault();
                        if (status != null) entity.StatusId = status.Id;
                    }
                    
                    var created = await uow.Users.AddAsync(entity);
                    await uow.SaveChangesAsync();
                    return _mapper.Map<UserDto>(created);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("An error occurred while creating the user.", ex);
                }
            });
        }
    }

    public async Task<UserDto?> UpdateAsync(Guid id, UpdateUserDto dto)
    {
        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            return await uow.ExecuteTransactionAsync(async () =>
            {
                var existing = await uow.Users.GetByIdAsync(id);
                if (existing == null) return null;

                _mapper.Map(dto, existing);
                if (dto.StatusId != Guid.Empty)
                {
                    var status = (await uow.MdmUserStatuses.FindAsync(s => s.Id == dto.StatusId)).FirstOrDefault();
                    if (status != null) existing.StatusId = status.Id;
                }
                var updated = await uow.Users.UpdateAsync(existing);
                await uow.SaveChangesAsync();
                return _mapper.Map<UserDto>(updated);
            });
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            return await uow.ExecuteTransactionAsync(async () =>
            {
                var existing = await uow.Users.GetByIdAsync(id);
                if (existing == null) return false;
                await uow.Users.DeleteAsync(id);
                await uow.SaveChangesAsync();
                return true;
            });
        }
    }


    private static string BCryptHash(string password)
    {
        using var sha = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(password);
        var hash = sha.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
