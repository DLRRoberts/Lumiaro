using Microsoft.EntityFrameworkCore;
using RedZone.LumiaroAdmin.Data;

namespace LumiaroAdmin.Data.Repositories;

public class UnitOfWork<TEntity> : IDisposable, IAsyncDisposable where TEntity : class
{
    private readonly DbContext _context;
    public DbSet<TEntity> Set { get; }
    
    public DbContext Db => _context;

    public UnitOfWork(Func<LumiaroDbContext> context)
    {
        _context = context();
        Set = _context.Set<TEntity>();
    }

    public void Cancel()
    {
        _context.Dispose();
    }

    public void Dispose()
    {
        _context.SaveChanges();
        _context.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
    }
}