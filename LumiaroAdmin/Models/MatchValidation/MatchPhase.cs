namespace Lumiaro.MatchValidation.Models;

/// <summary>
/// Wall-clock anchors for the four phase boundaries of a match. Any of these
/// may be null while the match is in progress. Once both ends of a segment
/// are recorded, the derived added-time or half-time duration is computable.
/// </summary>
public record MatchPhase(
    TimeOnly? StartFirstHalf  = null,
    TimeOnly? EndFirstHalf    = null,
    TimeOnly? StartSecondHalf = null,
    TimeOnly? EndSecondHalf   = null)
{
    public bool HasAnyMarker =>
        StartFirstHalf  is not null ||
        EndFirstHalf    is not null ||
        StartSecondHalf is not null ||
        EndSecondHalf   is not null;

    public MatchPhase With(TimingEventKind kind, TimeOnly time) => kind switch
    {
        TimingEventKind.StartFirstHalf  => this with { StartFirstHalf  = time },
        TimingEventKind.EndFirstHalf    => this with { EndFirstHalf    = time },
        TimingEventKind.StartSecondHalf => this with { StartSecondHalf = time },
        TimingEventKind.EndSecondHalf   => this with { EndSecondHalf   = time },
        _                               => this
    };
}
