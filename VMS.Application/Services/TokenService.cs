using AutoMapper;
using VMS.Application.DTOs;
using VMS.Application.DTOs.Common;
using VMS.Application.Interfaces;
using VMS.Domain.Entities;
using VMS.Infrastructure.Repositories.Specifications.Tokens;
using VMS.Infrastructure.Repositories.UnitOfWork;

namespace VMS.Application.Services;

public class TokenService : ITokenService
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IMapper _mapper;

    public TokenService(IUnitOfWorkFactory unitOfWorkFactory, IMapper mapper)
    {
        _unitOfWorkFactory = unitOfWorkFactory ?? throw new ArgumentNullException(nameof(unitOfWorkFactory));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<PagedResult<TokenDto>> GetAllAsync(PaginationParams pagination)
    {
        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            var spec = new GetTokensPagedSpecification(pagination.Page, pagination.PageSize, pagination.Search);
            var items = await uow.Tokens.GetBySpecificationAsync(spec);
            var count = await uow.Tokens.GetCountAsync(pagination.Search);
            return new PagedResult<TokenDto>
            {
                Items = _mapper.Map<IEnumerable<TokenDto>>(items),
                TotalCount = count,
                Page = pagination.Page,
                PageSize = pagination.PageSize
            };
        }
    }

    public async Task<TokenDto?> GetByIdAsync(Guid id)
    {
        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            var spec = new GetTokenByIdSpecification(id);
            var entity = await uow.Tokens.GetByIdWithSpecificationAsync(id, spec);
            return entity == null ? null : _mapper.Map<TokenDto>(entity);
        }
    }

    public async Task<TokenDto> CreateAsync(CreateTokenDto dto)
    {
        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            return await uow.ExecuteTransactionAsync(async () =>
            {
                var entity = _mapper.Map<VisitorToken>(dto);
                var tokenType = (await uow.MdmTokenTypes.FindAsync(t => t.Code == dto.Type)).FirstOrDefault();
                if (tokenType != null) entity.TokenTypeId = tokenType.Id;
                var defaultStatus = (await uow.MdmVisitStatuses.FindAsync(s => s.Code == MdmCodes.VisitStatus.CheckedIn)).First();
                entity.StatusId = defaultStatus.Id;
                var created = await uow.Tokens.AddAsync(entity);
                await uow.SaveChangesAsync();
                return _mapper.Map<TokenDto>(created);
            });
        }
    }

    public async Task<TokenDto?> UpdateAsync(Guid id, UpdateTokenDto dto)
    {
        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            return await uow.ExecuteTransactionAsync(async () =>
            {
                var existing = await uow.Tokens.GetByIdAsync(id);
                if (existing == null) return null;

                _mapper.Map(dto, existing);
                if (!string.IsNullOrEmpty(dto.Type))
                {
                    var tokenType = (await uow.MdmTokenTypes.FindAsync(t => t.Code == dto.Type)).FirstOrDefault();
                    if (tokenType != null) existing.TokenTypeId = tokenType.Id;
                }
                if (!string.IsNullOrEmpty(dto.Status))
                {
                    var status = (await uow.MdmVisitStatuses.FindAsync(s => s.Code == dto.Status)).FirstOrDefault();
                    if (status != null) existing.StatusId = status.Id;
                }
                var updated = await uow.Tokens.UpdateAsync(existing);
                await uow.SaveChangesAsync();
                return _mapper.Map<TokenDto>(updated);
            });
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            return await uow.ExecuteTransactionAsync(async () =>
            {
                var existing = await uow.Tokens.GetByIdAsync(id);
                if (existing == null) return false;
                await uow.Tokens.DeleteAsync(id);
                await uow.SaveChangesAsync();
                return true;
            });
        }
    }
}
