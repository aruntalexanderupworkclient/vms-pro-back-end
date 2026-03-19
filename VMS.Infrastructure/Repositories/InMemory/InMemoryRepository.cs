using System.Linq.Expressions;
using VMS.Domain.Entities;
using VMS.Infrastructure.Repositories.Interfaces;

namespace VMS.Infrastructure.Repositories.InMemory;

public class InMemoryRepository<T> : IRepository<T> where T : BaseEntity
{
    private readonly VMS.Infrastructure.InMemory.InMemoryDataStore _store;

    public InMemoryRepository(VMS.Infrastructure.InMemory.InMemoryDataStore store)
    {
        _store = store;
    }

    public Task<IEnumerable<T>> GetAllAsync()
    {
        var collection = _store.GetCollection<T>();
        var result = collection.Values.Where(e => !e.IsDeleted).AsEnumerable();
        return Task.FromResult(result);
    }

    public Task<IEnumerable<T>> GetPagedAsync(int page, int pageSize, string? search = null, Expression<Func<T, bool>>? filter = null)
    {
        var collection = _store.GetCollection<T>();
        var query = collection.Values.Where(e => !e.IsDeleted).AsQueryable();

        if (filter != null)
            query = query.Where(filter);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLowerInvariant();
            query = query.Where(e => MatchesSearch(e, searchLower));
        }

        var result = query
            .OrderByDescending(e => e.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsEnumerable();

        return Task.FromResult(result);
    }

    public Task<int> GetCountAsync(string? search = null, Expression<Func<T, bool>>? filter = null)
    {
        var collection = _store.GetCollection<T>();
        var query = collection.Values.Where(e => !e.IsDeleted).AsQueryable();

        if (filter != null)
            query = query.Where(filter);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLowerInvariant();
            query = query.Where(e => MatchesSearch(e, searchLower));
        }

        return Task.FromResult(query.Count());
    }

    public Task<T?> GetByIdAsync(Guid id)
    {
        var collection = _store.GetCollection<T>();
        collection.TryGetValue(id, out var entity);
        if (entity != null && entity.IsDeleted)
            return Task.FromResult<T?>(null);
        return Task.FromResult(entity);
    }

    public Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        var collection = _store.GetCollection<T>();
        var result = collection.Values.Where(e => !e.IsDeleted).AsQueryable().Where(predicate).AsEnumerable();
        return Task.FromResult(result);
    }

    public Task<T> AddAsync(T entity)
    {
        entity.CreatedAt = DateTime.UtcNow;
        var collection = _store.GetCollection<T>();
        collection.TryAdd(entity.Id, entity);
        return Task.FromResult(entity);
    }

    public Task<T> UpdateAsync(T entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        var collection = _store.GetCollection<T>();
        collection[entity.Id] = entity;
        return Task.FromResult(entity);
    }

    public Task DeleteAsync(Guid id)
    {
        var collection = _store.GetCollection<T>();
        if (collection.TryGetValue(id, out var entity))
        {
            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.UtcNow;
        }
        return Task.CompletedTask;
    }

    private static bool MatchesSearch(T entity, string searchLower)
    {
        var properties = typeof(T).GetProperties()
            .Where(p => p.PropertyType == typeof(string));

        foreach (var prop in properties)
        {
            var value = prop.GetValue(entity) as string;
            if (value != null && value.ToLowerInvariant().Contains(searchLower))
                return true;
        }

        return false;
    }
}
