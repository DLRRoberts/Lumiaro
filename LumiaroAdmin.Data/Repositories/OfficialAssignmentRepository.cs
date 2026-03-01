using Microsoft.EntityFrameworkCore;
using LumiaroAdmin.Data.Entities;
using LumiaroAdmin.Data.Enums;
using RedZone.LumiaroAdmin.Data;

namespace LumiaroAdmin.Data.Repositories;

public interface IOfficialAssignmentRepository : IRepository<OfficialAssignmentEntity>
{
    Task<List<OfficialAssignmentEntity>> GetByFixtureAsync(int fixtureId, CancellationToken ct = default);
    Task<List<OfficialAssignmentEntity>> GetByRefereeAsync(int refereeId, CancellationToken ct = default);
    Task<OfficialAssignmentEntity?> GetByFixtureAndRoleAsync(int fixtureId, OfficialRole role, CancellationToken ct = default);
    Task<bool> IsRoleFilledAsync(int fixtureId, OfficialRole role, CancellationToken ct = default);
    Task<List<OfficialAssignmentEntity>> GetConflictsAsync(int refereeId, DateTime fixtureDate,
        int excludeFixtureId, CancellationToken ct = default);
    Task RemoveByFixtureAndRoleAsync(int fixtureId, OfficialRole role, CancellationToken ct = default);
}

public class OfficialAssignmentRepository : RepositoryBase<OfficialAssignmentEntity>, IOfficialAssignmentRepository
{
    public OfficialAssignmentRepository(Func<LumiaroDbContext> db) : base(db) { }

    public async Task<List<OfficialAssignmentEntity>> GetByFixtureAsync(int fixtureId, CancellationToken ct = default)
    {
        await using var uow = GetUnitOfWork();
        return await uow.Set.AsNoTracking()
            .Include(a => a.Referee)
            .Where(a => a.FixtureId == fixtureId)
            .OrderBy(a => a.Role)
            .ToListAsync(ct);
    }

    public async Task<List<OfficialAssignmentEntity>> GetByRefereeAsync(int refereeId, CancellationToken ct = default)
    {
        await using var uow = GetUnitOfWork();
        return await uow.Set.AsNoTracking()
            .Include(a => a.Fixture)
                .ThenInclude(f => f.Division)
            .Where(a => a.RefereeId == refereeId)
            .OrderByDescending(a => a.Fixture.Date)
            .ToListAsync(ct);
    }

    public async Task<OfficialAssignmentEntity?> GetByFixtureAndRoleAsync(int fixtureId, OfficialRole role, CancellationToken ct = default)
    {
        await using var uow = GetUnitOfWork();
        return await uow.Set.AsNoTracking()
            .Include(a => a.Referee)
            .FirstOrDefaultAsync(a => a.FixtureId == fixtureId && a.Role == role, ct);
    }

    public async Task<bool> IsRoleFilledAsync(int fixtureId, OfficialRole role, CancellationToken ct = default)
    {
        await using var uow = GetUnitOfWork();
        return await uow.Set.AsNoTracking()
            .AnyAsync(a => a.FixtureId == fixtureId && a.Role == role, ct);
    }

    public async Task<List<OfficialAssignmentEntity>> GetConflictsAsync(int refereeId, DateTime fixtureDate,
        int excludeFixtureId, CancellationToken ct = default)
    {
        await using var uow = GetUnitOfWork();
        return await uow.Set.AsNoTracking()
            .Include(a => a.Fixture)
            .Where(a =>
                a.RefereeId == refereeId &&
                a.Fixture.Date == fixtureDate.Date &&
                a.FixtureId != excludeFixtureId &&
                !a.Fixture.IsCancelled)
            .ToListAsync(ct);
    }

    public async Task RemoveByFixtureAndRoleAsync(int fixtureId, OfficialRole role, CancellationToken ct = default)
    {
        await using var uow = GetUnitOfWork();
        var entity = await uow.Set.FirstOrDefaultAsync(a => a.FixtureId == fixtureId && a.Role == role, ct);
        if (entity != null)
            uow.Set.Remove(entity);
    }
}
