using System.Text.Json.Serialization;

namespace GarminConnect.Models;

/// <summary>
/// Detailed activity data with time series metrics.
/// </summary>
public record ActivityDetails
{
    [JsonPropertyName("activityId")]
    public long? ActivityId { get; init; }

    [JsonPropertyName("measurementCount")]
    public int? MeasurementCount { get; init; }

    [JsonPropertyName("metricsCount")]
    public int? MetricsCount { get; init; }

    [JsonPropertyName("metricDescriptors")]
    public List<MetricDescriptor>? MetricDescriptors { get; init; }

    [JsonPropertyName("activityDetailMetrics")]
    public List<ActivityDetailMetric>? ActivityDetailMetrics { get; init; }

    [JsonPropertyName("geoPolylineDTO")]
    public GeoPolyline? GeoPolyline { get; init; }

    [JsonPropertyName("heartRateDTOs")]
    public List<HeartRateDto>? HeartRates { get; init; }

    [JsonPropertyName("pendingData")]
    public bool? PendingData { get; init; }
}

/// <summary>
/// Metric descriptor for activity detail metrics.
/// </summary>
public record MetricDescriptor
{
    [JsonPropertyName("metricsIndex")]
    public int MetricsIndex { get; init; }

    [JsonPropertyName("key")]
    public string? Key { get; init; }

    [JsonPropertyName("unit")]
    public MetricUnit? Unit { get; init; }
}

/// <summary>
/// Unit information for metrics.
/// </summary>
public record MetricUnit
{
    [JsonPropertyName("id")]
    public int? Id { get; init; }

    [JsonPropertyName("key")]
    public string? Key { get; init; }

    [JsonPropertyName("factor")]
    public double? Factor { get; init; }
}

/// <summary>
/// Single metric data point in activity details.
/// </summary>
public record ActivityDetailMetric
{
    [JsonPropertyName("metrics")]
    public List<double?>? Metrics { get; init; }
}

/// <summary>
/// Geographic polyline data for activity route.
/// </summary>
public record GeoPolyline
{
    [JsonPropertyName("startPoint")]
    public GeoPoint? StartPoint { get; init; }

    [JsonPropertyName("endPoint")]
    public GeoPoint? EndPoint { get; init; }

    [JsonPropertyName("minLat")]
    public double? MinLat { get; init; }

    [JsonPropertyName("maxLat")]
    public double? MaxLat { get; init; }

    [JsonPropertyName("minLon")]
    public double? MinLon { get; init; }

    [JsonPropertyName("maxLon")]
    public double? MaxLon { get; init; }

    [JsonPropertyName("polyline")]
    public List<GeoPoint>? Polyline { get; init; }
}

/// <summary>
/// Geographic point with latitude and longitude.
/// </summary>
public record GeoPoint
{
    [JsonPropertyName("lat")]
    public double? Lat { get; init; }

    [JsonPropertyName("lon")]
    public double? Lon { get; init; }

    [JsonPropertyName("altitude")]
    public double? Altitude { get; init; }

    [JsonPropertyName("time")]
    public long? Time { get; init; }

    [JsonPropertyName("timerStart")]
    public bool? TimerStart { get; init; }

    [JsonPropertyName("timerStop")]
    public bool? TimerStop { get; init; }

    [JsonPropertyName("distanceFromPreviousPoint")]
    public double? DistanceFromPreviousPoint { get; init; }

    [JsonPropertyName("distanceInMeters")]
    public double? DistanceInMeters { get; init; }

    [JsonPropertyName("speed")]
    public double? Speed { get; init; }

    [JsonPropertyName("cumulativeAscent")]
    public double? CumulativeAscent { get; init; }

    [JsonPropertyName("cumulativeDescent")]
    public double? CumulativeDescent { get; init; }

    [JsonPropertyName("extendedCoordinate")]
    public bool? ExtendedCoordinate { get; init; }

    [JsonPropertyName("valid")]
    public bool? Valid { get; init; }
}

/// <summary>
/// Heart rate data point in activity details.
/// </summary>
public record HeartRateDto
{
    [JsonPropertyName("heartRate")]
    public int? HeartRate { get; init; }

    [JsonPropertyName("timestamp")]
    public long? Timestamp { get; init; }
}

/// <summary>
/// Activity splits data.
/// </summary>
public record ActivitySplits
{
    [JsonPropertyName("activityId")]
    public long? ActivityId { get; init; }

    [JsonPropertyName("lapDTOs")]
    public List<ActivityLap>? Laps { get; init; }

    [JsonPropertyName("eventDTOs")]
    public List<ActivityEvent>? Events { get; init; }
}

/// <summary>
/// Activity lap data.
/// </summary>
public record ActivityLap
{
    [JsonPropertyName("startTimeGMT")]
    public long? StartTimeGmt { get; init; }

    [JsonPropertyName("startLatitude")]
    public double? StartLatitude { get; init; }

    [JsonPropertyName("startLongitude")]
    public double? StartLongitude { get; init; }

    [JsonPropertyName("distance")]
    public double? Distance { get; init; }

    [JsonPropertyName("duration")]
    public double? Duration { get; init; }

    [JsonPropertyName("movingDuration")]
    public double? MovingDuration { get; init; }

    [JsonPropertyName("elapsedDuration")]
    public double? ElapsedDuration { get; init; }

    [JsonPropertyName("elevationGain")]
    public double? ElevationGain { get; init; }

    [JsonPropertyName("elevationLoss")]
    public double? ElevationLoss { get; init; }

    [JsonPropertyName("maxElevation")]
    public double? MaxElevation { get; init; }

    [JsonPropertyName("minElevation")]
    public double? MinElevation { get; init; }

    [JsonPropertyName("averageSpeed")]
    public double? AverageSpeed { get; init; }

    [JsonPropertyName("maxSpeed")]
    public double? MaxSpeed { get; init; }

    [JsonPropertyName("averageHR")]
    public int? AverageHeartRate { get; init; }

    [JsonPropertyName("maxHR")]
    public int? MaxHeartRate { get; init; }

    [JsonPropertyName("averageRunCadence")]
    public double? AverageRunCadence { get; init; }

    [JsonPropertyName("maxRunCadence")]
    public double? MaxRunCadence { get; init; }

    [JsonPropertyName("calories")]
    public int? Calories { get; init; }

    [JsonPropertyName("averagePower")]
    public double? AveragePower { get; init; }

    [JsonPropertyName("maxPower")]
    public double? MaxPower { get; init; }

    [JsonPropertyName("totalExerciseReps")]
    public int? TotalExerciseReps { get; init; }

    [JsonPropertyName("lapIndex")]
    public int? LapIndex { get; init; }

    [JsonPropertyName("lengthDTOs")]
    public List<LapLength>? Lengths { get; init; }

    [JsonPropertyName("connectIQMeasurement")]
    public List<ConnectIqMeasurement>? ConnectIqMeasurements { get; init; }

    [JsonPropertyName("messageIndex")]
    public int? MessageIndex { get; init; }

    [JsonPropertyName("strokes")]
    public int? Strokes { get; init; }

    [JsonPropertyName("avgSwolf")]
    public double? AvgSwolf { get; init; }

    [JsonPropertyName("avgStrokeDistance")]
    public double? AvgStrokeDistance { get; init; }

    [JsonPropertyName("swimStroke")]
    public string? SwimStroke { get; init; }

    [JsonPropertyName("unit")]
    public string? Unit { get; init; }
}

/// <summary>
/// Lap length data (for swimming).
/// </summary>
public record LapLength
{
    [JsonPropertyName("startTimeGMT")]
    public long? StartTimeGmt { get; init; }

    [JsonPropertyName("distance")]
    public double? Distance { get; init; }

    [JsonPropertyName("duration")]
    public double? Duration { get; init; }

    [JsonPropertyName("averageSpeed")]
    public double? AverageSpeed { get; init; }

    [JsonPropertyName("averageSwolf")]
    public double? AverageSwolf { get; init; }

    [JsonPropertyName("strokes")]
    public int? Strokes { get; init; }

    [JsonPropertyName("swimStroke")]
    public string? SwimStroke { get; init; }

    [JsonPropertyName("lengthType")]
    public string? LengthType { get; init; }

    [JsonPropertyName("lengthIndex")]
    public int? LengthIndex { get; init; }
}

/// <summary>
/// Connect IQ measurement.
/// </summary>
public record ConnectIqMeasurement
{
    [JsonPropertyName("appId")]
    public string? AppId { get; init; }

    [JsonPropertyName("fields")]
    public List<ConnectIqField>? Fields { get; init; }
}

/// <summary>
/// Connect IQ field.
/// </summary>
public record ConnectIqField
{
    [JsonPropertyName("fieldNumber")]
    public int? FieldNumber { get; init; }

    [JsonPropertyName("value")]
    public double? Value { get; init; }

    [JsonPropertyName("units")]
    public string? Units { get; init; }
}

/// <summary>
/// Activity event.
/// </summary>
public record ActivityEvent
{
    [JsonPropertyName("startTimeGMT")]
    public long? StartTimeGmt { get; init; }

    [JsonPropertyName("eventType")]
    public string? EventType { get; init; }

    [JsonPropertyName("messageIndex")]
    public int? MessageIndex { get; init; }
}

/// <summary>
/// Activity weather information.
/// </summary>
public record ActivityWeather
{
    [JsonPropertyName("temperature")]
    public int? Temperature { get; init; }

    [JsonPropertyName("apparentTemperature")]
    public int? ApparentTemperature { get; init; }

    [JsonPropertyName("dewPoint")]
    public int? DewPoint { get; init; }

    [JsonPropertyName("relativeHumidity")]
    public int? RelativeHumidity { get; init; }

    [JsonPropertyName("windDirection")]
    public int? WindDirection { get; init; }

    [JsonPropertyName("windDirectionCompassPoint")]
    public string? WindDirectionCompassPoint { get; init; }

    [JsonPropertyName("windSpeed")]
    public double? WindSpeed { get; init; }

    [JsonPropertyName("windGust")]
    public double? WindGust { get; init; }

    [JsonPropertyName("latitude")]
    public double? Latitude { get; init; }

    [JsonPropertyName("longitude")]
    public double? Longitude { get; init; }

    [JsonPropertyName("weatherStationDTO")]
    public WeatherStation? WeatherStation { get; init; }

    [JsonPropertyName("issueDate")]
    public string? IssueDate { get; init; }

    [JsonPropertyName("weatherTypeDTO")]
    public WeatherType? WeatherType { get; init; }
}

/// <summary>
/// Weather station information.
/// </summary>
public record WeatherStation
{
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("timezone")]
    public string? Timezone { get; init; }
}

/// <summary>
/// Weather type information.
/// </summary>
public record WeatherType
{
    [JsonPropertyName("weatherTypePK")]
    public int? WeatherTypePk { get; init; }

    [JsonPropertyName("desc")]
    public string? Description { get; init; }

    [JsonPropertyName("image")]
    public string? Image { get; init; }
}
