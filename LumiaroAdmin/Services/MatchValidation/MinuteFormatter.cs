using Lumiaro.MatchValidation.Models;

namespace Lumiaro.MatchValidation.Services;

/// <summary>
/// Converts a raw numeric minute into its display form, applying phase-aware
/// added-time conventions (e.g. a 48th-minute event during the first half
/// renders as "45+3'" once the first-half end whistle is in; an otherwise
/// identical minute after half-time kicks off renders as "48'").
/// </summary>
public interface IMinuteFormatter
{
    string FormatRaw(int minute, MatchPhase phase);

    string FormatTimingEvent(TimingEventKind kind, MatchPhase phase);
}

public sealed class MinuteFormatter : IMinuteFormatter
{
    public string FormatRaw(int minute, MatchPhase phase)
    {
        // Before the first-half whistle, anything past 45 is added time on 45.
        // After it, anything past 90 is added time on 90. This mirrors the JS
        // getDisplayMinute() branch that keyed on MATCH_PHASE.endFirstHalf.
        if (phase.EndFirstHalf is null)
        {
            return minute <= 45 ? $"{minute}'" : $"45+{minute - 45}'";
        }

        return minute <= 90 ? $"{minute}'" : $"90+{minute - 90}'";
    }

    public string FormatTimingEvent(TimingEventKind kind, MatchPhase phase) => kind switch
    {
        TimingEventKind.StartFirstHalf  => "0'",
        TimingEventKind.StartSecondHalf => "45'",
        TimingEventKind.EndFirstHalf    => FormatFirstHalfEnd(phase),
        TimingEventKind.EndSecondHalf   => FormatSecondHalfEnd(phase),
        _                               => "—"
    };

    private static string FormatFirstHalfEnd(MatchPhase phase) =>
        ComputeAddedTimeMinutes(phase.StartFirstHalf, phase.EndFirstHalf) is { } added && added > 0
            ? $"45+{added}'"
            : "45'";

    private static string FormatSecondHalfEnd(MatchPhase phase) =>
        ComputeAddedTimeMinutes(phase.StartSecondHalf, phase.EndSecondHalf) is { } added && added > 0
            ? $"90+{added}'"
            : "90'";

    private static int? ComputeAddedTimeMinutes(TimeOnly? start, TimeOnly? end)
    {
        if (start is null || end is null)
        {
            return null;
        }

        var elapsed = end.Value.ToTimeSpan() - start.Value.ToTimeSpan();
        var addedSeconds = elapsed.TotalSeconds - 45 * 60;
        return (int)Math.Ceiling(addedSeconds / 60.0);
    }
}
