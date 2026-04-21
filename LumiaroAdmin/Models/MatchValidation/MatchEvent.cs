namespace Lumiaro.MatchValidation.Models;

/// <summary>
/// Base record for all events flowing through the validation pipeline.
/// Concrete subtypes are:
/// <list type="bullet">
///   <item><see cref="GoalEvent"/> — goal or own goal</item>
///   <item><see cref="CardEvent"/> — yellow, second yellow, or red</item>
///   <item><see cref="SubstitutionEvent"/></item>
///   <item><see cref="TimingEvent"/></item>
/// </list>
/// An event moves through three states: received (pending), then validated
/// (after referee confirmation), then submitted (part of the final report).
/// </summary>
public abstract record MatchEvent(
    Guid Id,
    int Minute,
    EventSource Source,
    DateTimeOffset ReceivedAt,
    string? SourceNote);

/// <summary>
/// A goal. If <see cref="IsOwnGoal"/> is true, <see cref="ScoringTeam"/>
/// remains the scorer's team — the scoreboard credits the opposing team
/// but the factual record of who put the ball in the net doesn't change.
/// </summary>
public sealed record GoalEvent(
    Guid Id,
    int Minute,
    EventSource Source,
    DateTimeOffset ReceivedAt,
    string? SourceNote,
    TeamKey ScoringTeam,
    Player Scorer,
    bool IsOwnGoal) : MatchEvent(Id, Minute, Source, ReceivedAt, SourceNote);

/// <summary>
/// A disciplinary card. <see cref="Offence"/> is only populated when
/// <see cref="Charge"/> requires a sub-classification.
/// </summary>
public sealed record CardEvent(
    Guid Id,
    int Minute,
    EventSource Source,
    DateTimeOffset ReceivedAt,
    string? SourceNote,
    TeamKey Team,
    Player Player,
    CardType CardType,
    ChargeCode Charge,
    Offence? Offence) : MatchEvent(Id, Minute, Source, ReceivedAt, SourceNote);

/// <summary>
/// A substitution. Changes the roster state: <see cref="PlayerOff"/> moves
/// from onPitch to subbedOff; <see cref="PlayerOn"/> moves from available to onPitch.
/// </summary>
public sealed record SubstitutionEvent(
    Guid Id,
    int Minute,
    EventSource Source,
    DateTimeOffset ReceivedAt,
    string? SourceNote,
    TeamKey Team,
    Player PlayerOff,
    Player PlayerOn) : MatchEvent(Id, Minute, Source, ReceivedAt, SourceNote);

/// <summary>
/// A match-phase marker. <see cref="RealTime"/> is the wall-clock time at
/// which the kick or the whistle happened; the tool uses it to compute
/// added time and half-time duration.
/// </summary>
public sealed record TimingEvent(
    Guid Id,
    int Minute,
    EventSource Source,
    DateTimeOffset ReceivedAt,
    string? SourceNote,
    TimingEventKind Kind,
    TimeOnly RealTime) : MatchEvent(Id, Minute, Source, ReceivedAt, SourceNote);
