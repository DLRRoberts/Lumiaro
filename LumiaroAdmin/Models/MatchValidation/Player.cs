namespace Lumiaro.MatchValidation.Models;

/// <summary>
/// A player on a team sheet. <see cref="Token"/> returns the same
/// "num|name" encoding the original HTML used for select option values —
/// useful when we need to round-trip a player identity through a form
/// control without holding an object reference.
/// </summary>
public record Player(int ShirtNumber, string Name)
{
    public string Token => $"{ShirtNumber}|{Name}";

    public string DisplayLabel => $"{ShirtNumber} — {Name}";

    public static Player? FromToken(string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return null;
        }

        var parts = token.Split('|', 2);
        if (parts.Length != 2 || !int.TryParse(parts[0], out var number))
        {
            return null;
        }

        return new Player(number, parts[1]);
    }
}
