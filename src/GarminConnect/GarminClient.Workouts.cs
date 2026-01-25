using System.Text.Json;
using GarminConnect.Api;

namespace GarminConnect;

public sealed partial class GarminClient
{
    private const int DefaultWorkoutsLimit = 100;

    #region Workouts

    /// <summary>
    /// Gets workouts with pagination.
    /// </summary>
    /// <param name="start">Starting offset.</param>
    /// <param name="limit">Maximum number of workouts to return.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<JsonElement> GetWorkoutsAsync(
        int start = 0,
        int limit = DefaultWorkoutsLimit,
        CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        EnsureAuthenticated();

        ArgumentOutOfRangeException.ThrowIfNegative(start);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(limit);

        var url = $"{Endpoints.Workouts}?start={start}&limit={limit}";
        return await _apiClient.GetAsync<JsonElement>(url, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets a specific workout by ID.
    /// </summary>
    /// <param name="workoutId">Workout ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<JsonElement> GetWorkoutByIdAsync(long workoutId, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        EnsureAuthenticated();

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(workoutId);

        var url = string.Format(Endpoints.Workout, workoutId);
        return await _apiClient.GetAsync<JsonElement>(url, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Downloads a workout as FIT file.
    /// </summary>
    /// <param name="workoutId">Workout ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>FIT file bytes.</returns>
    public async Task<byte[]> DownloadWorkoutAsync(long workoutId, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        EnsureAuthenticated();

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(workoutId);

        var url = string.Format(Endpoints.WorkoutDownloadFit, workoutId);
        return await _apiClient.GetBytesAsync(url, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Uploads a workout using JSON data.
    /// </summary>
    /// <param name="workoutJson">Workout JSON data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<JsonElement> UploadWorkoutAsync(JsonElement workoutJson, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        EnsureAuthenticated();

        return await _apiClient.PostAsync<JsonElement>(Endpoints.WorkoutUpload, workoutJson, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Uploads a workout using JSON string.
    /// </summary>
    /// <param name="workoutJsonString">Workout JSON string.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<JsonElement> UploadWorkoutAsync(string workoutJsonString, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        EnsureAuthenticated();

        ArgumentException.ThrowIfNullOrWhiteSpace(workoutJsonString, nameof(workoutJsonString));

        var workout = JsonSerializer.Deserialize<JsonElement>(workoutJsonString);
        return await UploadWorkoutAsync(workout, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets a scheduled workout by ID.
    /// </summary>
    /// <param name="scheduledWorkoutId">Scheduled workout ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<JsonElement> GetScheduledWorkoutByIdAsync(long scheduledWorkoutId, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        EnsureAuthenticated();

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(scheduledWorkoutId);

        var url = string.Format(Endpoints.ScheduledWorkout, scheduledWorkoutId);
        return await _apiClient.GetAsync<JsonElement>(url, cancellationToken).ConfigureAwait(false);
    }

    #endregion
}
