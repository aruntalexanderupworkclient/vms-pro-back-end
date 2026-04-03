namespace VMS.Domain.Entities;

public class Organisation : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public Guid TypeId { get; set; }
    public string? LogoUrl { get; set; }
    public string Address { get; set; } = string.Empty;

    public MdmOrganisationType? Type { get; set; }
    public ICollection<User> Users { get; set; } = new List<User>();
}
