namespace Lumiaro.MatchValidation.Models;

/// <summary>
/// Disciplinary card outcomes recognised by the validation tool.
/// <see cref="SecondYellow"/> is modelled separately because it triggers
/// red-card consequences (mandatory post-match incident entry) while
/// preserving the original charge-code pool of a yellow.
/// </summary>
public enum CardType
{
    Yellow,
    SecondYellow,
    Red
}

public static class CardTypeExtensions
{
    public static bool IsSendOff(this CardType cardType) =>
        cardType is CardType.SecondYellow or CardType.Red;

    public static string DisplayLabel(this CardType cardType) => cardType switch
    {
        CardType.Yellow       => "Yellow Card",
        CardType.SecondYellow => "2nd Yellow Card",
        CardType.Red          => "Direct Red Card",
        _                     => "Unknown"
    };

    public static string FormValue(this CardType cardType) => cardType switch
    {
        CardType.Yellow       => "yellow",
        CardType.SecondYellow => "2y",
        CardType.Red          => "red",
        _                     => string.Empty
    };

    public static CardType? Parse(string? value) => value switch
    {
        "yellow" => CardType.Yellow,
        "2y"     => CardType.SecondYellow,
        "red"    => CardType.Red,
        _        => null
    };
}
