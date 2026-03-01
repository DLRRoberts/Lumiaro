using LumiaroAdmin.Data.Entities;
using LumiaroAdmin.Data.Enums;

namespace RedZone.LumiaroAdmin.Data.Mapping;

public static class FixtureMapper
{
    public static DivisionEntity ToDivisionEntity(string name, string shortName, RefereeTier tier, int sortOrder)
    {
        return new DivisionEntity
        {
            Name = name,
            ShortName = shortName,
            Tier = tier,
            SortOrder = sortOrder,
            IsActive = true
        };
    }

    public static FixtureEntity ToEntity(int divisionId, DateTime date, TimeSpan kickOff,
        string homeTeam, string awayTeam, string? venue, int matchday)
    {
        return new FixtureEntity
        {
            DivisionId = divisionId,
            Date = date,
            KickOff = kickOff,
            HomeTeam = homeTeam,
            AwayTeam = awayTeam,
            Venue = venue,
            Matchday = matchday,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static void UpdateEntity(FixtureEntity entity, DateTime date, TimeSpan kickOff,
        string homeTeam, string awayTeam, string? venue, int matchday)
    {
        entity.Date = date;
        entity.KickOff = kickOff;
        entity.HomeTeam = homeTeam;
        entity.AwayTeam = awayTeam;
        entity.Venue = venue;
        entity.Matchday = matchday;
        entity.UpdatedAt = DateTime.UtcNow;
    }

    public static OfficialAssignmentEntity ToAssignmentEntity(int fixtureId, int refereeId,
        OfficialRole role, string? assignedBy = null)
    {
        return new OfficialAssignmentEntity
        {
            FixtureId = fixtureId,
            RefereeId = refereeId,
            Role = role,
            AssignedAt = DateTime.UtcNow,
            AssignedBy = assignedBy
        };
    }
}
