using System.Text.Json;
using System.Text.Json.Serialization;

namespace GarminConnect.Models;

/// <summary>
/// Activity summary from Garmin Connect.
/// </summary>
public record Activity
{
    [JsonPropertyName("activityId")]
    public long ActivityId { get; init; }

    [JsonPropertyName("activityName")]
    public string? ActivityName { get; init; }

    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("startTimeLocal")]
    public string? StartTimeLocal { get; init; }

    [JsonPropertyName("startTimeGMT")]
    public string? StartTimeGmt { get; init; }

    [JsonPropertyName("activityType")]
    public ActivityType? ActivityType { get; init; }

    [JsonPropertyName("eventType")]
    public EventType? EventType { get; init; }

    [JsonPropertyName("distance")]
    public double? Distance { get; init; }

    [JsonPropertyName("duration")]
    public double? Duration { get; init; }

    [JsonPropertyName("elapsedDuration")]
    public double? ElapsedDuration { get; init; }

    [JsonPropertyName("movingDuration")]
    public double? MovingDuration { get; init; }

    [JsonPropertyName("elevationGain")]
    public double? ElevationGain { get; init; }

    [JsonPropertyName("elevationLoss")]
    public double? ElevationLoss { get; init; }

    [JsonPropertyName("averageSpeed")]
    public double? AverageSpeed { get; init; }

    [JsonPropertyName("maxSpeed")]
    public double? MaxSpeed { get; init; }

    [JsonPropertyName("startLatitude")]
    public double? StartLatitude { get; init; }

    [JsonPropertyName("startLongitude")]
    public double? StartLongitude { get; init; }

    [JsonPropertyName("hasPolyline")]
    public bool? HasPolyline { get; init; }

    [JsonPropertyName("ownerId")]
    public long? OwnerId { get; init; }

    [JsonPropertyName("ownerDisplayName")]
    public string? OwnerDisplayName { get; init; }

    [JsonPropertyName("ownerFullName")]
    public string? OwnerFullName { get; init; }

    [JsonPropertyName("ownerProfileImageUrlSmall")]
    public string? OwnerProfileImageUrlSmall { get; init; }

    [JsonPropertyName("ownerProfileImageUrlMedium")]
    public string? OwnerProfileImageUrlMedium { get; init; }

    [JsonPropertyName("ownerProfileImageUrlLarge")]
    public string? OwnerProfileImageUrlLarge { get; init; }

    [JsonPropertyName("calories")]
    public double? Calories { get; init; }

    [JsonPropertyName("bmrCalories")]
    public double? BmrCalories { get; init; }

    [JsonPropertyName("averageHR")]
    public double? AverageHeartRate { get; init; }

    [JsonPropertyName("maxHR")]
    public double? MaxHeartRate { get; init; }

    [JsonPropertyName("averageRunningCadenceInStepsPerMinute")]
    public double? AverageRunningCadence { get; init; }

    [JsonPropertyName("maxRunningCadenceInStepsPerMinute")]
    public double? MaxRunningCadence { get; init; }

    [JsonPropertyName("averageBikingCadenceInRevPerMinute")]
    public double? AverageBikingCadence { get; init; }

    [JsonPropertyName("maxBikingCadenceInRevPerMinute")]
    public double? MaxBikingCadence { get; init; }

    [JsonPropertyName("averageSwimCadenceInStrokesPerMinute")]
    public double? AverageSwimCadence { get; init; }

    [JsonPropertyName("maxSwimCadenceInStrokesPerMinute")]
    public double? MaxSwimCadence { get; init; }

    [JsonPropertyName("steps")]
    public double? Steps { get; init; }

    [JsonPropertyName("userRating")]
    public double? UserRating { get; init; }

    [JsonPropertyName("hasVideo")]
    public bool? HasVideo { get; init; }

    [JsonPropertyName("hasImages")]
    public bool? HasImages { get; init; }

    [JsonPropertyName("userPro")]
    public bool? UserPro { get; init; }

    [JsonPropertyName("favorite")]
    public bool? Favorite { get; init; }

    [JsonPropertyName("decoDive")]
    public bool? DecoDive { get; init; }

    [JsonPropertyName("parent")]
    public bool? Parent { get; init; }

    [JsonPropertyName("deviceId")]
    public long? DeviceId { get; init; }

    [JsonPropertyName("timeZoneId")]
    public double? TimeZoneId { get; init; }

    [JsonPropertyName("beginTimestamp")]
    public long? BeginTimestamp { get; init; }

    [JsonPropertyName("sportTypeId")]
    public double? SportTypeId { get; init; }

    [JsonPropertyName("avgPower")]
    public double? AveragePower { get; init; }

    [JsonPropertyName("maxPower")]
    public double? MaxPower { get; init; }

    [JsonPropertyName("aerobicTrainingEffect")]
    public double? AerobicTrainingEffect { get; init; }

    [JsonPropertyName("anaerobicTrainingEffect")]
    public double? AnaerobicTrainingEffect { get; init; }

    [JsonPropertyName("strokes")]
    public double? Strokes { get; init; }

    [JsonPropertyName("normPower")]
    public double? NormalizedPower { get; init; }

    [JsonPropertyName("leftBalance")]
    public double? LeftBalance { get; init; }

    [JsonPropertyName("rightBalance")]
    public double? RightBalance { get; init; }

    [JsonPropertyName("avgLeftBalance")]
    public double? AvgLeftBalance { get; init; }

    [JsonPropertyName("max20MinPower")]
    public double? Max20MinPower { get; init; }

    [JsonPropertyName("avgVerticalOscillation")]
    public double? AvgVerticalOscillation { get; init; }

    [JsonPropertyName("avgGroundContactTime")]
    public double? AvgGroundContactTime { get; init; }

    [JsonPropertyName("avgStrideLength")]
    public double? AvgStrideLength { get; init; }

    [JsonPropertyName("avgFractionalCadence")]
    public double? AvgFractionalCadence { get; init; }

    [JsonPropertyName("maxFractionalCadence")]
    public double? MaxFractionalCadence { get; init; }

    [JsonPropertyName("trainingStressScore")]
    public double? TrainingStressScore { get; init; }

    [JsonPropertyName("intensityFactor")]
    public double? IntensityFactor { get; init; }

    [JsonPropertyName("vO2MaxValue")]
    public double? VO2MaxValue { get; init; }

    [JsonPropertyName("avgVerticalRatio")]
    public double? AvgVerticalRatio { get; init; }

    [JsonPropertyName("avgGroundContactBalance")]
    public double? AvgGroundContactBalance { get; init; }

    [JsonPropertyName("lactateThresholdBpm")]
    public double? LactateThresholdBpm { get; init; }

    [JsonPropertyName("lactateThresholdSpeed")]
    public double? LactateThresholdSpeed { get; init; }

    [JsonPropertyName("maxFtp")]
    public double? MaxFtp { get; init; }

    [JsonPropertyName("avgStrokeDistance")]
    public double? AvgStrokeDistance { get; init; }

    [JsonPropertyName("avgStrokeCadence")]
    public double? AvgStrokeCadence { get; init; }

    [JsonPropertyName("maxStrokeCadence")]
    public double? MaxStrokeCadence { get; init; }

    [JsonPropertyName("workoutId")]
    public long? WorkoutId { get; init; }

    [JsonPropertyName("avgSwolf")]
    public double? AvgSwolf { get; init; }

    [JsonPropertyName("activeLengths")]
    public double? ActiveLengths { get; init; }

    [JsonPropertyName("numberOfActiveLengths")]
    public double? NumberOfActiveLengths { get; init; }

    [JsonPropertyName("poolLength")]
    public double? PoolLength { get; init; }

    [JsonPropertyName("unitOfPoolLength")]
    [JsonConverter(typeof(FlexibleStringConverter))]
    public string? UnitOfPoolLength { get; init; }

    [JsonPropertyName("numberOfSplits")]
    public double? NumberOfSplits { get; init; }

    [JsonPropertyName("maxBottomTime")]
    public double? MaxBottomTime { get; init; }

    [JsonPropertyName("hasSplits")]
    public bool? HasSplits { get; init; }

    [JsonPropertyName("eBikeMaxAssistMode")]
    public string? EBikeMaxAssistMode { get; init; }

    [JsonPropertyName("eBikeBatteryUsage")]
    public double? EBikeBatteryUsage { get; init; }

    [JsonPropertyName("eBikeBatteryRemaining")]
    public double? EBikeBatteryRemaining { get; init; }

    [JsonPropertyName("eBikeAssistMode")]
    public double? EBikeAssistMode { get; init; }

    [JsonPropertyName("eBikeAssistLevelMax")]
    public double? EBikeAssistLevelMax { get; init; }

    [JsonPropertyName("avgGrit")]
    public double? AvgGrit { get; init; }

    [JsonPropertyName("avgFlow")]
    public double? AvgFlow { get; init; }

    [JsonPropertyName("jumpCount")]
    public double? JumpCount { get; init; }

    [JsonPropertyName("caloriesEstimated")]
    public double? CaloriesEstimated { get; init; }

    [JsonPropertyName("caloriesConsumed")]
    public double? CaloriesConsumed { get; init; }

    [JsonPropertyName("waterEstimated")]
    public double? WaterEstimated { get; init; }

    [JsonPropertyName("waterConsumed")]
    public double? WaterConsumed { get; init; }

    [JsonPropertyName("maxAvgPower_1")]
    public double? MaxAvgPower1 { get; init; }

    [JsonPropertyName("maxAvgPower_2")]
    public double? MaxAvgPower2 { get; init; }

    [JsonPropertyName("maxAvgPower_5")]
    public double? MaxAvgPower5 { get; init; }

    [JsonPropertyName("maxAvgPower_10")]
    public double? MaxAvgPower10 { get; init; }

    [JsonPropertyName("maxAvgPower_20")]
    public double? MaxAvgPower20 { get; init; }

    [JsonPropertyName("maxAvgPower_30")]
    public double? MaxAvgPower30 { get; init; }

    [JsonPropertyName("maxAvgPower_60")]
    public double? MaxAvgPower60 { get; init; }

    [JsonPropertyName("maxAvgPower_120")]
    public double? MaxAvgPower120 { get; init; }

    [JsonPropertyName("maxAvgPower_300")]
    public double? MaxAvgPower300 { get; init; }

    [JsonPropertyName("maxAvgPower_600")]
    public double? MaxAvgPower600 { get; init; }

    [JsonPropertyName("maxAvgPower_1200")]
    public double? MaxAvgPower1200 { get; init; }

    [JsonPropertyName("maxAvgPower_1800")]
    public double? MaxAvgPower1800 { get; init; }

    [JsonPropertyName("maxAvgPower_3600")]
    public double? MaxAvgPower3600 { get; init; }

    [JsonPropertyName("maxAvgPower_7200")]
    public double? MaxAvgPower7200 { get; init; }

    [JsonPropertyName("maxAvgPower_18000")]
    public double? MaxAvgPower18000 { get; init; }

    [JsonPropertyName("excludeFromPowerCurveReports")]
    public bool? ExcludeFromPowerCurveReports { get; init; }

    [JsonPropertyName("minActivityLapDuration")]
    public double? MinActivityLapDuration { get; init; }

    [JsonPropertyName("minRespirationRate")]
    public double? MinRespirationRate { get; init; }

    [JsonPropertyName("maxRespirationRate")]
    public double? MaxRespirationRate { get; init; }

    [JsonPropertyName("avgRespirationRate")]
    public double? AvgRespirationRate { get; init; }

    [JsonPropertyName("trainingEffectLabel")]
    public string? TrainingEffectLabel { get; init; }

    [JsonPropertyName("activityTrainingLoad")]
    public double? ActivityTrainingLoad { get; init; }

    [JsonPropertyName("aerobicTrainingEffectMessage")]
    public string? AerobicTrainingEffectMessage { get; init; }

    [JsonPropertyName("anaerobicTrainingEffectMessage")]
    public string? AnaerobicTrainingEffectMessage { get; init; }

    [JsonPropertyName("splitSummaries")]
    public List<SplitSummary>? SplitSummaries { get; init; }

    [JsonPropertyName("moderateIntensityMinutes")]
    public double? ModerateIntensityMinutes { get; init; }

    [JsonPropertyName("vigorousIntensityMinutes")]
    public double? VigorousIntensityMinutes { get; init; }

    [JsonPropertyName("pr")]
    public bool? PersonalRecord { get; init; }

    [JsonPropertyName("purposeful")]
    public bool? Purposeful { get; init; }

    [JsonPropertyName("manualActivity")]
    public bool? ManualActivity { get; init; }

    [JsonPropertyName("autoCalcCalories")]
    public bool? AutoCalcCalories { get; init; }

    [JsonPropertyName("elevationCorrected")]
    public bool? ElevationCorrected { get; init; }

    [JsonPropertyName("atpActivity")]
    public bool? AtpActivity { get; init; }
}

/// <summary>
/// Converts JSON values of any type to string (handles number, bool, null).
/// Garmin API sometimes returns numeric values for fields documented as strings.
/// </summary>
internal class FlexibleStringConverter : JsonConverter<string?>
{
    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.String => reader.GetString(),
            JsonTokenType.Number => reader.TryGetInt64(out var l) ? l.ToString() : reader.GetDouble().ToString(),
            JsonTokenType.True => "true",
            JsonTokenType.False => "false",
            JsonTokenType.Null => null,
            _ => null
        };
    }

    public override void Write(Utf8JsonWriter writer, string? value, JsonSerializerOptions options)
    {
        if (value is null)
            writer.WriteNullValue();
        else
            writer.WriteStringValue(value);
    }
}

/// <summary>
/// Activity type information.
/// </summary>
public record ActivityType
{
    [JsonPropertyName("typeId")]
    public int TypeId { get; init; }

    [JsonPropertyName("typeKey")]
    public string? TypeKey { get; init; }

    [JsonPropertyName("parentTypeId")]
    public double? ParentTypeId { get; init; }

    [JsonPropertyName("isHidden")]
    public bool? IsHidden { get; init; }

    [JsonPropertyName("restricted")]
    public bool? Restricted { get; init; }

    [JsonPropertyName("trpiActivityType")]
    public bool? TrpiActivityType { get; init; }
}

/// <summary>
/// Event type information.
/// </summary>
public record EventType
{
    [JsonPropertyName("typeId")]
    public int TypeId { get; init; }

    [JsonPropertyName("typeKey")]
    public string? TypeKey { get; init; }

    [JsonPropertyName("sortOrder")]
    public double? SortOrder { get; init; }
}

/// <summary>
/// Split summary for activity.
/// </summary>
public record SplitSummary
{
    [JsonPropertyName("noOfSplits")]
    public double? NoOfSplits { get; init; }

    [JsonPropertyName("totalAscent")]
    public double? TotalAscent { get; init; }

    [JsonPropertyName("totalDescent")]
    public double? TotalDescent { get; init; }

    [JsonPropertyName("maxElevationGain")]
    public double? MaxElevationGain { get; init; }

    [JsonPropertyName("maxElevationLoss")]
    public double? MaxElevationLoss { get; init; }

    [JsonPropertyName("numClimbs")]
    public double? NumClimbs { get; init; }

    [JsonPropertyName("splitType")]
    public string? SplitType { get; init; }

    [JsonPropertyName("totalDistance")]
    public double? TotalDistance { get; init; }

    [JsonPropertyName("duration")]
    public double? Duration { get; init; }

    [JsonPropertyName("averageSpeed")]
    public double? AverageSpeed { get; init; }

    [JsonPropertyName("maxSpeed")]
    public double? MaxSpeed { get; init; }

    [JsonPropertyName("averageHR")]
    public double? AverageHeartRate { get; init; }

    [JsonPropertyName("maxHR")]
    public double? MaxHeartRate { get; init; }

    [JsonPropertyName("averagePower")]
    public double? AveragePower { get; init; }

    [JsonPropertyName("maxPower")]
    public double? MaxPower { get; init; }

    [JsonPropertyName("calories")]
    public double? Calories { get; init; }
}
