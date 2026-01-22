using System.Text.Json.Serialization;

namespace GarminConnect.Models;

/// <summary>
/// Daily body battery report from Garmin Connect.
/// </summary>
public record BodyBatteryReport
{
    [JsonPropertyName("date")]
    public string? Date { get; init; }

    [JsonPropertyName("charged")]
    public int? Charged { get; init; }

    [JsonPropertyName("drained")]
    public int? Drained { get; init; }

    [JsonPropertyName("startTimestampGMT")]
    public long? StartTimestampGmt { get; init; }

    [JsonPropertyName("endTimestampGMT")]
    public long? EndTimestampGmt { get; init; }

    [JsonPropertyName("startTimestampLocal")]
    public long? StartTimestampLocal { get; init; }

    [JsonPropertyName("endTimestampLocal")]
    public long? EndTimestampLocal { get; init; }

    [JsonPropertyName("confirmedDate")]
    public string? ConfirmedDate { get; init; }

    [JsonPropertyName("bodyBatteryVersion")]
    public double? BodyBatteryVersion { get; init; }
}

/// <summary>
/// Body battery event (activity, sleep, etc.).
/// </summary>
public record BodyBatteryEvent
{
    [JsonPropertyName("eventType")]
    public string? EventType { get; init; }

    [JsonPropertyName("startTimestampGMT")]
    public long? StartTimestampGmt { get; init; }

    [JsonPropertyName("endTimestampGMT")]
    public long? EndTimestampGmt { get; init; }

    [JsonPropertyName("startTimestampLocal")]
    public long? StartTimestampLocal { get; init; }

    [JsonPropertyName("endTimestampLocal")]
    public long? EndTimestampLocal { get; init; }

    [JsonPropertyName("bodyBatteryImpact")]
    public int? BodyBatteryImpact { get; init; }

    [JsonPropertyName("activityType")]
    public string? ActivityType { get; init; }

    [JsonPropertyName("activityId")]
    public long? ActivityId { get; init; }

    [JsonPropertyName("activityName")]
    public string? ActivityName { get; init; }

    [JsonPropertyName("calories")]
    public int? Calories { get; init; }

    [JsonPropertyName("durationInSeconds")]
    public int? DurationInSeconds { get; init; }

    [JsonPropertyName("feedback")]
    public string? Feedback { get; init; }
}

/// <summary>
/// Body battery aggregate data.
/// </summary>
public record BodyBatteryAggregate
{
    [JsonPropertyName("startTimestampGMT")]
    public long? StartTimestampGmt { get; init; }

    [JsonPropertyName("endTimestampGMT")]
    public long? EndTimestampGmt { get; init; }

    [JsonPropertyName("startTimestampLocal")]
    public long? StartTimestampLocal { get; init; }

    [JsonPropertyName("endTimestampLocal")]
    public long? EndTimestampLocal { get; init; }

    [JsonPropertyName("maxValue")]
    public int? MaxValue { get; init; }

    [JsonPropertyName("minValue")]
    public int? MinValue { get; init; }

    [JsonPropertyName("avgValue")]
    public double? AvgValue { get; init; }

    [JsonPropertyName("charged")]
    public int? Charged { get; init; }

    [JsonPropertyName("drained")]
    public int? Drained { get; init; }
}
