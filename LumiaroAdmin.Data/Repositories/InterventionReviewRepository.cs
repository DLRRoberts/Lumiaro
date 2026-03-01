using Microsoft.EntityFrameworkCore;
using RedZone.LumiaroAdmin.Data;
using RedZone.LumiaroAdmin.Data.Entities;

namespace LumiaroAdmin.Data.Repositories;

// ══════════════════════════════════════════════════
// INTERVENTION REVIEW REPOSITORY
// ══════════════════════════════════════════════════

public class InterventionReviewRepository : RepositoryBase<InterventionReviewEntity>, IInterventionReviewRepository
{
    public InterventionReviewRepository(Func<LumiaroDbContext> db) : base(db) { }

    public async Task<List<InterventionReviewEntity>> GetByReportAsync(int reportId, CancellationToken ct = default)
    {
        await using var uow = GetUnitOfWork();
        return await uow.Set.AsNoTracking()
            .Where(r => r.ReportId == reportId)
            .OrderBy(r => r.EventId)
            .ToListAsync(ct);
    }

    public async Task<List<InterventionReviewEntity>> GetByReportAndEventAsync(int reportId, int eventId, CancellationToken ct = default)
    {
        await using var uow = GetUnitOfWork();
        return await uow.Set.AsNoTracking()
            .Where(r => r.ReportId == reportId && r.EventId == eventId)
            .ToListAsync(ct);
    }

    public async Task<List<InterventionReviewEntity>> GetByReviewerAsync(int reportId, string reviewerName, CancellationToken ct = default)
    {
        await using var uow = GetUnitOfWork();
        return await uow.Set.AsNoTracking()
            .Where(r => r.ReportId == reportId && r.ReviewerName == reviewerName)
            .OrderBy(r => r.EventId)
            .ToListAsync(ct);
    }

    public async Task<bool> HasReviewedAsync(int reportId, string reviewerName, CancellationToken ct = default)
    {
        await using var uow = GetUnitOfWork();
        return await uow.Set.AsNoTracking()
            .AnyAsync(r => r.ReportId == reportId && r.ReviewerName == reviewerName, ct);
    }

    public async Task<int> GetReviewerCountAsync(int reportId, CancellationToken ct = default)
    {
        await using var uow = GetUnitOfWork();
        return await uow.Set.AsNoTracking()
            .Where(r => r.ReportId == reportId)
            .Select(r => r.ReviewerName)
            .Distinct()
            .CountAsync(ct);
    }

    public async Task<int> GetReviewCountAsync(int reportId, CancellationToken ct = default)
    {
        await using var uow = GetUnitOfWork();
        return await uow.Set.AsNoTracking()
            .CountAsync(r => r.ReportId == reportId, ct);
    }

    public async Task RemoveByReviewerAsync(int reportId, string reviewerName, CancellationToken ct = default)
    {
        await using var uow = GetUnitOfWork();
        var entities = await uow.Set
            .Where(r => r.ReportId == reportId && r.ReviewerName == reviewerName)
            .ToListAsync(ct);
        uow.Set.RemoveRange(entities);
    }

    public async Task SaveReviewsAsync(ICollection<InterventionReviewEntity> review)
    {
        await using var uow = GetUnitOfWork();
        await uow.Set.AddRangeAsync(review);
        await SaveChangesAsync(uow);
    }
}