namespace VMS.Domain.Entities;

/// <summary>
/// Static constants for MDM Code values used in business logic.
/// These replace the old C# enum references (e.g., VisitStatus.CheckedIn → MdmCodes.VisitStatus.CheckedIn).
/// </summary>
public static class MdmCodes
{
    public static class VisitStatus
    {
        public const string Scheduled = "Scheduled";
        public const string CheckedIn = "CheckedIn";
        public const string CheckedOut = "CheckedOut";
        public const string Cancelled = "Cancelled";
        public const string Expired = "Expired";
    }

    public static class UserStatus
    {
        public const string Active = "Active";
        public const string Inactive = "Inactive";
        public const string Suspended = "Suspended";
        public const string Deleted = "Deleted";
    }

    public static class TokenType
    {
        public const string Visitor = "Visitor";
        public const string Contractor = "Contractor";
        public const string Delivery = "Delivery";
        public const string Temporary = "Temporary";
        public const string VIP = "VIP";
        public const string Guest = "Guest";
    }

    public static class OrganisationType
    {
        public const string Hospital = "Hospital";
        public const string Residential = "Residential";
        public const string Corporate = "Corporate";
        public const string Factory = "Factory";
    }
}

