using Lumiaro.MatchValidation.Models;

namespace Lumiaro.MatchValidation.Services;

/// <summary>
/// Resolves the two squads participating in a match. The static implementation
/// is used for the prototype; the production implementation will hit the
/// Lumiaro squad API via <c>IHttpClientFactory</c>.
/// </summary>
public interface ISquadProvider
{
    Task<(Squad Home, Squad Away)> GetSquadsAsync(Guid matchId, CancellationToken ct = default);
}
