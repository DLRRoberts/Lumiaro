using System.Collections.Immutable;

namespace Lumiaro.MatchValidation.Models;

/// <summary>
/// Aggregate root for a single match validation session. Held as a single
/// immutable record so every state mutation goes through the session service
/// and can be traced, serialised, or replayed. This is also what the page
/// orchestrator cascades down to children.
/// </summary>
public record ValidationSession(
    MatchInfo Match,
    Squad HomeSquad,
    Squad AwaySquad,
    ImmutableList<MatchEvent> Pending,
    ImmutableList<MatchEvent> Validated,
    MatchRosterState Rosters,
    MatchPhase Phase,
    ImmutableList<IncidentReportItem> Incidents,
    int HomeScore,
    int AwayScore)
{
    public Squad SquadFor(TeamKey team) => team switch
    {
        TeamKey.Home => HomeSquad,
        TeamKey.Away => AwaySquad,
        _            => throw new ArgumentOutOfRangeException(nameof(team))
    };

    public bool CanFinalize =>
        Pending.Count == 0 &&
        Incidents.All(i => i.IsSatisfied);

    public int MandatoryPendingCount =>
        Incidents.Count(i => i.IsMandatory && !i.IsSatisfied);
}

/// <summary>
/// Static match-level metadata displayed in the match card.
/// </summary>
public record MatchInfo(
    string CompetitionLabel,
    string VenueName,
    DateTimeOffset KickOff)
{
    public static readonly MatchInfo Placeholder = new("Competition", "Venue", DateTimeOffset.UtcNow);
}
