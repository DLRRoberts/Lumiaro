using LumiaroAdmin.Data.Entities;
using LumiaroAdmin.Data.Repositories;
using LumiaroAdmin.Models;
using RedZone.LumiaroAdmin.Data.Entities;
using DataEnums = RedZone.LumiaroAdmin.Data;

namespace LumiaroAdmin.Services;

public class RefereeService
{
    private readonly IRefereeRepository _refereeRepo;
    private readonly IMatchRepository _matchRepo;
    private readonly IActivityLogRepository _activityRepo;
    private readonly IFixtureRepository _fixtureRepo;

    public RefereeService(
        IRefereeRepository refereeRepo,
        IMatchRepository matchRepo,
        IActivityLogRepository activityRepo,
        IFixtureRepository fixtureRepo)
    {
        _refereeRepo = refereeRepo;
        _matchRepo = matchRepo;
        _activityRepo = activityRepo;
        _fixtureRepo = fixtureRepo;
    }

    // ─── Dashboard ───

    public async Task<DashboardStats> GetDashboardStatsAsync()
    {
        var activeCount = await _refereeRepo.GetActiveCountAsync();
        var totalCount = await _refereeRepo.CountAsync();
        var fixtures = await _fixtureRepo.GetByDateRangeAsync(DateTime.Today, DateTime.Today.AddDays(14));
        var nextMatchday = fixtures.OrderBy(f => f.Date).FirstOrDefault();

        return new DashboardStats
        {
            ActiveReferees = activeCount,
            SeasonChange = totalCount - activeCount,
            MatchesThisSeason = await _fixtureRepo.CountAsync(),
            MatchesThisWeek = fixtures.Count(f => f.Date <= DateTime.Today.AddDays(7)),
            AvgAssessmentScore = 7.8,
            ScoreChangeVsLastSeason = 0.3,
            PendingAppointments = fixtures.Count,
            NextMatchday = nextMatchday != null ? nextMatchday.Date.ToString("ddd d MMM") : "—"
        };
    }

    public async Task<List<ActivityItem>> GetRecentActivityAsync()
    {
        var entities = await _activityRepo.GetRecentAsync(10);
        return entities.Select(e => e.ToDomain()).ToList();
    }

    public async Task<List<Match>> GetUpcomingAppointmentsAsync()
    {
        var fixtures = await _fixtureRepo.GetByDateRangeAsync(DateTime.Today, DateTime.Today.AddDays(14));
        var appointments = new List<Match>();
        foreach (var f in fixtures.Where(f => f.OfficialAssignments?.Any() == true))
        {
            var refA = f.OfficialAssignments?.FirstOrDefault(a => a.Role == LumiaroAdmin.Data.Enums.OfficialRole.Referee);
            if (refA == null) continue;
            appointments.Add(new Match
            {
                Id = f.Id, Date = f.Date, HomeTeam = f.HomeTeam, AwayTeam = f.AwayTeam,
                Competition = f.Division?.Name ?? "",
                CompetitionTier = (f.Division?.Tier ?? LumiaroAdmin.Data.Enums.RefereeTier.Grassroots).ToDomain(),
                RefereeId = refA.RefereeId,
                RefereeName = refA.Referee != null ? $"{refA.Referee.FirstName} {refA.Referee.LastName}" : "",
                RefereeInitials = refA.Referee != null ? $"{refA.Referee.FirstName[0]}{refA.Referee.LastName[0]}" : "",
                Assistants = string.Join(", ", f.OfficialAssignments!
                    .Where(a => a.Role == LumiaroAdmin.Data.Enums.OfficialRole.AssistantReferee1 || 
                                a.Role == LumiaroAdmin.Data.Enums.OfficialRole.AssistantReferee2)
                    .Select(a => a.Referee?.LastName ?? "")),
                FourthOfficial = f.OfficialAssignments!
                    .FirstOrDefault(a => a.Role == LumiaroAdmin.Data.Enums.OfficialRole.MatchOfficial)?.Referee?.LastName,
            });
        }
        return appointments;
    }

    // ─── Referee CRUD ───

    public async Task<List<Referee>> GetAllRefereesAsync()
    {
        var entities = await _refereeRepo.GetAllAsync();
        return entities.OrderBy(r => r.LastName).Select(e => e.ToDomain()).ToList();
    }

    public async Task<List<Referee>> GetRecentlyUpdatedAsync(int count = 5)
    {
        var entities = await _refereeRepo.GetAllAsync();
        return entities.OrderByDescending(r => r.LastActiveDate ?? r.RegistrationDate)
            .Take(count).Select(e => e.ToDomain()).ToList();
    }

    public async Task<Referee?> GetRefereeByIdAsync(int id)
    {
        var entity = await _refereeRepo.GetByIdWithDetailsAsync(id);
        return entity?.ToDomain();
    }

    public async Task<Referee?> GetRefereeByCodeAsync(string code)
    {
        var entity = await _refereeRepo.GetByRefCodeAsync(code);
        return entity?.ToDomain();
    }

    public async Task<List<Referee>> SearchRefereesAsync(string? query, RefereeTier? tier, RefereeStatus? status)
    {
        var entities = await _refereeRepo.SearchAsync(query,
            tier.HasValue ? tier.Value.ToData() : null,
            status.HasValue ? status.Value.ToData() : null, null);
        return entities.Select(e => e.ToDomain()).ToList();
    }

    public async Task<Referee> AddRefereeAsync(RefereeFormModel form)
    {
        var entity = new RefereeEntity
        {
            FirstName = form.FirstName, LastName = form.LastName,
            DateOfBirth = form.DateOfBirth ?? DateTime.Today,
            Nationality = form.Nationality, Location = form.Location, Region = form.Region,
            Tier = form.Tier?.ToData() ?? LumiaroAdmin.Data.Enums.RefereeTier.Grassroots,
            Status = form.Status.ToData(),
            RegistrationDate = DateTime.UtcNow, LastActiveDate = DateTime.UtcNow,
            Email = form.Email, Phone = form.Phone, Notes = form.Notes,
        };
        await _refereeRepo.AddAsync(entity);
        entity.RefCode = $"REF-{entity.Id:D4}";
        _refereeRepo.Update(entity);
        return entity.ToDomain();
    }

    public async Task<bool> UpdateRefereeAsync(RefereeFormModel form)
    {
        if (form.Id.HasValue == false)
        {
            return false;
        }
        
        var entity = await _refereeRepo.GetByIdAsync(form.Id.Value);
        if (entity == null) return false;
        entity.FirstName = form.FirstName; entity.LastName = form.LastName;
        entity.DateOfBirth = form.DateOfBirth ?? entity.DateOfBirth;
        entity.Nationality = form.Nationality; entity.Location = form.Location;
        entity.Region = form.Region;
        entity.Tier = form.Tier?.ToData() ?? entity.Tier;
        entity.Status = form.Status.ToData();
        entity.Email = form.Email; entity.Phone = form.Phone; entity.Notes = form.Notes;
        entity.LastActiveDate = DateTime.UtcNow; entity.UpdatedAt = DateTime.UtcNow;
        _refereeRepo.Update(entity);
        return true;
    }

    public async Task<bool> DeleteRefereeAsync(int id)
    {
        var entity = await _refereeRepo.GetByIdAsync(id);
        if (entity == null) return false;
        _refereeRepo.Remove(entity);
        return true;
    }

    // ─── Match History ───

    public async Task<List<Match>> GetMatchesForRefereeAsync(int refereeId, string? seasonFilter = null)
    {
        var entities = await _matchRepo.GetByRefereeAsync(refereeId);
        IEnumerable<MatchEntity> results = entities;
        if (!string.IsNullOrWhiteSpace(seasonFilter) && seasonFilter != "All Seasons")
            results = results.Where(m => m.Competition.Contains(seasonFilter) || m.Date.Year.ToString() == seasonFilter.Split('/')[0]);
        return results.Select(e => e.ToDomain()).ToList();
    }

    // ─── Stats ───

    public async Task<(int total, int active, int probation, int inactive)> GetStatusCountsAsync()
    {
        var all = await _refereeRepo.GetAllAsync();
        return (all.Count,
            all.Count(r => r.Status == LumiaroAdmin.Data.Enums.RefereeStatus.Active),
            all.Count(r => r.Status == LumiaroAdmin.Data.Enums.RefereeStatus.Probation),
            all.Count(r => r.Status == LumiaroAdmin.Data.Enums.RefereeStatus.Inactive || 
                           r.Status == LumiaroAdmin.Data.Enums.RefereeStatus.Suspended));
    }

    public Task<int> GetRefereeCount()
    {
        return _refereeRepo.CountAsync();
    }
}
