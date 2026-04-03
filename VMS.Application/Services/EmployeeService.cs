using AutoMapper;
using VMS.Application.DTOs;
using VMS.Application.DTOs.Common;
using VMS.Application.Interfaces;
using VMS.Domain.Entities;
using VMS.Infrastructure.Repositories.Specifications.Employees;
using VMS.Infrastructure.Repositories.UnitOfWork;

namespace VMS.Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IMapper _mapper;

    public EmployeeService(IUnitOfWorkFactory unitOfWorkFactory, IMapper mapper)
    {
        _unitOfWorkFactory = unitOfWorkFactory ?? throw new ArgumentNullException(nameof(unitOfWorkFactory));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<PagedResult<EmployeeDto>> GetAllAsync(PaginationParams pagination)
    {
        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            var spec = new GetEmployeesPagedSpecification(pagination.Page, pagination.PageSize, pagination.Search);
            var items = await uow.Employees.GetBySpecificationAsync(spec);
            var count = await uow.Employees.GetCountAsync(pagination.Search);
            return new PagedResult<EmployeeDto>
            {
                Items = _mapper.Map<IEnumerable<EmployeeDto>>(items),
                TotalCount = count,
                Page = pagination.Page,
                PageSize = pagination.PageSize
            };
        }
    }

    public async Task<EmployeeDto?> GetByIdAsync(Guid id)
    {
        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            var spec = new GetEmployeeByIdSpecification(id);
            var entity = await uow.Employees.GetByIdWithSpecificationAsync(id, spec);
            return entity == null ? null : _mapper.Map<EmployeeDto>(entity);
        }
    }

    public async Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto)
    {
        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            return await uow.ExecuteTransactionAsync(async () =>
            {
                var entity = _mapper.Map<Employee>(dto);
                var created = await uow.Employees.AddAsync(entity);
                await uow.SaveChangesAsync();
                return _mapper.Map<EmployeeDto>(created);
            });
        }
    }

    public async Task<EmployeeDto?> UpdateAsync(Guid id, UpdateEmployeeDto dto)
    {
        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            return await uow.ExecuteTransactionAsync(async () =>
            {
                var existing = await uow.Employees.GetByIdAsync(id);
                if (existing == null) return null;

                _mapper.Map(dto, existing);
                var updated = await uow.Employees.UpdateAsync(existing);
                await uow.SaveChangesAsync();
                return _mapper.Map<EmployeeDto>(updated);
            });
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            return await uow.ExecuteTransactionAsync(async () =>
            {
                var existing = await uow.Employees.GetByIdAsync(id);
                if (existing == null) return false;
                await uow.Employees.DeleteAsync(id);
                await uow.SaveChangesAsync();
                return true;
            });
        }
    }
}
