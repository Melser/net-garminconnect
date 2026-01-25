using GarminConnect.Api;
using GarminConnect.Auth;
using GarminConnect.Auth.Mfa;
using GarminConnect.Auth.OAuth;
using GarminConnect.Exceptions;
using Microsoft.Extensions.Logging;

namespace GarminConnect;

/// <summary>
/// Main client for interacting with Garmin Connect API.
/// </summary>
public sealed partial class GarminClient : IGarminClient
{
    private readonly IGarminApiClient _apiClient;
    private readonly IGarminAuthenticator _authenticator;
    private readonly ILogger<GarminClient>? _logger;
    private readonly bool _ownsApiClient;

    private bool _disposed;

    /// <summary>
    /// Creates a new GarminClient with default configuration.
    /// </summary>
    /// <param name="tokenStore">Optional token store for persisting authentication tokens.</param>
    /// <param name="mfaHandler">Optional MFA handler for two-factor authentication.</param>
    /// <param name="logger">Optional logger.</param>
    public GarminClient(
        IOAuthTokenStore? tokenStore = null,
        IMfaHandler? mfaHandler = null,
        ILogger<GarminClient>? logger = null)
    {
        _logger = logger;

        var httpClient = new HttpClient
        {
            BaseAddress = new Uri(Endpoints.BaseUrl)
        };

        _apiClient = new GarminApiClient(httpClient, ownsHttpClient: true);
        _authenticator = new GarminAuthenticator(tokenStore, mfaHandler);
        _ownsApiClient = true;
    }

    /// <summary>
    /// Creates a new GarminClient with custom API client and authenticator.
    /// </summary>
    /// <param name="apiClient">API client to use.</param>
    /// <param name="authenticator">Authenticator to use.</param>
    /// <param name="logger">Optional logger.</param>
    public GarminClient(
        IGarminApiClient apiClient,
        IGarminAuthenticator authenticator,
        ILogger<GarminClient>? logger = null)
    {
        _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        _authenticator = authenticator ?? throw new ArgumentNullException(nameof(authenticator));
        _logger = logger;
        _ownsApiClient = false;
    }

    #region Authentication

    /// <inheritdoc />
    public bool IsAuthenticated => _authenticator.IsAuthenticated;

    /// <inheritdoc />
    public string? AccessToken => _authenticator.AccessToken;

    /// <inheritdoc />
    public async Task<AuthResult> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        var result = await _authenticator.LoginAsync(email, password, cancellationToken).ConfigureAwait(false);

        if (result.Success)
        {
            _apiClient.SetAccessToken(_authenticator.AccessToken);
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<AuthResult> CompleteMfaAsync(string mfaCode, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        var result = await _authenticator.CompleteMfaAsync(mfaCode, cancellationToken).ConfigureAwait(false);

        if (result.Success)
        {
            _apiClient.SetAccessToken(_authenticator.AccessToken);
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<bool> ResumeSessionAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        var result = await _authenticator.ResumeSessionAsync(cancellationToken).ConfigureAwait(false);

        if (result)
        {
            _apiClient.SetAccessToken(_authenticator.AccessToken);
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<bool> RefreshTokenAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        var result = await _authenticator.RefreshTokenAsync(cancellationToken).ConfigureAwait(false);

        if (result)
        {
            _apiClient.SetAccessToken(_authenticator.AccessToken);
        }

        return result;
    }

    /// <inheritdoc />
    public async Task LogoutAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        await _authenticator.LogoutAsync(cancellationToken).ConfigureAwait(false);
        _apiClient.SetAccessToken(null);
        ClearUserCache();
    }

    #endregion

    #region Helpers

    /// <summary>
    /// Formats a date as YYYY-MM-DD for API requests.
    /// </summary>
    private static string FormatDate(DateOnly date) => date.ToString("yyyy-MM-dd");

    /// <summary>
    /// Ensures the client is authenticated before making API calls.
    /// </summary>
    private void EnsureAuthenticated()
    {
        if (!IsAuthenticated)
        {
            throw new GarminConnectAuthenticationException("Not authenticated. Call LoginAsync or ResumeSessionAsync first.");
        }
    }

    /// <summary>
    /// Throws if the client has been disposed.
    /// </summary>
    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }

    #endregion

    #region IDisposable

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed) return;

        if (_ownsApiClient && _apiClient is IDisposable disposable)
        {
            disposable.Dispose();
        }

        if (_authenticator is IDisposable authDisposable)
        {
            authDisposable.Dispose();
        }

        _disposed = true;
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;

        if (_ownsApiClient && _apiClient is IAsyncDisposable asyncDisposable)
        {
            await asyncDisposable.DisposeAsync().ConfigureAwait(false);
        }
        else if (_ownsApiClient && _apiClient is IDisposable disposable)
        {
            disposable.Dispose();
        }

        if (_authenticator is IDisposable authDisposable)
        {
            authDisposable.Dispose();
        }

        _disposed = true;
    }

    #endregion
}
