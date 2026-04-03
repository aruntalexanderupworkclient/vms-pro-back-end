using System.Collections.Concurrent;
using VMS.Domain.Entities;

namespace VMS.Infrastructure.InMemory;

public class InMemoryDataStore
{
    public ConcurrentDictionary<Guid, User> Users { get; } = new();
    public ConcurrentDictionary<Guid, Role> Roles { get; } = new();
    public ConcurrentDictionary<Guid, Permission> Permissions { get; } = new();
    public ConcurrentDictionary<Guid, Visitor> Visitors { get; } = new();
    public ConcurrentDictionary<Guid, VisitorToken> VisitorTokens { get; } = new();
    public ConcurrentDictionary<Guid, Appointment> Appointments { get; } = new();
    public ConcurrentDictionary<Guid, Employee> Employees { get; } = new();
    public ConcurrentDictionary<Guid, Host> Hosts { get; } = new();
    public ConcurrentDictionary<Guid, Organisation> Organisations { get; } = new();

    // MDM Collections
    public ConcurrentDictionary<Guid, MdmVisitStatus> MdmVisitStatuses { get; } = new();
    public ConcurrentDictionary<Guid, MdmUserStatus> MdmUserStatuses { get; } = new();
    public ConcurrentDictionary<Guid, MdmTokenType> MdmTokenTypes { get; } = new();
    public ConcurrentDictionary<Guid, MdmOrganisationType> MdmOrganisationTypes { get; } = new();

    public InMemoryDataStore()
    {
        SeedData.Seed(this);
    }

    public ConcurrentDictionary<Guid, T> GetCollection<T>() where T : BaseEntity
    {
        return typeof(T).Name switch
        {
            nameof(User) => (ConcurrentDictionary<Guid, T>)(object)Users,
            nameof(Role) => (ConcurrentDictionary<Guid, T>)(object)Roles,
            nameof(Permission) => (ConcurrentDictionary<Guid, T>)(object)Permissions,
            nameof(Visitor) => (ConcurrentDictionary<Guid, T>)(object)Visitors,
            nameof(VisitorToken) => (ConcurrentDictionary<Guid, T>)(object)VisitorTokens,
            nameof(Appointment) => (ConcurrentDictionary<Guid, T>)(object)Appointments,
            nameof(Employee) => (ConcurrentDictionary<Guid, T>)(object)Employees,
            nameof(Host) => (ConcurrentDictionary<Guid, T>)(object)Hosts,
            nameof(Organisation) => (ConcurrentDictionary<Guid, T>)(object)Organisations,
            nameof(MdmVisitStatus) => (ConcurrentDictionary<Guid, T>)(object)MdmVisitStatuses,
            nameof(MdmUserStatus) => (ConcurrentDictionary<Guid, T>)(object)MdmUserStatuses,
            nameof(MdmTokenType) => (ConcurrentDictionary<Guid, T>)(object)MdmTokenTypes,
            nameof(MdmOrganisationType) => (ConcurrentDictionary<Guid, T>)(object)MdmOrganisationTypes,
            _ => throw new InvalidOperationException($"No collection registered for type {typeof(T).Name}")
        };
    }
}
