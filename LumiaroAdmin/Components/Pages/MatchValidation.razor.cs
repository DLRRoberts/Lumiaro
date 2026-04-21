using System.Collections.Immutable;
using Lumiaro.MatchValidation.Components;
using Lumiaro.MatchValidation.Models;
using LumiaroAdmin.Services;
using Microsoft.AspNetCore.Components;

namespace Lumiaro.MatchValidation.Pages;

public partial class MatchValidation : ComponentBase, IDisposable
{
    [Inject] private FixtureService FixtureService { get; set; } = default!;

    [Parameter] public int MatchId { get; set; }

    /// <summary>
    /// Logged-in referee label. In Lumiaro this will come from
    /// AuthenticationStateProvider; kept as a simple parameter for now.
    /// </summary>
    [Parameter] public string UserLabel { get; set; } = "R. Garcia · Match Referee";

    /// <summary>
    /// Hide the demo toolbar in production builds via configuration.
    /// </summary>
    [Parameter] public bool ShowDemoToolbar { get; set; } = true;

    private ValidationSession Current => Session.Current;

    /// <summary>
    /// Match-card timer. Picks the latest pending event's minute so the
    /// display tracks the leading edge of the feed (mirrors the JS
    /// updateMatchTimer() behaviour). Could equally be driven by a live
    /// clock once wired to the real SignalR feed.
    /// </summary>
    private string LiveMatchTimer
    {
        get
        {
            var latest = Current.Pending.LastOrDefault() ?? Current.Validated.LastOrDefault();
            if (latest is null) return "0'";
            return latest is TimingEvent t
                ? MinuteFormatter.FormatTimingEvent(t.Kind, Current.Phase)
                : MinuteFormatter.FormatRaw(latest.Minute, Current.Phase);
        }
    }

    protected override async Task OnInitializedAsync()
    {
        Session.SessionChanged += HandleSessionChanged;

        // Load the real fixture from the database
        var fixture = await FixtureService.GetFixtureByIdAsync(MatchId);

        MatchInfo info;
        Squad homeSquad;
        Squad awaySquad;

        if (fixture is not null)
        {
            info = new MatchInfo(
                CompetitionLabel: fixture.DivisionName,
                VenueName:        fixture.Venue ?? string.Empty,
                KickOff:          new DateTimeOffset(fixture.Date.Add(fixture.KickOff), TimeSpan.Zero));

            // Build squads with the real team names from the fixture.
            // Player rosters use the demo dataset until a squad API is available.
            homeSquad = BuildDemoSquad(TeamKey.Home, fixture.HomeTeam);
            awaySquad = BuildDemoSquad(TeamKey.Away, fixture.AwayTeam);
        }
        else
        {
            // Fallback when the fixture cannot be found
            info = new MatchInfo(
                CompetitionLabel: "Unknown Competition",
                VenueName:        string.Empty,
                KickOff:          DateTimeOffset.Now);
            homeSquad = BuildDemoSquad(TeamKey.Home, "Home Team");
            awaySquad = BuildDemoSquad(TeamKey.Away, "Away Team");
        }

        await Session.InitializeAsync(info, homeSquad, awaySquad);
        SeedPreloadedEvents();
    }

    /// <summary>
    /// Builds a minimal squad with the correct team name but a demo set of
    /// numbered players. Replace with a real squad-API call once available.
    /// </summary>
    private static Squad BuildDemoSquad(TeamKey side, string teamName)
    {
        var team = new Team(side, teamName);
        var xi = ImmutableArray.Create(
            new Player(1, "Player 1"),   new Player(2,  "Player 2"),
            new Player(3, "Player 3"),   new Player(4,  "Player 4"),
            new Player(5, "Player 5"),   new Player(6,  "Player 6"),
            new Player(7, "Player 7"),   new Player(8,  "Player 8"),
            new Player(9, "Player 9"),   new Player(10, "Player 10"),
            new Player(11, "Player 11"));
        var subs = ImmutableArray.Create(
            new Player(12, "Player 12"), new Player(13, "Player 13"),
            new Player(14, "Player 14"), new Player(15, "Player 15"),
            new Player(16, "Player 16"), new Player(17, "Player 17"),
            new Player(18, "Player 18"));
        return new Squad(team, xi, subs);
    }

    /// <summary>
    /// Push the two preloaded events the HTML prototype had (first-half
    /// kickoff + an early goal) so the page boots into a representative
    /// state. Remove once the real SignalR feed is wired up.
    /// </summary>
    private void SeedPreloadedEvents()
    {
        var kickoff = new TimingEvent(
            Id: Guid.NewGuid(),
            Minute: 0,
            Source: EventSource.VdcPush,
            ReceivedAt: DateTimeOffset.UtcNow,
            SourceNote: "VDC input — 21:00:00",
            Kind: TimingEventKind.StartFirstHalf,
            RealTime: new TimeOnly(21, 0, 0));

        var earlyGoal = DemoFactory.NextSample(
            VdcDemoToolbar.VdcSampleKind.Goal, Current);

        Session.PushPending(kickoff);
        if (earlyGoal is not null)
        {
            Session.PushPending(earlyGoal);
        }
    }

    // ── child event handlers ───────────────────────────────────────────────

    private void HandleQuickValidate(Guid eventId) => Session.QuickValidate(eventId);

    private void HandleConfirmEdit(MatchEvent edited) => Session.ConfirmEdited(edited);

    private void HandleDescriptionChanged((Guid IncidentId, string Description) payload) =>
        Session.UpdateIncidentDescription(payload.IncidentId, payload.Description);

    private void HandleDemoPush(VdcDemoToolbar.VdcSampleKind kind)
    {
        var sample = DemoFactory.NextSample(kind, Current);
        if (sample is not null) Session.PushPending(sample);
    }

    private Task HandleFinalize()
    {
        // TODO: POST the session to the Lumiaro reporting API.
        // For now just no-op — the button is only enabled when all gates pass.
        return Task.CompletedTask;
    }

    // ── re-render plumbing ─────────────────────────────────────────────────

    private void HandleSessionChanged()
    {
        InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        Session.SessionChanged -= HandleSessionChanged;
    }
}
