using Microsoft.EntityFrameworkCore;
using VMS.Domain.Entities;

namespace VMS.Infrastructure.Data;

public class VmsDbContext : DbContext
{
    public VmsDbContext(DbContextOptions<VmsDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<Visitor> Visitors => Set<Visitor>();
    public DbSet<VisitorToken> VisitorTokens => Set<VisitorToken>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Host> Hosts => Set<Host>();
    public DbSet<Organisation> Organisations => Set<Organisation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(VmsDbContext).Assembly);
    }
}
