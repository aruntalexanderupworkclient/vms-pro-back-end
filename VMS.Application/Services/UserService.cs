using AutoMapper;
using VMS.Application.DTOs;
using VMS.Application.DTOs.Common;
using VMS.Application.Interfaces;
using VMS.Domain.Entities;
using VMS.Infrastructure.Repositories.Interfaces;

namespace VMS.Application.Services;

public class UserService : IUserService
{
    private readonly IRepository<User> _repository;
    private readonly IMapper _mapper;

    public UserService(IRepository<User> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PagedResult<UserDto>> GetAllAsync(PaginationParams pagination)
    {
        var items = await _repository.GetPagedAsync(pagination.Page, pagination.PageSize, pagination.Search);
        var count = await _repository.GetCountAsync(pagination.Search);
        return new PagedResult<UserDto>
        {
            Items = _mapper.Map<IEnumerable<UserDto>>(items),
            TotalCount = count,
            Page = pagination.Page,
            PageSize = pagination.PageSize
        };
    }

    public async Task<UserDto?> GetByIdAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity == null ? null : _mapper.Map<UserDto>(entity);
    }

    public async Task<UserDto> CreateAsync(CreateUserDto dto)
    {
        var entity = _mapper.Map<User>(dto);
        entity.PasswordHash = BCryptHash(dto.Password);
        var created = await _repository.AddAsync(entity);
        return _mapper.Map<UserDto>(created);
    }

    public async Task<UserDto?> UpdateAsync(Guid id, UpdateUserDto dto)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return null;

        _mapper.Map(dto, existing);
        var updated = await _repository.UpdateAsync(existing);
        return _mapper.Map<UserDto>(updated);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return false;
        await _repository.DeleteAsync(id);
        return true;
    }

    public async Task<UserDto?> GetByEmailAsync(string email)
    {
        var users = await _repository.FindAsync(u => u.Email.ToLower() == email.ToLower());
        var user = users.FirstOrDefault();
        return user == null ? null : _mapper.Map<UserDto>(user);
    }

    private static string BCryptHash(string password)
    {
        using var sha = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(password);
        var hash = sha.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
