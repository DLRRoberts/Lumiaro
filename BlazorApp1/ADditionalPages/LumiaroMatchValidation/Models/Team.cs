namespace Lumiaro.MatchValidation.Models;

/// <summary>
/// Stable identifier for a match participant. Keeping this as an enum rather
/// than a string keeps switch expressions exhaustive and lets us pattern-match
/// in own-goal attribution logic.
/// </summary>
public enum TeamKey
{
    Home,
    Away
}

/// <summary>
/// A team taking part in the match. <see cref="Key"/> is the side (home/away);
/// <see cref="ShortName"/> is the display label (e.g. "FC Bayern München").
/// </summary>
public record Team(TeamKey Key, string ShortName)
{
    public static readonly Team Empty = new(TeamKey.Home, string.Empty);
}
