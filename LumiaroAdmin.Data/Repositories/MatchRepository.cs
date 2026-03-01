using Microsoft.EntityFrameworkCore;
using LumiaroAdmin.Data.Entities;
using RedZone.LumiaroAdmin.Data;

namespace LumiaroAdmin.Data.Repositories;

public interface IMatchRepository : IRepository<MatchEntity>
{
    Task<List<MatchEntity>> GetByRefereeAsync(int refereeId, CancellationToken ct = default);
    Task<List<MatchEntity>> GetRecentByRefereeAsync(int refereeId, int count = 10, CancellationToken ct = default);
    Task<List<MatchEntity>> GetUpcomingByRefereeAsync(int refereeId, CancellationToken ct = default);
    Task<List<MatchEntity>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken ct = default);
}

public class MatchRepository(Func<LumiaroDbContext> db) : RepositoryBase<MatchEntity>(db), IMatchRepository
{
    private IQueryable<MatchEntity> WithIncludes(UnitOfWork<MatchEntity> uow) =>
        uow.Set.AsNoTracking()
            .Include(m => m.Referee)
            .Include(m => m.Incidents);

    public async Task<List<MatchEntity>> GetByRefereeAsync(int refereeId, CancellationToken ct = default)
    {
        await using var uow = GetUnitOfWork();
        return await WithIncludes(uow)
            .Where(m => m.RefereeId == refereeId)
            .OrderByDescending(m => m.Date)
            .ToListAsync(ct);
    }

    public async Task<List<MatchEntity>> GetRecentByRefereeAsync(int refereeId, int count = 10, CancellationToken ct = default)
    {
        await using var uow = GetUnitOfWork();
        return await WithIncludes(uow)
            .Where(m => m.RefereeId == refereeId && m.Date <= DateTime.Today)
            .OrderByDescending(m => m.Date)
            .Take(count)
            .ToListAsync(ct);
    }

    public async Task<List<MatchEntity>> GetUpcomingByRefereeAsync(int refereeId, CancellationToken ct = default)
    {
        await using var uow = GetUnitOfWork();
        return await WithIncludes(uow)
            .Where(m => m.RefereeId == refereeId && m.Date > DateTime.Today)
            .OrderBy(m => m.Date)
            .ToListAsync(ct);
    }

    public async Task<List<MatchEntity>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken ct = default)
    {
        await using var uow = GetUnitOfWork();
        return await WithIncludes(uow)
            .Where(m => m.Date >= from.Date && m.Date <= to.Date)
            .OrderByDescending(m => m.Date)
            .ToListAsync(ct);
    }
}
