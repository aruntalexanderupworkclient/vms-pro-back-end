using System.Collections.Generic;
using System.Linq;
using VMS.Domain.Enums;

namespace VMS.Application.Utilities;

public class EnumOption
{
    public int Id { get; set; }
    public string Value { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
}

public static class EnumHelper
{
    // Enum metadata mappings
    private static readonly Dictionary<int, (string value, string label)> OrganisationTypeMap = new()
    {
        { 0, ("Hospital", "Hospital Building") },
        { 1, ("Residential", "Residential Tower") },
        { 2, ("Corporate", "Corporate Office") },
        { 3, ("Factory", "Manufacturing Facility") }
    };

    private static readonly Dictionary<int, (string value, string label)> UserStatusMap = new()
    {
        { 0, ("Active", "User is Active") },
        { 1, ("Inactive", "User is Inactive") },
        { 2, ("Suspended", "User is Suspended") },
        { 3, ("Deleted", "User is Deleted") }
    };

    private static readonly Dictionary<int, (string value, string label)> TokenTypeMap = new()
    {
        { 0, ("Visitor", "Visitor Pass") },
        { 1, ("Contractor", "Contractor Pass") },
        { 2, ("Delivery", "Delivery Person") },
        { 3, ("Temporary", "Temporary Pass") },
        { 4, ("VIP", "VIP Pass") },
        { 5, ("Guest", "Guest Pass") }
    };

    private static readonly Dictionary<int, (string value, string label)> VisitStatusMap = new()
    {
        { 0, ("Scheduled", "Visit is Scheduled") },
        { 1, ("CheckedIn", "Visitor Checked In") },
        { 2, ("CheckedOut", "Visitor Checked Out") },
        { 3, ("Cancelled", "Visit Cancelled") },
        { 4, ("Expired", "Visit Expired") }
    };

    // Get all enum options as lists
    public static List<EnumOption> GetOrganisationTypeOptions()
        => ConvertToOptions(OrganisationTypeMap);

    public static List<EnumOption> GetUserStatusOptions()
        => ConvertToOptions(UserStatusMap);

    public static List<EnumOption> GetTokenTypeOptions()
        => ConvertToOptions(TokenTypeMap);

    public static List<EnumOption> GetVisitStatusOptions()
        => ConvertToOptions(VisitStatusMap);

    // Get specific enum display info
    public static (string value, string label) GetOrganisationTypeInfo(OrganisationType type)
        => OrganisationTypeMap.TryGetValue((int)type, out var info) 
            ? info 
            : (type.ToString(), type.ToString());

    public static (string value, string label) GetUserStatusInfo(UserStatus status)
        => UserStatusMap.TryGetValue((int)status, out var info) 
            ? info 
            : (status.ToString(), status.ToString());

    public static (string value, string label) GetTokenTypeInfo(TokenType type)
        => TokenTypeMap.TryGetValue((int)type, out var info) 
            ? info 
            : (type.ToString(), type.ToString());

    public static (string value, string label) GetVisitStatusInfo(VisitStatus status)
        => VisitStatusMap.TryGetValue((int)status, out var info) 
            ? info 
            : (status.ToString(), status.ToString());

    // Helper to convert dictionary to options list
    private static List<EnumOption> ConvertToOptions(Dictionary<int, (string, string)> map)
        => map.Select(x => new EnumOption { Id = x.Key, Value = x.Value.Item1, Label = x.Value.Item2 })
            .ToList();
}

