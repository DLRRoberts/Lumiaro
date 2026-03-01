using Microsoft.EntityFrameworkCore;
using LumiaroAdmin.Data.Entities;
using LumiaroAdmin.Data.Models;
using LumiaroAdmin.Models;
using RedZone.LumiaroAdmin.Data;
using AssessmentStatus = LumiaroAdmin.Data.Enums.AssessmentStatus;
using OfficialRole = LumiaroAdmin.Data.Enums.OfficialRole;

namespace LumiaroAdmin.Data.Repositories;

public interface IAssessmentRepository : IRepository<AssessmentEntity>
{
    Task<AssessmentEntity?> GetByIdWithAllReportsAsync(int id, CancellationToken ct = default);
    Task<List<AssessmentEntity>> GetByFixtureAsync(int fixtureId, CancellationToken ct = default);
    Task<List<AssessmentEntity>> GetByRefereeAsync(int refereeId, CancellationToken ct = default);
    Task<AssessmentEntity?> FindExistingAsync(int fixtureId, int refereeId, OfficialRole role, CancellationToken ct = default);
    Task<List<AssessmentEntity>> SearchAsync(string? query, int? refereeId, int? fixtureId,
        DateTime? dateFrom, DateTime? dateTo, AssessmentStatus? status, CancellationToken ct = default);
    Task<(int Total, int Draft, int Submitted, int Reviewed, int Finalised)> GetStatusCountsAsync(CancellationToken ct = default);
    Task<List<(int RefereeId, string RefereeName)>> GetAssessedRefereesAsync(CancellationToken ct = default);
    Task<List<(int FixtureId, string HomeTeam, string AwayTeam, DateTime Date)>> GetAssessedFixturesAsync(CancellationToken ct = default);

    Task<AssessmentEntity> CreateAssessment(Assessment assessment, Fixture fixture, Referee referee);
}

public class AssessmentRepository : RepositoryBase<AssessmentEntity>, IAssessmentRepository
{
    public AssessmentRepository(Func<LumiaroDbContext> db) : base(db) { }

    /// <summary>
    /// Returns an assessment with all child reports eagerly loaded.
    /// Used for the full assessment detail page.
    /// </summary>
    public async Task<AssessmentEntity?> GetByIdWithAllReportsAsync(int id, CancellationToken ct = default)
    {
        await using var uow = GetUnitOfWork();
        return await uow.Set.AsNoTracking()
            .Include(a => a.Fixture)
                .ThenInclude(f => f.Division)
            .Include(a => a.Referee)
            .Include(a => a.TechnicalReport)
            .Include(a => a.DelegateReport)
            .Include(a => a.MatchStatReport)
            .Include(a => a.KeyMatchIncidents.OrderBy(k => k.SortOrder))
            .FirstOrDefaultAsync(a => a.Id == id, ct);
    }

    public async Task<List<AssessmentEntity>> GetByFixtureAsync(int fixtureId, CancellationToken ct = default)
    {
        await using var uow = GetUnitOfWork();
        return await uow.Set.AsNoTracking()
            .Include(a => a.Referee)
            .Where(a => a.FixtureId == fixtureId)
            .OrderBy(a => a.OfficialRole)
            .ToListAsync(ct);
    }

    public async Task<List<AssessmentEntity>> GetByRefereeAsync(int refereeId, CancellationToken ct = default)
    {
        await using var uow = GetUnitOfWork();
        return await uow.Set.AsNoTracking()
            .Include(a => a.Fixture)
                .ThenInclude(f => f.Division)
            .Where(a => a.RefereeId == refereeId)
            .OrderByDescending(a => a.Fixture.Date)
            .ToListAsync(ct);
    }

    public async Task<AssessmentEntity?> FindExistingAsync(int fixtureId, int refereeId, OfficialRole role, CancellationToken ct = default)
    {
        await using var uow = GetUnitOfWork();
        return await uow.Set.AsNoTracking()
            .FirstOrDefaultAsync(a =>
                a.FixtureId == fixtureId &&
                a.RefereeId == refereeId &&
                a.OfficialRole == role, ct);
    }

    public async Task<List<AssessmentEntity>> SearchAsync(string? query, int? refereeId, int? fixtureId,
        DateTime? dateFrom, DateTime? dateTo, AssessmentStatus? status, CancellationToken ct = default)
    {
        await using var uow = GetUnitOfWork();
        var q = uow.Set.AsNoTracking()
            .Include(a => a.Fixture)
                .ThenInclude(f => f.Division)
            .Include(a => a.Referee)
            .Include(a => a.TechnicalReport)
            .Include(a => a.DelegateReport)
            .Include(a => a.KeyMatchIncidents)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query))
        {
            var term = query.ToLower();
            q = q.Where(a =>
                (a.Referee.FirstName + " " + a.Referee.LastName).ToLower().Contains(term) ||
                a.Referee.RefCode.ToLower().Contains(term) ||
                (a.Fixture.HomeTeam + " vs " + a.Fixture.AwayTeam).ToLower().Contains(term) ||
                a.Fixture.Division.Name.ToLower().Contains(term));
        }

        if (refereeId.HasValue && refereeId.Value > 0)
            q = q.Where(a => a.RefereeId == refereeId.Value);

        if (fixtureId.HasValue && fixtureId.Value > 0)
            q = q.Where(a => a.FixtureId == fixtureId.Value);

        if (dateFrom.HasValue)
            q = q.Where(a => a.Fixture.Date >= dateFrom.Value.Date);

        if (dateTo.HasValue)
            q = q.Where(a => a.Fixture.Date <= dateTo.Value.Date);

        if (status.HasValue)
            q = q.Where(a => a.Status == status.Value);

        return await q
            .OrderByDescending(a => a.Fixture.Date)
            .ThenBy(a => a.Fixture.HomeTeam)
            .ToListAsync(ct);
    }

    public async Task<(int Total, int Draft, int Submitted, int Reviewed, int Finalised)> GetStatusCountsAsync(CancellationToken ct = default)
    {
        await using var uow = GetUnitOfWork();
        var counts = await uow.Set.AsNoTracking()
            .GroupBy(a => a.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(ct);

        return (
            counts.Sum(c => c.Count),
            counts.FirstOrDefault(c => c.Status == AssessmentStatus.Draft)?.Count ?? 0,
            counts.FirstOrDefault(c => c.Status == AssessmentStatus.Submitted)?.Count ?? 0,
            counts.FirstOrDefault(c => c.Status == AssessmentStatus.Reviewed)?.Count ?? 0,
            counts.FirstOrDefault(c => c.Status == AssessmentStatus.Finalised)?.Count ?? 0
        );
    }

    public async Task<List<(int RefereeId, string RefereeName)>> GetAssessedRefereesAsync(CancellationToken ct = default)
    {
        await using var uow = GetUnitOfWork();
        var results = await uow.Set.AsNoTracking()
            .Include(a => a.Referee)
            .GroupBy(a => new { a.RefereeId, a.Referee.FirstName, a.Referee.LastName })
            .Select(g => new { g.Key.RefereeId, Name = g.Key.FirstName + " " + g.Key.LastName })
            .OrderBy(x => x.Name)
            .ToListAsync(ct);

        return results.Select(r => (r.RefereeId, r.Name)).ToList();
    }

    public async Task<List<(int FixtureId, string HomeTeam, string AwayTeam, DateTime Date)>> GetAssessedFixturesAsync(CancellationToken ct = default)
    {
        await using var uow = GetUnitOfWork();
        var results = await uow.Set.AsNoTracking()
            .Include(a => a.Fixture)
            .GroupBy(a => new { a.FixtureId, a.Fixture.HomeTeam, a.Fixture.AwayTeam, a.Fixture.Date })
            .Select(g => new { g.Key.FixtureId, g.Key.HomeTeam, g.Key.AwayTeam, g.Key.Date })
            .OrderByDescending(x => x.Date)
            .ToListAsync(ct);

        return results.Select(f => (f.FixtureId, f.HomeTeam, f.AwayTeam, f.Date)).ToList();
    }

    public Task<AssessmentEntity> CreateAssessment(Assessment assessment, Fixture fixture, Referee referee)
    {
        throw new NotImplementedException();
    }
}
