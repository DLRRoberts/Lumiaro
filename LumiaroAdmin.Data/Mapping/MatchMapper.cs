using LumiaroAdmin.Data.Entities;
using LumiaroAdmin.Data.Enums;

namespace LumiaroAdmin.Data.Mapping;

public static class MatchMapper
{
    public static MatchEntity ToEntity(DateTime date, string homeTeam, string awayTeam,
        string competition, RefereeTier competitionTier, string role,
        int? refereeId = null, int? fixtureId = null,
        int? homeScore = null, int? awayScore = null,
        int yellowCards = 0, int redCards = 0,
        decimal? assessmentScore = null,
        string? assistants = null, string? fourthOfficial = null)
    {
        return new MatchEntity
        {
            Date = date,
            HomeTeam = homeTeam,
            AwayTeam = awayTeam,
            Competition = competition,
            CompetitionTier = competitionTier,
            Role = role,
            RefereeId = refereeId,
            FixtureId = fixtureId,
            HomeScore = homeScore,
            AwayScore = awayScore,
            YellowCards = yellowCards,
            RedCards = redCards,
            AssessmentScore = assessmentScore,
            Assistants = assistants,
            FourthOfficial = fourthOfficial
        };
    }

    public static MatchIncidentEntity ToIncidentEntity(int matchId,
        string description, string? incidentType = null,
        int? minute = null, string? notes = null)
    {
        return new MatchIncidentEntity
        {
            MatchId = matchId,
            Description = description,
            IncidentType = incidentType,
            Minute = minute,
            Notes = notes
        };
    }
}
