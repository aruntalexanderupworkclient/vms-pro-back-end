using AutoMapper;
using VMS.Application.DTOs;
using VMS.Application.DTOs.Common;
using VMS.Application.Interfaces;
using VMS.Domain.Entities;
using VMS.Infrastructure.Repositories.Specifications.Appointments;
using VMS.Infrastructure.Repositories.UnitOfWork;

namespace VMS.Application.Services;

public class AppointmentService : IAppointmentService
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IMapper _mapper;

    public AppointmentService(IUnitOfWorkFactory unitOfWorkFactory, IMapper mapper)
    {
        _unitOfWorkFactory = unitOfWorkFactory ?? throw new ArgumentNullException(nameof(unitOfWorkFactory));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<PagedResult<AppointmentDto>> GetAllAsync(PaginationParams pagination)
    {
        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            var spec = new GetAppointmentsPagedSpecification(pagination.Page, pagination.PageSize, pagination.Search);
            var items = await uow.Appointments.GetBySpecificationAsync(spec);
            var count = await uow.Appointments.GetCountAsync(pagination.Search);
            return new PagedResult<AppointmentDto>
            {
                Items = _mapper.Map<IEnumerable<AppointmentDto>>(items),
                TotalCount = count,
                Page = pagination.Page,
                PageSize = pagination.PageSize
            };
        }
    }

    public async Task<AppointmentDto?> GetByIdAsync(Guid id)
    {
        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            var spec = new GetAppointmentByIdSpecification(id);
            var entity = await uow.Appointments.GetByIdWithSpecificationAsync(id, spec);
            return entity == null ? null : _mapper.Map<AppointmentDto>(entity);
        }
    }

    public async Task<AppointmentDto> CreateAsync(CreateAppointmentDto dto)
    {
        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            return await uow.ExecuteTransactionAsync(async () =>
            {
                try
                {
                    var entity = _mapper.Map<Appointment>(dto);
                    var created = await uow.Appointments.AddAsync(entity);
                    await uow.SaveChangesAsync();
                    return _mapper.Map<AppointmentDto>(created);
                }
                catch (Exception ex)
                {
                    // Log the exception (you can use a logging framework like Serilog, NLog, etc.)
                    Console.Error.WriteLine($"Error creating appointment: {ex.Message}");
                    throw; // Rethrow to ensure transaction is rolled back
                }
            });
        }
    }

    public async Task<AppointmentDto?> UpdateAsync(Guid id, UpdateAppointmentDto dto)
    {
        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            return await uow.ExecuteTransactionAsync(async () =>
            {
                var existing = await uow.Appointments.GetByIdAsync(id);
                if (existing == null) return null;

                _mapper.Map(dto, existing);
                var updated = await uow.Appointments.UpdateAsync(existing);
                await uow.SaveChangesAsync();
                return _mapper.Map<AppointmentDto>(updated);
            });
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            return await uow.ExecuteTransactionAsync(async () =>
            {
                var existing = await uow.Appointments.GetByIdAsync(id);
                if (existing == null) return false;
                await uow.Appointments.DeleteAsync(id);
                await uow.SaveChangesAsync();
                return true;
            });
        }
    }
}
