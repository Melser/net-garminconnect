namespace GarminConnect.Auth.Mfa;

/// <summary>
/// MFA handler that uses a callback function to obtain the MFA code.
/// Useful for integrating with UI frameworks or external services.
/// </summary>
public sealed class CallbackMfaHandler : IMfaHandler
{
    private readonly Func<CancellationToken, Task<string?>> _callback;

    /// <summary>
    /// Creates a new CallbackMfaHandler with an async callback.
    /// </summary>
    /// <param name="callback">Async function that returns the MFA code.</param>
    public CallbackMfaHandler(Func<CancellationToken, Task<string?>> callback)
    {
        _callback = callback ?? throw new ArgumentNullException(nameof(callback));
    }

    /// <summary>
    /// Creates a new CallbackMfaHandler with a sync callback.
    /// </summary>
    /// <param name="callback">Sync function that returns the MFA code.</param>
    public CallbackMfaHandler(Func<string?> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        _callback = _ => Task.FromResult(callback());
    }

    /// <inheritdoc />
    public Task<string?> GetMfaCodeAsync(CancellationToken cancellationToken = default)
    {
        return _callback(cancellationToken);
    }
}
