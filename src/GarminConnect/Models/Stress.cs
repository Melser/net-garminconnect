using System.Text.Json.Serialization;

namespace GarminConnect.Models;

/// <summary>
/// Daily stress data from Garmin Connect.
/// </summary>
public record StressData
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

    [JsonPropertyName("maxStressLevel")]
    public int? MaxStressLevel { get; init; }

    [JsonPropertyName("avgStressLevel")]
    public int? AvgStressLevel { get; init; }

    [JsonPropertyName("stressChartValueOffset")]
    public int? StressChartValueOffset { get; init; }

    [JsonPropertyName("stressChartYAxisOrigin")]
    public int? StressChartYAxisOrigin { get; init; }

    [JsonPropertyName("stressValuesArray")]
    public List<List<int>>? StressValuesArray { get; init; }

    [JsonPropertyName("bodyBatteryValuesArray")]
    public List<List<int>>? BodyBatteryValuesArray { get; init; }

    [JsonPropertyName("stressValueDescriptorsDTOList")]
    public List<StressValueDescriptor>? StressValueDescriptors { get; init; }

    [JsonPropertyName("bodyBatteryValueDescriptorsDTOList")]
    public List<BodyBatteryValueDescriptor>? BodyBatteryValueDescriptors { get; init; }
}

/// <summary>
/// Stress value field descriptor.
/// </summary>
public record StressValueDescriptor
{
    [JsonPropertyName("stressValueDescriptorIndex")]
    public int Index { get; init; }

    [JsonPropertyName("stressValueDescriptorKey")]
    public string? Key { get; init; }
}

/// <summary>
/// Body battery value field descriptor.
/// </summary>
public record BodyBatteryValueDescriptor
{
    [JsonPropertyName("bodyBatteryValueDescriptorIndex")]
    public int Index { get; init; }

    [JsonPropertyName("bodyBatteryValueDescriptorKey")]
    public string? Key { get; init; }
}
