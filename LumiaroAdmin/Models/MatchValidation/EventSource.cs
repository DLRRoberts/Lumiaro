namespace Lumiaro.MatchValidation.Models;

/// <summary>
/// Where an event came from. VDC pushes arrive from the Video Data Collection
/// feed (in the demo this is the toolbar; in production this will be a
/// Submaster SignalR event). Manual entry comes from the referee adding an
/// event that VDC missed. Correction covers post-validation amendments.
/// </summary>
public enum EventSource
{
    VdcPush,
    ManualEntry,
    Correction
}

/// <summary>
/// Exhaustive set of match-phase timing markers the tool handles.
/// These drive the minute-display logic (e.g. "End First Half" may render
/// as "45+2'" once both the start and end of the first half are recorded).
/// </summary>
public enum TimingEventKind
{
    StartFirstHalf,
    EndFirstHalf,
    StartSecondHalf,
    EndSecondHalf
}

public static class TimingEventKindExtensions
{
    public static string DisplayLabel(this TimingEventKind kind) => kind switch
    {
        TimingEventKind.StartFirstHalf  => "Start First Half",
        TimingEventKind.EndFirstHalf    => "End First Half",
        TimingEventKind.StartSecondHalf => "Start Second Half",
        TimingEventKind.EndSecondHalf   => "End Second Half",
        _                               => "Unknown"
    };
}
