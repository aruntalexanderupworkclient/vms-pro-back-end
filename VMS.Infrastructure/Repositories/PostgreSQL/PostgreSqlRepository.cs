using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using VMS.Domain.Entities;
using VMS.Infrastructure.Data;
using VMS.Infrastructure.Repositories.Interfaces;

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
