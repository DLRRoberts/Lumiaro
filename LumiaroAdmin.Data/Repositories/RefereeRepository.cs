using Microsoft.EntityFrameworkCore;
using LumiaroAdmin.Data.Entities;
using LumiaroAdmin.Data.Enums;
using RedZone.LumiaroAdmin.Data;

namespace LumiaroAdmin.Data.Repositories;

public interface IRefereeRepository : IRepository<RefereeEntity>
{
    Task<RefereeEntity?> GetByIdWithDetailsAsync(int id, CancellationToken ct = default);
    Task<RefereeEntity?> GetByRefCodeAsync(string refCode, CancellationToken ct = default);
    Task<List<RefereeEntity>> GetByStatusAsync(RefereeStatus status, CancellationToken ct = default);
    Task<List<RefereeEntity>> GetByTierAsync(RefereeTier tier, CancellationToken ct = default);
    Task<List<RefereeEntity>> SearchAsync(string? query, RefereeTier? tier, RefereeStatus? status,
        string? region, CancellationToken ct = default);
    Task<List<RefereeEntity>> GetActiveByTierAsync(RefereeTier tier, CancellationToken ct = default);
    Task<int> GetActiveCountAsync(CancellationToken ct = default);
}

public class RefereeRepository : RepositoryBase<RefereeEntity>, IRefereeRepository
{
    public RefereeRepository(Func<LumiaroDbContext> db) : base(db) { }

    public async Task<RefereeEntity?> GetByIdWithDetailsAsync(int id, CancellationToken ct = default)
    {
        await using var uow = GetUnitOfWork();
        return await uow.Set
            .Include(r => r.CareerHistory.OrderBy(c => c.SortOrder))
            .Include(r => r.SeasonBreakdown.OrderByDescending(s => s.Season))
            .FirstOrDefaultAsync(r => r.Id == id, ct);
    }

    public async Task<RefereeEntity?> GetByRefCodeAsync(string refCode, CancellationToken ct = default)
    {
        await using var uow = GetUnitOfWork();
        return await uow.Set.AsNoTracking()
            .FirstOrDefaultAsync(r => r.RefCode == refCode, ct);
    }

    public async Task<List<RefereeEntity>> GetByStatusAsync(RefereeStatus status, CancellationToken ct = default)
    {
        await using var uow = GetUnitOfWork();
        return await uow.Set.AsNoTracking()
            .Where(r => r.Status == status)
            .OrderBy(r => r.LastName).ThenBy(r => r.FirstName)
            .ToListAsync(ct);
    }

    public async Task<List<RefereeEntity>> GetByTierAsync(RefereeTier tier, CancellationToken ct = default)
    {
        await using var uow = GetUnitOfWork();
        return await uow.Set.AsNoTracking()
            .Where(r => r.Tier == tier)
            .OrderBy(r => r.LastName).ThenBy(r => r.FirstName)
            .ToListAsync(ct);
    }

    public async Task<List<RefereeEntity>> SearchAsync(string? query, RefereeTier? tier,
        RefereeStatus? status, string? region, CancellationToken ct = default)
    {
        await using var uow = GetUnitOfWork();
        var q = uow.Set.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query))
        {
            var term = query.ToLower();
            q = q.Where(r =>
                r.FirstName.ToLower().Contains(term) ||
                r.LastName.ToLower().Contains(term) ||
                r.RefCode.ToLower().Contains(term) ||
                r.Location.ToLower().Contains(term));
        }

        if (tier.HasValue)
            q = q.Where(r => r.Tier == tier.Value);

        if (status.HasValue)
            q = q.Where(r => r.Status == status.Value);

        if (!string.IsNullOrWhiteSpace(region))
            q = q.Where(r => r.Region == region);

        return await q.OrderBy(r => r.LastName).ThenBy(r => r.FirstName).ToListAsync(ct);
    }

    public async Task<List<RefereeEntity>> GetActiveByTierAsync(RefereeTier tier, CancellationToken ct = default)
    {
        await using var uow = GetUnitOfWork();
        return await uow.Set.AsNoTracking()
            .Where(r => r.Status == RefereeStatus.Active && r.Tier == tier)
            .OrderBy(r => r.LastName)
            .ToListAsync(ct);
    }

    public async Task<int> GetActiveCountAsync(CancellationToken ct = default)
    {
        await using var uow = GetUnitOfWork();
        return await uow.Set.AsNoTracking()
            .CountAsync(r => r.Status == RefereeStatus.Active, ct);
    }
}
