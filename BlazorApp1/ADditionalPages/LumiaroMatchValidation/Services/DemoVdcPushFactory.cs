using Lumiaro.MatchValidation.Components;
using Lumiaro.MatchValidation.Models;

namespace Lumiaro.MatchValidation.Services;

/// <summary>
/// Produces sample <see cref="MatchEvent"/> instances for the development
/// toolbar. Each call rotates through a fixed pool so repeated clicks produce
/// varied events. In production this whole service can be removed along
/// with the VdcDemoToolbar.
/// </summary>
public interface IDemoVdcPushFactory
{
    MatchEvent? NextSample(VdcDemoToolbar.VdcSampleKind kind, ValidationSession session);
}

public sealed class DemoVdcPushFactory : IDemoVdcPushFactory
{
    private readonly IChargeCodeProvider _charges;
    private readonly Dictionary<VdcDemoToolbar.VdcSampleKind, int> _cursors = new();

    public DemoVdcPushFactory(IChargeCodeProvider charges)
    {
        _charges = charges;
    }

    public MatchEvent? NextSample(VdcDemoToolbar.VdcSampleKind kind, ValidationSession session) => kind switch
    {
        VdcDemoToolbar.VdcSampleKind.Goal         => BuildGoal(session),
        VdcDemoToolbar.VdcSampleKind.YellowCard   => BuildYellow(session),
        VdcDemoToolbar.VdcSampleKind.RedCard      => BuildRed(session),
        VdcDemoToolbar.VdcSampleKind.Substitution => BuildSub(session),
        VdcDemoToolbar.VdcSampleKind.Timing       => BuildTiming(session),
        _                                         => null
    };

    private MatchEvent BuildGoal(ValidationSession session)
    {
        var samples = new[]
        {
            (TeamKey.Home, PlayerByNumber(session.HomeSquad, 10), 55),
            (TeamKey.Away, PlayerByNumber(session.AwaySquad, 10), 93)
        };
        var (team, scorer, minute) = Rotate(VdcDemoToolbar.VdcSampleKind.Goal, samples);
        return NewGoal(team, scorer, minute, isOwnGoal: false);
    }

    private MatchEvent BuildYellow(ValidationSession session)
    {
        var yellowCharges = _charges.GetChargesFor(CardType.Yellow);
        var c2 = yellowCharges.First(c => c.Code == "C2");
        var c1 = yellowCharges.First(c => c.Code == "C1");
        var ftOffence = c1.Offences.First(o => o.Code == "FT");

        var samples = new[]
        {
            (TeamKey.Home, PlayerByNumber(session.HomeSquad, 6), 25, CardType.Yellow, c2, (Offence?)null),
            (TeamKey.Away, PlayerByNumber(session.AwaySquad, 8), 60, CardType.Yellow, c1, (Offence?)ftOffence)
        };
        var sample = Rotate(VdcDemoToolbar.VdcSampleKind.YellowCard, samples);
        return NewCard(sample.Item1, sample.Item2, sample.Item3, sample.Item4, sample.Item5, sample.Item6);
    }

    private MatchEvent BuildRed(ValidationSession session)
    {
        var s1 = _charges.GetChargesFor(CardType.Red).First(c => c.Code == "S1");
        var samples = new[] { (TeamKey.Home, PlayerByNumber(session.HomeSquad, 27), 73, s1) };
        var (team, player, minute, charge) = Rotate(VdcDemoToolbar.VdcSampleKind.RedCard, samples);
        return NewCard(team, player, minute, CardType.Red, charge, null);
    }

    private MatchEvent BuildSub(ValidationSession session)
    {
        var samples = new[]
        {
            (TeamKey.Home, PlayerByNumber(session.HomeSquad, 7),  PlayerByNumber(session.HomeSquad, 10), 62),
            (TeamKey.Away, PlayerByNumber(session.AwaySquad, 15), PlayerByNumber(session.AwaySquad, 6),  67),
            (TeamKey.Home, PlayerByNumber(session.HomeSquad, 6),  PlayerByNumber(session.HomeSquad, 8),  81),
            (TeamKey.Away, PlayerByNumber(session.AwaySquad, 22), PlayerByNumber(session.AwaySquad, 2),  84)
        };
        var (team, off, on, minute) = Rotate(VdcDemoToolbar.VdcSampleKind.Substitution, samples);
        return NewSub(team, off, on, minute);
    }

    private MatchEvent BuildTiming(ValidationSession _)
    {
        var samples = new[]
        {
            (TimingEventKind.EndFirstHalf,    new TimeOnly(21, 48, 0),  48),
            (TimingEventKind.StartSecondHalf, new TimeOnly(22,  3, 24), 46),
            (TimingEventKind.EndSecondHalf,   new TimeOnly(22, 53, 24), 95)
        };
        var (kind, time, minute) = Rotate(VdcDemoToolbar.VdcSampleKind.Timing, samples);
        return new TimingEvent(
            Id: Guid.NewGuid(),
            Minute: minute,
            Source: EventSource.VdcPush,
            ReceivedAt: DateTimeOffset.UtcNow,
            SourceNote: $"VDC input — {time:HH:mm:ss}",
            Kind: kind,
            RealTime: time);
    }

    // ── event builders ─────────────────────────────────────────────────────

    private static GoalEvent NewGoal(TeamKey team, Player scorer, int minute, bool isOwnGoal) =>
        new(
            Id: Guid.NewGuid(),
            Minute: minute,
            Source: EventSource.VdcPush,
            ReceivedAt: DateTimeOffset.UtcNow,
            SourceNote: $"VDC input — {DateTime.Now:HH:mm:ss}",
            ScoringTeam: team,
            Scorer: scorer,
            IsOwnGoal: isOwnGoal);

    private static CardEvent NewCard(TeamKey team, Player player, int minute, CardType type, ChargeCode charge, Offence? offence) =>
        new(
            Id: Guid.NewGuid(),
            Minute: minute,
            Source: EventSource.VdcPush,
            ReceivedAt: DateTimeOffset.UtcNow,
            SourceNote: $"VDC input — {DateTime.Now:HH:mm:ss}",
            Team: team,
            Player: player,
            CardType: type,
            Charge: charge,
            Offence: offence);

    private static SubstitutionEvent NewSub(TeamKey team, Player off, Player on, int minute) =>
        new(
            Id: Guid.NewGuid(),
            Minute: minute,
            Source: EventSource.VdcPush,
            ReceivedAt: DateTimeOffset.UtcNow,
            SourceNote: $"VDC input — {DateTime.Now:HH:mm:ss}",
            Team: team,
            PlayerOff: off,
            PlayerOn: on);

    // ── helpers ────────────────────────────────────────────────────────────

    private static Player PlayerByNumber(Squad squad, int shirtNumber) =>
        squad.StartingXi.Concat(squad.Substitutes).First(p => p.ShirtNumber == shirtNumber);

    private T Rotate<T>(VdcDemoToolbar.VdcSampleKind kind, IReadOnlyList<T> pool)
    {
        if (!_cursors.TryGetValue(kind, out var cursor)) cursor = 0;
        var picked = pool[cursor % pool.Count];
        _cursors[kind] = cursor + 1;
        return picked;
    }
}
