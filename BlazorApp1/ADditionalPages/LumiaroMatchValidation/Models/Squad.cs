using System.Collections.Immutable;

namespace Lumiaro.MatchValidation.Models;

/// <summary>
/// A team's pre-match sheet. <see cref="StartingXi"/> and <see cref="Substitutes"/>
/// are immutable — mutation happens via <see cref="MatchRosterState"/> which
/// tracks who is currently on the pitch.
/// </summary>
public record Squad(Team Team, ImmutableArray<Player> StartingXi, ImmutableArray<Player> Substitutes);
