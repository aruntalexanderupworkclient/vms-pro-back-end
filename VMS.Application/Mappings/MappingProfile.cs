using AutoMapper;
using VMS.Domain.Entities;
using VMS.Application.DTOs;

namespace VMS.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User
        CreateMap<User, UserDto>()
            .ForMember(d => d.RoleName, opt => opt.MapFrom(s => s.Role != null ? s.Role.Name : null))
            .ForMember(d => d.OrganisationName, opt => opt.MapFrom(s => s.Organisation != null ? s.Organisation.Name : null));
        CreateMap<CreateUserDto, User>();
        CreateMap<UpdateUserDto, User>();

        // Role
        CreateMap<Role, RoleDto>()
            .ForMember(d => d.UsersCount, opt => opt.MapFrom(s => s.Users != null ? s.Users.Count : 0))
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
            .ForMember(d => d.TokenId, opt => opt.MapFrom(s => s.Tokens != null && s.Tokens.Any() ? s.Tokens.First().Id : (Guid?)null));
        CreateMap<CreateVisitorDto, Visitor>()
            .ForMember(d => d.FullName, opt => opt.MapFrom(s => s.Name))
            .ForMember(d => d.PhotoUrl, opt => opt.MapFrom(s => s.Photo));
        CreateMap<UpdateVisitorDto, Visitor>()
            .ForMember(d => d.FullName, opt => opt.MapFrom(s => s.Name))
            .ForMember(d => d.PhotoUrl, opt => opt.MapFrom(s => s.Photo))
            .ForMember(d => d.CheckInTime, opt => opt.MapFrom(s => !string.IsNullOrEmpty(s.CheckIn) ? DateTime.Parse(s.CheckIn) : (DateTime?)null))
            .ForMember(d => d.CheckOutTime, opt => opt.MapFrom(s => !string.IsNullOrEmpty(s.CheckOut) ? DateTime.Parse(s.CheckOut) : (DateTime?)null))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => Enum.Parse<VMS.Domain.Enums.VisitStatus>(s.Status, true)));

        // VisitorToken
        CreateMap<VisitorToken, TokenDto>()
            .ForMember(d => d.TokenNo, opt => opt.MapFrom(s => s.TokenNumber))
            .ForMember(d => d.VisitorName, opt => opt.MapFrom(s => s.Visitor != null ? s.Visitor.FullName : string.Empty))
            .ForMember(d => d.Type, opt => opt.MapFrom(s => s.TokenType.ToString()))
            .ForMember(d => d.IssueTime, opt => opt.MapFrom(s => s.IssuedAt.ToString("o")))
            .ForMember(d => d.Expiry, opt => opt.MapFrom(s => s.ExpiresAt.ToString("o")))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.HostName, opt => opt.MapFrom(s => s.Visitor != null && s.Visitor.Host != null ? s.Visitor.Host.Name : string.Empty));
        CreateMap<CreateTokenDto, VisitorToken>()
            .ForMember(d => d.TokenNumber, opt => opt.MapFrom(s => s.TokenNo))
            .ForMember(d => d.TokenType, opt => opt.MapFrom(s => Enum.Parse<VMS.Domain.Enums.TokenType>(s.Type, true)))
            .ForMember(d => d.ExpiresAt, opt => opt.MapFrom(s => DateTime.Parse(s.Expiry)));
        CreateMap<UpdateTokenDto, VisitorToken>()
            .ForMember(d => d.TokenNumber, opt => opt.MapFrom(s => s.TokenNo))
            .ForMember(d => d.TokenType, opt => opt.MapFrom(s => Enum.Parse<VMS.Domain.Enums.TokenType>(s.Type, true)))
            .ForMember(d => d.ExpiresAt, opt => opt.MapFrom(s => DateTime.Parse(s.Expiry)))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => Enum.Parse<VMS.Domain.Enums.VisitStatus>(s.Status, true)));

        // Appointment
        CreateMap<Appointment, AppointmentDto>()
            .ForMember(d => d.HostName, opt => opt.MapFrom(s => s.Host != null ? s.Host.Name : string.Empty))
            .ForMember(d => d.Date, opt => opt.MapFrom(s => s.ScheduledAt.ToString("yyyy-MM-dd")))
            .ForMember(d => d.Time, opt => opt.MapFrom(s => s.ScheduledAt.ToString("HH:mm")))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()));
        CreateMap<CreateAppointmentDto, Appointment>()
            .ForMember(d => d.ScheduledAt, opt => opt.MapFrom(s => DateTime.Parse($"{s.Date} {s.Time}")));
        CreateMap<UpdateAppointmentDto, Appointment>()
            .ForMember(d => d.ScheduledAt, opt => opt.MapFrom(s => DateTime.Parse($"{s.Date} {s.Time}")))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => Enum.Parse<VMS.Domain.Enums.VisitStatus>(s.Status, true)));

        // Employee
        CreateMap<Employee, EmployeeDto>()
            .ForMember(d => d.Name, opt => opt.MapFrom(s => s.FullName))
            .ForMember(d => d.EmployeeId, opt => opt.MapFrom(s => s.EmployeeCode))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()));
        CreateMap<CreateEmployeeDto, Employee>()
            .ForMember(d => d.FullName, opt => opt.MapFrom(s => s.Name))
            .ForMember(d => d.EmployeeCode, opt => opt.MapFrom(s => s.EmployeeId));
        CreateMap<UpdateEmployeeDto, Employee>()
            .ForMember(d => d.FullName, opt => opt.MapFrom(s => s.Name))
            .ForMember(d => d.EmployeeCode, opt => opt.MapFrom(s => s.EmployeeId))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => Enum.Parse<VMS.Domain.Enums.UserStatus>(s.Status, true)));

        // Host
        CreateMap<Host, HostDto>()
            .ForMember(d => d.Unit, opt => opt.MapFrom(s => s.Department))
            .ForMember(d => d.Contact, opt => opt.MapFrom(s => s.ContactNumber))
            .ForMember(d => d.OrgType, opt => opt.MapFrom(s => s.OrganisationType.ToString()))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()));
        CreateMap<CreateHostDto, Host>()
            .ForMember(d => d.Department, opt => opt.MapFrom(s => s.Unit))
            .ForMember(d => d.ContactNumber, opt => opt.MapFrom(s => s.Contact))
            .ForMember(d => d.OrganisationType, opt => opt.MapFrom(s => Enum.Parse<VMS.Domain.Enums.OrganisationType>(s.OrgType, true)));
        CreateMap<UpdateHostDto, Host>()
            .ForMember(d => d.Department, opt => opt.MapFrom(s => s.Unit))
            .ForMember(d => d.ContactNumber, opt => opt.MapFrom(s => s.Contact))
            .ForMember(d => d.OrganisationType, opt => opt.MapFrom(s => Enum.Parse<VMS.Domain.Enums.OrganisationType>(s.OrgType, true)))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => Enum.Parse<VMS.Domain.Enums.UserStatus>(s.Status, true)));

        // Organisation
        CreateMap<Organisation, OrganisationDto>()
            .ForMember(d => d.Type, opt => opt.MapFrom(s => s.Type.ToString()));
        CreateMap<CreateOrganisationDto, Organisation>()
            .ForMember(d => d.Type, opt => opt.MapFrom(s => Enum.Parse<VMS.Domain.Enums.OrganisationType>(s.Type, true)));
        CreateMap<UpdateOrganisationDto, Organisation>()
            .ForMember(d => d.Type, opt => opt.MapFrom(s => Enum.Parse<VMS.Domain.Enums.OrganisationType>(s.Type, true)));
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
}
