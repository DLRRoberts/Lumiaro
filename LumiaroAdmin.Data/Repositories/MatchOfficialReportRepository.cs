using LumiaroAdmin.Data.Enums;
using Microsoft.EntityFrameworkCore;
using RedZone.LumiaroAdmin.Data;
using RedZone.LumiaroAdmin.Data.Entities;

namespace LumiaroAdmin.Data.Repositories;

// ══════════════════════════════════════════════════
// MATCH OFFICIAL REPORT REPOSITORY
// ══════════════════════════════════════════════════

public class MatchOfficialReportRepository : RepositoryBase<MatchOfficialReportEntity>, IMatchOfficialReportRepository
{
    public MatchOfficialReportRepository(Func<LumiaroDbContext> db) : base(db) { }

    public async Task<MatchOfficialReportEntity?> GetByIdWithAllAsync(int id, CancellationToken ct = default)
    {
        await using var uow = GetUnitOfWork();
        
        return await uow.Set
            .Include(r => r.Sections.OrderBy(s => s.SortOrder))
            .Include(r => r.Events.OrderBy(e => e.Minute).ThenBy(e => e.AddedTime))
            .Include(r => r.VideoClips.OrderBy(v => v.SortOrder))
            .FirstOrDefaultAsync(r => r.Id == id, ct);
    }

    public async Task<List<MatchOfficialReportEntity>> GetByRefereeAsync(int refereeId, CancellationToken ct = default)
    {
        await using var uow = GetUnitOfWork();
        return await uow.Set.AsNoTracking()
            .Include(r => r.Events)
            .Include(r => r.VideoClips)
            .Where(r => r.RefereeId == refereeId)
            .OrderByDescending(r => r.MatchDate)
            .ToListAsync(ct);
    }

    public async Task<List<MatchOfficialReportEntity>> GetByFixtureAsync(int fixtureId, CancellationToken ct = default)
    {
        await using var uow = GetUnitOfWork();
        return await uow.Set.AsNoTracking()
            .Where(r => r.FixtureId == fixtureId)
            .OrderBy(r => r.OfficialRole)
            .ToListAsync(ct);
    }

    public async Task<MatchOfficialReportEntity?> FindExistingAsync(int fixtureId, int refereeId, OfficialRole role, CancellationToken ct = default)
    {
        await using var uow = GetUnitOfWork();
        return await uow.Set.AsNoTracking()
            .FirstOrDefaultAsync(r => r.FixtureId == fixtureId && r.RefereeId == refereeId && r.OfficialRole == role, ct);
    }

    public async Task<List<MatchOfficialReportEntity>> SearchAsync(string? query, int? refereeId, int? fixtureId,
        DateTime? dateFrom, DateTime? dateTo, ReportStatus? status,
        double? minRating, EventType? eventType, CancellationToken ct = default)
    {
        await using var uow = GetUnitOfWork();
        var q = uow.Set.AsNoTracking()
            .Include(r => r.Events)
            .Include(r => r.VideoClips)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query))
        {
            var term = query.ToLower();
            q = q.Where(r =>
                (r.RefereeName ?? "").ToLower().Contains(term) ||
                (r.RefereeCode ?? "").ToLower().Contains(term) ||
                r.MatchTitle.ToLower().Contains(term));
        }

        if (refereeId.HasValue && refereeId.Value > 0)
            q = q.Where(r => r.RefereeId == refereeId.Value);
        if (fixtureId.HasValue && fixtureId.Value > 0)
            q = q.Where(r => r.FixtureId == fixtureId.Value);
        if (dateFrom.HasValue)
            q = q.Where(r => r.MatchDate >= dateFrom.Value.Date);
        if (dateTo.HasValue)
            q = q.Where(r => r.MatchDate <= dateTo.Value.Date);
        if (status.HasValue)
            q = q.Where(r => r.Status == status.Value);
        if (minRating.HasValue)
            q = q.Where(r => (double)r.OverallRating >= minRating.Value);
        if (eventType.HasValue)
            q = q.Where(r => r.Events.Any(e => e.EventType == eventType.Value));

        return await q.OrderByDescending(r => r.MatchDate).ThenBy(r => r.MatchTitle).ToListAsync(ct);
    }

    public async Task<(int Total, int Draft, int Submitted, int Published)> GetStatusCountsAsync(CancellationToken ct = default)
    {
        await using var uow = GetUnitOfWork();
        
        var counts = await uow.Set.AsNoTracking()
            .GroupBy(r => r.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(ct);

        return (
            counts.Sum(c => c.Count),
            counts.FirstOrDefault(c => c.Status == ReportStatus.Draft)?.Count ?? 0,
            counts.FirstOrDefault(c => c.Status == ReportStatus.Submitted)?.Count ?? 0,
            counts.FirstOrDefault(c => c.Status == ReportStatus.Published)?.Count ?? 0
        );
    }

    public async Task<List<MatchOfficialReportEntity>> GetAllWithEventsAsync(CancellationToken ct = default)
    {
        await using var uow = GetUnitOfWork();
        return await uow.Set.AsNoTracking()
            .Include(r => r.Sections)
            .Include(r => r.Events)
            .Include(r => r.VideoClips)
            .OrderByDescending(r => r.MatchDate)
            .ToListAsync(ct);
    }
}