using Lumiaro.MatchValidation.Models;
using Lumiaro.MatchValidation.Services;
using Microsoft.AspNetCore.Components;

namespace Lumiaro.MatchValidation.Components;

public enum RowMode { Pending, Validated }

public partial class EventRow : ComponentBase
{
    [Inject] public IMinuteFormatter MinuteFormatter { get; set; } = default!;

    [Parameter, EditorRequired] public MatchEvent Event { get; set; } = default!;
    [Parameter, EditorRequired] public RowMode Mode { get; set; }
    [Parameter, EditorRequired] public Team HomeTeam { get; set; } = Team.Empty;
    [Parameter, EditorRequired] public Team AwayTeam { get; set; } = Team.Empty;
    [Parameter, EditorRequired] public Squad HomeSquad { get; set; } = default!;
    [Parameter, EditorRequired] public Squad AwaySquad { get; set; } = default!;
    [Parameter, EditorRequired] public MatchRosterState Rosters { get; set; } = default!;
    [Parameter, EditorRequired] public MatchPhase Phase { get; set; } = default!;
    [Parameter] public bool IsNewArrival { get; set; } = true;
    [Parameter] public EventCallback<Guid> OnQuickValidate { get; set; }
    [Parameter] public EventCallback<MatchEvent> OnConfirmEdit { get; set; }

    private bool IsEditOpen { get; set; }

    // ── derived presentation properties ────────────────────────────────────

    private (string Main, string Sub) Summary => BuildSummary(Event);

    private string SourceLabel => Event.SourceNote ?? Event.Source switch
    {
        EventSource.VdcPush     => "VDC input",
        EventSource.ManualEntry => "Manual entry",
        EventSource.Correction  => "Correction",
        _                       => string.Empty
    };

    private string MinuteDisplay => Event switch
    {
        TimingEvent t => MinuteFormatter.FormatTimingEvent(t.Kind, Phase),
        _             => MinuteFormatter.FormatRaw(Event.Minute, Phase)
    };

    private string IconCssClass => Event switch
    {
        GoalEvent { IsOwnGoal: true }           => "icon-og",
        GoalEvent                               => "icon-goal",
        CardEvent { CardType: CardType.Red }    => "icon-red",
        CardEvent                               => "icon-yellow",
        SubstitutionEvent                       => "icon-sub",
        TimingEvent                             => "icon-timing",
        _                                       => string.Empty
    };

    private string IconLetter => Event switch
    {
        GoalEvent { IsOwnGoal: true }                => "OG",
        GoalEvent                                    => "G",
        CardEvent { CardType: CardType.Yellow }      => "Y",
        CardEvent { CardType: CardType.SecondYellow }=> "Y2",
        CardEvent { CardType: CardType.Red }         => "R",
        SubstitutionEvent                            => "SUB",
        TimingEvent                                  => "T",
        _                                            => "?"
    };

    // ── interaction handlers ───────────────────────────────────────────────

    private void HandleToggleEdit() => IsEditOpen = !IsEditOpen;

    private void HandleCancelEdit() => IsEditOpen = false;

    private async Task HandleQuickValidate() => await OnQuickValidate.InvokeAsync(Event.Id);

    private async Task HandleConfirmEdit(MatchEvent updated)
    {
        IsEditOpen = false;
        await OnConfirmEdit.InvokeAsync(updated);
    }

    // ── summary text composition ───────────────────────────────────────────

    private (string Main, string Sub) BuildSummary(MatchEvent @event) => @event switch
    {
        GoalEvent g          => BuildGoalSummary(g),
        CardEvent c          => BuildCardSummary(c),
        SubstitutionEvent s  => BuildSubSummary(s),
        TimingEvent t        => (t.Kind.DisplayLabel(), string.Empty),
        _                    => ("Unknown event", string.Empty)
    };

    private (string Main, string Sub) BuildGoalSummary(GoalEvent g)
    {
        var teamName = TeamNameFor(g.ScoringTeam);
        var player   = g.Scorer.DisplayLabel;
        if (!g.IsOwnGoal)
        {
            return ($"{player} · {teamName}", "Goal");
        }

        var creditedTo = TeamNameFor(OppositeSide(g.ScoringTeam));
        return ($"{player} · {teamName} <span class=\"og-badge\">OG</span>",
                $"Own goal · credited to {creditedTo}");
    }

    private (string Main, string Sub) BuildCardSummary(CardEvent c)
    {
        var teamName = TeamNameFor(c.Team);
        var typeLabel = c.CardType switch
        {
            CardType.Yellow       => "Yellow card",
            CardType.SecondYellow => "2nd Yellow → Red",
            CardType.Red          => "Red card",
            _                     => "Card"
        };
        var offenceTail = c.Offence is null ? string.Empty : $" · {c.Offence.Code}";
        return ($"{c.Player.DisplayLabel} · {teamName}",
                $"{typeLabel} · {c.Charge.Code}{offenceTail}");
    }

    private (string Main, string Sub) BuildSubSummary(SubstitutionEvent s) =>
        ($"↓ {s.PlayerOff.DisplayLabel} → ↑ {s.PlayerOn.DisplayLabel}",
         $"Substitution · {TeamNameFor(s.Team)}");

    private string TeamNameFor(TeamKey team) => team == TeamKey.Home ? HomeTeam.ShortName : AwayTeam.ShortName;

    private static TeamKey OppositeSide(TeamKey team) =>
        team == TeamKey.Home ? TeamKey.Away : TeamKey.Home;
}
