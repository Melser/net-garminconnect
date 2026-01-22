using GarminConnect.Auth.Mfa;
using GarminConnect.Auth.OAuth;
using GarminConnect.Exceptions;
using Microsoft.Extensions.Logging;

namespace GarminConnect.Auth;

/// <summary>
/// Handles authentication with Garmin Connect.
/// </summary>
public sealed class GarminAuthenticator : IGarminAuthenticator, IDisposable
{
    private readonly IOAuthTokenStore? _tokenStore;
    private readonly IMfaHandler? _mfaHandler;
    private readonly ILogger<GarminAuthenticator>? _logger;
    private readonly GarminSsoClient _ssoClient;

    private GarminConnectTokens? _tokens;
    private string? _pendingMfaState;

    /// <summary>
    /// Creates a new GarminAuthenticator.
    /// </summary>
    /// <param name="tokenStore">Optional token store for persisting tokens.</param>
    /// <param name="mfaHandler">Optional MFA handler for two-factor authentication.</param>
    /// <param name="logger">Optional logger.</param>
    public GarminAuthenticator(
        IOAuthTokenStore? tokenStore = null,
        IMfaHandler? mfaHandler = null,
        ILogger<GarminAuthenticator>? logger = null)
    {
        _tokenStore = tokenStore;
        _mfaHandler = mfaHandler;
        _logger = logger;
        _ssoClient = new GarminSsoClient(logger);
    }

    /// <inheritdoc />
    public bool IsAuthenticated => _tokens?.IsValid == true;

    /// <inheritdoc />
    public bool RequiresMfa => _pendingMfaState is not null;

    /// <inheritdoc />
    public string? AccessToken => _tokens?.AccessToken;

    /// <inheritdoc />
    public GarminConnectTokens? Tokens => _tokens;

    /// <inheritdoc />
    public async Task<AuthResult> LoginAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        _logger?.LogInformation("Attempting login for {Email}", email);

        try
        {
            var tokens = await _ssoClient.LoginAsync(email, password, cancellationToken).ConfigureAwait(false);

            if (tokens is null)
            {
                // MFA required
                _pendingMfaState = _ssoClient.MfaClientState;

                if (_mfaHandler is not null)
                {
                    // Automatically handle MFA if handler is provided
                    _logger?.LogInformation("MFA required, prompting for code");

                    var mfaCode = await _mfaHandler.GetMfaCodeAsync(cancellationToken).ConfigureAwait(false);

                    if (string.IsNullOrWhiteSpace(mfaCode))
                    {
                        return AuthResult.Failed("MFA code not provided");
                    }

                    return await CompleteMfaAsync(mfaCode, cancellationToken).ConfigureAwait(false);
                }

                return AuthResult.MfaRequired(_pendingMfaState ?? "unknown");
            }

            await SetTokensAsync(tokens, cancellationToken).ConfigureAwait(false);

            _logger?.LogInformation("Login successful");
            return AuthResult.Succeeded();
        }
        catch (GarminConnectAuthenticationException ex)
        {
            _logger?.LogWarning(ex, "Login failed: {Message}", ex.Message);
            return AuthResult.Failed(ex.Message);
        }
        catch (GarminConnectTooManyRequestsException ex)
        {
            _logger?.LogWarning(ex, "Rate limited: {Message}", ex.Message);
            return AuthResult.Failed(ex.Message);
        }
    }

    /// <inheritdoc />
    public async Task<AuthResult> CompleteMfaAsync(
        string mfaCode,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(mfaCode);

        if (_pendingMfaState is null)
        {
            return AuthResult.Failed("No pending MFA authentication. Call LoginAsync first.");
        }

        return await ResumeMfaAsync(_pendingMfaState, mfaCode, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<AuthResult> ResumeMfaAsync(
        string clientState,
        string mfaCode,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(clientState);
        ArgumentException.ThrowIfNullOrWhiteSpace(mfaCode);

        _logger?.LogInformation("Completing MFA authentication");

        try
        {
            var tokens = await _ssoClient.CompleteMfaAsync(mfaCode, clientState, cancellationToken).ConfigureAwait(false);

            await SetTokensAsync(tokens, cancellationToken).ConfigureAwait(false);
            _pendingMfaState = null;

            _logger?.LogInformation("MFA authentication successful");
            return AuthResult.Succeeded();
        }
        catch (GarminConnectAuthenticationException ex)
        {
            _logger?.LogWarning(ex, "MFA failed: {Message}", ex.Message);
            return AuthResult.Failed(ex.Message);
        }
        catch (GarminConnectTooManyRequestsException ex)
        {
            _logger?.LogWarning(ex, "Rate limited: {Message}", ex.Message);
            return AuthResult.Failed(ex.Message);
        }
    }

    /// <inheritdoc />
    public async Task<bool> ResumeSessionAsync(CancellationToken cancellationToken = default)
    {
        if (_tokenStore is null)
        {
            _logger?.LogDebug("No token store configured, cannot resume session");
            return false;
        }

        try
        {
            var tokens = await _tokenStore.LoadAsync(cancellationToken).ConfigureAwait(false);

            if (tokens is null)
            {
                _logger?.LogDebug("No stored tokens found");
                return false;
            }

            if (!tokens.IsValid)
            {
                if (tokens.CanRefresh)
                {
                    _logger?.LogInformation("Stored tokens expired, attempting refresh");
                    _tokens = tokens;
                    return await RefreshTokenAsync(cancellationToken).ConfigureAwait(false);
                }

                _logger?.LogDebug("Stored tokens are invalid and cannot be refreshed");
                return false;
            }

            _tokens = tokens;
            _logger?.LogInformation("Session restored from stored tokens");
            return true;
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "Failed to resume session from stored tokens");
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<bool> RefreshTokenAsync(CancellationToken cancellationToken = default)
    {
        if (_tokens?.OAuth1 is null)
        {
            _logger?.LogWarning("Cannot refresh token: OAuth1 token not available");
            return false;
        }

        try
        {
            _logger?.LogInformation("Refreshing OAuth2 token");

            var newOAuth2 = await _ssoClient.RefreshOAuth2TokenAsync(_tokens.OAuth1, cancellationToken).ConfigureAwait(false);

            var newTokens = _tokens with
            {
                OAuth2 = newOAuth2,
                UpdatedAt = DateTimeOffset.UtcNow
            };

            await SetTokensAsync(newTokens, cancellationToken).ConfigureAwait(false);

            _logger?.LogInformation("Token refresh successful");
            return true;
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "Failed to refresh token");
            return false;
        }
    }

    /// <inheritdoc />
    public async Task LogoutAsync(CancellationToken cancellationToken = default)
    {
        _logger?.LogInformation("Logging out");

        _tokens = null;
        _pendingMfaState = null;

        if (_tokenStore is not null)
        {
            await _tokenStore.ClearAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    private async Task SetTokensAsync(GarminConnectTokens tokens, CancellationToken cancellationToken)
    {
        _tokens = tokens;

        if (_tokenStore is not null)
        {
            await _tokenStore.SaveAsync(tokens, cancellationToken).ConfigureAwait(false);
            _logger?.LogDebug("Tokens saved to store");
        }
    }

    public void Dispose()
    {
        _ssoClient.Dispose();
    }
}
