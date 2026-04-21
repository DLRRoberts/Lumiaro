using System.Collections.Immutable;

namespace Lumiaro.MatchValidation.Models;

/// <summary>
/// Mutable-through-`with`-expressions roster state for one team.
/// <see cref="OnPitch"/> starts as the XI and evolves as subs are made.
/// <see cref="Available"/> starts as the bench.
/// <see cref="SubbedOff"/> is the audit trail.
/// </summary>
public record TeamRoster(
    ImmutableArray<Player> OnPitch,
    ImmutableArray<Player> Available,
    ImmutableArray<Player> SubbedOff)
{
    public static TeamRoster FromSquad(Squad squad) =>
        new(squad.StartingXi, squad.Substitutes, ImmutableArray<Player>.Empty);

    public TeamRoster ApplySubstitution(Player playerOff, Player playerOn) => new(
        OnPitch:   OnPitch.Remove(playerOff).Add(playerOn),
        Available: Available.Remove(playerOn),
        SubbedOff: SubbedOff.Add(playerOff));
}

/// <summary>
/// Roster state for both teams indexed by side.
/// </summary>
public record MatchRosterState(TeamRoster Home, TeamRoster Away)
{
    public TeamRoster For(TeamKey team) => team switch
    {
        TeamKey.Home => Home,
        TeamKey.Away => Away,
        _            => throw new ArgumentOutOfRangeException(nameof(team))
    };

    public MatchRosterState With(TeamKey team, TeamRoster updated) => team switch
    {
        TeamKey.Home => this with { Home = updated },
        TeamKey.Away => this with { Away = updated },
        _            => throw new ArgumentOutOfRangeException(nameof(team))
    };
}
