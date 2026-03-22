using AutoMapper;
using VMS.Application.DTOs;
using VMS.Application.DTOs.Common;
using VMS.Application.Interfaces;
using VMS.Domain.Entities;
using VMS.Infrastructure.Repositories.Specifications.Visitors;
using VMS.Infrastructure.Repositories.UnitOfWork;

namespace VMS.Application.Services;

public class VisitorService : IVisitorService
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IMapper _mapper;

    public VisitorService(IUnitOfWorkFactory unitOfWorkFactory, IMapper mapper)
    {
        _unitOfWorkFactory = unitOfWorkFactory ?? throw new ArgumentNullException(nameof(unitOfWorkFactory));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<PagedResult<VisitorDto>> GetAllAsync(PaginationParams pagination)
    {
        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            var spec = new GetVisitorsPagedSpecification(pagination.Page, pagination.PageSize, pagination.Search);
            var items = await uow.Visitors.GetBySpecificationAsync(spec);
            var count = await uow.Visitors.GetCountAsync(pagination.Search);
            return new PagedResult<VisitorDto>
            {
                Items = _mapper.Map<IEnumerable<VisitorDto>>(items),
                TotalCount = count,
                Page = pagination.Page,
                PageSize = pagination.PageSize
            };
        }
    }

    public async Task<VisitorDto?> GetByIdAsync(Guid id)
    {
        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            var spec = new GetVisitorByIdSpecification(id);
            var entity = await uow.Visitors.GetByIdWithSpecificationAsync(id, spec);
            return entity == null ? null : _mapper.Map<VisitorDto>(entity);
        }
    }

    public async Task<VisitorDto> CreateAsync(CreateVisitorDto dto)
    {
        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            return await uow.ExecuteTransactionAsync(async () =>
            {
                var entity = _mapper.Map<Visitor>(dto);
                var created = await uow.Visitors.AddAsync(entity);
                await uow.SaveChangesAsync();
                return _mapper.Map<VisitorDto>(created);
            });
        }
    }

    public async Task<VisitorDto?> UpdateAsync(Guid id, UpdateVisitorDto dto)
    {
        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            return await uow.ExecuteTransactionAsync(async () =>
            {
                var existing = await uow.Visitors.GetByIdAsync(id);
                if (existing == null) return null;

                _mapper.Map(dto, existing);
                var updated = await uow.Visitors.UpdateAsync(existing);
                await uow.SaveChangesAsync();
                return _mapper.Map<VisitorDto>(updated);
            });
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            return await uow.ExecuteTransactionAsync(async () =>
            {
                var existing = await uow.Visitors.GetByIdAsync(id);
                if (existing == null) return false;

                // Soft delete the visitor
                await uow.Visitors.DeleteAsync(id);

                // Soft delete associated tokens (cascade delete)
                var existingTokens = await uow.Tokens.FindAsync(t => t.VisitorId == id && !t.IsDeleted);
                foreach (var token in existingTokens)
                {
                    await uow.Tokens.DeleteAsync(token.Id);
                }

                await uow.SaveChangesAsync();
                return true;
            });
        }
    }
}
