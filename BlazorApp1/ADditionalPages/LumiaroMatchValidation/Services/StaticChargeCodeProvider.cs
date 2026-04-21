using System.Collections.Immutable;
using Lumiaro.MatchValidation.Models;

namespace Lumiaro.MatchValidation.Services;

/// <summary>
/// Hardcoded UEFA disciplinary code table lifted from the HTML prototype.
/// The yellow (C1) and red (S2) codes carry sub-offence pools; the others
/// are single-select. Replace with a reference-data-backed implementation
/// when the shared UEFA code registry is available.
/// </summary>
public sealed class StaticChargeCodeProvider : IChargeCodeProvider
{
    private readonly ImmutableArray<ChargeCode> _yellow;
    private readonly ImmutableArray<ChargeCode> _red;

    public StaticChargeCodeProvider()
    {
        _yellow = BuildYellowCodes();
        _red    = BuildRedCodes();
    }

    public ImmutableArray<ChargeCode> GetChargesFor(CardType cardType) => cardType switch
    {
        CardType.Yellow or CardType.SecondYellow => _yellow,
        CardType.Red                             => _red,
        _                                        => ImmutableArray<ChargeCode>.Empty
    };

    public ChargeCode? FindCharge(CardType cardType, string code) =>
        GetChargesFor(cardType).FirstOrDefault(c => c.Code == code);

    private static ImmutableArray<ChargeCode> BuildYellowCodes() => ImmutableArray.Create(
        new ChargeCode("C1", "C1 — Unsporting Behaviour", RequiresOffence: true, BuildC1Offences()),
        Simple("C2", "C2 — Dissent"),
        Simple("C3", "C3 — Persistently infringing the Laws of the Game"),
        Simple("C4", "C4 — Delays the restart of play"),
        Simple("C5", "C5 — Fails to respect the required distance at a restart"),
        Simple("C6", "C6 — Enters or re-enters the field of play without the referee's permission"),
        Simple("C7", "C7 — Deliberately leaves the field of play without the referee's permission"));

    private static ImmutableArray<ChargeCode> BuildRedCodes() => ImmutableArray.Create(
        Simple("S1", "S1 — Serious Foul Play"),
        new ChargeCode("S2", "S2 — Violent Conduct", RequiresOffence: true, BuildS2Offences()),
        Simple("S3", "S3 — Spitting"),
        Simple("S4", "S4 — DOGSO (Handball)"),
        Simple("S5", "S5 — DOGSO"),
        Simple("S6", "S6 — Use of offensive, insulting or abusive language"));

    private static ChargeCode Simple(string code, string label) =>
        new(code, label, RequiresOffence: false, ImmutableArray<Offence>.Empty);

    private static ImmutableArray<Offence> BuildC1Offences() => ImmutableArray.Create(
        new Offence("AA", "AA — Adopting an aggressive attitude"),
        new Offence("DI", "DI — Simulation"),
        new Offence("DP", "DP — Dangerous Play"),
        new Offence("FT", "FT — Foul Tackle"),
        new Offence("GC", "GC — Goal Celebration"),
        new Offence("HB", "HB — Handball"),
        new Offence("RP", "RP — Reckless Play"),
        new Offence("SP", "SP — Pushing or Pulling an opponent"),
        new Offence("TR", "TR — Tripping"),
        new Offence("UB", "UB — Unspecified Behaviour"));

    private static ImmutableArray<Offence> BuildS2Offences() => ImmutableArray.Create(
        new Offence("H2H", "Head to Head"),
        new Offence("ELB", "Elbowing"),
        new Offence("KCK", "Kicking"),
        new Offence("STP", "Stamping"),
        new Offence("STR", "Striking"),
        new Offence("BIT", "Biting"),
        new Offence("OTH", "Other Unspecified Behaviour"));
}
