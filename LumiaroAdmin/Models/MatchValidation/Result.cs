namespace Lumiaro.MatchValidation.Models;

/// <summary>
/// Minimal discriminated union for operations that may fail with a reason
/// rather than an exception. Matches the DLR house style used in Sefe and
/// FIFA Edge so the page orchestrator can branch cleanly on validation
/// outcomes without try/catch around flow control.
/// </summary>
public abstract record Result<T>
{
    public sealed record Ok(T Value) : Result<T>;

    public sealed record Err(string Reason, IReadOnlyList<string>? FieldErrors = null) : Result<T>;

    public bool IsOk => this is Ok;

    public T? ValueOrDefault => this is Ok ok ? ok.Value : default;
}

public static class Result
{
    public static Result<T>.Ok Ok<T>(T value) => new(value);
    public static Result<T>.Err Err<T>(string reason, IReadOnlyList<string>? fieldErrors = null)
        => new(reason, fieldErrors);
}
