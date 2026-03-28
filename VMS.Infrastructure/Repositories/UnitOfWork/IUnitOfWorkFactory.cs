using VMS.Domain.Entities;
using VMS.Infrastructure.Data;

namespace VMS.Infrastructure.Repositories.UnitOfWork;

/// <summary>
/// Factory for creating Unit of Work instances
/// Handles both PostgreSQL and InMemory implementations
/// </summary>
public interface IUnitOfWorkFactory
{
    /// <summary>
    /// Create a new Unit of Work instance
    /// </summary>
    IUnitOfWork CreateUnitOfWork();
}

/// <summary>
/// PostgreSQL Unit of Work Factory
/// Injects ICurrentUserProvider to automatically pass the current user's ID to repositories for audit tracking
/// </summary>
public class PostgreSqlUnitOfWorkFactory : IUnitOfWorkFactory
{
    private readonly VmsDbContext _context;
    private readonly ICurrentUserProvider _currentUserProvider;

    public PostgreSqlUnitOfWorkFactory(VmsDbContext context, ICurrentUserProvider currentUserProvider)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
    }

    public IUnitOfWork CreateUnitOfWork()
    {
        // Get current user ID from the HTTP context; null if not authenticated
        Guid? currentUserId = _currentUserProvider.GetCurrentUserId();
        return new PostgreSqlUnitOfWork(_context, currentUserId);
    }
}

/// <summary>
/// InMemory Unit of Work Factory
/// </summary>
public class InMemoryUnitOfWorkFactory : IUnitOfWorkFactory
{
    private readonly VMS.Infrastructure.InMemory.InMemoryDataStore _store;

    public InMemoryUnitOfWorkFactory(VMS.Infrastructure.InMemory.InMemoryDataStore store)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
    }

    public IUnitOfWork CreateUnitOfWork()
    {
        return new InMemoryUnitOfWork(_store);
    }
}

