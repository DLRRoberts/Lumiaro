using Lumiaro.MatchValidation.Models;

namespace Lumiaro.MatchValidation.Services;

/// <summary>
/// Scoped-per-circuit session service. Owns the canonical <see cref="ValidationSession"/>
/// for the current referee and exposes an event every time it changes so the
/// page orchestrator can re-render. All mutations happen through this service
/// so the session state remains consistent and auditable.
/// </summary>
public interface IMatchValidationSessionService
{
    ValidationSession Current { get; }

    event Action? SessionChanged;

    Task InitializeAsync(Guid matchId, MatchInfo matchInfo, CancellationToken ct = default);

    /// <summary>
    /// Overload that accepts pre-built squads so the caller can supply real
    /// team names and rosters (e.g. loaded from a fixture) instead of relying
    /// on the squad provider.
    /// </summary>
    Task InitializeAsync(MatchInfo matchInfo, Squad homeSquad, Squad awaySquad, CancellationToken ct = default);

    /// <summary>Add an event to the pending queue (typically from the VDC feed).</summary>
    void PushPending(MatchEvent @event);

    /// <summary>Validate an event as-is without editing.</summary>
    Result<MatchEvent> QuickValidate(Guid eventId);

    /// <summary>Replace a pending event with an edited version and move it to validated.</summary>
    Result<MatchEvent> ConfirmEdited(MatchEvent edited);

    /// <summary>Update the description on a post-match incident entry.</summary>
    void UpdateIncidentDescription(Guid incidentId, string description);

    /// <summary>Gate check — true iff the referee may submit the report.</summary>
    bool CanFinalize();
}
