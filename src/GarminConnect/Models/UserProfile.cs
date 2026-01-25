using System.Text.Json.Serialization;

namespace GarminConnect.Models;

/// <summary>
/// User profile information from Garmin Connect.
/// </summary>
public record UserProfile
{
    [JsonPropertyName("displayName")]
    public string? DisplayName { get; init; }

    [JsonPropertyName("firstName")]
    public string? FirstName { get; init; }

    [JsonPropertyName("lastName")]
    public string? LastName { get; init; }

    [JsonPropertyName("userName")]
    public string? UserName { get; init; }

    [JsonPropertyName("profileImageUrlLarge")]
    public string? ProfileImageUrlLarge { get; init; }

    [JsonPropertyName("profileImageUrlMedium")]
    public string? ProfileImageUrlMedium { get; init; }

    [JsonPropertyName("profileImageUrlSmall")]
    public string? ProfileImageUrlSmall { get; init; }

    [JsonPropertyName("location")]
    public string? Location { get; init; }

    [JsonPropertyName("facebookUrl")]
    public string? FacebookUrl { get; init; }

    [JsonPropertyName("twitterUrl")]
    public string? TwitterUrl { get; init; }

    [JsonPropertyName("websiteUrl")]
    public string? WebsiteUrl { get; init; }

    [JsonPropertyName("userLevel")]
    public int? UserLevel { get; init; }

    [JsonPropertyName("userPoint")]
    public int? UserPoint { get; init; }

    [JsonPropertyName("levelUpdateDate")]
    public string? LevelUpdateDate { get; init; }
}

/// <summary>
/// User settings from Garmin Connect.
/// </summary>
public record UserSettings
{
    [JsonPropertyName("id")]
    public long? Id { get; init; }

    [JsonPropertyName("displayName")]
    public string? DisplayName { get; init; }

    [JsonPropertyName("preferredLocale")]
    public string? PreferredLocale { get; init; }

    [JsonPropertyName("measurementSystem")]
    public string? MeasurementSystem { get; init; }

    [JsonPropertyName("firstDayOfWeek")]
    public FirstDayOfWeek? FirstDayOfWeek { get; init; }

    [JsonPropertyName("numberFormat")]
    public string? NumberFormat { get; init; }

    [JsonPropertyName("dateFormat")]
    public DateFormat? DateFormat { get; init; }

    [JsonPropertyName("timeFormat")]
    public TimeFormat? TimeFormat { get; init; }

    [JsonPropertyName("timeZone")]
    public string? TimeZone { get; init; }

    [JsonPropertyName("hydrationMeasurementUnit")]
    public string? HydrationMeasurementUnit { get; init; }

    [JsonPropertyName("hydrationContainers")]
    public List<HydrationContainer>? HydrationContainers { get; init; }

    [JsonPropertyName("golfDistanceUnit")]
    public string? GolfDistanceUnit { get; init; }

    [JsonPropertyName("golfElevationUnit")]
    public string? GolfElevationUnit { get; init; }

    [JsonPropertyName("golfSpeedUnit")]
    public string? GolfSpeedUnit { get; init; }

    [JsonPropertyName("powerFormat")]
    public PowerFormat? PowerFormat { get; init; }

    [JsonPropertyName("handedness")]
    public string? Handedness { get; init; }
}

public record FirstDayOfWeek
{
    [JsonPropertyName("dayId")]
    public int DayId { get; init; }

    [JsonPropertyName("dayName")]
    public string? DayName { get; init; }

    [JsonPropertyName("sortOrder")]
    public int SortOrder { get; init; }
}

public record DateFormat
{
    [JsonPropertyName("formatId")]
    public int FormatId { get; init; }

    [JsonPropertyName("formatKey")]
    public string? FormatKey { get; init; }

    [JsonPropertyName("dateFormat")]
    public string? Format { get; init; }
}

public record TimeFormat
{
    [JsonPropertyName("formatId")]
    public int FormatId { get; init; }

    [JsonPropertyName("formatKey")]
    public string? FormatKey { get; init; }

    [JsonPropertyName("timeFormat")]
    public string? Format { get; init; }

    [JsonPropertyName("displayFormat")]
    public string? DisplayFormat { get; init; }
}

public record PowerFormat
{
    [JsonPropertyName("formatId")]
    public int FormatId { get; init; }

    [JsonPropertyName("formatKey")]
    public string? FormatKey { get; init; }

    [JsonPropertyName("displayFormat")]
    public string? DisplayFormat { get; init; }
}

public record HydrationContainer
{
    [JsonPropertyName("id")]
    public long Id { get; init; }

    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("volume")]
    public int Volume { get; init; }

    [JsonPropertyName("unit")]
    public string? Unit { get; init; }
}
