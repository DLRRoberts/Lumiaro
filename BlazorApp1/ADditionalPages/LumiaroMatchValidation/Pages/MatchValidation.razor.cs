using Lumiaro.MatchValidation.Components;
using Lumiaro.MatchValidation.Models;
using Microsoft.AspNetCore.Components;

namespace Lumiaro.MatchValidation.Pages;

public partial class MatchValidation : ComponentBase, IDisposable
{
    [Parameter] public Guid MatchId { get; set; }

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

        var info = new MatchInfo(
            CompetitionLabel: "UCL QF 2nd Leg",
            VenueName:        "Fußball Arena München",
            KickOff:          DateTimeOffset.Now);

        await Session.InitializeAsync(MatchId, info);
        SeedPreloadedEvents();
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
