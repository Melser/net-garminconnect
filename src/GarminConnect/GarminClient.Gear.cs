using System.Text.Json;
using GarminConnect.Api;

namespace GarminConnect;

public sealed partial class GarminClient
{
    private const int DefaultGearActivitiesLimit = 1000;

    #region Gear

    /// <summary>
    /// Gets all gear for a user.
    /// </summary>
    /// <param name="userProfileNumber">User profile number.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<JsonElement> GetGearAsync(string userProfileNumber, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        EnsureAuthenticated();

        ArgumentException.ThrowIfNullOrWhiteSpace(userProfileNumber, nameof(userProfileNumber));

        var url = $"{Endpoints.Gear}?userProfilePk={userProfileNumber}";
        return await _apiClient.GetAsync<JsonElement>(url, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets statistics for specific gear.
    /// </summary>
    /// <param name="gearUuid">Gear UUID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<JsonElement> GetGearStatsAsync(string gearUuid, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        EnsureAuthenticated();

        ArgumentException.ThrowIfNullOrWhiteSpace(gearUuid, nameof(gearUuid));

        var url = string.Format(Endpoints.GearStats, gearUuid);
        return await _apiClient.GetAsync<JsonElement>(url, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets gear defaults for activity types.
    /// </summary>
    /// <param name="userProfileNumber">User profile number.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<JsonElement> GetGearDefaultsAsync(string userProfileNumber, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        EnsureAuthenticated();

        ArgumentException.ThrowIfNullOrWhiteSpace(userProfileNumber, nameof(userProfileNumber));

        var url = string.Format(Endpoints.GearDefaults, userProfileNumber);
        return await _apiClient.GetAsync<JsonElement>(url, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Sets or unsets gear as default for an activity type.
    /// </summary>
    /// <param name="activityType">Activity type (e.g., "running", "cycling").</param>
    /// <param name="gearUuid">Gear UUID.</param>
    /// <param name="isDefault">True to set as default, false to unset.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task SetGearDefaultAsync(
        string activityType,
        string gearUuid,
        bool isDefault = true,
        CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        EnsureAuthenticated();

        ArgumentException.ThrowIfNullOrWhiteSpace(activityType, nameof(activityType));
        ArgumentException.ThrowIfNullOrWhiteSpace(gearUuid, nameof(gearUuid));

        var url = string.Format(Endpoints.SetGearDefault, gearUuid, activityType);

        if (isDefault)
        {
            url += "/default/true";
            await _apiClient.PutAsync(url, new { }, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            await _apiClient.DeleteAsync(url, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Gets activities where specific gear was used.
    /// </summary>
    /// <param name="gearUuid">Gear UUID.</param>
    /// <param name="limit">Maximum number of activities to return (default: 1000).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<JsonElement> GetGearActivitiesAsync(
        string gearUuid,
        int limit = DefaultGearActivitiesLimit,
        CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        EnsureAuthenticated();

        ArgumentException.ThrowIfNullOrWhiteSpace(gearUuid, nameof(gearUuid));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(limit);

        var url = $"{string.Format(Endpoints.GearActivities, gearUuid)}?start=0&limit={limit}";
        return await _apiClient.GetAsync<JsonElement>(url, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Associates gear with an activity.
    /// </summary>
    /// <param name="gearUuid">Gear UUID.</param>
    /// <param name="activityId">Activity ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<JsonElement> AddGearToActivityAsync(
        string gearUuid,
        long activityId,
        CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        EnsureAuthenticated();

        ArgumentException.ThrowIfNullOrWhiteSpace(gearUuid, nameof(gearUuid));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(activityId);

        var url = string.Format(Endpoints.AddGearToActivity, gearUuid, activityId);
        return await _apiClient.PutAsync<JsonElement>(url, new { }, cancellationToken).ConfigureAwait(false);
    }

    #endregion
}
