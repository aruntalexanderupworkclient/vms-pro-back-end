using AutoMapper;
using VMS.Domain.Entities;
using VMS.Domain.Enums;
using VMS.Application.DTOs;
using VMS.Application.Utilities;

namespace VMS.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User
        CreateMap<User, UserDto>()
            .ForMember(d => d.RoleName, opt => opt.MapFrom(s => s.Role != null ? s.Role.Name : null))
            .ForMember(d => d.OrganisationName, opt => opt.MapFrom(s => s.Organisation != null ? s.Organisation.Name : null))
            .ForMember(d => d.StatusId, opt => opt.MapFrom(s => (int)s.Status))
            .ForMember(d => d.StatusLabel, opt => opt.MapFrom(s => EnumHelper.GetUserStatusInfo(s.Status).label));
        CreateMap<CreateUserDto, User>();
        CreateMap<UpdateUserDto, User>();

        // Role
        CreateMap<Role, RoleDto>()
            .ForMember(d => d.UsersCount, opt => opt.MapFrom(s => s.Users.Count))
            .ForMember(d => d.Permissions, opt => opt.MapFrom(s => MapPermissionsToDict(s.Permissions)));
        CreateMap<CreateRoleDto, Role>();
        CreateMap<UpdateRoleDto, Role>()
            .ForMember(d => d.Permissions, opt => opt.Ignore());

        // Permission
        CreateMap<Permission, PermissionDto>()
            .ForMember(d => d.Key, opt => opt.MapFrom(s => $"{s.Module}.{s.Action}"))
            .ForMember(d => d.Roles, opt => opt.MapFrom(s => s.Role != null ? new List<string> { s.Role.Name } : new List<string>()));
        CreateMap<CreatePermissionDto, Permission>();
        CreateMap<UpdatePermissionDto, Permission>();

        // Visitor
        CreateMap<Visitor, VisitorDto>()
            .ForMember(d => d.Name, opt => opt.MapFrom(s => s.FullName))
            .ForMember(d => d.Photo, opt => opt.MapFrom(s => s.PhotoUrl))
            .ForMember(d => d.HostName, opt => opt.MapFrom(s => s.Host != null ? s.Host.Name : string.Empty))
            .ForMember(d => d.CheckIn, opt => opt.MapFrom(s => s.CheckInTime.HasValue ? s.CheckInTime.Value.ToString("o") : null))
            .ForMember(d => d.CheckOut, opt => opt.MapFrom(s => s.CheckOutTime.HasValue ? s.CheckOutTime.Value.ToString("o") : null))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.StatusId, opt => opt.MapFrom(s => (int)s.Status))
            .ForMember(d => d.StatusLabel, opt => opt.MapFrom(s => EnumHelper.GetVisitStatusInfo(s.Status).label))
            .ForMember(d => d.OrgType, opt => opt.MapFrom(s => s.OrgType))
            .ForMember(d => d.OrgTypeId, opt => opt.MapFrom(s => ParseOrgTypeToId(s.OrgType)))
            .ForMember(d => d.OrgTypeLabel, opt => opt.MapFrom(s => ParseOrgTypeToLabel(s.OrgType)))
            .ForMember(d => d.TokenId, opt => opt.MapFrom(s => s.Tokens != null && s.Tokens.Any() ? s.Tokens.First().Id : (Guid?)null));
        CreateMap<CreateVisitorDto, Visitor>()
            .ForMember(d => d.FullName, opt => opt.MapFrom(s => s.Name))
            .ForMember(d => d.PhotoUrl, opt => opt.MapFrom(s => s.Photo));
        CreateMap<UpdateVisitorDto, Visitor>()
            .ForMember(d => d.FullName, opt => opt.MapFrom(s => s.Name))
            .ForMember(d => d.PhotoUrl, opt => opt.MapFrom(s => s.Photo))
            .ForMember(d => d.CheckInTime, opt => opt.MapFrom(s => !string.IsNullOrEmpty(s.CheckIn) ? DateTime.SpecifyKind(DateTime.Parse(s.CheckIn), DateTimeKind.Utc) : (DateTime?)null))
            .ForMember(d => d.CheckOutTime, opt => opt.MapFrom(s => !string.IsNullOrEmpty(s.CheckOut) ? DateTime.SpecifyKind(DateTime.Parse(s.CheckOut), DateTimeKind.Utc) : (DateTime?)null))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => Enum.Parse<VisitStatus>(s.Status, true)));

        // VisitorToken
        CreateMap<VisitorToken, TokenDto>()
            .ForMember(d => d.TokenNo, opt => opt.MapFrom(s => s.TokenNumber))
            .ForMember(d => d.VisitorId, opt => opt.MapFrom(s => s.VisitorId))
            .ForMember(d => d.VisitorName, opt => opt.MapFrom(s => s.Visitor != null ? s.Visitor.FullName : string.Empty))
            .ForMember(d => d.Type, opt => opt.MapFrom(s => s.TokenType.ToString()))
            .ForMember(d => d.TypeId, opt => opt.MapFrom(s => (int)s.TokenType))
            .ForMember(d => d.TypeLabel, opt => opt.MapFrom(s => EnumHelper.GetTokenTypeInfo(s.TokenType).label))
            .ForMember(d => d.IssueTime, opt => opt.MapFrom(s => s.IssuedAt.ToString("o")))
            .ForMember(d => d.Expiry, opt => opt.MapFrom(s => s.ExpiresAt))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.StatusId, opt => opt.MapFrom(s => (int)s.Status))
            .ForMember(d => d.StatusLabel, opt => opt.MapFrom(s => EnumHelper.GetVisitStatusInfo(s.Status).label))
            .ForMember(d => d.HostName, opt => opt.MapFrom(s => s.Visitor != null && s.Visitor.Host != null ? s.Visitor.Host.Name : string.Empty));
        CreateMap<CreateTokenDto, VisitorToken>()
            .ForMember(d => d.TokenNumber, opt => opt.MapFrom(s => s.TokenNo))
            .ForMember(d => d.VisitorId, opt => opt.MapFrom(s => s.VisitorId))
            .ForMember(d => d.TokenType, opt => opt.MapFrom(s => Enum.Parse<TokenType>(s.Type, true)))
            .ForMember(d => d.IssuedAt, opt => opt.MapFrom(s => DateTime.SpecifyKind(s.IssueTime, DateTimeKind.Utc)))
            .ForMember(d => d.ExpiresAt, opt => opt.MapFrom(s => DateTime.SpecifyKind(s.Expiry, DateTimeKind.Utc)))
            .ForMember(d => d.Status, opt => opt.Ignore());
        CreateMap<UpdateTokenDto, VisitorToken>()
            .ForMember(d => d.TokenNumber, opt => opt.MapFrom(s => s.TokenNo))
            .ForMember(d => d.VisitorId, opt => opt.MapFrom(s => s.VisitorId))
            .ForMember(d => d.TokenType, opt => opt.MapFrom(s => Enum.Parse<TokenType>(s.Type, true)))
            .ForMember(d => d.ExpiresAt, opt => opt.MapFrom(s => DateTime.SpecifyKind(s.Expiry, DateTimeKind.Utc)))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => Enum.Parse<VisitStatus>(s.Status, true)))
            .ForMember(d => d.IssuedAt, opt => opt.Ignore());

        // Appointment
        CreateMap<Appointment, AppointmentDto>()
            .ForMember(d => d.HostName, opt => opt.MapFrom(s => s.Host != null ? s.Host.Name : string.Empty))
            .ForMember(d => d.Date, opt => opt.MapFrom(s => s.ScheduledAt.ToString("yyyy-MM-dd")))
            .ForMember(d => d.Time, opt => opt.MapFrom(s => s.ScheduledAt.ToString("HH:mm")))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.StatusId, opt => opt.MapFrom(s => (int)s.Status))
            .ForMember(d => d.StatusLabel, opt => opt.MapFrom(s => EnumHelper.GetVisitStatusInfo(s.Status).label));
        CreateMap<CreateAppointmentDto, Appointment>()
            .ForMember(d => d.ScheduledAt, opt => opt.MapFrom(s => DateTime.SpecifyKind(DateTime.Parse($"{s.Date} {s.Time}"), DateTimeKind.Utc)));
        CreateMap<UpdateAppointmentDto, Appointment>()
            .ForMember(d => d.ScheduledAt, opt => opt.MapFrom(s => DateTime.SpecifyKind(DateTime.Parse($"{s.Date} {s.Time}"), DateTimeKind.Utc)))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => Enum.Parse<VisitStatus>(s.Status, true)));

        // Employee
        CreateMap<Employee, EmployeeDto>()
            .ForMember(d => d.Name, opt => opt.MapFrom(s => s.FullName))
            .ForMember(d => d.EmployeeId, opt => opt.MapFrom(s => s.EmployeeCode))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.StatusId, opt => opt.MapFrom(s => (int)s.Status))
            .ForMember(d => d.StatusLabel, opt => opt.MapFrom(s => EnumHelper.GetUserStatusInfo(s.Status).label));
        CreateMap<CreateEmployeeDto, Employee>()
            .ForMember(d => d.FullName, opt => opt.MapFrom(s => s.Name))
            .ForMember(d => d.EmployeeCode, opt => opt.MapFrom(s => s.EmployeeId));
        CreateMap<UpdateEmployeeDto, Employee>()
            .ForMember(d => d.FullName, opt => opt.MapFrom(s => s.Name))
            .ForMember(d => d.EmployeeCode, opt => opt.MapFrom(s => s.EmployeeId))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => Enum.Parse<UserStatus>(s.Status, true)));

        // Host
        CreateMap<Host, HostDto>()
            .ForMember(d => d.Unit, opt => opt.MapFrom(s => s.Department))
            .ForMember(d => d.Contact, opt => opt.MapFrom(s => s.ContactNumber))
            .ForMember(d => d.OrgType, opt => opt.MapFrom(s => s.OrganisationType.ToString()))
            .ForMember(d => d.OrgTypeId, opt => opt.MapFrom(s => (int)s.OrganisationType))
            .ForMember(d => d.OrgTypeLabel, opt => opt.MapFrom(s => EnumHelper.GetOrganisationTypeInfo(s.OrganisationType).label))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.StatusId, opt => opt.MapFrom(s => (int)s.Status))
            .ForMember(d => d.StatusLabel, opt => opt.MapFrom(s => EnumHelper.GetUserStatusInfo(s.Status).label));
        CreateMap<CreateHostDto, Host>()
            .ForMember(d => d.Department, opt => opt.MapFrom(s => s.Unit))
            .ForMember(d => d.ContactNumber, opt => opt.MapFrom(s => s.Contact))
            .ForMember(d => d.OrganisationType, opt => opt.MapFrom(s => Enum.Parse<OrganisationType>(s.OrgType, true)));
        CreateMap<UpdateHostDto, Host>()
            .ForMember(d => d.Department, opt => opt.MapFrom(s => s.Unit))
            .ForMember(d => d.ContactNumber, opt => opt.MapFrom(s => s.Contact))
            .ForMember(d => d.OrganisationType, opt => opt.MapFrom(s => Enum.Parse<OrganisationType>(s.OrgType, true)))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => Enum.Parse<UserStatus>(s.Status, true)));

        // Organisation
        CreateMap<Organisation, OrganisationDto>()
            .ForMember(d => d.Type, opt => opt.MapFrom(s => s.Type.ToString()));
        CreateMap<CreateOrganisationDto, Organisation>()
            .ForMember(d => d.Type, opt => opt.MapFrom(s => Enum.Parse<OrganisationType>(s.Type, true)));
        CreateMap<UpdateOrganisationDto, Organisation>()
            .ForMember(d => d.Type, opt => opt.MapFrom(s => Enum.Parse<OrganisationType>(s.Type, true)));
    }

    private static Dictionary<string, ModulePermissions> MapPermissionsToDict(ICollection<Permission>? permissions)
    {
        var dict = new Dictionary<string, ModulePermissions>();
        if (permissions == null) return dict;

        foreach (var perm in permissions)
        {
            if (!dict.ContainsKey(perm.Module))
            {
                dict[perm.Module] = new ModulePermissions();
            }

            var mp = dict[perm.Module];
            switch (perm.Action.ToLower())
            {
                case "view": mp.View = true; break;
                case "create": mp.Create = true; break;
                case "edit": mp.Edit = true; break;
                case "delete": mp.Delete = true; break;
            }
        }

        return dict;
    }

    private static int ParseOrgTypeToId(string? orgType)
    {
        if (string.IsNullOrEmpty(orgType)) return 0;
        if (Enum.TryParse<OrganisationType>(orgType, true, out var result))
            return (int)result;
        return 0;
    }

    private static string ParseOrgTypeToLabel(string? orgType)
    {
        if (string.IsNullOrEmpty(orgType)) return string.Empty;
        if (Enum.TryParse<OrganisationType>(orgType, true, out var result))
            return EnumHelper.GetOrganisationTypeInfo(result).label;
        return string.Empty;
    }
}
