using Microsoft.EntityFrameworkCore;
using RedZone.LumiaroAdmin.Data;
using RedZone.LumiaroAdmin.Data.Entities;

namespace LumiaroAdmin.Data.Repositories;

// ══════════════════════════════════════════════════
// ACTIVITY LOG REPOSITORY
// ══════════════════════════════════════════════════
public class ActivityLogRepository : RepositoryBase<ActivityLogEntity>, IActivityLogRepository
{
    public ActivityLogRepository(Func<LumiaroDbContext> db) : base(db) { }

    public async Task<List<ActivityLogEntity>> GetRecentAsync(int count = 10, CancellationToken ct = default)
    {
        await using var uow = GetUnitOfWork();
        return await uow.Set.AsNoTracking()
            .OrderByDescending(a => a.Timestamp)
            .Take(count)
            .ToListAsync(ct);
    }
}