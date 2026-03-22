using System.Linq.Expressions;
using VMS.Domain.Entities;

namespace VMS.Infrastructure.Repositories.Specifications;

/// <summary>
/// Base Specification class for building reusable query specifications
/// Works with both PostgreSQL and InMemory repositories
/// </summary>
public class Specification<T> where T : BaseEntity
{
    /// <summary>
    /// Filter criteria (WHERE clause)
    /// </summary>
    public Expression<Func<T, bool>>? Criteria { get; set; }

    /// <summary>
    /// Navigation properties to include (eager loading)
    /// </summary>
    public List<string> Includes { get; set; } = new();

    /// <summary>
    /// Ascending order property
    /// </summary>
    public Expression<Func<T, object>>? OrderBy { get; set; }

    /// <summary>
    /// Descending order property
    /// </summary>
    public Expression<Func<T, object>>? OrderByDescending { get; set; }

    /// <summary>
    /// Number of records to skip (for pagination)
    /// </summary>
    public int Skip { get; set; }

    /// <summary>
    /// Number of records to take (for pagination)
    /// </summary>
    public int Take { get; set; }

    /// <summary>
    /// Flag to enable/disable pagination
    /// </summary>
    public bool IsPagingEnabled { get; set; }
}

