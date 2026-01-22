namespace GarminConnect.Fit;

/// <summary>
/// FIT encoder for blood pressure data.
/// </summary>
public sealed class FitEncoderBloodPressure : FitEncoder
{
    private const int LMsgTypeBloodPressure = 14;
    private bool _bloodPressureDefined;

    /// <summary>
    /// Writes a blood pressure record.
    /// </summary>
    /// <param name="timestamp">Measurement timestamp.</param>
    /// <param name="systolicPressure">Systolic blood pressure in mmHg.</param>
    /// <param name="diastolicPressure">Diastolic blood pressure in mmHg.</param>
    /// <param name="meanArterialPressure">Mean arterial pressure in mmHg.</param>
    /// <param name="map3SampleMean">MAP 3-sample mean.</param>
    /// <param name="mapMorningValues">MAP morning values.</param>
    /// <param name="mapEveningValues">MAP evening values.</param>
    /// <param name="heartRate">Heart rate in bpm.</param>
    public void WriteBloodPressure(
        DateTime timestamp,
        ushort? systolicPressure = null,
        ushort? diastolicPressure = null,
        ushort? meanArterialPressure = null,
        ushort? map3SampleMean = null,
        ushort? mapMorningValues = null,
        ushort? mapEveningValues = null,
        byte? heartRate = null)
    {
        var content = new (int Num, FitType Type, object? Value, int? Scale)[]
        {
            (253, FitBaseType.UInt32, ToFitTimestamp(timestamp), 1),
            (0, FitBaseType.UInt16, systolicPressure, 1),
            (1, FitBaseType.UInt16, diastolicPressure, 1),
            (2, FitBaseType.UInt16, meanArterialPressure, 1),
            (3, FitBaseType.UInt16, map3SampleMean, 1),
            (4, FitBaseType.UInt16, mapMorningValues, 1),
            (5, FitBaseType.UInt16, mapEveningValues, 1),
            (6, FitBaseType.UInt8, heartRate, 1)
        };

        var (fields, values) = BuildContentBlock(content);

        if (!_bloodPressureDefined)
        {
            WriteRecordHeader(definition: true, lmsgType: LMsgTypeBloodPressure);
            Buffer.WriteByte(0); // Reserved
            Buffer.WriteByte(0); // Architecture: little endian
            WriteUInt16(GlobalMessageNumber.BloodPressure);
            Buffer.WriteByte((byte)content.Length);
            Buffer.Write(fields);
            _bloodPressureDefined = true;
        }

        WriteRecordHeader(definition: false, lmsgType: LMsgTypeBloodPressure);
        Buffer.Write(values);
    }

    /// <summary>
    /// Creates a complete FIT file with blood pressure data.
    /// </summary>
    /// <param name="timestamp">Measurement timestamp.</param>
    /// <param name="systolicPressure">Systolic blood pressure in mmHg.</param>
    /// <param name="diastolicPressure">Diastolic blood pressure in mmHg.</param>
    /// <param name="meanArterialPressure">Mean arterial pressure in mmHg (optional, calculated if not provided).</param>
    /// <param name="heartRate">Heart rate in bpm.</param>
    /// <returns>Complete FIT file as byte array.</returns>
    public static byte[] CreateBloodPressureFile(
        DateTime timestamp,
        ushort systolicPressure,
        ushort diastolicPressure,
        ushort? meanArterialPressure = null,
        byte? heartRate = null)
    {
        // Calculate MAP if not provided: MAP = DBP + 1/3(SBP - DBP)
        meanArterialPressure ??= (ushort)(diastolicPressure + (systolicPressure - diastolicPressure) / 3);

        using var encoder = new FitEncoderBloodPressure();

        encoder.WriteFileInfo(timeCreated: timestamp);
        encoder.WriteFileCreator();
        encoder.WriteDeviceInfo(timestamp);
        encoder.WriteBloodPressure(
            timestamp,
            systolicPressure,
            diastolicPressure,
            meanArterialPressure,
            heartRate: heartRate);
        encoder.Finish();

        return encoder.GetBytes();
    }
}
