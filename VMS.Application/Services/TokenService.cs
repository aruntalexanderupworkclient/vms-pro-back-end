using AutoMapper;
using VMS.Application.DTOs;
using VMS.Application.DTOs.Common;
using VMS.Application.Interfaces;
using VMS.Domain.Entities;
using VMS.Infrastructure.Repositories.Interfaces;

namespace VMS.Application.Services;

public class TokenService : ITokenService
{
    private readonly IRepository<VisitorToken> _repository;
    private readonly IMapper _mapper;

    public TokenService(IRepository<VisitorToken> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PagedResult<TokenDto>> GetAllAsync(PaginationParams pagination)
    {
        var items = await _repository.GetPagedAsync(pagination.Page, pagination.PageSize, pagination.Search);
        var count = await _repository.GetCountAsync(pagination.Search);
        return new PagedResult<TokenDto>
        {
            Items = _mapper.Map<IEnumerable<TokenDto>>(items),
            TotalCount = count,
            Page = pagination.Page,
            PageSize = pagination.PageSize
        };
    }

    public async Task<TokenDto?> GetByIdAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity == null ? null : _mapper.Map<TokenDto>(entity);
    }

    public async Task<TokenDto> CreateAsync(CreateTokenDto dto)
    {
        var entity = _mapper.Map<VisitorToken>(dto);
        entity.IssuedAt = DateTime.UtcNow;
        var created = await _repository.AddAsync(entity);
        return _mapper.Map<TokenDto>(created);
    }

    public async Task<TokenDto?> UpdateAsync(Guid id, UpdateTokenDto dto)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return null;

        _mapper.Map(dto, existing);
        var updated = await _repository.UpdateAsync(existing);
        return _mapper.Map<TokenDto>(updated);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return false;
        await _repository.DeleteAsync(id);
        return true;
    }
}
