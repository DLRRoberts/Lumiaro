using System.Collections.Immutable;

namespace Lumiaro.MatchValidation.Models;

/// <summary>
/// A sub-classification of a charge code — e.g. under C1 (Unsporting Behaviour)
/// you pick "DI — Simulation" or "FT — Foul Tackle".
/// </summary>
public record Offence(string Code, string Label);

/// <summary>
/// A disciplinary charge code. Yellow charges are coded C1–C7, red are S1–S6.
/// A charge may or may not require a further offence selection
/// (<see cref="RequiresOffence"/>). When it does, <see cref="Offences"/> is the
/// pool to choose from.
/// </summary>
public record ChargeCode(string Code, string Label, bool RequiresOffence, ImmutableArray<Offence> Offences)
{
    /// <summary>
    /// The label with the code prefix stripped — e.g. for
    /// "C1 — Unsporting Behaviour" this returns "Unsporting Behaviour".
    /// Used in the charge summary strip.
    /// </summary>
    public string Description
    {
        get
        {
            var parts = Label.Split(" — ", 2);
            return parts.Length == 2 ? parts[1] : Label;
        }
    }
}
