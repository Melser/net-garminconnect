using GarminConnect.Auth.OAuth;

namespace GarminConnect.Auth;

/// <summary>
/// Interface for Garmin Connect authentication.
/// </summary>
public interface IGarminAuthenticator
{
    /// <summary>
    /// Gets whether the client is currently authenticated.
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Gets whether MFA is required to complete authentication.
    /// </summary>
    bool RequiresMfa { get; }

    /// <summary>
    /// Gets the current access token, or null if not authenticated.
    /// </summary>
    string? AccessToken { get; }

    /// <summary>
    /// Gets the current tokens, or null if not authenticated.
    /// </summary>
    GarminConnectTokens? Tokens { get; }

    /// <summary>
    /// Authenticates with Garmin Connect using email and password.
    /// </summary>
    /// <param name="email">Garmin account email.</param>
    /// <param name="password">Garmin account password.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Authentication result.</returns>
    Task<AuthResult> LoginAsync(string email, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// Completes MFA authentication with the provided code.
    /// </summary>
    /// <param name="mfaCode">The MFA code from authenticator app.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Authentication result.</returns>
    Task<AuthResult> CompleteMfaAsync(string mfaCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resumes authentication using the provided client state and MFA code.
    /// </summary>
    /// <param name="clientState">Client state from previous login attempt.</param>
    /// <param name="mfaCode">The MFA code from authenticator app.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Authentication result.</returns>
    Task<AuthResult> ResumeMfaAsync(string clientState, string mfaCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Attempts to restore session from stored tokens.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if session was restored successfully.</returns>
    Task<bool> ResumeSessionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Refreshes the OAuth2 access token.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if token was refreshed successfully.</returns>
    Task<bool> RefreshTokenAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs out and clears stored tokens.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task LogoutAsync(CancellationToken cancellationToken = default);
}
