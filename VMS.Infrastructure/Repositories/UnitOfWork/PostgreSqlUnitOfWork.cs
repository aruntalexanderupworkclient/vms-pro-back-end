using Microsoft.EntityFrameworkCore;
using VMS.Domain.Entities;
using VMS.Infrastructure.Data;
using VMS.Infrastructure.Repositories.Interfaces;
using VMS.Infrastructure.Repositories.PostgreSQL;

namespace VMS.Infrastructure.Repositories.UnitOfWork;

/// <summary>
/// PostgreSQL implementation of Unit of Work pattern
/// Uses Entity Framework Core DbContext for transaction management
/// </summary>
public class PostgreSqlUnitOfWork : IUnitOfWork
{
    private readonly VmsDbContext _context;
    private readonly Lazy<IRepository<User>> _userRepository;
    private readonly Lazy<IRepository<Role>> _roleRepository;
    private readonly Lazy<IRepository<Permission>> _permissionRepository;
    private readonly Lazy<IRepository<Visitor>> _visitorRepository;
    private readonly Lazy<IRepository<VisitorToken>> _tokenRepository;
    private readonly Lazy<IRepository<Host>> _hostRepository;
    private readonly Lazy<IRepository<Appointment>> _appointmentRepository;
    private readonly Lazy<IRepository<Employee>> _employeeRepository;
    private readonly Lazy<IRepository<Organisation>> _organisationRepository;

    private bool _disposed = false;

    public PostgreSqlUnitOfWork(VmsDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));

        // Lazy-load repositories to avoid unnecessary instantiation
        _userRepository = new Lazy<IRepository<User>>(() => new PostgreSqlRepository<User>(_context));
        _roleRepository = new Lazy<IRepository<Role>>(() => new PostgreSqlRepository<Role>(_context));
        _permissionRepository = new Lazy<IRepository<Permission>>(() => new PostgreSqlRepository<Permission>(_context));
        _visitorRepository = new Lazy<IRepository<Visitor>>(() => new PostgreSqlRepository<Visitor>(_context));
        _tokenRepository = new Lazy<IRepository<VisitorToken>>(() => new PostgreSqlRepository<VisitorToken>(_context));
        _hostRepository = new Lazy<IRepository<Host>>(() => new PostgreSqlRepository<Host>(_context));
        _appointmentRepository = new Lazy<IRepository<Appointment>>(() => new PostgreSqlRepository<Appointment>(_context));
        _employeeRepository = new Lazy<IRepository<Employee>>(() => new PostgreSqlRepository<Employee>(_context));
        _organisationRepository = new Lazy<IRepository<Organisation>>(() => new PostgreSqlRepository<Organisation>(_context));
    }

    // Repository properties
    public IRepository<User> Users => _userRepository.Value;
    public IRepository<Role> Roles => _roleRepository.Value;
    public IRepository<Permission> Permissions => _permissionRepository.Value;
    public IRepository<Visitor> Visitors => _visitorRepository.Value;
    public IRepository<VisitorToken> Tokens => _tokenRepository.Value;
    public IRepository<Host> Hosts => _hostRepository.Value;
    public IRepository<Appointment> Appointments => _appointmentRepository.Value;
    public IRepository<Employee> Employees => _employeeRepository.Value;
    public IRepository<Organisation> Organisations => _organisationRepository.Value;

    /// <summary>
    /// Check if a transaction is currently active
    /// </summary>
    public bool HasActiveTransaction => _context.Database.CurrentTransaction != null;

    /// <summary>
    /// Begin a new transaction
    /// </summary>
    public async Task BeginTransactionAsync()
    {
        if (HasActiveTransaction)
        {
            throw new InvalidOperationException("A transaction is already active. Complete the current transaction before starting a new one.");
        }

        await _context.Database.BeginTransactionAsync();
    }

    /// <summary>
    /// Save all changes made through repositories
    /// </summary>
    public async Task<int> SaveChangesAsync()
    {
        try
        {
            var result = await _context.SaveChangesAsync();
            return result;
        }
        catch (Exception ex)
        {
            // If transaction is active, it will be rolled back automatically on dispose
            throw new Exception("An error occurred while saving changes to the database.", ex);
        }
    }

    /// <summary>
    /// Rollback the current transaction
    /// </summary>
    public async Task RollbackAsync()
    {
        try
        {
            if (HasActiveTransaction)
            {
                await _context.Database.RollbackTransactionAsync();
            }
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while rolling back the transaction.", ex);
        }
    }

    /// <summary>
    /// Execute an operation with automatic transaction management
    /// Commits if successful, rolls back if an exception occurs
    /// </summary>
    public async Task<T> ExecuteTransactionAsync<T>(Func<Task<T>> operation)
    {
        await BeginTransactionAsync();
        try
        {
            var result = await operation();
            await _context.Database.CommitTransactionAsync();
            return result;
        }
        catch
        {
            await RollbackAsync();
            throw;
        }
    }

    /// <summary>
    /// Dispose of the Unit of Work and underlying DbContext
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
            return;

        if (HasActiveTransaction)
        {
            _context.Database.RollbackTransaction();
        }

        _context?.Dispose();
        _disposed = true;
        GC.SuppressFinalize(this);
    }
}

