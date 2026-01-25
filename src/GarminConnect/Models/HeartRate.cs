using System.Text.Json.Serialization;

namespace GarminConnect.Models;

/// <summary>
/// Daily heart rate data from Garmin Connect.
/// </summary>
public record HeartRateData
{
    [JsonPropertyName("calendarDate")]
    public string? CalendarDate { get; init; }

    [JsonPropertyName("startTimestampGMT")]
    public long? StartTimestampGmt { get; init; }

    [JsonPropertyName("endTimestampGMT")]
    public long? EndTimestampGmt { get; init; }

    [JsonPropertyName("startTimestampLocal")]
    public long? StartTimestampLocal { get; init; }

    [JsonPropertyName("endTimestampLocal")]
    public long? EndTimestampLocal { get; init; }

    [JsonPropertyName("maxHeartRate")]
    public int? MaxHeartRate { get; init; }

    [JsonPropertyName("minHeartRate")]
    public int? MinHeartRate { get; init; }

    [JsonPropertyName("restingHeartRate")]
    public int? RestingHeartRate { get; init; }

    [JsonPropertyName("lastSevenDaysAvgRestingHeartRate")]
    public int? LastSevenDaysAvgRestingHeartRate { get; init; }

    [JsonPropertyName("heartRateValues")]
    public List<HeartRateValue>? HeartRateValues { get; init; }

    [JsonPropertyName("heartRateValueDescriptors")]
    public List<HeartRateValueDescriptor>? HeartRateValueDescriptors { get; init; }
}

/// <summary>
/// A single heart rate reading.
/// </summary>
public record HeartRateValue
{
    /// <summary>
    /// Timestamp in milliseconds since epoch.
    /// </summary>
    [JsonPropertyName("timestamp")]
    public long? Timestamp { get; init; }

    /// <summary>
    /// Heart rate in beats per minute.
    /// </summary>
    [JsonPropertyName("heartRate")]
    public int? HeartRate { get; init; }
}

/// <summary>
/// Descriptor for heart rate value fields.
/// </summary>
public record HeartRateValueDescriptor
{
    [JsonPropertyName("key")]
    public string? Key { get; init; }

    [JsonPropertyName("index")]
    public int Index { get; init; }
}

/// <summary>
/// Resting heart rate data.
/// </summary>
public record RestingHeartRate
{
    [JsonPropertyName("calendarDate")]
    public string? CalendarDate { get; init; }

    [JsonPropertyName("userProfilePK")]
    public long? UserProfilePk { get; init; }

    [JsonPropertyName("statisticsStartDate")]
    public string? StatisticsStartDate { get; init; }

    [JsonPropertyName("statisticsEndDate")]
    public string? StatisticsEndDate { get; init; }

    [JsonPropertyName("metricsMap")]
    public Dictionary<string, MetricValue>? MetricsMap { get; init; }
}

/// <summary>
/// A metric value with timestamp.
/// </summary>
public record MetricValue
{
    [JsonPropertyName("calendarDate")]
    public string? CalendarDate { get; init; }

    [JsonPropertyName("value")]
    public double? Value { get; init; }
}
