using System.Linq.Expressions;
using VMS.Domain.Entities;
using VMS.Infrastructure.Repositories.Specifications;

namespace VMS.Infrastructure.Repositories.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    // ✅ NEW: Specification-based methods (with includes)
    Task<IEnumerable<T>> GetBySpecificationAsync(Specification<T> spec);
    Task<T?> GetByIdWithSpecificationAsync(Guid id, Specification<T> spec);

    // ✅ Standard methods (without includes - lightweight)
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> GetPagedAsync(int page, int pageSize, string? search = null, Expression<Func<T, bool>>? filter = null);
    Task<int> GetCountAsync(string? search = null, Expression<Func<T, bool>>? filter = null);
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

    // CRUD (no includes needed)
    Task<T> AddAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task DeleteAsync(Guid id);
}
