using Microsoft.EntityFrameworkCore;
using LumiaroAdmin.Data.Entities;
using LumiaroAdmin.Data.Enums;
using RedZone.LumiaroAdmin.Data;

namespace LumiaroAdmin.Data.Repositories;

public interface IFixtureRepository : IRepository<FixtureEntity>
{
    Task<FixtureEntity?> GetByIdWithAssignmentsAsync(int id, CancellationToken ct = default);
    Task<FixtureEntity?> GetByIdWithFullDetailsAsync(int id, CancellationToken ct = default);
    Task<List<FixtureEntity>> GetByDateAsync(DateTime date, CancellationToken ct = default);
    Task<List<FixtureEntity>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken ct = default);
    Task<List<FixtureEntity>> GetByDivisionAsync(int divisionId, CancellationToken ct = default);
    Task<List<FixtureEntity>> GetSameDayFixturesAsync(DateTime date, int excludeFixtureId, CancellationToken ct = default);
    Task<List<FixtureEntity>> GetByMatchdayAsync(int matchday, int divisionId, CancellationToken ct = default);
    Task<List<DateTime>> GetDistinctDatesAsync(CancellationToken ct = default);
    Task<bool> IsRefereeAssignedOnDateAsync(int refereeId, DateTime date, int excludeFixtureId, CancellationToken ct = default);
}

public class FixtureRepository : RepositoryBase<FixtureEntity>, IFixtureRepository
{
    public FixtureRepository(Func<LumiaroDbContext> db) : base(db) { }

    public async Task<FixtureEntity?> GetByIdWithAssignmentsAsync(int id, CancellationToken ct = default)
    {
        await using var uow = GetUnitOfWork();
        return await uow.Set
            .Include(f => f.Division)
            .Include(f => f.OfficialAssignments)
                .ThenInclude(a => a.Referee)
            .FirstOrDefaultAsync(f => f.Id == id, ct);
    }

    public async Task<FixtureEntity?> GetByIdWithFullDetailsAsync(int id, CancellationToken ct = default)
    {
        await using var uow = GetUnitOfWork();
        return await uow.Set
            .Include(f => f.Division)
            .Include(f => f.OfficialAssignments)
                .ThenInclude(a => a.Referee)
            .Include(f => f.Assessments)
            .FirstOrDefaultAsync(f => f.Id == id, ct);
    }

    public async Task<List<FixtureEntity>> GetByDateAsync(DateTime date, CancellationToken ct = default)
    {
        await using var uow = GetUnitOfWork();
        return await uow.Set.AsNoTracking()
            .Include(f => f.Division)
            .Include(f => f.OfficialAssignments)
                .ThenInclude(a => a.Referee)
            .Where(f => f.Date == date.Date && !f.IsCancelled)
            .OrderBy(f => f.KickOff)
            .ThenBy(f => f.Division.SortOrder)
            .ToListAsync(ct);
    }

    public async Task<List<FixtureEntity>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken ct = default)
    {
        await using var uow = GetUnitOfWork();
        return await uow.Set.AsNoTracking()
            .Include(f => f.Division)
            .Include(f => f.OfficialAssignments)
                .ThenInclude(a => a.Referee)
            .Where(f => f.Date >= from.Date && f.Date <= to.Date && !f.IsCancelled)
            .OrderBy(f => f.Date)
            .ThenBy(f => f.KickOff)
            .ToListAsync(ct);
    }

    public async Task<List<FixtureEntity>> GetByDivisionAsync(int divisionId, CancellationToken ct = default)
    {
        await using var uow = GetUnitOfWork();
        return await uow.Set.AsNoTracking()
            .Include(f => f.Division)
            .Include(f => f.OfficialAssignments)
            .Where(f => f.DivisionId == divisionId && !f.IsCancelled)
            .OrderBy(f => f.Date)
            .ThenBy(f => f.KickOff)
            .ToListAsync(ct);
    }

    public async Task<List<FixtureEntity>> GetSameDayFixturesAsync(DateTime date, int excludeFixtureId, CancellationToken ct = default)
    {
        await using var uow = GetUnitOfWork();
        return await uow.Set.AsNoTracking()
            .Include(f => f.Division)
            .Include(f => f.OfficialAssignments)
                .ThenInclude(a => a.Referee)
            .Where(f => f.Date == date.Date && f.Id != excludeFixtureId && !f.IsCancelled)
            .OrderBy(f => f.KickOff)
            .ToListAsync(ct);
    }

    public async Task<List<FixtureEntity>> GetByMatchdayAsync(int matchday, int divisionId, CancellationToken ct = default)
    {
        await using var uow = GetUnitOfWork();
        return await uow.Set.AsNoTracking()
            .Include(f => f.Division)
            .Where(f => f.Matchday == matchday && f.DivisionId == divisionId)
            .OrderBy(f => f.KickOff)
            .ToListAsync(ct);
    }

    public async Task<List<DateTime>> GetDistinctDatesAsync(CancellationToken ct = default)
    {
        await using var uow = GetUnitOfWork();
        return await uow.Set.AsNoTracking()
            .Where(f => !f.IsCancelled)
            .Select(f => f.Date)
            .Distinct()
            .OrderBy(d => d)
            .ToListAsync(ct);
    }

    public async Task<bool> IsRefereeAssignedOnDateAsync(int refereeId, DateTime date, int excludeFixtureId, CancellationToken ct = default)
    {
        await using var context = _db();
        return await context.OfficialAssignments.AsNoTracking()
            .AnyAsync(a =>
                a.RefereeId == refereeId &&
                a.Fixture.Date == date.Date &&
                a.FixtureId != excludeFixtureId &&
                !a.Fixture.IsCancelled, ct);
    }
}
