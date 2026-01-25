using System.Text.Json;
using GarminConnect.Api;

namespace GarminConnect;

public sealed partial class GarminClient
{
    #region Badges

    /// <summary>
    /// Gets earned badges.
    /// </summary>
    public async Task<JsonElement> GetEarnedBadgesAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        EnsureAuthenticated();

        return await _apiClient.GetAsync<JsonElement>(Endpoints.EarnedBadges, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets available badges.
    /// </summary>
    public async Task<JsonElement> GetAvailableBadgesAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        EnsureAuthenticated();

        return await _apiClient.GetAsync<JsonElement>(Endpoints.AvailableBadges, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets in-progress badges.
    /// </summary>
    public async Task<JsonElement> GetInProgressBadgesAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        EnsureAuthenticated();

        return await _apiClient.GetAsync<JsonElement>(Endpoints.InProgressBadges, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets badge challenges with pagination.
    /// </summary>
    /// <param name="start">Starting offset.</param>
    /// <param name="limit">Maximum number of challenges to return.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<JsonElement> GetBadgeChallengesAsync(
        int start = 0,
        int limit = 100,
        CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        EnsureAuthenticated();

        ArgumentOutOfRangeException.ThrowIfNegative(start);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(limit);

        var url = $"{Endpoints.BadgeChallenges}?start={start}&limit={limit}";
        return await _apiClient.GetAsync<JsonElement>(url, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets available badge challenges with pagination.
    /// </summary>
    public async Task<JsonElement> GetAvailableBadgeChallengesAsync(
        int start = 0,
        int limit = 100,
        CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        EnsureAuthenticated();

        ArgumentOutOfRangeException.ThrowIfNegative(start);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(limit);

        var url = $"{Endpoints.AvailableBadgeChallenges}?start={start}&limit={limit}";
        return await _apiClient.GetAsync<JsonElement>(url, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets non-completed badge challenges with pagination.
    /// </summary>
    public async Task<JsonElement> GetNonCompletedBadgeChallengesAsync(
        int start = 0,
        int limit = 100,
        CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        EnsureAuthenticated();

        ArgumentOutOfRangeException.ThrowIfNegative(start);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(limit);

        var url = $"{Endpoints.NonCompletedBadgeChallenges}?start={start}&limit={limit}";
        return await _apiClient.GetAsync<JsonElement>(url, cancellationToken).ConfigureAwait(false);
    }

    #endregion
}
