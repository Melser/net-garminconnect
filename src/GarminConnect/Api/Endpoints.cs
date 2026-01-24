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

    /// <summary>Daily summary endpoint. Format: {0} = date (yyyy-MM-dd)</summary>
    public const string DailySummary = "/usersummary-service/usersummary/daily/{0}";

    /// <summary>User summary chart endpoint. Format: {0} = date (yyyy-MM-dd)</summary>
    public const string UserSummaryChart = "/wellness-service/wellness/dailySummaryChart/{0}";

    #endregion

    #region Steps

    /// <summary>Steps data endpoint. Format: {0} = date (yyyy-MM-dd)</summary>
    public const string StepsData = "/wellness-service/wellness/dailySteps/{0}";

    /// <summary>Daily steps range endpoint (use with query params).</summary>
    public const string DailySteps = "/wellness-service/wellness/dailySteps";

    /// <summary>Weekly steps endpoint. Format: {0} = date (yyyy-MM-dd), {1} = weeks</summary>
    public const string WeeklySteps = "/wellness-service/stats/weekly/steps/{0}/{1}";

    #endregion

    #region Heart Rate

    /// <summary>Heart rate endpoint. Format: {0} = date (yyyy-MM-dd)</summary>
    public const string DailyHeartRate = "/wellness-service/wellness/dailyHeartRate/{0}";

    /// <summary>Resting heart rate endpoint. Format: {0} = date (yyyy-MM-dd)</summary>
    public const string RestingHeartRate = "/userstats-service/wellness/daily/{0}?metricId=37";

    #endregion

    #region Sleep

    /// <summary>Sleep data endpoint (use with query params).</summary>
    public const string DailySleep = "/wellness-service/wellness/dailySleepData";

    #endregion

    #region Stress

    /// <summary>Stress data endpoint. Format: {0} = date (yyyy-MM-dd)</summary>
    public const string DailyStress = "/wellness-service/wellness/dailyStress/{0}";

    /// <summary>Weekly stress endpoint. Format: {0} = date (yyyy-MM-dd), {1} = weeks</summary>
    public const string WeeklyStress = "/wellness-service/stats/weekly/stress/{0}/{1}";

    #endregion

    #region Body Battery

    /// <summary>Body battery daily reports endpoint (use with query params).</summary>
    public const string BodyBatteryDaily = "/wellness-service/wellness/bodyBattery/reports/daily";

    /// <summary>Body battery events endpoint. Append /{date}</summary>
    public const string BodyBatteryEvents = "/wellness-service/wellness/bodyBattery/events";

    #endregion

    #region Respiration & SpO2

    /// <summary>Respiration endpoint. Format: {0} = date (yyyy-MM-dd)</summary>
    public const string Respiration = "/wellness-service/wellness/dailyRespiration/{0}";

    /// <summary>SpO2 endpoint. Format: {0} = date (yyyy-MM-dd)</summary>
    public const string SpO2 = "/wellness-service/wellness/dailyPulseOx/{0}";

    #endregion

    #region HRV

    /// <summary>HRV endpoint. Format: {0} = date (yyyy-MM-dd)</summary>
    public const string HrvData = "/hrv-service/hrv/{0}";

    #endregion

    #region Activities

    /// <summary>Activities search endpoint (use with query params).</summary>
    public const string Activities = "/activitylist-service/activities/search/activities";

    /// <summary>Activities for date endpoint. Format: {0} = date (yyyy-MM-dd)</summary>
    public const string ActivitiesForDate = "/activitylist-service/activities/fordate/{0}";

    /// <summary>Single activity endpoint. Format: {0} = activityId</summary>
    public const string Activity = "/activity-service/activity/{0}";

    /// <summary>Activity details endpoint. Format: {0} = activityId</summary>
    public const string ActivityDetails = "/activity-service/activity/{0}/details";

    /// <summary>Activity splits endpoint. Format: {0} = activityId</summary>
    public const string ActivitySplits = "/activity-service/activity/{0}/splits";

    /// <summary>Activity weather endpoint. Format: {0} = activityId</summary>
    public const string ActivityWeather = "/activity-service/activity/{0}/weather";

    /// <summary>Activity HR time in zones endpoint. Format: {0} = activityId</summary>
    public const string ActivityHrTimezones = "/activity-service/activity/{0}/hrTimeInZones";

    /// <summary>Activity gear endpoint. Format: {0} = activityId</summary>
    public const string ActivityGear = "/activity-service/activity/{0}/gear";

    public const string ActivityTypes = "/activity-service/activity/activityTypes";

    #endregion

    #region Activity Download/Upload

    /// <summary>Download activity as FIT. Format: {0} = activityId</summary>
    public const string ActivityDownloadFit = "/download-service/export/fit/activity/{0}";

    /// <summary>Download activity as TCX. Format: {0} = activityId</summary>
    public const string ActivityDownloadTcx = "/download-service/export/tcx/activity/{0}";

    /// <summary>Download activity as GPX. Format: {0} = activityId</summary>
    public const string ActivityDownloadGpx = "/download-service/export/gpx/activity/{0}";

    /// <summary>Download activity as KML. Format: {0} = activityId</summary>
    public const string ActivityDownloadKml = "/download-service/export/kml/activity/{0}";

    /// <summary>Download activity as CSV. Format: {0} = activityId</summary>
    public const string ActivityDownloadCsv = "/download-service/export/csv/activity/{0}";

    /// <summary>Upload activity endpoint.</summary>
    public const string ActivityUpload = "/upload-service/upload";

    #endregion

    #region Body Composition & Weight

    /// <summary>Weight date range endpoint (use with query params).</summary>
    public const string WeightDateRange = "/weight-service/weight/dateRange";

    /// <summary>Daily weigh-ins endpoint. Format: {0} = date (yyyy-MM-dd)</summary>
    public const string DailyWeighIns = "/weight-service/weight/dayview/{0}";

    public const string LatestWeight = "/weight-service/weight/latest";

    public const string AddWeighIn = "/weight-service/user-weight";

    /// <summary>Delete weigh-in endpoint. Format: {0} = date (yyyy-MM-dd), {1} = weight_pk</summary>
    public const string DeleteWeighIn = "/weight-service/weight/{0}/byversion/{1}";

    #endregion

    #region Devices

    public const string Devices = "/device-service/deviceregistration/devices";

    /// <summary>Device settings endpoint. Format: {0} = deviceId</summary>
    public const string DeviceSettings = "/device-service/deviceservice/device-info/settings/{0}";

    public const string PrimaryTrainingDevice = "/web-gateway/device-info/primary-training-device";

    /// <summary>Device solar data endpoint. Format: {0} = deviceId, {1} = startDate, {2} = endDate (yyyy-MM-dd)</summary>
    public const string DeviceSolarData = "/web-gateway/solar/{0}/{1}/{2}";

    public const string DeviceLastUsed = "/device-service/deviceservice/mylastused";

    #endregion

    #region Gear

    /// <summary>Gear filter endpoint (use with query params).</summary>
    public const string Gear = "/gear-service/gear/filterGear";

    public const string GearBase = "/gear-service/gear";

    /// <summary>Gear stats endpoint. Format: {0} = gearUuid</summary>
    public const string GearStats = "/gear-service/gear/stats/{0}";

    /// <summary>Gear defaults endpoint. Format: {0} = userProfileNumber</summary>
    public const string GearDefaults = "/gear-service/gear/user/{0}/activityTypes";

    /// <summary>Set/Unset gear default endpoint. Format: {0} = gearUuid, {1} = activityType</summary>
    public const string SetGearDefault = "/gear-service/gear/{0}/activityType/{1}";

    /// <summary>Gear activities endpoint. Format: {0} = gearUuid</summary>
    public const string GearActivities = "/activitylist-service/activities/{0}/gear";

    /// <summary>Add gear to activity endpoint. Format: {0} = gearUuid, {1} = activityId</summary>
    public const string AddGearToActivity = "/gear-service/gear/link/{0}/activity/{1}";

    #endregion

    #region Workouts

    /// <summary>Workouts list endpoint (use with query params).</summary>
    public const string Workouts = "/workout-service/workouts";

    /// <summary>Single workout endpoint. Format: {0} = workoutId</summary>
    public const string Workout = "/workout-service/workout/{0}";

    /// <summary>Download workout as FIT. Format: {0} = workoutId</summary>
    public const string WorkoutDownloadFit = "/workout-service/workout/FIT/{0}";

    /// <summary>Upload workout endpoint.</summary>
    public const string WorkoutUpload = "/workout-service/workout";

    public const string ScheduleWorkout = "/workout-service/schedule";

    /// <summary>Scheduled workout endpoint. Format: {0} = scheduledWorkoutId</summary>
    public const string ScheduledWorkout = "/workout-service/schedule/{0}";

    #endregion

    #region Badges & Challenges

    public const string EarnedBadges = "/badge-service/badge/earned";
    public const string AvailableBadges = "/badge-service/badge/available";
    public const string InProgressBadges = "/badge-service/badge/inProgress";
    public const string BadgeChallenges = "/badgechallenge-service/badgeChallenge/available";
    public const string AvailableBadgeChallenges = "/badgechallenge-service/badgeChallenge/available";
    public const string NonCompletedBadgeChallenges = "/badgechallenge-service/badgeChallenge/nonCompleted";
    public const string AdhocChallenges = "/adhocchallenge-service/adHocChallenge/historical";

    #endregion

    #region Goals

    public const string Goals = "/goal-service/goal/goals";

    #endregion

    #region Personal Records

    /// <summary>Personal records endpoint. Format: {0} = userProfileNumber</summary>
    public const string PersonalRecords = "/personalrecord-service/personalrecord/prs/{0}";

    #endregion

    #region Training

    /// <summary>Training readiness endpoint. Format: {0} = date (yyyy-MM-dd)</summary>
    public const string TrainingReadiness = "/metrics-service/metrics/trainingreadiness/{0}";

    /// <summary>Training status endpoint. Format: {0} = date (yyyy-MM-dd)</summary>
    public const string TrainingStatus = "/metrics-service/metrics/trainingstatus/aggregated/{0}";

    public const string RacePredictions = "/metrics-service/metrics/racepredictions";

    public const string TrainingPlans = "/trainingplan-service/trainingplan/all";

    /// <summary>Training plan by ID endpoint. Format: {0} = planId</summary>
    public const string TrainingPlanById = "/trainingplan-service/trainingplan/{0}";

    /// <summary>Adaptive training plan by ID endpoint. Format: {0} = planId</summary>
    public const string AdaptiveTrainingPlanById = "/trainingplan-service/trainingplan/adaptive/{0}";

    #endregion

    #region Hydration

    /// <summary>Hydration endpoint. Format: {0} = date (yyyy-MM-dd)</summary>
    public const string Hydration = "/usersummary-service/usersummary/hydration/daily/{0}";

    public const string AddHydration = "/usersummary-service/usersummary/hydration/log";

    #endregion

    #region Blood Pressure

    /// <summary>Blood pressure range endpoint. Format: {0} = startDate, {1} = endDate (yyyy-MM-dd)</summary>
    public const string BloodPressure = "/bloodpressure-service/bloodpressure/range/{0}/{1}";

    #endregion

    #region Floors

    /// <summary>Floors chart endpoint. Format: {0} = date (yyyy-MM-dd)</summary>
    public const string Floors = "/wellness-service/wellness/floorsChartData/daily/{0}";

    #endregion

    #region Max Metrics

    /// <summary>Max metrics endpoint. Format: {0} = date (yyyy-MM-dd)</summary>
    public const string MaxMetrics = "/metrics-service/metrics/maxmet/daily/{0}";

    #endregion
}
