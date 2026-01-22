using GarminConnect.Api;
using GarminConnect.Models;

namespace GarminConnect;

public sealed partial class GarminClient
{
    private UserSettings? _cachedUserSettings;
    private string? _cachedDisplayName;
    private string? _cachedFullName;

    /// <inheritdoc />
    public async Task<string?> GetDisplayNameAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        EnsureAuthenticated();

        if (_cachedDisplayName is not null)
        {
            return _cachedDisplayName;
        }

        var settings = await GetUserSettingsAsync(cancellationToken).ConfigureAwait(false);
        _cachedDisplayName = settings?.DisplayName;

        return _cachedDisplayName;
    }

    /// <inheritdoc />
    public async Task<string?> GetFullNameAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        EnsureAuthenticated();

        if (_cachedFullName is not null)
        {
            return _cachedFullName;
        }

        var profile = await GetUserProfileAsync(cancellationToken).ConfigureAwait(false);

        if (profile?.FirstName is not null && profile.LastName is not null)
        {
            _cachedFullName = $"{profile.FirstName} {profile.LastName}";
        }
        else
        {
            _cachedFullName = profile?.DisplayName;
        }

        return _cachedFullName;
    }

    /// <inheritdoc />
    public async Task<string?> GetUnitSystemAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        EnsureAuthenticated();

        var settings = await GetUserSettingsAsync(cancellationToken).ConfigureAwait(false);
        return settings?.MeasurementSystem;
    }

    /// <inheritdoc />
    public async Task<UserProfile?> GetUserProfileAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        EnsureAuthenticated();

        return await _apiClient.GetAsync<UserProfile>(Endpoints.UserProfile, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<UserSettings?> GetUserSettingsAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        EnsureAuthenticated();

        if (_cachedUserSettings is not null)
        {
            return _cachedUserSettings;
        }

        _cachedUserSettings = await _apiClient.GetAsync<UserSettings>(Endpoints.UserSettings, cancellationToken).ConfigureAwait(false);

        return _cachedUserSettings;
    }
}
