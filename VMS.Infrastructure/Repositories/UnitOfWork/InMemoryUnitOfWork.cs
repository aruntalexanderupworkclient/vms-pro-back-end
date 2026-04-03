using VMS.Domain.Entities;
using VMS.Infrastructure.Repositories.Interfaces;
using VMS.Infrastructure.Repositories.InMemory;

namespace VMS.Infrastructure.Repositories.UnitOfWork;

/// <summary>
/// InMemory implementation of Unit of Work pattern
/// Simulates transactions using a change set approach
/// For testing and development purposes
/// </summary>
public class InMemoryUnitOfWork : IUnitOfWork
{
    private readonly VMS.Infrastructure.InMemory.InMemoryDataStore _store;
    private readonly Lazy<IRepository<User>> _userRepository;
    private readonly Lazy<IRepository<Role>> _roleRepository;
    private readonly Lazy<IRepository<Permission>> _permissionRepository;
    private readonly Lazy<IRepository<Visitor>> _visitorRepository;
    private readonly Lazy<IRepository<VisitorToken>> _tokenRepository;
    private readonly Lazy<IRepository<Host>> _hostRepository;
    private readonly Lazy<IRepository<Appointment>> _appointmentRepository;
    private readonly Lazy<IRepository<Employee>> _employeeRepository;
    private readonly Lazy<IRepository<Organisation>> _organisationRepository;
    private readonly Lazy<IRepository<MdmVisitStatus>> _mdmVisitStatusRepository;
    private readonly Lazy<IRepository<MdmUserStatus>> _mdmUserStatusRepository;
    private readonly Lazy<IRepository<MdmTokenType>> _mdmTokenTypeRepository;
    private readonly Lazy<IRepository<MdmOrganisationType>> _mdmOrganisationTypeRepository;

    private bool _transactionActive = false;
    private readonly Dictionary<string, object> _changeSet = new();
    private bool _disposed = false;

    public InMemoryUnitOfWork(VMS.Infrastructure.InMemory.InMemoryDataStore store)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));

        // Lazy-load repositories
        _userRepository = new Lazy<IRepository<User>>(() => new InMemoryRepository<User>(_store));
        _roleRepository = new Lazy<IRepository<Role>>(() => new InMemoryRepository<Role>(_store));
        _permissionRepository = new Lazy<IRepository<Permission>>(() => new InMemoryRepository<Permission>(_store));
        _visitorRepository = new Lazy<IRepository<Visitor>>(() => new InMemoryRepository<Visitor>(_store));
        _tokenRepository = new Lazy<IRepository<VisitorToken>>(() => new InMemoryRepository<VisitorToken>(_store));
        _hostRepository = new Lazy<IRepository<Host>>(() => new InMemoryRepository<Host>(_store));
        _appointmentRepository = new Lazy<IRepository<Appointment>>(() => new InMemoryRepository<Appointment>(_store));
        _employeeRepository = new Lazy<IRepository<Employee>>(() => new InMemoryRepository<Employee>(_store));
        _organisationRepository = new Lazy<IRepository<Organisation>>(() => new InMemoryRepository<Organisation>(_store));
        _mdmVisitStatusRepository = new Lazy<IRepository<MdmVisitStatus>>(() => new InMemoryRepository<MdmVisitStatus>(_store));
        _mdmUserStatusRepository = new Lazy<IRepository<MdmUserStatus>>(() => new InMemoryRepository<MdmUserStatus>(_store));
        _mdmTokenTypeRepository = new Lazy<IRepository<MdmTokenType>>(() => new InMemoryRepository<MdmTokenType>(_store));
        _mdmOrganisationTypeRepository = new Lazy<IRepository<MdmOrganisationType>>(() => new InMemoryRepository<MdmOrganisationType>(_store));
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
    public IRepository<MdmVisitStatus> MdmVisitStatuses => _mdmVisitStatusRepository.Value;
    public IRepository<MdmUserStatus> MdmUserStatuses => _mdmUserStatusRepository.Value;
    public IRepository<MdmTokenType> MdmTokenTypes => _mdmTokenTypeRepository.Value;
    public IRepository<MdmOrganisationType> MdmOrganisationTypes => _mdmOrganisationTypeRepository.Value;

    /// <summary>
    /// Check if a transaction is currently active
    /// </summary>
    public bool HasActiveTransaction => _transactionActive;

    /// <summary>
    /// Begin a new transaction
    /// </summary>
    public Task BeginTransactionAsync()
    {
        if (_transactionActive)
        {
            throw new InvalidOperationException("A transaction is already active. Complete the current transaction before starting a new one.");
        }

        _transactionActive = true;
        _changeSet.Clear();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Save all changes (in-memory, so just clears change set)
    /// </summary>
    public Task<int> SaveChangesAsync()
    {
        try
        {
            var changeCount = _changeSet.Count;
            _changeSet.Clear();
            _transactionActive = false;
            return Task.FromResult(changeCount);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while saving changes.", ex);
        }
    }

    /// <summary>
    /// Rollback the current transaction
    /// </summary>
    public Task RollbackAsync()
    {
        try
        {
            if (_transactionActive)
            {
                _changeSet.Clear();
                _transactionActive = false;
            }
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while rolling back the transaction.", ex);
        }
    }

    /// <summary>
    /// Execute an operation with automatic transaction management
    /// </summary>
    public async Task<T> ExecuteTransactionAsync<T>(Func<Task<T>> operation)
    {
        await BeginTransactionAsync();
        try
        {
            var result = await operation();
            await SaveChangesAsync();
            return result;
        }
        catch
        {
            await RollbackAsync();
            throw;
        }
    }

    /// <summary>
    /// Dispose of the Unit of Work
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
            return;

        if (_transactionActive)
        {
            _changeSet.Clear();
            _transactionActive = false;
        }

        _disposed = true;
        GC.SuppressFinalize(this);
    }
}

