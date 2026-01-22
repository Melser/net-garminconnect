namespace GarminConnect.Api;

/// <summary>
/// Contains all Garmin Connect API endpoints.
/// </summary>
internal static class Endpoints
{
    public const string BaseUrl = "https://connectapi.garmin.com";

    #region User & Profile

    public const string UserProfile = "/userprofile-service/socialProfile";
    public const string UserSettings = "/userprofile-service/userprofile/settings";
    public const string PersonalInformation = "/userprofile-service/userprofile/personal-information";

    #endregion

    #region Daily Summary & Stats

    public static string DailySummary(DateOnly date) =>
        $"/usersummary-service/usersummary/daily/{date:yyyy-MM-dd}";

    public static string UserSummaryChart(DateOnly date) =>
        $"/wellness-service/wellness/dailySummaryChart/{date:yyyy-MM-dd}";

    #endregion

    #region Steps

    public static string StepsData(DateOnly date) =>
        $"/wellness-service/wellness/dailySteps/{date:yyyy-MM-dd}";

    public static string DailySteps(DateOnly startDate, DateOnly endDate) =>
        $"/wellness-service/wellness/dailySteps?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}";

    public static string WeeklySteps(DateOnly endDate, int weeks = 1) =>
        $"/wellness-service/stats/weekly/steps/{endDate:yyyy-MM-dd}/{weeks}";

    #endregion

    #region Heart Rate

    public static string HeartRates(DateOnly date) =>
        $"/wellness-service/wellness/dailyHeartRate/{date:yyyy-MM-dd}";

    public static string RestingHeartRate(DateOnly date) =>
        $"/userstats-service/wellness/daily/{date:yyyy-MM-dd}?metricId=37";

    #endregion

    #region Sleep

    public static string SleepData(DateOnly date) =>
        $"/wellness-service/wellness/dailySleepData/{date:yyyy-MM-dd}";

    #endregion

    #region Stress

    public static string StressData(DateOnly date) =>
        $"/wellness-service/wellness/dailyStress/{date:yyyy-MM-dd}";

    public static string WeeklyStress(DateOnly endDate, int weeks = 1) =>
        $"/wellness-service/stats/weekly/stress/{endDate:yyyy-MM-dd}/{weeks}";

    #endregion

    #region Body Battery

    public static string BodyBattery(DateOnly startDate, DateOnly endDate) =>
        $"/wellness-service/wellness/bodyBattery?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}";

    #endregion

    #region Respiration & SpO2

    public static string Respiration(DateOnly date) =>
        $"/wellness-service/wellness/dailyRespiration/{date:yyyy-MM-dd}";

    public static string SpO2(DateOnly date) =>
        $"/wellness-service/wellness/dailyPulseOx/{date:yyyy-MM-dd}";

    #endregion

    #region HRV

    public static string HrvData(DateOnly date) =>
        $"/hrv-service/hrv/{date:yyyy-MM-dd}";

    #endregion

    #region Activities

    public static string Activities(int start, int limit, string? activityType = null)
    {
        var url = $"/activitylist-service/activities/search/activities?start={start}&limit={limit}";
        if (!string.IsNullOrEmpty(activityType))
            url += $"&activityType={activityType}";
        return url;
    }

    public static string ActivitiesByDate(DateOnly startDate, DateOnly endDate, string? activityType = null)
    {
        var url = $"/activitylist-service/activities/search/activities?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}";
        if (!string.IsNullOrEmpty(activityType))
            url += $"&activityType={activityType}";
        return url;
    }

    public static string ActivitiesForDate(DateOnly date) =>
        $"/activitylist-service/activities/fordate/{date:yyyy-MM-dd}";

    public static string Activity(long activityId) =>
        $"/activity-service/activity/{activityId}";

    public static string ActivityDetails(long activityId, int? maxChartSize = null, int? maxPolylineSize = null) =>
        $"/activity-service/activity/{activityId}/details?maxChartSize={maxChartSize ?? 2000}&maxPolylineSize={maxPolylineSize ?? 4000}";

    public static string ActivitySplits(long activityId) =>
        $"/activity-service/activity/{activityId}/splits";

    public static string ActivityWeather(long activityId) =>
        $"/activity-service/activity/{activityId}/weather";

    public static string ActivityHrTimezones(long activityId) =>
        $"/activity-service/activity/{activityId}/hrTimeInZones";

    public static string ActivityGear(long activityId) =>
        $"/activity-service/activity/{activityId}/gear";

    public const string ActivityTypes = "/activity-service/activity/activityTypes";

    #endregion

    #region Activity Download/Upload

    public static string DownloadActivity(long activityId, string format) =>
        $"/download-service/export/{format}/activity/{activityId}";

    public const string UploadActivity = "/upload-service/upload/.fit";

    public static string DeleteActivity(long activityId) =>
        $"/activity-service/activity/{activityId}";

    #endregion

    #region Body Composition & Weight

    public static string BodyComposition(DateOnly date) =>
        $"/weight-service/weight/dateRange?startDate={date:yyyy-MM-dd}&endDate={date:yyyy-MM-dd}";

    public static string WeighIns(DateOnly startDate, DateOnly endDate) =>
        $"/weight-service/weight/dateRange?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}";

    public static string DailyWeighIns(DateOnly date) =>
        $"/weight-service/weight/dayview/{date:yyyy-MM-dd}";

    public const string LatestWeight = "/weight-service/weight/latest";

    public const string AddWeighIn = "/weight-service/user-weight";

    public static string DeleteWeighIn(long weighInId) =>
        $"/weight-service/weight/{weighInId}";

    #endregion

    #region Devices

    public const string Devices = "/device-service/deviceregistration/devices";

    public static string DeviceSettings(long deviceId) =>
        $"/device-service/deviceservice/device-info/settings/{deviceId}";

    public const string DeviceLastUsed = "/device-service/deviceregistration/devices/lastused";

    #endregion

    #region Gear

    public static string Gear(long userProfileNumber) =>
        $"/gear-service/gear/filterGear?userProfilePK={userProfileNumber}";

    public static string GearStats(string gearUuid) =>
        $"/gear-service/gear/stats/{gearUuid}";

    public static string GearDefaults(long userProfileNumber) =>
        $"/gear-service/gear/user/{userProfileNumber}/activityType-defaults";

    #endregion

    #region Workouts

    public static string Workouts(int start, int limit) =>
        $"/workout-service/workouts?start={start}&limit={limit}";

    public static string Workout(long workoutId) =>
        $"/workout-service/workout/{workoutId}";

    public const string ScheduleWorkout = "/workout-service/schedule";

    #endregion

    #region Badges & Challenges

    public const string EarnedBadges = "/badge-service/badge/earned";
    public const string AvailableBadges = "/badge-service/badge/available";
    public const string BadgeChallenges = "/badgechallenge-service/badgeChallenge/available";
    public const string AdhocChallenges = "/adhocchallenge-service/adHocChallenge/historical";

    #endregion

    #region Goals

    public const string Goals = "/goal-service/goal/goals";

    #endregion

    #region Personal Records

    public static string PersonalRecords(long userProfileNumber) =>
        $"/personalrecord-service/personalrecord/prs/{userProfileNumber}";

    #endregion

    #region Training

    public static string TrainingReadiness(DateOnly date) =>
        $"/metrics-service/metrics/trainingreadiness/{date:yyyy-MM-dd}";

    public static string TrainingStatus(DateOnly date) =>
        $"/metrics-service/metrics/trainingstatus/aggregated/{date:yyyy-MM-dd}";

    public const string RacePredictions = "/metrics-service/metrics/racepredictions";

    public const string TrainingPlans = "/trainingplan-service/trainingplan/all";

    #endregion

    #region Hydration

    public static string Hydration(DateOnly date) =>
        $"/usersummary-service/usersummary/hydration/daily/{date:yyyy-MM-dd}";

    public const string AddHydration = "/usersummary-service/usersummary/hydration/log";

    #endregion

    #region Blood Pressure

    public static string BloodPressure(DateOnly startDate, DateOnly endDate) =>
        $"/bloodpressure-service/bloodpressure/range/{startDate:yyyy-MM-dd}/{endDate:yyyy-MM-dd}";

    #endregion

    #region Floors

    public static string Floors(DateOnly date) =>
        $"/wellness-service/wellness/floorsChartData/daily/{date:yyyy-MM-dd}";

    #endregion

    #region Max Metrics

    public static string MaxMetrics(DateOnly date) =>
        $"/metrics-service/metrics/maxmet/daily/{date:yyyy-MM-dd}";

    #endregion
}
