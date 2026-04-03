using VMS.Domain.Entities;

namespace VMS.Infrastructure.InMemory;

public static class SeedData
{
    // ─── Well-known MDM IDs (stable GUIDs for seed data) ───
    private static class MdmIds
    {
        // VisitStatus
        public static readonly Guid Scheduled   = Guid.Parse("00000000-0000-0000-0001-000000000001");
        public static readonly Guid CheckedIn   = Guid.Parse("00000000-0000-0000-0001-000000000002");
        public static readonly Guid CheckedOut  = Guid.Parse("00000000-0000-0000-0001-000000000003");
        public static readonly Guid Cancelled   = Guid.Parse("00000000-0000-0000-0001-000000000004");
        public static readonly Guid Expired     = Guid.Parse("00000000-0000-0000-0001-000000000005");

        // UserStatus
        public static readonly Guid Active      = Guid.Parse("00000000-0000-0000-0002-000000000001");
        public static readonly Guid Inactive    = Guid.Parse("00000000-0000-0000-0002-000000000002");
        public static readonly Guid Suspended   = Guid.Parse("00000000-0000-0000-0002-000000000003");
        public static readonly Guid Deleted     = Guid.Parse("00000000-0000-0000-0002-000000000004");

        // TokenType
        public static readonly Guid TkVisitor    = Guid.Parse("00000000-0000-0000-0003-000000000001");
        public static readonly Guid TkContractor = Guid.Parse("00000000-0000-0000-0003-000000000002");
        public static readonly Guid TkDelivery   = Guid.Parse("00000000-0000-0000-0003-000000000003");
        public static readonly Guid TkTemporary  = Guid.Parse("00000000-0000-0000-0003-000000000004");
        public static readonly Guid TkVIP        = Guid.Parse("00000000-0000-0000-0003-000000000005");
        public static readonly Guid TkGuest      = Guid.Parse("00000000-0000-0000-0003-000000000006");

        // OrganisationType
        public static readonly Guid Hospital     = Guid.Parse("00000000-0000-0000-0004-000000000001");
        public static readonly Guid Residential  = Guid.Parse("00000000-0000-0000-0004-000000000002");
        public static readonly Guid Corporate    = Guid.Parse("00000000-0000-0000-0004-000000000003");
        public static readonly Guid Factory      = Guid.Parse("00000000-0000-0000-0004-000000000004");
    }

    public static void Seed(InMemoryDataStore store)
    {
        SeedMdmTables(store);
        var orgIds = SeedOrganisations(store);
        var roleIds = SeedRoles(store);
        SeedPermissions(store, roleIds);
        SeedUsers(store, roleIds, orgIds);
        var hostIds = SeedHosts(store);
        SeedVisitors(store, hostIds);
        SeedAppointments(store, hostIds);
        SeedEmployees(store);
    }

    private static void SeedMdmTables(InMemoryDataStore store)
    {
        // VisitStatus
        var visitStatuses = new List<MdmVisitStatus>
        {
            new() { Id = MdmIds.Scheduled,  Code = "Scheduled",  Value = "Visit is Scheduled",   SortOrder = 0 },
            new() { Id = MdmIds.CheckedIn,  Code = "CheckedIn",  Value = "Visitor Checked In",    SortOrder = 1 },
            new() { Id = MdmIds.CheckedOut, Code = "CheckedOut", Value = "Visitor Checked Out",   SortOrder = 2 },
            new() { Id = MdmIds.Cancelled,  Code = "Cancelled",  Value = "Visit Cancelled",       SortOrder = 3 },
            new() { Id = MdmIds.Expired,    Code = "Expired",    Value = "Visit Expired",          SortOrder = 4 }
        };
        foreach (var s in visitStatuses) store.MdmVisitStatuses.TryAdd(s.Id, s);

        // UserStatus
        var userStatuses = new List<MdmUserStatus>
        {
            new() { Id = MdmIds.Active,    Code = "Active",    Value = "User is Active",    SortOrder = 0 },
            new() { Id = MdmIds.Inactive,  Code = "Inactive",  Value = "User is Inactive",  SortOrder = 1 },
            new() { Id = MdmIds.Suspended, Code = "Suspended", Value = "User is Suspended", SortOrder = 2 },
            new() { Id = MdmIds.Deleted,   Code = "Deleted",   Value = "User is Deleted",   SortOrder = 3 }
        };
        foreach (var s in userStatuses) store.MdmUserStatuses.TryAdd(s.Id, s);

        // TokenType
        var tokenTypes = new List<MdmTokenType>
        {
            new() { Id = MdmIds.TkVisitor,    Code = "Visitor",    Value = "Visitor Pass",    SortOrder = 0 },
            new() { Id = MdmIds.TkContractor, Code = "Contractor", Value = "Contractor Pass", SortOrder = 1 },
            new() { Id = MdmIds.TkDelivery,   Code = "Delivery",   Value = "Delivery Person", SortOrder = 2 },
            new() { Id = MdmIds.TkTemporary,  Code = "Temporary",  Value = "Temporary Pass",  SortOrder = 3 },
            new() { Id = MdmIds.TkVIP,        Code = "VIP",        Value = "VIP Pass",        SortOrder = 4 },
            new() { Id = MdmIds.TkGuest,      Code = "Guest",      Value = "Guest Pass",      SortOrder = 5 }
        };
        foreach (var t in tokenTypes) store.MdmTokenTypes.TryAdd(t.Id, t);

        // OrganisationType
        var orgTypes = new List<MdmOrganisationType>
        {
            new() { Id = MdmIds.Hospital,    Code = "Hospital",    Value = "Hospital Building",       SortOrder = 0 },
            new() { Id = MdmIds.Residential, Code = "Residential", Value = "Residential Tower",       SortOrder = 1 },
            new() { Id = MdmIds.Corporate,   Code = "Corporate",   Value = "Corporate Office",        SortOrder = 2 },
            new() { Id = MdmIds.Factory,     Code = "Factory",     Value = "Manufacturing Facility",  SortOrder = 3 }
        };
        foreach (var o in orgTypes) store.MdmOrganisationTypes.TryAdd(o.Id, o);
    }

    private static Dictionary<string, Guid> SeedOrganisations(InMemoryDataStore store)
    {
        var orgIds = new Dictionary<string, Guid>
        {
            { "Hospital", Guid.Parse("550e8400-e29b-41d4-a716-446655440001") },
            { "Residential", Guid.Parse("550e8400-e29b-41d4-a716-446655440002") },
            { "Corporate", Guid.Parse("550e8400-e29b-41d4-a716-446655440003") },
            { "Factory", Guid.Parse("550e8400-e29b-41d4-a716-446655440004") }
        };

        var orgs = new List<Organisation>
        {
            new() { Id = orgIds["Hospital"], Name = "City General Hospital", TypeId = MdmIds.Hospital, LogoUrl = "/logos/hospital.png", Address = "123 Medical Drive, Mumbai" },
            new() { Id = orgIds["Residential"], Name = "Greenview Residences", TypeId = MdmIds.Residential, LogoUrl = "/logos/residential.png", Address = "45 Green Avenue, Bangalore" },
            new() { Id = orgIds["Corporate"], Name = "TechNova Corp", TypeId = MdmIds.Corporate, LogoUrl = "/logos/corporate.png", Address = "78 Tech Park, Hyderabad" },
            new() { Id = orgIds["Factory"], Name = "SteelWorks Manufacturing", TypeId = MdmIds.Factory, LogoUrl = "/logos/factory.png", Address = "90 Industrial Zone, Pune" }
        };

        var map = new Dictionary<string, Guid>();
        foreach (var org in orgs)
        {
            store.Organisations.TryAdd(org.Id, org);
            map[org.Name.Split(' ')[0]] = org.Id; // rough key
        }
        // Use stable keys
        return new Dictionary<string, Guid>
        {
            { "Hospital", orgIds["Hospital"] },
            { "Residential", orgIds["Residential"] },
            { "Corporate", orgIds["Corporate"] },
            { "Factory", orgIds["Factory"] }
        };
    }

    private static Dictionary<string, Guid> SeedRoles(InMemoryDataStore store)
    {
        var roleIds = new Dictionary<string, Guid>
        {
            { "Admin", Guid.Parse("550e8400-e29b-41d4-a716-446655440101") },
            { "Manager", Guid.Parse("550e8400-e29b-41d4-a716-446655440102") },
            { "Receptionist", Guid.Parse("550e8400-e29b-41d4-a716-446655440103") },
            { "Security", Guid.Parse("550e8400-e29b-41d4-a716-446655440104") },
            { "Viewer", Guid.Parse("550e8400-e29b-41d4-a716-446655440105") }
        };

        var roles = new List<Role>
        {
            new() { Id = roleIds["Admin"], Name = "Admin", Description = "Full system access with all permissions" },
            new() { Id = roleIds["Manager"], Name = "Manager", Description = "Can manage visitors, employees, and view reports" },
            new() { Id = roleIds["Receptionist"], Name = "Receptionist", Description = "Can check in/out visitors and manage tokens" },
            new() { Id = roleIds["Security"], Name = "Security", Description = "Can view visitor logs and verify tokens" },
            new() { Id = roleIds["Viewer"], Name = "Viewer", Description = "Read-only access to visitor and appointment data" }
        };

        var map = new Dictionary<string, Guid>();
        foreach (var role in roles)
        {
            store.Roles.TryAdd(role.Id, role);
            map[role.Name] = role.Id;
        }
        return map;
    }

    private static void SeedPermissions(InMemoryDataStore store, Dictionary<string, Guid> roleIds)
    {
        var modules = new[] { "Users", "Roles", "Visitors", "Tokens", "Appointments", "Employees", "Hosts", "Reports", "Settings" };
        var actions = new[] { "View", "Create", "Edit", "Delete" };
        int permId = 200;

        foreach (var module in modules)
            foreach (var action in actions)
            {
                var perm = new Permission { Id = Guid.Parse($"550e8400-e29b-41d4-a716-44665544{permId:D4}"), Module = module, Action = action, RoleId = roleIds["Admin"] };
                store.Permissions.TryAdd(perm.Id, perm);
                permId++;
            }

        var managerModules = new[] { "Visitors", "Tokens", "Appointments", "Employees", "Hosts", "Reports" };
        foreach (var module in managerModules)
            foreach (var action in new[] { "View", "Create", "Edit" })
            {
                var perm = new Permission { Id = Guid.Parse($"550e8400-e29b-41d4-a716-44665544{permId:D4}"), Module = module, Action = action, RoleId = roleIds["Manager"] };
                store.Permissions.TryAdd(perm.Id, perm);
                permId++;
            }

        var receptionistModules = new[] { "Visitors", "Tokens", "Appointments" };
        foreach (var module in receptionistModules)
            foreach (var action in new[] { "View", "Create", "Edit" })
            {
                var perm = new Permission { Id = Guid.Parse($"550e8400-e29b-41d4-a716-44665544{permId:D4}"), Module = module, Action = action, RoleId = roleIds["Receptionist"] };
                store.Permissions.TryAdd(perm.Id, perm);
                permId++;
            }

        foreach (var module in new[] { "Visitors", "Tokens" })
        {
            var perm = new Permission { Id = Guid.Parse($"550e8400-e29b-41d4-a716-44665544{permId:D4}"), Module = module, Action = "View", RoleId = roleIds["Security"] };
            store.Permissions.TryAdd(perm.Id, perm);
            permId++;
        }

        foreach (var module in new[] { "Visitors", "Appointments" })
        {
            var perm = new Permission { Id = Guid.Parse($"550e8400-e29b-41d4-a716-44665544{permId:D4}"), Module = module, Action = "View", RoleId = roleIds["Viewer"] };
            store.Permissions.TryAdd(perm.Id, perm);
            permId++;
        }
    }

    private static void SeedUsers(InMemoryDataStore store, Dictionary<string, Guid> roleIds, Dictionary<string, Guid> orgIds)
    {
        var demoHash = "$2a$11$K0jYhRqJb0VlGKzzSHU2XOFqLmDn8Re3LUjHQhSvE7VZRnMXi6kCa";

        var userIds = new Dictionary<string, Guid>
        {
            { "admin", Guid.Parse("550e8400-e29b-41d4-a716-446655440501") },
            { "manager", Guid.Parse("550e8400-e29b-41d4-a716-446655440502") },
            { "receptionist", Guid.Parse("550e8400-e29b-41d4-a716-446655440503") },
            { "security", Guid.Parse("550e8400-e29b-41d4-a716-446655440504") },
            { "viewer", Guid.Parse("550e8400-e29b-41d4-a716-446655440505") }
        };

        var users = new List<User>
        {
            new() { Id = userIds["admin"], FullName = "Arun Alexander", Email = "admin@vms.com", Phone = "+91-9000000001", PasswordHash = demoHash, RoleId = roleIds["Admin"], StatusId = MdmIds.Active, OrganisationId = orgIds["Corporate"] },
            new() { Id = userIds["manager"], FullName = "Priya Sharma", Email = "manager@vms.com", Phone = "+91-9000000002", PasswordHash = demoHash, RoleId = roleIds["Manager"], StatusId = MdmIds.Active, OrganisationId = orgIds["Corporate"] },
            new() { Id = userIds["receptionist"], FullName = "Rahul Verma", Email = "reception@vms.com", Phone = "+91-9000000003", PasswordHash = demoHash, RoleId = roleIds["Receptionist"], StatusId = MdmIds.Active, OrganisationId = orgIds["Hospital"] },
            new() { Id = userIds["security"], FullName = "Deepak Kumar", Email = "security@vms.com", Phone = "+91-9000000004", PasswordHash = demoHash, RoleId = roleIds["Security"], StatusId = MdmIds.Active, OrganisationId = orgIds["Factory"] },
            new() { Id = userIds["viewer"], FullName = "Meera Patel", Email = "viewer@vms.com", Phone = "+91-9000000005", PasswordHash = demoHash, RoleId = roleIds["Viewer"], StatusId = MdmIds.Active, OrganisationId = orgIds["Residential"] }
        };

        foreach (var user in users)
            store.Users.TryAdd(user.Id, user);
    }

    private static List<Guid> SeedHosts(InMemoryDataStore store)
    {
        var hostIds = new List<Guid>
        {
            Guid.Parse("550e8400-e29b-41d4-a716-446655440601"),
            Guid.Parse("550e8400-e29b-41d4-a716-446655440602"),
            Guid.Parse("550e8400-e29b-41d4-a716-446655440603"),
            Guid.Parse("550e8400-e29b-41d4-a716-446655440604"),
            Guid.Parse("550e8400-e29b-41d4-a716-446655440605"),
            Guid.Parse("550e8400-e29b-41d4-a716-446655440606")
        };

        var hosts = new List<Host>
        {
            new() { Id = hostIds[0], Name = "Dr. Anjali Mehta", Department = "Cardiology", ContactNumber = "+91-9100000001", Email = "anjali.mehta@hospital.com", OrganisationTypeId = MdmIds.Hospital, IsActive = true },
            new() { Id = hostIds[1], Name = "Vikram Singh", Department = "IT Infrastructure", ContactNumber = "+91-9100000002", Email = "vikram.singh@technova.com", OrganisationTypeId = MdmIds.Corporate, IsActive = true },
            new() { Id = hostIds[2], Name = "Sunita Reddy", Department = "Human Resources", ContactNumber = "+91-9100000003", Email = "sunita.reddy@technova.com", OrganisationTypeId = MdmIds.Corporate, IsActive = true },
            new() { Id = hostIds[3], Name = "Rajesh Gupta", Department = "Production", ContactNumber = "+91-9100000004", Email = "rajesh.gupta@steelworks.com", OrganisationTypeId = MdmIds.Factory, IsActive = true },
            new() { Id = hostIds[4], Name = "Kavitha Nair", Department = "Administration", ContactNumber = "+91-9100000005", Email = "kavitha.nair@greenview.com", OrganisationTypeId = MdmIds.Residential, IsActive = true },
            new() { Id = hostIds[5], Name = "Mohammed Farooq", Department = "Maintenance", ContactNumber = "+91-9100000006", Email = "farooq@greenview.com", OrganisationTypeId = MdmIds.Residential, IsActive = false }
        };

        foreach (var host in hosts)
            store.Hosts.TryAdd(host.Id, host);
        return hostIds;
    }

    private static void SeedVisitors(InMemoryDataStore store, List<Guid> hostIds)
    {
        var visitorIds = new List<Guid>
        {
            Guid.Parse("550e8400-e29b-41d4-a716-446655440701"), Guid.Parse("550e8400-e29b-41d4-a716-446655440702"),
            Guid.Parse("550e8400-e29b-41d4-a716-446655440703"), Guid.Parse("550e8400-e29b-41d4-a716-446655440704"),
            Guid.Parse("550e8400-e29b-41d4-a716-446655440705"), Guid.Parse("550e8400-e29b-41d4-a716-446655440706"),
            Guid.Parse("550e8400-e29b-41d4-a716-446655440707"), Guid.Parse("550e8400-e29b-41d4-a716-446655440708"),
            Guid.Parse("550e8400-e29b-41d4-a716-446655440709"), Guid.Parse("550e8400-e29b-41d4-a716-446655440710")
        };

        var visitors = new List<Visitor>
        {
            new() { Id = visitorIds[0], FullName = "Amit Joshi", IdType = "Aadhaar", IdNumber = "1234-5678-9012", Phone = "+91-9200000001", HostId = hostIds[0], Purpose = "Medical consultation", CheckInTime = DateTime.UtcNow.AddHours(-2), StatusId = MdmIds.CheckedIn, ExpectedDuration = "2 hours", OrgTypeId = MdmIds.Hospital },
            new() { Id = visitorIds[1], FullName = "Neha Kapoor", IdType = "Driving License", IdNumber = "DL-0420180012345", Phone = "+91-9200000002", HostId = hostIds[1], Purpose = "Job interview", CheckInTime = DateTime.UtcNow.AddHours(-3), CheckOutTime = DateTime.UtcNow.AddHours(-1), StatusId = MdmIds.CheckedOut, ExpectedDuration = "1 hour", OrgTypeId = MdmIds.Corporate },
            new() { Id = visitorIds[2], FullName = "Sanjay Dubey", IdType = "Passport", IdNumber = "J8765432", Phone = "+91-9200000003", HostId = hostIds[2], Purpose = "Vendor meeting", StatusId = MdmIds.Scheduled, ExpectedDuration = "45 mins", OrgTypeId = MdmIds.Corporate },
            new() { Id = visitorIds[3], FullName = "Lakshmi Iyer", IdType = "Aadhaar", IdNumber = "9876-5432-1098", Phone = "+91-9200000004", HostId = hostIds[3], Purpose = "Equipment delivery", CheckInTime = DateTime.UtcNow.AddHours(-1), StatusId = MdmIds.CheckedIn, ExpectedDuration = "30 mins", OrgTypeId = MdmIds.Factory },
            new() { Id = visitorIds[4], FullName = "Omar Sheikh", IdType = "Voter ID", IdNumber = "XYZ1234567", Phone = "+91-9200000005", HostId = hostIds[4], Purpose = "Property viewing", StatusId = MdmIds.Scheduled, ExpectedDuration = "1 hour", OrgTypeId = MdmIds.Residential },
            new() { Id = visitorIds[5], FullName = "Pooja Malhotra", IdType = "PAN Card", IdNumber = "ABCDE1234F", Phone = "+91-9200000006", HostId = hostIds[1], Purpose = "Client presentation", CheckInTime = DateTime.UtcNow.AddDays(-1), CheckOutTime = DateTime.UtcNow.AddDays(-1).AddHours(2), StatusId = MdmIds.CheckedOut, ExpectedDuration = "2 hours", OrgTypeId = MdmIds.Corporate },
            new() { Id = visitorIds[6], FullName = "Karthik Rajan", IdType = "Aadhaar", IdNumber = "5678-1234-9012", Phone = "+91-9200000007", HostId = hostIds[0], Purpose = "Lab test follow-up", StatusId = MdmIds.Cancelled, ExpectedDuration = "30 mins", OrgTypeId = MdmIds.Hospital },
            new() { Id = visitorIds[7], FullName = "Fatima Begum", IdType = "Driving License", IdNumber = "KA-0520190067890", Phone = "+91-9200000008", HostId = hostIds[2], Purpose = "HR documentation", CheckInTime = DateTime.UtcNow.AddMinutes(-30), StatusId = MdmIds.CheckedIn, ExpectedDuration = "1 hour", OrgTypeId = MdmIds.Corporate },
            new() { Id = visitorIds[8], FullName = "Ravi Teja", IdType = "Aadhaar", IdNumber = "3456-7890-1234", Phone = "+91-9200000009", HostId = hostIds[3], Purpose = "Safety audit", StatusId = MdmIds.Expired, ExpectedDuration = "3 hours", OrgTypeId = MdmIds.Factory },
            new() { Id = visitorIds[9], FullName = "Anita Deshmukh", IdType = "Passport", IdNumber = "K1234567", Phone = "+91-9200000010", HostId = hostIds[5], Purpose = "Maintenance request", CheckInTime = DateTime.UtcNow.AddHours(-4), StatusId = MdmIds.CheckedIn, ExpectedDuration = "1 hour", OrgTypeId = MdmIds.Residential }
        };

        var tokenTypeIds = new[] { MdmIds.TkVisitor, MdmIds.TkContractor, MdmIds.TkDelivery, MdmIds.TkTemporary, MdmIds.TkVIP };
        var tokenIds = new List<Guid>
        {
            Guid.Parse("550e8400-e29b-41d4-a716-446655440801"), Guid.Parse("550e8400-e29b-41d4-a716-446655440802"),
            Guid.Parse("550e8400-e29b-41d4-a716-446655440803"), Guid.Parse("550e8400-e29b-41d4-a716-446655440804"),
            Guid.Parse("550e8400-e29b-41d4-a716-446655440805"), Guid.Parse("550e8400-e29b-41d4-a716-446655440806"),
            Guid.Parse("550e8400-e29b-41d4-a716-446655440807"), Guid.Parse("550e8400-e29b-41d4-a716-446655440808"),
            Guid.Parse("550e8400-e29b-41d4-a716-446655440809"), Guid.Parse("550e8400-e29b-41d4-a716-446655440810"),
            Guid.Parse("550e8400-e29b-41d4-a716-446655440811"), Guid.Parse("550e8400-e29b-41d4-a716-446655440812"),
            Guid.Parse("550e8400-e29b-41d4-a716-446655440813"), Guid.Parse("550e8400-e29b-41d4-a716-446655440814"),
            Guid.Parse("550e8400-e29b-41d4-a716-446655440815"), Guid.Parse("550e8400-e29b-41d4-a716-446655440816")
        };
        int tokenIndex = 0;

        foreach (var visitor in visitors)
        {
            store.Visitors.TryAdd(visitor.Id, visitor);

            if (visitor.StatusId == MdmIds.CheckedIn || visitor.StatusId == MdmIds.CheckedOut)
            {
                var token = new VisitorToken
                {
                    Id = tokenIds[tokenIndex],
                    TokenNumber = $"TKN-{1000 + tokenIndex}",
                    VisitorId = visitor.Id,
                    TokenTypeId = tokenTypeIds[tokenIndex % tokenTypeIds.Length],
                    IssuedAt = visitor.CheckInTime ?? DateTime.UtcNow,
                    ExpiresAt = (visitor.CheckInTime ?? DateTime.UtcNow).AddHours(8),
                    StatusId = visitor.StatusId
                };
                store.VisitorTokens.TryAdd(token.Id, token);
                tokenIndex++;
            }
        }

        var extraTokens = new List<VisitorToken>
        {
            new() { Id = tokenIds[10], TokenNumber = "TKN-2001", VisitorId = visitors[2].Id, TokenTypeId = MdmIds.TkVisitor, IssuedAt = DateTime.UtcNow.AddDays(-5), ExpiresAt = DateTime.UtcNow.AddDays(-5).AddHours(8), StatusId = MdmIds.Expired },
            new() { Id = tokenIds[11], TokenNumber = "TKN-2002", VisitorId = visitors[4].Id, TokenTypeId = MdmIds.TkVIP, IssuedAt = DateTime.UtcNow.AddDays(-3), ExpiresAt = DateTime.UtcNow.AddDays(-3).AddHours(8), StatusId = MdmIds.Expired },
            new() { Id = tokenIds[12], TokenNumber = "TKN-2003", VisitorId = visitors[0].Id, TokenTypeId = MdmIds.TkTemporary, IssuedAt = DateTime.UtcNow.AddDays(-10), ExpiresAt = DateTime.UtcNow.AddDays(-10).AddHours(4), StatusId = MdmIds.CheckedOut },
            new() { Id = tokenIds[13], TokenNumber = "TKN-2004", VisitorId = visitors[3].Id, TokenTypeId = MdmIds.TkContractor, IssuedAt = DateTime.UtcNow.AddDays(-7), ExpiresAt = DateTime.UtcNow.AddDays(-7).AddHours(8), StatusId = MdmIds.CheckedOut },
            new() { Id = tokenIds[14], TokenNumber = "TKN-2005", VisitorId = visitors[7].Id, TokenTypeId = MdmIds.TkDelivery, IssuedAt = DateTime.UtcNow.AddDays(-2), ExpiresAt = DateTime.UtcNow.AddDays(-2).AddHours(4), StatusId = MdmIds.Expired },
            new() { Id = tokenIds[15], TokenNumber = "TKN-2006", VisitorId = visitors[9].Id, TokenTypeId = MdmIds.TkVisitor, IssuedAt = DateTime.UtcNow.AddDays(-1), ExpiresAt = DateTime.UtcNow.AddDays(-1).AddHours(8), StatusId = MdmIds.CheckedOut }
        };

        foreach (var token in extraTokens)
            store.VisitorTokens.TryAdd(token.Id, token);
    }

    private static void SeedAppointments(InMemoryDataStore store, List<Guid> hostIds)
    {
        var appointmentIds = new List<Guid>
        {
            Guid.Parse("550e8400-e29b-41d4-a716-446655440901"), Guid.Parse("550e8400-e29b-41d4-a716-446655440902"),
            Guid.Parse("550e8400-e29b-41d4-a716-446655440903"), Guid.Parse("550e8400-e29b-41d4-a716-446655440904"),
            Guid.Parse("550e8400-e29b-41d4-a716-446655440905"), Guid.Parse("550e8400-e29b-41d4-a716-446655440906"),
            Guid.Parse("550e8400-e29b-41d4-a716-446655440907"), Guid.Parse("550e8400-e29b-41d4-a716-446655440908")
        };

        var appointments = new List<Appointment>
        {
            new() { Id = appointmentIds[0], VisitorName = "Amit Joshi", HostId = hostIds[0], ScheduledAt = DateTime.UtcNow.AddHours(1), Duration = 30, Purpose = "Follow-up consultation", StatusId = MdmIds.Scheduled },
            new() { Id = appointmentIds[1], VisitorName = "Neha Kapoor", HostId = hostIds[1], ScheduledAt = DateTime.UtcNow.AddHours(-3), Duration = 60, Purpose = "Technical interview round 2", StatusId = MdmIds.CheckedOut },
            new() { Id = appointmentIds[2], VisitorName = "Sanjay Dubey", HostId = hostIds[2], ScheduledAt = DateTime.UtcNow.AddHours(3), Duration = 45, Purpose = "Contract negotiation", StatusId = MdmIds.Scheduled },
            new() { Id = appointmentIds[3], VisitorName = "Omar Sheikh", HostId = hostIds[4], ScheduledAt = DateTime.UtcNow.AddDays(1), Duration = 60, Purpose = "Site visit and property tour", StatusId = MdmIds.Scheduled },
            new() { Id = appointmentIds[4], VisitorName = "Ravi Teja", HostId = hostIds[3], ScheduledAt = DateTime.UtcNow.AddDays(-1), Duration = 120, Purpose = "Annual safety audit", Notes = "Bring safety compliance documents", StatusId = MdmIds.Expired },
            new() { Id = appointmentIds[5], VisitorName = "Pooja Malhotra", HostId = hostIds[1], ScheduledAt = DateTime.UtcNow.AddDays(2), Duration = 90, Purpose = "Product demo for Q3 roadmap", Notes = "Conference room B reserved", StatusId = MdmIds.Scheduled },
            new() { Id = appointmentIds[6], VisitorName = "Karthik Rajan", HostId = hostIds[0], ScheduledAt = DateTime.UtcNow.AddDays(-2), Duration = 15, Purpose = "Lab report collection", StatusId = MdmIds.Cancelled },
            new() { Id = appointmentIds[7], VisitorName = "Fatima Begum", HostId = hostIds[2], ScheduledAt = DateTime.UtcNow.AddMinutes(-30), Duration = 30, Purpose = "Submit joining documents", StatusId = MdmIds.CheckedIn }
        };

        foreach (var appt in appointments)
            store.Appointments.TryAdd(appt.Id, appt);
    }

    private static void SeedEmployees(InMemoryDataStore store)
    {
        var employeeIds = new List<Guid>
        {
            Guid.Parse("550e8400-e29b-41d4-a716-446655441001"), Guid.Parse("550e8400-e29b-41d4-a716-446655441002"),
            Guid.Parse("550e8400-e29b-41d4-a716-446655441003"), Guid.Parse("550e8400-e29b-41d4-a716-446655441004"),
            Guid.Parse("550e8400-e29b-41d4-a716-446655441005"), Guid.Parse("550e8400-e29b-41d4-a716-446655441006"),
            Guid.Parse("550e8400-e29b-41d4-a716-446655441007"), Guid.Parse("550e8400-e29b-41d4-a716-446655441008"),
            Guid.Parse("550e8400-e29b-41d4-a716-446655441009"), Guid.Parse("550e8400-e29b-41d4-a716-446655441010")
        };

        var employees = new List<Employee>
        {
            new() { Id = employeeIds[0], FullName = "Dr. Anjali Mehta", Department = "Cardiology", Designation = "Senior Consultant", EmployeeCode = "EMP-001", Phone = "+91-9100000001", Email = "anjali.mehta@hospital.com", IsActive = true },
            new() { Id = employeeIds[1], FullName = "Vikram Singh", Department = "IT Infrastructure", Designation = "Technical Lead", EmployeeCode = "EMP-002", Phone = "+91-9100000002", Email = "vikram.singh@technova.com", IsActive = true },
            new() { Id = employeeIds[2], FullName = "Sunita Reddy", Department = "Human Resources", Designation = "HR Manager", EmployeeCode = "EMP-003", Phone = "+91-9100000003", Email = "sunita.reddy@technova.com", IsActive = true },
            new() { Id = employeeIds[3], FullName = "Rajesh Gupta", Department = "Production", Designation = "Plant Supervisor", EmployeeCode = "EMP-004", Phone = "+91-9100000004", Email = "rajesh.gupta@steelworks.com", IsActive = true },
            new() { Id = employeeIds[4], FullName = "Kavitha Nair", Department = "Administration", Designation = "Admin Head", EmployeeCode = "EMP-005", Phone = "+91-9100000005", Email = "kavitha.nair@greenview.com", IsActive = true },
            new() { Id = employeeIds[5], FullName = "Mohammed Farooq", Department = "Maintenance", Designation = "Maintenance Lead", EmployeeCode = "EMP-006", Phone = "+91-9100000006", Email = "farooq@greenview.com", IsActive = false },
            new() { Id = employeeIds[6], FullName = "Nidhi Agarwal", Department = "Finance", Designation = "Accounts Manager", EmployeeCode = "EMP-007", Phone = "+91-9100000007", Email = "nidhi.agarwal@technova.com", IsActive = true },
            new() { Id = employeeIds[7], FullName = "Arjun Nair", Department = "Quality Assurance", Designation = "QA Engineer", EmployeeCode = "EMP-008", Phone = "+91-9100000008", Email = "arjun.nair@steelworks.com", IsActive = true },
            new() { Id = employeeIds[8], FullName = "Divya Krishnan", Department = "Marketing", Designation = "Marketing Executive", EmployeeCode = "EMP-009", Phone = "+91-9100000009", Email = "divya.k@technova.com", IsActive = true },
            new() { Id = employeeIds[9], FullName = "Suresh Babu", Department = "Security", Designation = "Chief Security Officer", EmployeeCode = "EMP-010", Phone = "+91-9100000010", Email = "suresh.babu@technova.com", IsActive = true }
        };

        foreach (var emp in employees)
            store.Employees.TryAdd(emp.Id, emp);
    }
}
