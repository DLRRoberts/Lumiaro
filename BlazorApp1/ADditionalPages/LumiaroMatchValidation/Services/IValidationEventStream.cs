using Lumiaro.MatchValidation.Models;

namespace Lumiaro.MatchValidation.Services;

/// <summary>
/// Inbound stream of events arriving from the VDC feed. In production this
/// will be backed by a Submaster SignalR hub subscription; the demo toolbar
/// injects events via the same interface so the page doesn't care where the
/// events originate.
/// </summary>
public interface IValidationEventStream
{
    /// <summary>
    /// Raised each time a new candidate event arrives from the source.
    /// The handler is expected to feed it into the session service.
    /// </summary>
    event Func<MatchEvent, Task>? OnEventReceived;

    /// <summary>
    /// Starts listening. Safe to call multiple times — implementations should
    /// be idempotent.
    /// </summary>
    Task StartAsync(CancellationToken ct = default);

    Task StopAsync(CancellationToken ct = default);
}
