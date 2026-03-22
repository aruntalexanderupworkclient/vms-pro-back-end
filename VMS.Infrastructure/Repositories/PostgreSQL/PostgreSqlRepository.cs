using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using VMS.Domain.Entities;
using VMS.Infrastructure.Data;
using VMS.Infrastructure.Repositories.Interfaces;
using VMS.Infrastructure.Repositories.Specifications;

namespace VMS.Infrastructure.Repositories.PostgreSQL;

public class PostgreSqlRepository<T> : IRepository<T> where T : BaseEntity
{
    private readonly VmsDbContext _context;
    private readonly DbSet<T> _dbSet;

    public PostgreSqlRepository(VmsDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    // ✅ NEW: Specification-based retrieval with includes
    public async Task<IEnumerable<T>> GetBySpecificationAsync(Specification<T> spec)
    {
        var query = ApplySpecification(spec);
        return await query.ToListAsync();
    }

    // ✅ NEW: Get single by ID with specification
    public async Task<T?> GetByIdWithSpecificationAsync(Guid id, Specification<T> spec)
    {
        spec.Criteria = e => e.Id == id;
        var query = ApplySpecification(spec);
        return await query.FirstOrDefaultAsync();
    }

    // ✅ HELPER: Apply specification to query
    private IQueryable<T> ApplySpecification(Specification<T> spec)
    {
        var query = _dbSet.AsQueryable();

        // Apply includes
        foreach (var include in spec.Includes)
            query = query.Include(include);

        // Apply criteria
        if (spec.Criteria != null)
            query = query.Where(spec.Criteria);

        // Apply ordering
        if (spec.OrderBy != null)
            query = query.OrderBy(spec.OrderBy);

        if (spec.OrderByDescending != null)
            query = query.OrderByDescending(spec.OrderByDescending);

        // Apply paging
        if (spec.IsPagingEnabled)
        {
            query = query.Skip(spec.Skip).Take(spec.Take);
        }

        return query;
    }

    // =========== STANDARD METHODS (No includes) ===========

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.OrderByDescending(e => e.CreatedAt).ToListAsync();
    }

    public async Task<IEnumerable<T>> GetPagedAsync(int page, int pageSize, string? search = null, Expression<Func<T, bool>>? filter = null)
    {
        IQueryable<T> query = _dbSet;

        if (filter != null)
            query = query.Where(filter);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLower();
            query = query.Where(e => EF.Functions.ILike(EF.Property<string>(e, "FullName") ?? EF.Property<string>(e, "Name") ?? "", $"%{search}%"));
        }

        return await query
            .OrderByDescending(e => e.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetCountAsync(string? search = null, Expression<Func<T, bool>>? filter = null)
    {
        IQueryable<T> query = _dbSet;

        if (filter != null)
            query = query.Where(filter);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(e => EF.Functions.ILike(EF.Property<string>(e, "FullName") ?? EF.Property<string>(e, "Name") ?? "", $"%{search}%"));
        }

        return await query.CountAsync();
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    // =========== CRUD (No includes needed) ===========

    public async Task<T> AddAsync(T entity)
    {
        entity.CreatedAt = DateTime.UtcNow;
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<T> UpdateAsync(T entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity != null)
        {
            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}


