using System.Collections.Immutable;
using Lumiaro.MatchValidation.Models;

namespace Lumiaro.MatchValidation.Services;

/// <summary>
/// Resolves the pool of disciplinary charge codes. Split by card type because
/// yellow and red have disjoint code families (C-codes vs S-codes). Behind
/// this interface the production implementation would call a reference-data
/// service so UEFA can amend the code list without a redeploy.
/// </summary>
public interface IChargeCodeProvider
{
    ImmutableArray<ChargeCode> GetChargesFor(CardType cardType);

    ChargeCode? FindCharge(CardType cardType, string code);
}
