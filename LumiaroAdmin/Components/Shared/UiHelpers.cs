using LumiaroAdmin.Models;

namespace LumiaroAdmin.Components.Shared;

public static class UiHelpers
{
    public static string TierCssClass(RefereeTier tier) => tier switch
    {
        RefereeTier.FifaInternational => "tier-fifa",
        RefereeTier.PremierLeague => "tier-premier",
        RefereeTier.Championship => "tier-championship",
        RefereeTier.LeagueOne or RefereeTier.LeagueTwo => "tier-league1",
        RefereeTier.NationalLeague or RefereeTier.Grassroots => "tier-grassroots",
        _ => "tier-grassroots"
    };

    public static string TierLabel(RefereeTier tier) => tier switch
    {
        RefereeTier.FifaInternational => "FIFA",
        RefereeTier.PremierLeague => "PREMIER",
        RefereeTier.Championship => "CHAMPIONSHIP",
        RefereeTier.LeagueOne => "LEAGUE 1",
        RefereeTier.LeagueTwo => "LEAGUE 2",
        RefereeTier.NationalLeague => "NATIONAL",
        RefereeTier.Grassroots => "GRASSROOTS",
        _ => "—"
    };

    public static string TierLabelFull(RefereeTier tier) => tier switch
    {
        RefereeTier.FifaInternational => "FIFA International",
        RefereeTier.PremierLeague => "Premier League",
        RefereeTier.Championship => "Championship",
        RefereeTier.LeagueOne => "League One",
        RefereeTier.LeagueTwo => "League Two",
        RefereeTier.NationalLeague => "National League",
        RefereeTier.Grassroots => "Grassroots",
        _ => "—"
    };

    public static string StatusCssClass(RefereeStatus status) => status switch
    {
        RefereeStatus.Active => "badge-active",
        RefereeStatus.Inactive => "badge-inactive",
        RefereeStatus.Probation => "badge-probation",
        RefereeStatus.Suspended => "badge-suspended",
        _ => "badge-inactive"
    };

    public static string StatusLabel(RefereeStatus status) => status switch
    {
        RefereeStatus.Active => "Active",
        RefereeStatus.Inactive => "Inactive",
        RefereeStatus.Probation => "Probation",
        RefereeStatus.Suspended => "Suspended",
        _ => "Unknown"
    };

    public static string ScoreColor(double? score) => score switch
    {
        >= 8.0 => "var(--accent-lime)",
        >= 7.5 => "var(--accent-cyan)",
        >= 7.0 => "var(--accent-orange)",
        _ => "var(--accent-red)"
    };

    public static string TrendArrow(double change) => change switch
    {
        > 0 => $"▲ +{change:F1}",
        < 0 => $"▼ {change:F1}",
        _ => "—"
    };

    public static string TrendClass(double change) => change switch
    {
        > 0 => "color:var(--accent-lime)",
        < 0 => "color:var(--accent-red)",
        _ => "color:var(--text-dim)"
    };
}
