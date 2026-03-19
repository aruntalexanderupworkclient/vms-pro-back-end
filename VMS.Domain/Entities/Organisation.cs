using VMS.Domain.Enums;

namespace VMS.Domain.Entities;

public class Organisation : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public OrganisationType Type { get; set; } = OrganisationType.Corporate;
    public string? LogoUrl { get; set; }
    public string Address { get; set; } = string.Empty;

    public ICollection<User> Users { get; set; } = new List<User>();
}
