using VMS.Domain.Enums;

namespace VMS.Domain.Entities;

public class Host : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string ContactNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public OrganisationType OrganisationType { get; set; } = OrganisationType.Corporate;
    public UserStatus Status { get; set; } = UserStatus.Active;

    public ICollection<Visitor> Visitors { get; set; } = new List<Visitor>();
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
