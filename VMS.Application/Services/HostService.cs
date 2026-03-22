using AutoMapper;
using VMS.Application.DTOs;
using VMS.Application.DTOs.Common;
using VMS.Application.Interfaces;
using VMS.Domain.Entities;
using VMS.Infrastructure.Repositories.Specifications.Hosts;
using VMS.Infrastructure.Repositories.UnitOfWork;

namespace VMS.Application.Services;

public class HostService : IHostService
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IMapper _mapper;

    public HostService(IUnitOfWorkFactory unitOfWorkFactory, IMapper mapper)
    {
        _unitOfWorkFactory = unitOfWorkFactory ?? throw new ArgumentNullException(nameof(unitOfWorkFactory));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<PagedResult<HostDto>> GetAllAsync(PaginationParams pagination)
    {
        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            var spec = new GetHostsPagedSpecification(pagination.Page, pagination.PageSize, pagination.Search);
            var items = await uow.Hosts.GetBySpecificationAsync(spec);
            var count = await uow.Hosts.GetCountAsync(pagination.Search);
            return new PagedResult<HostDto>
            {
                Items = _mapper.Map<IEnumerable<HostDto>>(items),
                TotalCount = count,
                Page = pagination.Page,
                PageSize = pagination.PageSize
            };
        }
    }

    public async Task<HostDto?> GetByIdAsync(Guid id)
    {
        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            var spec = new GetHostByIdSpecification(id);
            var entity = await uow.Hosts.GetByIdWithSpecificationAsync(id, spec);
            return entity == null ? null : _mapper.Map<HostDto>(entity);
        }
    }

    public async Task<HostDto> CreateAsync(CreateHostDto dto)
    {
        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            return await uow.ExecuteTransactionAsync(async () =>
            {
                var entity = _mapper.Map<Host>(dto);
                var created = await uow.Hosts.AddAsync(entity);
                await uow.SaveChangesAsync();
                return _mapper.Map<HostDto>(created);
            });
        }
    }

    public async Task<HostDto?> UpdateAsync(Guid id, UpdateHostDto dto)
    {
        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            return await uow.ExecuteTransactionAsync(async () =>
            {
                var existing = await uow.Hosts.GetByIdAsync(id);
                if (existing == null) return null;

                _mapper.Map(dto, existing);
                var updated = await uow.Hosts.UpdateAsync(existing);
                await uow.SaveChangesAsync();
                return _mapper.Map<HostDto>(updated);
            });
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            return await uow.ExecuteTransactionAsync(async () =>
            {
                var existing = await uow.Hosts.GetByIdAsync(id);
                if (existing == null) return false;

                // Soft delete the host
                await uow.Hosts.DeleteAsync(id);

                // Soft delete associated appointments (cascade delete)
                var existingAppointments = await uow.Appointments.FindAsync(a => a.HostId == id && !a.IsDeleted);
                foreach (var appointment in existingAppointments)
                {
                    await uow.Appointments.DeleteAsync(appointment.Id);
                }

                await uow.SaveChangesAsync();
                return true;
            });
        }
    }
}
