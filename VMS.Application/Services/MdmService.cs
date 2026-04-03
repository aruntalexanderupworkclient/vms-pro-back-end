using AutoMapper;
using VMS.Application.DTOs;
using VMS.Application.Interfaces;
using VMS.Domain.Entities;
using VMS.Infrastructure.Repositories.Interfaces;
using VMS.Infrastructure.Repositories.UnitOfWork;

namespace VMS.Application.Services;

public class MdmService : IMdmService
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IMapper _mapper;

    public MdmService(IUnitOfWorkFactory unitOfWorkFactory, IMapper mapper)
    {
        _unitOfWorkFactory = unitOfWorkFactory ?? throw new ArgumentNullException(nameof(unitOfWorkFactory));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<List<MdmDto>> GetAllAsync(string mdmType)
    {
        using var uow = _unitOfWorkFactory.CreateUnitOfWork();
        IEnumerable<BaseMdmEntity> items = mdmType switch
        {
            "VisitStatus" => await uow.MdmVisitStatuses.GetAllAsync(),
            "UserStatus" => await uow.MdmUserStatuses.GetAllAsync(),
            "TokenType" => await uow.MdmTokenTypes.GetAllAsync(),
            "OrganisationType" => await uow.MdmOrganisationTypes.GetAllAsync(),
            _ => throw new ArgumentException($"Unknown MDM type: {mdmType}")
        };
        return _mapper.Map<List<MdmDto>>(items.OrderBy(x => x.SortOrder));
    }

    public async Task<MdmDto?> GetByIdAsync(string mdmType, Guid id)
    {
        using var uow = _unitOfWorkFactory.CreateUnitOfWork();
        BaseMdmEntity? entity = mdmType switch
        {
            "VisitStatus" => await uow.MdmVisitStatuses.GetByIdAsync(id),
            "UserStatus" => await uow.MdmUserStatuses.GetByIdAsync(id),
            "TokenType" => await uow.MdmTokenTypes.GetByIdAsync(id),
            "OrganisationType" => await uow.MdmOrganisationTypes.GetByIdAsync(id),
            _ => throw new ArgumentException($"Unknown MDM type: {mdmType}")
        };
        return entity == null ? null : _mapper.Map<MdmDto>(entity);
    }

    public async Task<MdmDto> CreateAsync(string mdmType, CreateMdmDto dto)
    {
        using var uow = _unitOfWorkFactory.CreateUnitOfWork();
        return await uow.ExecuteTransactionAsync(async () =>
        {
            BaseMdmEntity created = mdmType switch
            {
                "VisitStatus" => await uow.MdmVisitStatuses.AddAsync(_mapper.Map<MdmVisitStatus>(dto)),
                "UserStatus" => await uow.MdmUserStatuses.AddAsync(_mapper.Map<MdmUserStatus>(dto)),
                "TokenType" => await uow.MdmTokenTypes.AddAsync(_mapper.Map<MdmTokenType>(dto)),
                "OrganisationType" => await uow.MdmOrganisationTypes.AddAsync(_mapper.Map<MdmOrganisationType>(dto)),
                _ => throw new ArgumentException($"Unknown MDM type: {mdmType}")
            };
            await uow.SaveChangesAsync();
            return _mapper.Map<MdmDto>(created);
        });
    }

    public async Task<MdmDto?> UpdateAsync(string mdmType, Guid id, UpdateMdmDto dto)
    {
        using var uow = _unitOfWorkFactory.CreateUnitOfWork();
        return await uow.ExecuteTransactionAsync(async () =>
        {
            BaseMdmEntity? existing = mdmType switch
            {
                "VisitStatus" => await uow.MdmVisitStatuses.GetByIdAsync(id),
                "UserStatus" => await uow.MdmUserStatuses.GetByIdAsync(id),
                "TokenType" => await uow.MdmTokenTypes.GetByIdAsync(id),
                "OrganisationType" => await uow.MdmOrganisationTypes.GetByIdAsync(id),
                _ => throw new ArgumentException($"Unknown MDM type: {mdmType}")
            };
            if (existing == null) return null;

            existing.Code = dto.Code;
            existing.Value = dto.Value;
            existing.SortOrder = dto.SortOrder;
            existing.IsActive = dto.IsActive;

            BaseMdmEntity updated = mdmType switch
            {
                "VisitStatus" => await uow.MdmVisitStatuses.UpdateAsync((MdmVisitStatus)existing),
                "UserStatus" => await uow.MdmUserStatuses.UpdateAsync((MdmUserStatus)existing),
                "TokenType" => await uow.MdmTokenTypes.UpdateAsync((MdmTokenType)existing),
                "OrganisationType" => await uow.MdmOrganisationTypes.UpdateAsync((MdmOrganisationType)existing),
                _ => throw new ArgumentException($"Unknown MDM type: {mdmType}")
            };
            await uow.SaveChangesAsync();
            return _mapper.Map<MdmDto>(updated);
        });
    }

    public async Task<bool> DeleteAsync(string mdmType, Guid id)
    {
        using var uow = _unitOfWorkFactory.CreateUnitOfWork();
        return await uow.ExecuteTransactionAsync(async () =>
        {
            switch (mdmType)
            {
                case "VisitStatus": await uow.MdmVisitStatuses.DeleteAsync(id); break;
                case "UserStatus": await uow.MdmUserStatuses.DeleteAsync(id); break;
                case "TokenType": await uow.MdmTokenTypes.DeleteAsync(id); break;
                case "OrganisationType": await uow.MdmOrganisationTypes.DeleteAsync(id); break;
                default: throw new ArgumentException($"Unknown MDM type: {mdmType}");
            }
            await uow.SaveChangesAsync();
            return true;
        });
    }
}

