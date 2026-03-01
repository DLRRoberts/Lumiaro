using LumiaroAdmin.Data.Enums;
using RedZone.LumiaroAdmin.Data.Entities;

namespace LumiaroAdmin.Data.Repositories;

public interface IMatchOfficialReportRepository : IRepository<MatchOfficialReportEntity>
{
    Task<MatchOfficialReportEntity?> GetByIdWithAllAsync(int id, CancellationToken ct = default);
    Task<List<MatchOfficialReportEntity>> GetByRefereeAsync(int refereeId, CancellationToken ct = default);
    Task<List<MatchOfficialReportEntity>> GetByFixtureAsync(int fixtureId, CancellationToken ct = default);
    Task<MatchOfficialReportEntity?> FindExistingAsync(int fixtureId, int refereeId, OfficialRole role, CancellationToken ct = default);
    Task<List<MatchOfficialReportEntity>> SearchAsync(string? query, int? refereeId, int? fixtureId,
        DateTime? dateFrom, DateTime? dateTo, ReportStatus? status,
        double? minRating, EventType? eventType, CancellationToken ct = default);
    Task<(int Total, int Draft, int Submitted, int Published)> GetStatusCountsAsync(CancellationToken ct = default);
    Task<List<MatchOfficialReportEntity>> GetAllWithEventsAsync(CancellationToken ct = default);
}