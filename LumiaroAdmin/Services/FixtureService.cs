using LumiaroAdmin.Data.Entities;
using LumiaroAdmin.Data.Repositories;
using LumiaroAdmin.Models;
using RedZone.LumiaroAdmin.Data.Mapping;
using DataEnums = LumiaroAdmin.Data.Enums;
using LumiaroAdmin.Data.Enums;

namespace LumiaroAdmin.Services;

public class FixtureService
{
    private readonly IFixtureRepository _fixtureRepo;
    private readonly IDivisionRepository _divisionRepo;
    private readonly IOfficialAssignmentRepository _assignmentRepo;
    private readonly IRefereeRepository _refereeRepo;

    public FixtureService(
        IFixtureRepository fixtureRepo,
        IDivisionRepository divisionRepo,
        IOfficialAssignmentRepository assignmentRepo,
        IRefereeRepository refereeRepo)
    {
        _fixtureRepo = fixtureRepo;
        _divisionRepo = divisionRepo;
        _assignmentRepo = assignmentRepo;
        _refereeRepo = refereeRepo;
    }

    // ─── Divisions ───

    public async Task<List<Division>> GetDivisionsAsync()
    {
        var entities = await _divisionRepo.GetAllOrderedAsync();
        return entities.Select(e => e.ToDomain()).ToList();
    }

    // ─── Fixture Queries ───

    public async Task<List<Fixture>> GetAllFixturesAsync()
    {
        var entities = await _fixtureRepo.GetAllAsync();
        return entities.OrderBy(f => f.Date).ThenBy(f => f.KickOff).Select(e => e.ToDomain()).ToList();
    }

    public async Task<List<Fixture>> GetFixturesAsync(DateTime? date = null, int? divisionId = null)
    {
        List<FixtureEntity> entities;
        if (date.HasValue)
            entities = await _fixtureRepo.GetByDateAsync(date.Value);
        else if (divisionId.HasValue && divisionId.Value > 0)
            entities = await _fixtureRepo.GetByDivisionAsync(divisionId.Value);
        else
            entities = await _fixtureRepo.GetAllAsync();

        if (date.HasValue && divisionId.HasValue && divisionId.Value > 0)
            entities = entities.Where(f => f.DivisionId == divisionId.Value).ToList();

        return entities.OrderBy(f => f.Date).ThenBy(f => f.KickOff)
            .ThenBy(f => f.DivisionId).Select(e => e.ToDomain()).ToList();
    }

    public async Task<Fixture?> GetFixtureByIdAsync(int id)
    {
        var entity = await _fixtureRepo.GetByIdWithAssignmentsAsync(id);
        return entity?.ToDomain();
    }

    public async Task<List<DateTime>> GetFixtureDatesAsync()
    {
        return await _fixtureRepo.GetDistinctDatesAsync();
    }

    public async Task<List<(DateTime Date, int FixtureCount, int AssignedCount)>> GetDateSummaryAsync()
    {
        var all = await _fixtureRepo.GetAllAsync();
        return all.GroupBy(f => f.Date.Date)
            .Select(g => (
                Date: g.Key,
                FixtureCount: g.Count(),
                AssignedCount: g.Count(f => f.OfficialAssignments?.Count(a => true) >= 4)
            ))
            .OrderBy(x => x.Date).ToList();
    }

    // ─── Assignment Logic ───

    public async Task<AssignmentConflict?> AssignOfficialAsync(int fixtureId, int refereeId, Models.OfficialRole role)
    {
        var fixture = await _fixtureRepo.GetByIdWithAssignmentsAsync(fixtureId);
        var refereeEntity = await _refereeRepo.GetByIdAsync(refereeId);
        if (fixture == null || refereeEntity == null) return null;

        var dataRole = role.ToData();

        // Check same-day conflict
        var conflicts = await _assignmentRepo.GetConflictsAsync(refereeId, fixture.Date, fixtureId);
        if (conflicts.Any())
        {
            var c = conflicts.First();
            return new AssignmentConflict
            {
                RefereeId = refereeId,
                RefereeName = $"{refereeEntity.FirstName} {refereeEntity.LastName}",
                ConflictFixtureId = c.FixtureId,
                ConflictMatch = $"{c.Fixture.HomeTeam} vs {c.Fixture.AwayTeam}",
                ConflictRole = c.Role.ToDomain(),
            };
        }

        // Check if already assigned in THIS fixture
        var existingInFixture = fixture.OfficialAssignments?.FirstOrDefault(a => a.RefereeId == refereeId);
        if (existingInFixture != null)
        {
            return new AssignmentConflict
            {
                RefereeId = refereeId,
                RefereeName = $"{refereeEntity.FirstName} {refereeEntity.LastName}",
                ConflictFixtureId = fixtureId,
                ConflictMatch = $"{fixture.HomeTeam} vs {fixture.AwayTeam}",
                ConflictRole = existingInFixture.Role.ToDomain(),
            };
        }

        // Remove existing assignment in this role if any
        await _assignmentRepo.RemoveByFixtureAndRoleAsync(fixtureId, dataRole);

        // Add new assignment
        var assignment = FixtureMapper.ToAssignmentEntity(fixtureId, refereeId, dataRole);
        await _assignmentRepo.AddAsync(assignment);
        return null;
    }

    public async Task UnassignOfficialAsync(int fixtureId, Models.OfficialRole role)
    {
        await _assignmentRepo.RemoveByFixtureAndRoleAsync(fixtureId, role.ToData());
    }

    public async Task<AssignmentConflict?> GetConflictAsync(int refereeId, DateTime date, int excludeFixtureId)
    {
        var conflicts = await _assignmentRepo.GetConflictsAsync(refereeId, date, excludeFixtureId);
        if (!conflicts.Any()) return null;

        var c = conflicts.First();
        var referee = await _refereeRepo.GetByIdAsync(refereeId);
        return new AssignmentConflict
        {
            RefereeId = refereeId,
            RefereeName = referee != null ? $"{referee.FirstName} {referee.LastName}" : "",
            ConflictFixtureId = c.FixtureId,
            ConflictMatch = $"{c.Fixture?.HomeTeam} vs {c.Fixture?.AwayTeam}",
            ConflictRole = c.Role.ToDomain(),
        };
    }

    public async Task<List<(Referee Referee, AssignmentConflict? Conflict, bool AlreadyInFixture)>>
        GetAvailableRefereesAsync(int fixtureId, DateTime date, string? searchQuery = null)
    {
        var fixture = await _fixtureRepo.GetByIdWithAssignmentsAsync(fixtureId);
        var referees = await _refereeRepo.SearchAsync(searchQuery, null, null, null);
        var activeReferees = referees.Where(r =>
            r.Status == DataEnums.RefereeStatus.Active || r.Status == DataEnums.RefereeStatus.Probation);

        var result = new List<(Referee, AssignmentConflict?, bool)>();
        foreach (var r in activeReferees)
        {
            var conflict = await GetConflictAsync(r.Id, date, fixtureId);
            var alreadyIn = fixture?.OfficialAssignments?.Any(a => a.RefereeId == r.Id) ?? false;
            result.Add((r.ToDomain(), conflict, alreadyIn));
        }

        return result
            .OrderBy(x => x.Item2 != null ? 1 : 0)
            .ThenBy(x => x.Item3 ? 1 : 0)
            .ThenBy(x => x.Item1.LastName)
            .ToList();
    }

    // ─── Stats ───

    public async Task<(int totalFixtures, int fullyAssigned, int partiallyAssigned, int unassigned)> GetAssignmentStatsAsync()
    {
        var all = await _fixtureRepo.GetAllAsync();
        var fixtures = all.Select(f => f.ToDomain()).ToList();
        var total = fixtures.Count;
        var full = fixtures.Count(f => f.IsFullyAssigned);
        var partial = fixtures.Count(f => f.AssignedCount > 0 && !f.IsFullyAssigned);
        var none = fixtures.Count(f => f.AssignedCount == 0);
        return (total, full, partial, none);
    }
}
