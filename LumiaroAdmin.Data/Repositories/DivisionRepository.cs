using Microsoft.EntityFrameworkCore;
using LumiaroAdmin.Data.Entities;
using RedZone.LumiaroAdmin.Data;

namespace LumiaroAdmin.Data.Repositories;

public interface IDivisionRepository : IRepository<DivisionEntity>
{
    Task<List<DivisionEntity>> GetAllOrderedAsync(CancellationToken ct = default);
    Task<DivisionEntity?> GetByShortNameAsync(string shortName, CancellationToken ct = default);
}

public class DivisionRepository : RepositoryBase<DivisionEntity>, IDivisionRepository
{
    public DivisionRepository(Func<LumiaroDbContext> db) : base(db) { }

    public async Task<List<DivisionEntity>> GetAllOrderedAsync(CancellationToken ct = default)
    {
        await using var uow = GetUnitOfWork();
        return await uow.Set.AsNoTracking()
            .Where(d => d.IsActive)
            .OrderBy(d => d.SortOrder)
            .ToListAsync(ct);
    }

    public async Task<DivisionEntity?> GetByShortNameAsync(string shortName, CancellationToken ct = default)
    {
        await using var uow = GetUnitOfWork();
        return await uow.Set.AsNoTracking()
            .FirstOrDefaultAsync(d => d.ShortName == shortName, ct);
    }
}
