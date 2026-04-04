using AutoMapper;
using VMS.Application.DTOs;
using VMS.Application.DTOs.Common;
using VMS.Application.Interfaces;
using VMS.Domain.Entities;
using VMS.Infrastructure.Repositories.Specifications.Organisations;
using VMS.Infrastructure.Repositories.UnitOfWork;

namespace VMS.Application.Services;

public class OrganisationService : IOrganisationService
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IMapper _mapper;

    public OrganisationService(IUnitOfWorkFactory unitOfWorkFactory, IMapper mapper)
    {
        _unitOfWorkFactory = unitOfWorkFactory ?? throw new ArgumentNullException(nameof(unitOfWorkFactory));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<PagedResult<OrganisationDto>> GetAllAsync(PaginationParams pagination)
    {
        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            var spec = new GetOrganisationsPagedSpecification(pagination.Page, pagination.PageSize, pagination.Search);
            var items = await uow.Organisations.GetBySpecificationAsync(spec);
            var count = await uow.Organisations.GetCountAsync(pagination.Search);
            return new PagedResult<OrganisationDto>
            {
                Items = _mapper.Map<IEnumerable<OrganisationDto>>(items),
                TotalCount = count,
                Page = pagination.Page,
                PageSize = pagination.PageSize
            };
        }
    }

    public async Task<OrganisationDto?> GetByIdAsync(Guid id)
    {
        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            var spec = new GetOrganisationByIdSpecification(id);
            var entity = await uow.Organisations.GetByIdWithSpecificationAsync(id, spec);
            return entity == null ? null : _mapper.Map<OrganisationDto>(entity);
        }
    }

    public async Task<OrganisationDto> CreateAsync(CreateOrganisationDto dto)
    {
        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            return await uow.ExecuteTransactionAsync(async () =>
            {
                var entity = _mapper.Map<Organisation>(dto);

                if (!string.IsNullOrEmpty(dto.Type))
                {
                    var orgType = (await uow.MdmOrganisationTypes.FindAsync(o => o.Code == dto.Type)).FirstOrDefault();
                    if (orgType != null) entity.TypeId = orgType.Id;
                }

                var created = await uow.Organisations.AddAsync(entity);
                await uow.SaveChangesAsync();
                return _mapper.Map<OrganisationDto>(created);
            });
        }
    }

    public async Task<OrganisationDto?> UpdateAsync(Guid id, UpdateOrganisationDto dto)
    {
        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            return await uow.ExecuteTransactionAsync(async () =>
            {
                var existing = await uow.Organisations.GetByIdAsync(id);
                if (existing == null) return null;

                _mapper.Map(dto, existing);

                if (!string.IsNullOrEmpty(dto.Type))
                {
                    var orgType = (await uow.MdmOrganisationTypes.FindAsync(o => o.Code == dto.Type)).FirstOrDefault();
                    if (orgType != null) existing.TypeId = orgType.Id;
                }

                var updated = await uow.Organisations.UpdateAsync(existing);
                await uow.SaveChangesAsync();
                return _mapper.Map<OrganisationDto>(updated);
            });
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            return await uow.ExecuteTransactionAsync(async () =>
            {
                var existing = await uow.Organisations.GetByIdAsync(id);
                if (existing == null) return false;

                await uow.Organisations.DeleteAsync(id);
                await uow.SaveChangesAsync();
                return true;
            });
        }
    }
}

