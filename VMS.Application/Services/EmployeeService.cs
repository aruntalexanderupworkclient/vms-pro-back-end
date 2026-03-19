using AutoMapper;
using VMS.Application.DTOs;
using VMS.Application.DTOs.Common;
using VMS.Application.Interfaces;
using VMS.Domain.Entities;
using VMS.Infrastructure.Repositories.Interfaces;

namespace VMS.Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IRepository<Employee> _repository;
    private readonly IMapper _mapper;

    public EmployeeService(IRepository<Employee> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PagedResult<EmployeeDto>> GetAllAsync(PaginationParams pagination)
    {
        var items = await _repository.GetPagedAsync(pagination.Page, pagination.PageSize, pagination.Search);
        var count = await _repository.GetCountAsync(pagination.Search);
        return new PagedResult<EmployeeDto>
        {
            Items = _mapper.Map<IEnumerable<EmployeeDto>>(items),
            TotalCount = count,
            Page = pagination.Page,
            PageSize = pagination.PageSize
        };
    }

    public async Task<EmployeeDto?> GetByIdAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity == null ? null : _mapper.Map<EmployeeDto>(entity);
    }

    public async Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto)
    {
        var entity = _mapper.Map<Employee>(dto);
        var created = await _repository.AddAsync(entity);
        return _mapper.Map<EmployeeDto>(created);
    }

    public async Task<EmployeeDto?> UpdateAsync(Guid id, UpdateEmployeeDto dto)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return null;

        _mapper.Map(dto, existing);
        var updated = await _repository.UpdateAsync(existing);
        return _mapper.Map<EmployeeDto>(updated);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return false;
        await _repository.DeleteAsync(id);
        return true;
    }
}
