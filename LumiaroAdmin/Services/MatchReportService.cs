using LumiaroAdmin.Data.Repositories;
using LumiaroAdmin.Models;
using RedZone.LumiaroAdmin.Data.Entities;

namespace LumiaroAdmin.Services;

public class MatchReportService
{
    private readonly IMatchOfficialReportRepository _reportRepo;

    public MatchReportService(IMatchOfficialReportRepository reportRepo)
    {
        _reportRepo = reportRepo;
    }

    // ─── Queries ───

    public async Task<List<MatchOfficialReport>> GetAllAsync()
    {
        var entities = await _reportRepo.GetAllWithEventsAsync();
        return entities.Select(e => e.ToDomain()).ToList();
    }

    public async Task<MatchOfficialReport?> GetByIdAsync(int id)
    {
        var entity = await _reportRepo.GetByIdWithAllAsync(id);
        return entity?.ToDomain();
    }

    public async Task<List<MatchOfficialReport>> GetByRefereeAsync(int refereeId)
    {
        var entities = await _reportRepo.GetByRefereeAsync(refereeId);
        return entities.Select(e => e.ToDomain()).ToList();
    }

    public async Task<List<MatchOfficialReport>> GetByFixtureAsync(int fixtureId)
    {
        var entities = await _reportRepo.GetByFixtureAsync(fixtureId);
        return entities.Select(e => e.ToDomain()).ToList();
    }

    public async Task<MatchOfficialReport?> FindExistingAsync(int fixtureId, int refereeId, OfficialRole role)
    {
        var entity = await _reportRepo.FindExistingAsync(fixtureId, refereeId, role.ToData());
        return entity?.ToDomain();
    }

    public async Task<List<MatchOfficialReport>> SearchAsync(string? query, int? refereeId, int? fixtureId,
        DateTime? dateFrom, DateTime? dateTo, ReportStatus? status,
        double? minRating, EventType? eventType)
    {
        var entities = await _reportRepo.SearchAsync(query, refereeId, fixtureId,
            dateFrom, dateTo,
            status.HasValue ? status.Value.ToData() : null,
            minRating,
            eventType.HasValue ? eventType.Value.ToData() : null);
        return entities.Select(e => e.ToDomain()).ToList();
    }

    public async Task<(int total, int draft, int submitted, int published)> GetStatusCountsAsync()
    {
        var counts = await _reportRepo.GetStatusCountsAsync();
        return (counts.Total, counts.Draft, counts.Submitted, counts.Published);
    }

    public async Task<List<(int Id, string Name)>> GetReportedRefereesAsync()
    {
        var all = await _reportRepo.GetAllAsync();
        return all
            .GroupBy(r => new { r.RefereeId, r.RefereeName })
            .Select(g => (g.Key.RefereeId, g.Key.RefereeName ?? ""))
            .OrderBy(x => x.Item2)
            .ToList();
    }

    // ─── CRUD ───

    public async Task<MatchOfficialReport> CreateAsync(MatchOfficialReport report, Fixture fixture, Referee referee)
    {
        var entity = new MatchOfficialReportEntity
        {
            FixtureId = fixture.Id,
            RefereeId = referee.Id,
            OfficialRole = report.OfficialRole.ToData(),
            MatchTitle = fixture.MatchTitle,
            MatchDate = fixture.Date,
            Venue = fixture.Venue,
            RefereeName = referee.FullName,
            RefereeCode = referee.RefCode,
            RefereeInitials = referee.Initials,
            RefereeTier = referee.Tier.ToData(),
            CompetitionTier = fixture.DivisionTier.ToData(),
            Status = report.Status.ToData(),
            OverallRating = (decimal)report.OverallRating,
            AuthorName = report.AuthorName,
            ExecutiveSummary = report.Conclusion,
            DevelopmentPriorities = report.Recommendations,
            CreatedAt = DateTime.UtcNow,
        };

        await _reportRepo.AddAsync(entity);

        var result = await _reportRepo.GetByIdWithAllAsync(entity.Id);
        return result!.ToDomain();
    }

    public async Task UpdateAsync(MatchOfficialReport updated)
    {
        var entity = await _reportRepo.GetByIdWithAllAsync(updated.Id);
        if (entity == null) return;

        entity.Status = updated.Status.ToData();
        entity.OverallRating = (decimal)updated.OverallRating;
        entity.AuthorName = updated.AuthorName;
        entity.ExecutiveSummary = updated.Conclusion;
        entity.DevelopmentPriorities = updated.Recommendations;
        entity.UpdatedAt = DateTime.UtcNow;

        _reportRepo.Update(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _reportRepo.GetByIdAsync(id);
        if (entity != null)
        {
            _reportRepo.Remove(entity);
        }
    }
}
