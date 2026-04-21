using System.Collections.Immutable;
using Lumiaro.MatchValidation.Models;

namespace Lumiaro.MatchValidation.Services;

/// <summary>
/// Scoped (per Blazor Server circuit) state service for a validation session.
/// Follows the same ownership pattern as the Sefe ThemeService — mutable inside
/// the service, exposed as an immutable record outside it, with an event
/// raised on every change so consumers re-render via StateHasChanged().
///
/// The orchestration methods in here are intentionally short; every non-trivial
/// transformation is delegated to a pure helper so the mutation flow reads top
/// to bottom as prose.
/// </summary>
public sealed class MatchValidationSessionService : IMatchValidationSessionService
{
    private readonly ISquadProvider _squadProvider;
    private ValidationSession _session = CreateEmptySession();

    public MatchValidationSessionService(ISquadProvider squadProvider)
    {
        _squadProvider = squadProvider;
    }

    public ValidationSession Current => _session;

    public event Action? SessionChanged;

    public async Task InitializeAsync(Guid matchId, MatchInfo matchInfo, CancellationToken ct = default)
    {
        var (home, away) = await _squadProvider.GetSquadsAsync(matchId, ct);
        _session = BuildFreshSession(matchInfo, home, away);
        Notify();
    }

    public void PushPending(MatchEvent @event)
    {
        _session = _session with
        {
            Pending = InsertInMinuteOrder(_session.Pending, @event),
            Phase   = MaybePreloadPhase(_session.Phase, @event)
        };
        Notify();
    }

    public Result<MatchEvent> QuickValidate(Guid eventId)
    {
        var target = _session.Pending.FirstOrDefault(e => e.Id == eventId);
        if (target is null)
        {
            return Result.Err<MatchEvent>("Event not found in pending queue.");
        }

        return ConfirmInternal(target);
    }

    public Result<MatchEvent> ConfirmEdited(MatchEvent edited)
    {
        var existing = _session.Pending.FirstOrDefault(e => e.Id == edited.Id);
        if (existing is null)
        {
            return Result.Err<MatchEvent>("Edited event not found in pending queue.");
        }

        return ConfirmInternal(edited);
    }

    public void UpdateIncidentDescription(Guid incidentId, string description)
    {
        var updated = _session.Incidents
            .Select(i => i.Id == incidentId ? i with { Description = description } : i)
            .ToImmutableList();
        _session = _session with { Incidents = updated };
        Notify();
    }

    public bool CanFinalize() => _session.CanFinalize;

    // ── orchestration helpers ──────────────────────────────────────────────

    private Result<MatchEvent> ConfirmInternal(MatchEvent @event)
    {
        var next = _session with
        {
            Pending   = _session.Pending.Where(e => e.Id != @event.Id).ToImmutableList(),
            Validated = InsertInMinuteOrder(_session.Validated, @event)
        };

        next = ApplySideEffects(next, @event);
        _session = next;
        Notify();
        return Result.Ok(@event);
    }

    private static ValidationSession ApplySideEffects(ValidationSession session, MatchEvent @event) => @event switch
    {
        GoalEvent goal         => ApplyGoal(session, goal),
        SubstitutionEvent sub  => ApplySubstitution(session, sub),
        TimingEvent timing     => ApplyTiming(session, timing),
        CardEvent card         => ApplyCardIncident(session, card),
        _                      => session
    };

    private static ValidationSession ApplyGoal(ValidationSession session, GoalEvent goal)
    {
        // Own goals credit the opposition. Regular goals credit the scoring team.
        var creditedSide = goal.IsOwnGoal
            ? OppositeSide(goal.ScoringTeam)
            : goal.ScoringTeam;

        return creditedSide == TeamKey.Home
            ? session with { HomeScore = session.HomeScore + 1 }
            : session with { AwayScore = session.AwayScore + 1 };
    }

    private static ValidationSession ApplySubstitution(ValidationSession session, SubstitutionEvent sub)
    {
        var roster = session.Rosters.For(sub.Team).ApplySubstitution(sub.PlayerOff, sub.PlayerOn);
        return session with { Rosters = session.Rosters.With(sub.Team, roster) };
    }

    private static ValidationSession ApplyTiming(ValidationSession session, TimingEvent timing) =>
        session with { Phase = session.Phase.With(timing.Kind, timing.RealTime) };

    private static ValidationSession ApplyCardIncident(ValidationSession session, CardEvent card)
    {
        if (!card.CardType.IsSendOff())
        {
            return session;
        }

        var chargeLabel = card.Offence is null
            ? card.Charge.Code
            : $"{card.Charge.Code} · {card.Offence.Label}";

        var incident = new IncidentReportItem(
            Id:            Guid.NewGuid(),
            Category:      IncidentCategory.RedCard,
            Title:         $"{card.Player.DisplayLabel} — {card.Minute}' · {chargeLabel}",
            IsMandatory:   true,
            Description:   string.Empty,
            LinkedEventId: card.Id);

        // Keep the General entry at the tail of the list to mirror the HTML layout.
        var general = session.Incidents.Where(i => i.Category == IncidentCategory.General);
        var others  = session.Incidents.Where(i => i.Category != IncidentCategory.General);
        var rebuilt = others.Append(incident).Concat(general).ToImmutableList();
        return session with { Incidents = rebuilt };
    }

    private static MatchPhase MaybePreloadPhase(MatchPhase phase, MatchEvent @event) =>
        @event is TimingEvent timing
            ? phase.With(timing.Kind, timing.RealTime)
            : phase;

    private static ImmutableList<MatchEvent> InsertInMinuteOrder(
        ImmutableList<MatchEvent> list, MatchEvent @event)
    {
        var index = list.FindIndex(existing => @event.Minute <= existing.Minute);
        return index < 0 ? list.Add(@event) : list.Insert(index, @event);
    }

    private static TeamKey OppositeSide(TeamKey team) =>
        team == TeamKey.Home ? TeamKey.Away : TeamKey.Home;

    // ── session construction ───────────────────────────────────────────────

    private static ValidationSession BuildFreshSession(MatchInfo info, Squad home, Squad away) =>
        new(
            Match:      info,
            HomeSquad:  home,
            AwaySquad:  away,
            Pending:    ImmutableList<MatchEvent>.Empty,
            Validated:  ImmutableList<MatchEvent>.Empty,
            Rosters:    new MatchRosterState(TeamRoster.FromSquad(home), TeamRoster.FromSquad(away)),
            Phase:      new MatchPhase(),
            Incidents:  ImmutableList.Create(BuildDefaultGeneralIncident()),
            HomeScore:  0,
            AwayScore:  0);

    private static IncidentReportItem BuildDefaultGeneralIncident() =>
        new(
            Id:          Guid.NewGuid(),
            Category:    IncidentCategory.General,
            Title:       "Additional match incidents",
            IsMandatory: false,
            Description: string.Empty);

    private static ValidationSession CreateEmptySession() =>
        new(
            Match:      MatchInfo.Placeholder,
            HomeSquad:  new Squad(Team.Empty, ImmutableArray<Player>.Empty, ImmutableArray<Player>.Empty),
            AwaySquad:  new Squad(Team.Empty, ImmutableArray<Player>.Empty, ImmutableArray<Player>.Empty),
            Pending:    ImmutableList<MatchEvent>.Empty,
            Validated:  ImmutableList<MatchEvent>.Empty,
            Rosters:    new MatchRosterState(
                            new TeamRoster(ImmutableArray<Player>.Empty, ImmutableArray<Player>.Empty, ImmutableArray<Player>.Empty),
                            new TeamRoster(ImmutableArray<Player>.Empty, ImmutableArray<Player>.Empty, ImmutableArray<Player>.Empty)),
            Phase:      new MatchPhase(),
            Incidents:  ImmutableList<IncidentReportItem>.Empty,
            HomeScore:  0,
            AwayScore:  0);

    private void Notify() => SessionChanged?.Invoke();
}
