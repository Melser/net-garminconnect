namespace GarminConnect.Auth.OAuth;

/// <summary>
/// Interface for persisting and retrieving OAuth tokens.
/// </summary>
public interface IOAuthTokenStore
{
    /// <summary>
    /// Loads tokens from storage.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The stored tokens, or null if no tokens are stored.</returns>
    Task<GarminConnectTokens?> LoadAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves tokens to storage.
    /// </summary>
    /// <param name="tokens">The tokens to save.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SaveAsync(GarminConnectTokens tokens, CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears stored tokens.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task ClearAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if tokens exist in storage.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if tokens exist, false otherwise.</returns>
    Task<bool> ExistsAsync(CancellationToken cancellationToken = default);
}
