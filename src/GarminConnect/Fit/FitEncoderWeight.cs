namespace GarminConnect.Fit;

/// <summary>
/// FIT encoder for weight scale and body composition data.
/// </summary>
public sealed class FitEncoderWeight : FitEncoder
{
    private const int LMsgTypeWeightScale = 3;
    private bool _weightScaleDefined;

    /// <summary>
    /// Writes a weight scale record with body composition data.
    /// </summary>
    /// <param name="timestamp">Measurement timestamp.</param>
    /// <param name="weight">Weight in kilograms.</param>
    /// <param name="percentFat">Body fat percentage (0-100).</param>
    /// <param name="percentHydration">Body hydration percentage (0-100).</param>
    /// <param name="visceralFatMass">Visceral fat mass in kg.</param>
    /// <param name="boneMass">Bone mass in kg.</param>
    /// <param name="muscleMass">Muscle mass in kg.</param>
    /// <param name="basalMet">Basal metabolic rate.</param>
    /// <param name="activeMet">Active metabolic rate.</param>
    /// <param name="physiqueRating">Physique rating (1-9).</param>
    /// <param name="metabolicAge">Metabolic age in years.</param>
    /// <param name="visceralFatRating">Visceral fat rating.</param>
    /// <param name="bmi">Body mass index.</param>
    public void WriteWeightScale(
        DateTime timestamp,
        double weight,
        double? percentFat = null,
        double? percentHydration = null,
        double? visceralFatMass = null,
        double? boneMass = null,
        double? muscleMass = null,
        double? basalMet = null,
        double? activeMet = null,
        double? physiqueRating = null,
        double? metabolicAge = null,
        double? visceralFatRating = null,
        double? bmi = null)
    {
        // Weight scale content with scale factors matching Garmin's FIT protocol
        // Scale factors convert values to the storage format:
        // - weight, fat%, hydration%, masses: scale 100 (stored as value * 100)
        // - basal/active met: scale 4
        // - BMI: scale 10
        var content = new (int Num, FitType Type, object? Value, int? Scale)[]
        {
            (253, FitBaseType.UInt32, ToFitTimestamp(timestamp), 1),
            (0, FitBaseType.UInt16, weight, 100),           // weight in 0.01 kg
            (1, FitBaseType.UInt16, percentFat, 100),       // percent fat in 0.01 %
            (2, FitBaseType.UInt16, percentHydration, 100), // percent hydration in 0.01 %
            (3, FitBaseType.UInt16, visceralFatMass, 100),  // visceral fat mass in 0.01 kg
            (4, FitBaseType.UInt16, boneMass, 100),         // bone mass in 0.01 kg
            (5, FitBaseType.UInt16, muscleMass, 100),       // muscle mass in 0.01 kg
            (7, FitBaseType.UInt16, basalMet, 4),           // basal met in 0.25 kcal/day
            (9, FitBaseType.UInt16, activeMet, 4),          // active met in 0.25 kcal/day
            (8, FitBaseType.UInt8, physiqueRating, 1),      // physique rating
            (10, FitBaseType.UInt8, metabolicAge, 1),       // metabolic age in years
            (11, FitBaseType.UInt8, visceralFatRating, 1),  // visceral fat rating
            (13, FitBaseType.UInt16, bmi, 10)               // BMI in 0.1 kg/m^2
        };

        var (fields, values) = BuildContentBlock(content);

        if (!_weightScaleDefined)
        {
            WriteRecordHeader(definition: true, lmsgType: LMsgTypeWeightScale);
            Buffer.WriteByte(0); // Reserved
            Buffer.WriteByte(0); // Architecture: little endian
            WriteUInt16(GlobalMessageNumber.WeightScale);
            Buffer.WriteByte((byte)content.Length);
            Buffer.Write(fields);
            _weightScaleDefined = true;
        }

        WriteRecordHeader(definition: false, lmsgType: LMsgTypeWeightScale);
        Buffer.Write(values);
    }

    /// <summary>
    /// Creates a complete FIT file with weight data.
    /// </summary>
    /// <param name="timestamp">Measurement timestamp.</param>
    /// <param name="weight">Weight in kilograms.</param>
    /// <param name="percentFat">Body fat percentage (0-100).</param>
    /// <param name="percentHydration">Body hydration percentage (0-100).</param>
    /// <param name="visceralFatMass">Visceral fat mass in kg.</param>
    /// <param name="boneMass">Bone mass in kg.</param>
    /// <param name="muscleMass">Muscle mass in kg.</param>
    /// <param name="basalMet">Basal metabolic rate.</param>
    /// <param name="activeMet">Active metabolic rate.</param>
    /// <param name="physiqueRating">Physique rating (1-9).</param>
    /// <param name="metabolicAge">Metabolic age in years.</param>
    /// <param name="visceralFatRating">Visceral fat rating.</param>
    /// <param name="bmi">Body mass index.</param>
    /// <returns>Complete FIT file as byte array.</returns>
    public static byte[] CreateWeightFile(
        DateTime timestamp,
        double weight,
        double? percentFat = null,
        double? percentHydration = null,
        double? visceralFatMass = null,
        double? boneMass = null,
        double? muscleMass = null,
        double? basalMet = null,
        double? activeMet = null,
        double? physiqueRating = null,
        double? metabolicAge = null,
        double? visceralFatRating = null,
        double? bmi = null)
    {
        using var encoder = new FitEncoderWeight();

        encoder.WriteFileInfo(timeCreated: timestamp);
        encoder.WriteFileCreator();
        encoder.WriteDeviceInfo(timestamp);
        encoder.WriteWeightScale(
            timestamp,
            weight,
            percentFat,
            percentHydration,
            visceralFatMass,
            boneMass,
            muscleMass,
            basalMet,
            activeMet,
            physiqueRating,
            metabolicAge,
            visceralFatRating,
            bmi);
        encoder.Finish();

        return encoder.GetBytes();
    }
}
