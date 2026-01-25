using System.Text.Json;
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

    #region Body Composition

    /// <summary>
    /// Adds a body composition (weight) measurement to Garmin Connect.
    /// </summary>
    /// <param name="weight">Weight in kilograms.</param>
    /// <param name="timestamp">Measurement timestamp (defaults to now).</param>
    /// <param name="percentFat">Body fat percentage (0-100).</param>
    /// <param name="percentHydration">Body hydration percentage (0-100).</param>
    /// <param name="visceralFatMass">Visceral fat mass in kg.</param>
    /// <param name="boneMass">Bone mass in kg.</param>
    /// <param name="muscleMass">Muscle mass in kg.</param>
    /// <param name="basalMet">Basal metabolic rate.</param>
    /// <param name="activeMet">Active metabolic rate.</param>
    /// <param name="physiqueRating">Physique rating (1-9).</param>
    /// <param name="metabolicAge">Metabolic age in years.</param>
    /// <param name="visceralFatRating">Visceral fat rating.</param>
    /// <param name="bmi">Body mass index.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Upload result.</returns>
    Task<WeightUploadResult> AddBodyCompositionAsync(
        double weight,
        DateTime? timestamp = null,
        double? percentFat = null,
        double? percentHydration = null,
        double? visceralFatMass = null,
        double? boneMass = null,
        double? muscleMass = null,
        double? basalMet = null,
        double? activeMet = null,
        double? physiqueRating = null,
        double? metabolicAge = null,
        double? visceralFatRating = null,
        double? bmi = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets body composition data for a date range.
    /// </summary>
    /// <param name="startDate">Start date.</param>
    /// <param name="endDate">End date (defaults to startDate if null).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Body composition data as JSON element.</returns>
    Task<JsonElement> GetBodyCompositionAsync(
        DateOnly startDate,
        DateOnly? endDate = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets daily weigh-ins for a specific date.
    /// </summary>
    /// <param name="date">Date to get weigh-ins for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Daily weigh-ins as JSON element.</returns>
    Task<JsonElement> GetDailyWeighInsAsync(
        DateOnly date,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a specific weigh-in entry.
    /// </summary>
    /// <param name="weightPk">Weight entry primary key.</param>
    /// <param name="date">Date of the weigh-in.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteWeighInAsync(
        string weightPk,
        DateOnly date,
        CancellationToken cancellationToken = default);

    #endregion

    #region Devices

    /// <summary>
    /// Gets the list of devices registered to the user's Garmin Connect account.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Devices as JSON element.</returns>
    Task<JsonElement> GetDevicesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets device settings for a specific device.
    /// </summary>
    /// <param name="deviceId">Device ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Device settings as JSON element.</returns>
    Task<JsonElement> GetDeviceSettingsAsync(string deviceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the primary training device.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Primary training device as JSON element.</returns>
    Task<JsonElement> GetPrimaryTrainingDeviceAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets solar data for a specific device and date range.
    /// </summary>
    /// <param name="deviceId">Device ID.</param>
    /// <param name="startDate">Start date.</param>
    /// <param name="endDate">End date.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Solar data as JSON element.</returns>
    Task<JsonElement> GetDeviceSolarDataAsync(
        string deviceId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the last used device information.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Last used device as JSON element.</returns>
    Task<JsonElement> GetDeviceLastUsedAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets list of active alarms from all devices.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of alarms as JSON elements.</returns>
    Task<List<JsonElement>> GetDeviceAlarmsAsync(CancellationToken cancellationToken = default);

    #endregion

    #region Gear

    /// <summary>
    /// Gets all gear for a user.
    /// </summary>
    /// <param name="userProfileNumber">User profile number.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Gear as JSON element.</returns>
    Task<JsonElement> GetGearAsync(string userProfileNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets statistics for specific gear.
    /// </summary>
    /// <param name="gearUuid">Gear UUID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Gear stats as JSON element.</returns>
    Task<JsonElement> GetGearStatsAsync(string gearUuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets gear defaults for activity types.
    /// </summary>
    /// <param name="userProfileNumber">User profile number.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Gear defaults as JSON element.</returns>
    Task<JsonElement> GetGearDefaultsAsync(string userProfileNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets or unsets gear as default for an activity type.
    /// </summary>
    /// <param name="activityType">Activity type (e.g., "running", "cycling").</param>
    /// <param name="gearUuid">Gear UUID.</param>
    /// <param name="isDefault">True to set as default, false to unset.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SetGearDefaultAsync(
        string activityType,
        string gearUuid,
        bool isDefault = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets activities where specific gear was used.
    /// </summary>
    /// <param name="gearUuid">Gear UUID.</param>
    /// <param name="limit">Maximum number of activities to return.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Gear activities as JSON element.</returns>
    Task<JsonElement> GetGearActivitiesAsync(
        string gearUuid,
        int limit = 1000,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Associates gear with an activity.
    /// </summary>
    /// <param name="gearUuid">Gear UUID.</param>
    /// <param name="activityId">Activity ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result as JSON element.</returns>
    Task<JsonElement> AddGearToActivityAsync(
        string gearUuid,
        long activityId,
        CancellationToken cancellationToken = default);

    #endregion

    #region Workouts

    /// <summary>
    /// Gets workouts with pagination.
    /// </summary>
    /// <param name="start">Starting offset.</param>
    /// <param name="limit">Maximum number of workouts to return.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Workouts as JSON element.</returns>
    Task<JsonElement> GetWorkoutsAsync(
        int start = 0,
        int limit = 100,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a specific workout by ID.
    /// </summary>
    /// <param name="workoutId">Workout ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Workout as JSON element.</returns>
    Task<JsonElement> GetWorkoutByIdAsync(long workoutId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads a workout as FIT file.
    /// </summary>
    /// <param name="workoutId">Workout ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>FIT file bytes.</returns>
    Task<byte[]> DownloadWorkoutAsync(long workoutId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uploads a workout using JSON data.
    /// </summary>
    /// <param name="workoutJson">Workout JSON data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Upload result as JSON element.</returns>
    Task<JsonElement> UploadWorkoutAsync(JsonElement workoutJson, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uploads a workout using JSON string.
    /// </summary>
    /// <param name="workoutJsonString">Workout JSON string.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Upload result as JSON element.</returns>
    Task<JsonElement> UploadWorkoutAsync(string workoutJsonString, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a scheduled workout by ID.
    /// </summary>
    /// <param name="scheduledWorkoutId">Scheduled workout ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Scheduled workout as JSON element.</returns>
    Task<JsonElement> GetScheduledWorkoutByIdAsync(long scheduledWorkoutId, CancellationToken cancellationToken = default);

    #endregion

    #region Badges

    /// <summary>
    /// Gets earned badges.
    /// </summary>
    Task<JsonElement> GetEarnedBadgesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets available badges.
    /// </summary>
    Task<JsonElement> GetAvailableBadgesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets in-progress badges.
    /// </summary>
    Task<JsonElement> GetInProgressBadgesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets badge challenges with pagination.
    /// </summary>
    Task<JsonElement> GetBadgeChallengesAsync(int start = 0, int limit = 100, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets available badge challenges with pagination.
    /// </summary>
    Task<JsonElement> GetAvailableBadgeChallengesAsync(int start = 0, int limit = 100, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets non-completed badge challenges with pagination.
    /// </summary>
    Task<JsonElement> GetNonCompletedBadgeChallengesAsync(int start = 0, int limit = 100, CancellationToken cancellationToken = default);

    #endregion

    #region Goals

    /// <summary>
    /// Gets user goals.
    /// </summary>
    /// <param name="goalType">Goal type filter (optional).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<JsonElement> GetGoalsAsync(string? goalType = null, CancellationToken cancellationToken = default);

    #endregion

    #region Training Plans

    /// <summary>
    /// Gets all training plans.
    /// </summary>
    Task<JsonElement> GetTrainingPlansAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a training plan by ID.
    /// </summary>
    Task<JsonElement> GetTrainingPlanByIdAsync(long planId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an adaptive training plan by ID.
    /// </summary>
    Task<JsonElement> GetAdaptiveTrainingPlanByIdAsync(long planId, CancellationToken cancellationToken = default);

    #endregion

    #region Blood Pressure

    /// <summary>
    /// Adds a blood pressure measurement to Garmin Connect.
    /// </summary>
    /// <param name="systolicPressure">Systolic blood pressure in mmHg.</param>
    /// <param name="diastolicPressure">Diastolic blood pressure in mmHg.</param>
    /// <param name="timestamp">Measurement timestamp (defaults to now).</param>
    /// <param name="heartRate">Heart rate in bpm (optional).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Upload result.</returns>
    Task<BloodPressureUploadResult> AddBloodPressureAsync(
        int systolicPressure,
        int diastolicPressure,
        DateTime? timestamp = null,
        int? heartRate = null,
        CancellationToken cancellationToken = default);

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
