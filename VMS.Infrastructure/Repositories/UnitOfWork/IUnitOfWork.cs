using VMS.Domain.Entities;
using VMS.Infrastructure.Repositories.Interfaces;

namespace VMS.Infrastructure.Repositories.UnitOfWork;

/// <summary>
/// Unit of Work pattern interface for coordinating repository operations
/// Manages transactions across multiple repositories
/// Ensures atomic operations (all-or-nothing commits)
/// </summary>
public interface IUnitOfWork : IDisposable
{
    // Repository access
    IRepository<User> Users { get; }
    IRepository<Role> Roles { get; }
    IRepository<Permission> Permissions { get; }
    IRepository<Visitor> Visitors { get; }
    IRepository<VisitorToken> Tokens { get; }
    IRepository<Host> Hosts { get; }
    IRepository<Appointment> Appointments { get; }
    IRepository<Employee> Employees { get; }
    IRepository<Organisation> Organisations { get; }

    /// <summary>
    /// Begin a new transaction
    /// </summary>
    Task BeginTransactionAsync();

    /// <summary>
    /// Commit all changes made through repositories
    /// </summary>
    Task<int> SaveChangesAsync();

    /// <summary>
    /// Rollback all changes (transaction only)
    /// </summary>
    Task RollbackAsync();

    /// <summary>
    /// Check if transaction is active
    /// </summary>
    bool HasActiveTransaction { get; }

    /// <summary>
    /// Execute an operation with automatic transaction management
    /// If operation succeeds, commits; if fails, rolls back
    /// </summary>
    Task<T> ExecuteTransactionAsync<T>(Func<Task<T>> operation);
}

