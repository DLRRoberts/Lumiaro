using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using RedZone.LumiaroAdmin.Data;

namespace LumiaroAdmin.Data.Repositories;

/// <summary>
/// Generic repository interface providing standard CRUD operations.
/// All repositories inherit from this to ensure a consistent data access contract.
/// </summary>
public interface IRepository<TEntity> where TEntity : class
{
    Task<TEntity?> GetByIdAsync(int id, UnitOfWork<TEntity>? uow = null, CancellationToken ct = default);
    Task<List<TEntity>> GetAllAsync(UnitOfWork<TEntity>? uow = null, CancellationToken ct = default);
    Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, UnitOfWork<TEntity>? uow = null, CancellationToken ct = default);
    Task<TEntity> AddAsync(TEntity entity, UnitOfWork<TEntity>? uow = null, CancellationToken ct = default);
    Task AddRangeAsync(IEnumerable<TEntity> entities, UnitOfWork<TEntity>? uow = null, CancellationToken ct = default);
    void Update(TEntity entity, UnitOfWork<TEntity>? uow = null);
    void Remove(TEntity entity, UnitOfWork<TEntity>? uow = null);
    Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, UnitOfWork<TEntity>? uow = null, CancellationToken ct = default);
    IQueryable<TEntity> Query(UnitOfWork<TEntity>? uow = null);
}

/// <summary>
/// Generic repository base implementation backed by EF Core.
/// Provides standard CRUD against the LumiaroDbContext.
/// </summary>
public abstract class RepositoryBase<TEntity> : IRepository<TEntity> where TEntity : class
{
    protected readonly Func<LumiaroDbContext> _db;

    protected RepositoryBase(Func<LumiaroDbContext> db)
    {
        _db = db;
    }

    protected UnitOfWork<TEntity> GetUnitOfWork() => new(_db);

    public virtual async Task<TEntity?> GetByIdAsync(int id, UnitOfWork<TEntity>? uow, CancellationToken ct = default)
    {
        var uowToUse = uow ?? GetUnitOfWork();
        var result = await uowToUse.Set.FindAsync([id], ct);
        if (uow is null)
        {
            await uowToUse.DisposeAsync();
        }

        return result;
    }

    public virtual async Task<List<TEntity>> GetAllAsync(UnitOfWork<TEntity>? uow = null,
        CancellationToken ct = default)
    {
        var uowToUse = uow ?? GetUnitOfWork();
        var result = await uowToUse.Set.AsNoTracking().ToListAsync(ct);
        
        if (uow is null)
        {
            await uowToUse.DisposeAsync();
        }

        return result;
    }

    public virtual async Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate,
        UnitOfWork<TEntity>? uow = null, CancellationToken ct = default)
    {
        var uowToUse = uow ?? GetUnitOfWork();
        var result = await uowToUse.Set.AsNoTracking().Where(predicate).ToListAsync(ct);
        if (uow is null)
        {
            await uowToUse.DisposeAsync();
        }

        return result;   
    }

    public virtual async Task<TEntity> AddAsync(TEntity entity, UnitOfWork<TEntity>? uow = null, CancellationToken ct = default)
    {
        var uowToUse = uow ?? GetUnitOfWork();
        var result = await uowToUse.Set.AddAsync(entity, ct);
        if (uow is null)
        {
            await uowToUse.DisposeAsync();
        }

        return result.Entity;
    }

    public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities, UnitOfWork<TEntity>? uow = null,
        CancellationToken ct = default)
    {
        var uowToUse = uow ?? GetUnitOfWork();

        await uowToUse.Set.AddRangeAsync(entities, ct);
        if (uow is null)
        {
            await uowToUse.DisposeAsync();
        }
    }

    public virtual void Update(TEntity entity, UnitOfWork<TEntity>? uow = null)
    {
        var uowToUse = uow ?? GetUnitOfWork();
        uowToUse.Set.Update(entity);
        if (uow is null)
        {
            uowToUse.Dispose();
        }
    }

    public virtual void Remove(TEntity entity, UnitOfWork<TEntity>? uow = null)
    {
        var uowToUse = uow ?? GetUnitOfWork();
        uowToUse.Set.Remove(entity);
        if (uow is null)
        {
            uowToUse.Dispose();
        }
    }

    public virtual void RemoveRange(IEnumerable<TEntity> entities, UnitOfWork<TEntity>? uow = null)
    {
        var uowToUse = uow ?? GetUnitOfWork();
        uowToUse.Set.RemoveRange(entities);
        if (uow is null)
        {
            uowToUse.Dispose();
        }
    }

    public virtual async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate,
        UnitOfWork<TEntity>? uow = null, CancellationToken ct = default)
    {
        var uowToUse = uow ?? GetUnitOfWork();
        var result = await uowToUse.Set.AsNoTracking().AnyAsync(predicate, ct);
        if (uow is null)
        {
            uowToUse.Dispose();
        }

        return result;
    }

    public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, UnitOfWork<TEntity>? uow = null, CancellationToken ct = default)
    {
        var uowToUse = uow ?? GetUnitOfWork(); 
        var count = predicate != null
            ? await uowToUse.Set.AsNoTracking().CountAsync(predicate, ct)
            : await uowToUse.Set.AsNoTracking().CountAsync(ct);

        if (uow == null)
        {
            await uowToUse.DisposeAsync();
        }

        return count;
    }

    protected virtual async Task<int> SaveChangesAsync(UnitOfWork<TEntity> uow, CancellationToken ct = default)
        => await uow.Db.SaveChangesAsync(ct);

    public virtual IQueryable<TEntity> Query(UnitOfWork<TEntity>? uow = null)
    {
        var uowToUse = uow ?? GetUnitOfWork();
        var result = uowToUse.Set.AsNoTracking();
        if (uow == null)
        {
            uowToUse.Dispose();
        }

        return result;
    }
}
