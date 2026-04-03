namespace VMS.Domain.Entities;

public class Host : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string ContactNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public Guid OrganisationTypeId { get; set; }
    public bool IsActive { get; set; } = true;

    public MdmOrganisationType? OrganisationType { get; set; }
    public ICollection<Visitor> Visitors { get; set; } = new List<Visitor>();
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
