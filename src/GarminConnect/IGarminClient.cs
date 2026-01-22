using GarminConnect.Auth;
using GarminConnect.Models;

namespace GarminConnect;

/// <summary>
/// Main interface for interacting with Garmin Connect API.
/// </summary>
public interface IGarminClient : IDisposable, IAsyncDisposable
{
    #region Authentication

    /// <summary>
    /// Gets whether the client is currently authenticated.
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Gets the current access token if authenticated.
    /// </summary>
    string? AccessToken { get; }

    /// <summary>
    /// Authenticates with Garmin Connect using email and password.
    /// </summary>
    /// <param name="email">Garmin Connect email.</param>
    /// <param name="password">Garmin Connect password.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Authentication result.</returns>
    Task<AuthResult> LoginAsync(string email, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// Completes MFA authentication with the provided code.
    /// </summary>
    /// <param name="mfaCode">The MFA code from authenticator app or SMS.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Authentication result.</returns>
    Task<AuthResult> CompleteMfaAsync(string mfaCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Attempts to resume a session from stored tokens.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if session was resumed successfully.</returns>
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

    #endregion

    #region User & Profile

    /// <summary>
    /// Gets the user's display name.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Display name or null.</returns>
    Task<string?> GetDisplayNameAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the user's full name.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Full name or null.</returns>
    Task<string?> GetFullNameAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the user's unit system (metric/imperial).
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Unit system string.</returns>
    Task<string?> GetUnitSystemAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the full user profile.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>User profile.</returns>
    Task<UserProfile?> GetUserProfileAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets user settings.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>User settings.</returns>
    Task<UserSettings?> GetUserSettingsAsync(CancellationToken cancellationToken = default);

    #endregion

    #region Health - Daily Summary

    /// <summary>
    /// Gets the daily activity summary for a specific date.
    /// </summary>
    /// <param name="date">The date to get summary for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Daily summary data.</returns>
    Task<DailySummary?> GetDailySummaryAsync(DateOnly date, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the daily activity summary for today.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Daily summary data.</returns>
    Task<DailySummary?> GetTodaySummaryAsync(CancellationToken cancellationToken = default);

    #endregion

    #region Health - Heart Rate

    /// <summary>
    /// Gets heart rate data for a specific date.
    /// </summary>
    /// <param name="date">The date to get heart rate data for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Heart rate data.</returns>
    Task<HeartRateData?> GetHeartRatesAsync(DateOnly date, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets resting heart rate for a specific date.
    /// </summary>
    /// <param name="date">The date to get resting heart rate for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Resting heart rate value or null.</returns>
    Task<int?> GetRestingHeartRateAsync(DateOnly date, CancellationToken cancellationToken = default);

    #endregion

    #region Health - Sleep

    /// <summary>
    /// Gets sleep data for a specific date.
    /// </summary>
    /// <param name="date">The date to get sleep data for (the wake-up date).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Sleep data.</returns>
    Task<SleepData?> GetSleepDataAsync(DateOnly date, CancellationToken cancellationToken = default);

    #endregion

    #region Health - Stress

    /// <summary>
    /// Gets stress data for a specific date.
    /// </summary>
    /// <param name="date">The date to get stress data for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Stress data.</returns>
    Task<StressData?> GetStressDataAsync(DateOnly date, CancellationToken cancellationToken = default);

    #endregion

    #region Health - Body Battery

    /// <summary>
    /// Gets body battery data for a date range.
    /// </summary>
    /// <param name="startDate">Start date.</param>
    /// <param name="endDate">End date (optional, defaults to start date).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of body battery reports.</returns>
    Task<List<BodyBatteryReport>> GetBodyBatteryAsync(DateOnly startDate, DateOnly? endDate = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets body battery events for a specific date.
    /// </summary>
    /// <param name="date">The date to get events for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of body battery events.</returns>
    Task<List<BodyBatteryEvent>> GetBodyBatteryEventsAsync(DateOnly date, CancellationToken cancellationToken = default);

    #endregion

    #region Activities - List

    /// <summary>
    /// Gets a list of activities with pagination.
    /// </summary>
    /// <param name="start">Starting offset (0-based).</param>
    /// <param name="limit">Number of activities to return (max 1000).</param>
    /// <param name="activityType">Optional activity type filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of activities.</returns>
    Task<List<Activity>> GetActivitiesAsync(int start = 0, int limit = 20, string? activityType = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets activities within a date range.
    /// </summary>
    /// <param name="startDate">Start date.</param>
    /// <param name="endDate">End date (optional, defaults to today).</param>
    /// <param name="activityType">Optional activity type filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of activities.</returns>
    Task<List<Activity>> GetActivitiesByDateAsync(DateOnly startDate, DateOnly? endDate = null, string? activityType = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the most recent activity.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Most recent activity or null.</returns>
    Task<Activity?> GetLastActivityAsync(CancellationToken cancellationToken = default);

    #endregion

    #region Activities - Details

    /// <summary>
    /// Gets basic activity information.
    /// </summary>
    /// <param name="activityId">Activity ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Activity information.</returns>
    Task<Activity?> GetActivityAsync(long activityId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets detailed activity data including time series metrics.
    /// </summary>
    /// <param name="activityId">Activity ID.</param>
    /// <param name="maxChartSize">Maximum number of chart data points (default 2000).</param>
    /// <param name="maxPolySize">Maximum polyline size (default 4000).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Activity details.</returns>
    Task<ActivityDetails?> GetActivityDetailsAsync(long activityId, int maxChartSize = 2000, int maxPolySize = 4000, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets activity splits (laps).
    /// </summary>
    /// <param name="activityId">Activity ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Activity splits data.</returns>
    Task<ActivitySplits?> GetActivitySplitsAsync(long activityId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets weather data for an activity.
    /// </summary>
    /// <param name="activityId">Activity ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Weather data.</returns>
    Task<ActivityWeather?> GetActivityWeatherAsync(long activityId, CancellationToken cancellationToken = default);

    #endregion

    #region Activities - Download/Upload

    /// <summary>
    /// Downloads an activity file in the specified format.
    /// </summary>
    /// <param name="activityId">Activity ID.</param>
    /// <param name="format">File format (FIT, TCX, GPX, KML, CSV).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Activity file as byte array.</returns>
    Task<byte[]> DownloadActivityAsync(long activityId, ActivityFileFormat format = ActivityFileFormat.Fit, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uploads an activity file.
    /// </summary>
    /// <param name="fileData">File data.</param>
    /// <param name="fileName">File name with extension.</param>
    /// <param name="format">File format.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Uploaded activity ID.</returns>
    Task<long> UploadActivityAsync(byte[] fileData, string fileName, ActivityFileFormat format = ActivityFileFormat.Fit, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uploads an activity file from a stream.
    /// </summary>
    /// <param name="fileStream">File stream.</param>
    /// <param name="fileName">File name with extension.</param>
    /// <param name="format">File format.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Uploaded activity ID.</returns>
    Task<long> UploadActivityAsync(Stream fileStream, string fileName, ActivityFileFormat format = ActivityFileFormat.Fit, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an activity.
    /// </summary>
    /// <param name="activityId">Activity ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteActivityAsync(long activityId, CancellationToken cancellationToken = default);

    #endregion
}

/// <summary>
/// Supported activity file formats.
/// </summary>
public enum ActivityFileFormat
{
    /// <summary>FIT format (Garmin native format).</summary>
    Fit,

    /// <summary>Training Center XML format.</summary>
    Tcx,

    /// <summary>GPS Exchange Format.</summary>
    Gpx,

    /// <summary>Keyhole Markup Language.</summary>
    Kml,

    /// <summary>Comma-Separated Values.</summary>
    Csv
}
