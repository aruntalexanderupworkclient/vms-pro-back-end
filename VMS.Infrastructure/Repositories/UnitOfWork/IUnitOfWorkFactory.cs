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
/// </summary>
public class PostgreSqlUnitOfWorkFactory : IUnitOfWorkFactory
{
    private readonly VmsDbContext _context;

    public PostgreSqlUnitOfWorkFactory(VmsDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public IUnitOfWork CreateUnitOfWork()
    {
        return new PostgreSqlUnitOfWork(_context);
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

