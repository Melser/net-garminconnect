using System.Text.Json.Serialization;

namespace GarminConnect.Models;

/// <summary>
/// Daily activity summary from Garmin Connect.
/// </summary>
public record DailySummary
{
    [JsonPropertyName("calendarDate")]
    public string? CalendarDate { get; init; }

    [JsonPropertyName("totalKilocalories")]
    public int? TotalKilocalories { get; init; }

    [JsonPropertyName("activeKilocalories")]
    public int? ActiveKilocalories { get; init; }

    [JsonPropertyName("bmrKilocalories")]
    public int? BmrKilocalories { get; init; }

    [JsonPropertyName("wellnessKilocalories")]
    public int? WellnessKilocalories { get; init; }

    [JsonPropertyName("totalSteps")]
    public int? TotalSteps { get; init; }

    [JsonPropertyName("dailyStepGoal")]
    public int? DailyStepGoal { get; init; }

    [JsonPropertyName("totalDistanceMeters")]
    public double? TotalDistanceMeters { get; init; }

    [JsonPropertyName("floorsAscendedInMeters")]
    public double? FloorsAscendedInMeters { get; init; }

    [JsonPropertyName("floorsDescendedInMeters")]
    public double? FloorsDescendedInMeters { get; init; }

    [JsonPropertyName("floorsAscended")]
    public int? FloorsAscended { get; init; }

    [JsonPropertyName("floorsDescended")]
    public int? FloorsDescended { get; init; }

    [JsonPropertyName("userFloorsAscendedGoal")]
    public int? UserFloorsAscendedGoal { get; init; }

    [JsonPropertyName("minHeartRate")]
    public int? MinHeartRate { get; init; }

    [JsonPropertyName("maxHeartRate")]
    public int? MaxHeartRate { get; init; }

    [JsonPropertyName("restingHeartRate")]
    public int? RestingHeartRate { get; init; }

    [JsonPropertyName("lastSevenDaysAvgRestingHeartRate")]
    public int? LastSevenDaysAvgRestingHeartRate { get; init; }

    [JsonPropertyName("averageStressLevel")]
    public int? AverageStressLevel { get; init; }

    [JsonPropertyName("maxStressLevel")]
    public int? MaxStressLevel { get; init; }

    [JsonPropertyName("stressDuration")]
    public int? StressDuration { get; init; }

    [JsonPropertyName("restStressDuration")]
    public int? RestStressDuration { get; init; }

    [JsonPropertyName("activityStressDuration")]
    public int? ActivityStressDuration { get; init; }

    [JsonPropertyName("uncategorizedStressDuration")]
    public int? UncategorizedStressDuration { get; init; }

    [JsonPropertyName("totalStressDuration")]
    public int? TotalStressDuration { get; init; }

    [JsonPropertyName("lowStressDuration")]
    public int? LowStressDuration { get; init; }

    [JsonPropertyName("mediumStressDuration")]
    public int? MediumStressDuration { get; init; }

    [JsonPropertyName("highStressDuration")]
    public int? HighStressDuration { get; init; }

    [JsonPropertyName("stressQualifier")]
    public string? StressQualifier { get; init; }

    [JsonPropertyName("bodyBatteryChargedValue")]
    public int? BodyBatteryChargedValue { get; init; }

    [JsonPropertyName("bodyBatteryDrainedValue")]
    public int? BodyBatteryDrainedValue { get; init; }

    [JsonPropertyName("bodyBatteryHighestValue")]
    public int? BodyBatteryHighestValue { get; init; }

    [JsonPropertyName("bodyBatteryLowestValue")]
    public int? BodyBatteryLowestValue { get; init; }

    [JsonPropertyName("bodyBatteryMostRecentValue")]
    public int? BodyBatteryMostRecentValue { get; init; }

    [JsonPropertyName("averageSpo2")]
    public double? AverageSpo2 { get; init; }

    [JsonPropertyName("lowestSpo2")]
    public int? LowestSpo2 { get; init; }

    [JsonPropertyName("latestSpo2")]
    public int? LatestSpo2 { get; init; }

    [JsonPropertyName("latestSpo2ReadingTimeGmt")]
    public string? LatestSpo2ReadingTimeGmt { get; init; }

    [JsonPropertyName("moderateIntensityMinutes")]
    public int? ModerateIntensityMinutes { get; init; }

    [JsonPropertyName("vigorousIntensityMinutes")]
    public int? VigorousIntensityMinutes { get; init; }

    [JsonPropertyName("intensityMinutesGoal")]
    public int? IntensityMinutesGoal { get; init; }

    [JsonPropertyName("measurableAwakeDuration")]
    public int? MeasurableAwakeDuration { get; init; }

    [JsonPropertyName("measurableAsleepDuration")]
    public int? MeasurableAsleepDuration { get; init; }

    [JsonPropertyName("privacyProtected")]
    public bool? PrivacyProtected { get; init; }
}
