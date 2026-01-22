using System.Text.Json.Serialization;

namespace GarminConnect.Models;

/// <summary>
/// Daily sleep data from Garmin Connect.
/// </summary>
public record SleepData
{
    [JsonPropertyName("dailySleepDTO")]
    public DailySleepDto? DailySleep { get; init; }

    [JsonPropertyName("sleepMovement")]
    public List<SleepMovement>? SleepMovement { get; init; }

    [JsonPropertyName("remSleepData")]
    public bool? RemSleepData { get; init; }

    [JsonPropertyName("sleepLevels")]
    public List<SleepLevel>? SleepLevels { get; init; }

    [JsonPropertyName("sleepScores")]
    public SleepScores? SleepScores { get; init; }

    [JsonPropertyName("wellnessSpO2SleepSummaryDTO")]
    public WellnessSpO2SleepSummary? WellnessSpO2SleepSummary { get; init; }

    [JsonPropertyName("restlessMomentsCount")]
    public int? RestlessMomentsCount { get; init; }
}

/// <summary>
/// Daily sleep summary details.
/// </summary>
public record DailySleepDto
{
    [JsonPropertyName("id")]
    public long? Id { get; init; }

    [JsonPropertyName("userProfilePK")]
    public long? UserProfilePk { get; init; }

    [JsonPropertyName("calendarDate")]
    public string? CalendarDate { get; init; }

    [JsonPropertyName("sleepTimeSeconds")]
    public int? SleepTimeSeconds { get; init; }

    [JsonPropertyName("napTimeSeconds")]
    public int? NapTimeSeconds { get; init; }

    [JsonPropertyName("sleepWindowConfirmed")]
    public bool? SleepWindowConfirmed { get; init; }

    [JsonPropertyName("sleepWindowConfirmationType")]
    public string? SleepWindowConfirmationType { get; init; }

    [JsonPropertyName("sleepStartTimestampGMT")]
    public long? SleepStartTimestampGmt { get; init; }

    [JsonPropertyName("sleepEndTimestampGMT")]
    public long? SleepEndTimestampGmt { get; init; }

    [JsonPropertyName("sleepStartTimestampLocal")]
    public long? SleepStartTimestampLocal { get; init; }

    [JsonPropertyName("sleepEndTimestampLocal")]
    public long? SleepEndTimestampLocal { get; init; }

    [JsonPropertyName("autoSleepStartTimestampGMT")]
    public long? AutoSleepStartTimestampGmt { get; init; }

    [JsonPropertyName("autoSleepEndTimestampGMT")]
    public long? AutoSleepEndTimestampGmt { get; init; }

    [JsonPropertyName("unmeasurableSleepSeconds")]
    public int? UnmeasurableSleepSeconds { get; init; }

    [JsonPropertyName("deepSleepSeconds")]
    public int? DeepSleepSeconds { get; init; }

    [JsonPropertyName("lightSleepSeconds")]
    public int? LightSleepSeconds { get; init; }

    [JsonPropertyName("remSleepSeconds")]
    public int? RemSleepSeconds { get; init; }

    [JsonPropertyName("awakeSleepSeconds")]
    public int? AwakeSleepSeconds { get; init; }

    [JsonPropertyName("deviceRemCapable")]
    public bool? DeviceRemCapable { get; init; }

    [JsonPropertyName("retro")]
    public bool? Retro { get; init; }

    [JsonPropertyName("sleepFromDevice")]
    public bool? SleepFromDevice { get; init; }

    [JsonPropertyName("averageSpO2Value")]
    public double? AverageSpO2Value { get; init; }

    [JsonPropertyName("lowestSpO2Value")]
    public int? LowestSpO2Value { get; init; }

    [JsonPropertyName("highestSpO2Value")]
    public int? HighestSpO2Value { get; init; }

    [JsonPropertyName("averageSpO2HRSleep")]
    public double? AverageSpO2HrSleep { get; init; }

    [JsonPropertyName("averageRespirationValue")]
    public double? AverageRespirationValue { get; init; }

    [JsonPropertyName("lowestRespirationValue")]
    public double? LowestRespirationValue { get; init; }

    [JsonPropertyName("highestRespirationValue")]
    public double? HighestRespirationValue { get; init; }

    [JsonPropertyName("awakeCount")]
    public int? AwakeCount { get; init; }

    [JsonPropertyName("avgSleepStress")]
    public double? AvgSleepStress { get; init; }

    [JsonPropertyName("sleepScoreFeedback")]
    public string? SleepScoreFeedback { get; init; }

    [JsonPropertyName("sleepScoreInsight")]
    public string? SleepScoreInsight { get; init; }

    [JsonPropertyName("sleepNeed")]
    public int? SleepNeed { get; init; }
}

/// <summary>
/// Sleep movement data point.
/// </summary>
public record SleepMovement
{
    [JsonPropertyName("startGMT")]
    public long? StartGmt { get; init; }

    [JsonPropertyName("endGMT")]
    public long? EndGmt { get; init; }

    [JsonPropertyName("activityLevel")]
    public double? ActivityLevel { get; init; }
}

/// <summary>
/// Sleep level data point (deep, light, REM, awake).
/// </summary>
public record SleepLevel
{
    [JsonPropertyName("startGMT")]
    public long? StartGmt { get; init; }

    [JsonPropertyName("endGMT")]
    public long? EndGmt { get; init; }

    [JsonPropertyName("activityLevel")]
    public double? ActivityLevel { get; init; }
}

/// <summary>
/// Sleep score components.
/// </summary>
public record SleepScores
{
    [JsonPropertyName("totalDuration")]
    public SleepScoreComponent? TotalDuration { get; init; }

    [JsonPropertyName("stress")]
    public SleepScoreComponent? Stress { get; init; }

    [JsonPropertyName("awakeCount")]
    public SleepScoreComponent? AwakeCount { get; init; }

    [JsonPropertyName("overall")]
    public SleepScoreComponent? Overall { get; init; }

    [JsonPropertyName("remPercentage")]
    public SleepScoreComponent? RemPercentage { get; init; }

    [JsonPropertyName("restfulness")]
    public SleepScoreComponent? Restfulness { get; init; }

    [JsonPropertyName("lightPercentage")]
    public SleepScoreComponent? LightPercentage { get; init; }

    [JsonPropertyName("deepPercentage")]
    public SleepScoreComponent? DeepPercentage { get; init; }
}

/// <summary>
/// Individual sleep score component.
/// </summary>
public record SleepScoreComponent
{
    [JsonPropertyName("qualifierKey")]
    public string? QualifierKey { get; init; }

    [JsonPropertyName("optimalStart")]
    public double? OptimalStart { get; init; }

    [JsonPropertyName("optimalEnd")]
    public double? OptimalEnd { get; init; }

    [JsonPropertyName("value")]
    public double? Value { get; init; }

    [JsonPropertyName("idealStartInSeconds")]
    public int? IdealStartInSeconds { get; init; }

    [JsonPropertyName("idealEndInSeconds")]
    public int? IdealEndInSeconds { get; init; }
}

/// <summary>
/// SpO2 summary during sleep.
/// </summary>
public record WellnessSpO2SleepSummary
{
    [JsonPropertyName("averageSpO2")]
    public double? AverageSpO2 { get; init; }

    [JsonPropertyName("lowestSpO2")]
    public int? LowestSpO2 { get; init; }

    [JsonPropertyName("averageHR")]
    public double? AverageHr { get; init; }

    [JsonPropertyName("spO2HRSleep")]
    public double? SpO2HrSleep { get; init; }
}
