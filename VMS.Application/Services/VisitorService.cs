using AutoMapper;
using VMS.Application.DTOs;
using VMS.Application.DTOs.Common;
using VMS.Application.Interfaces;
using VMS.Domain.Entities;
using VMS.Infrastructure.Repositories.Interfaces;

namespace VMS.Application.Services;

public class VisitorService : IVisitorService
{
    private readonly IRepository<Visitor> _repository;
    private readonly IMapper _mapper;

    public VisitorService(IRepository<Visitor> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PagedResult<VisitorDto>> GetAllAsync(PaginationParams pagination)
    {
        var items = await _repository.GetPagedAsync(pagination.Page, pagination.PageSize, pagination.Search);
        var count = await _repository.GetCountAsync(pagination.Search);
        return new PagedResult<VisitorDto>
        {
            Items = _mapper.Map<IEnumerable<VisitorDto>>(items),
            TotalCount = count,
            Page = pagination.Page,
            PageSize = pagination.PageSize
        };
    }

    public async Task<VisitorDto?> GetByIdAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity == null ? null : _mapper.Map<VisitorDto>(entity);
    }

    public async Task<VisitorDto> CreateAsync(CreateVisitorDto dto)
    {
        var entity = _mapper.Map<Visitor>(dto);
        var created = await _repository.AddAsync(entity);
        return _mapper.Map<VisitorDto>(created);
    }

    public async Task<VisitorDto?> UpdateAsync(Guid id, UpdateVisitorDto dto)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return null;

        _mapper.Map(dto, existing);
        var updated = await _repository.UpdateAsync(existing);
        return _mapper.Map<VisitorDto>(updated);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return false;
        await _repository.DeleteAsync(id);
        return true;
    }
}
