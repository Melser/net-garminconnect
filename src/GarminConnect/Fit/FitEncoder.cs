using System.Buffers.Binary;

namespace GarminConnect.Fit;

/// <summary>
/// Base class for encoding FIT (Flexible and Interoperable Data Transfer) files.
/// FIT is a binary format used by Garmin devices.
/// </summary>
public class FitEncoder : IDisposable
{
    /// <summary>
    /// FIT file header size in bytes.
    /// </summary>
    protected const int HeaderSize = 12;

    /// <summary>
    /// FIT epoch: December 31, 1989 00:00:00 UTC.
    /// </summary>
    private static readonly DateTime FitEpoch = new(1989, 12, 31, 0, 0, 0, DateTimeKind.Utc);

    /// <summary>
    /// CRC lookup table for FIT protocol checksum.
    /// </summary>
    private static readonly ushort[] CrcTable =
    [
        0x0000, 0xCC01, 0xD801, 0x1400,
        0xF001, 0x3C00, 0x2800, 0xE401,
        0xA001, 0x6C00, 0x7800, 0xB401,
        0x5000, 0x9C01, 0x8801, 0x4400
    ];

    /// <summary>
    /// Global message numbers for FIT protocol.
    /// </summary>
    protected static class GlobalMessageNumber
    {
        public const ushort FileId = 0;
        public const ushort DeviceInfo = 23;
        public const ushort WeightScale = 30;
        public const ushort FileCreator = 49;
        public const ushort BloodPressure = 51;
    }

    /// <summary>
    /// Local message types.
    /// </summary>
    protected const int LMsgTypeFileInfo = 0;
    protected const int LMsgTypeFileCreator = 1;
    protected const int LMsgTypeDeviceInfo = 2;

    /// <summary>
    /// File type for weight scale files.
    /// </summary>
    protected const int FileTypeWeight = 9;

    private readonly MemoryStream _buffer;
    private bool _deviceInfoDefined;
    private bool _disposed;

    /// <summary>
    /// Creates a new FIT encoder.
    /// </summary>
    public FitEncoder()
    {
        _buffer = new MemoryStream();
        WriteHeader();
    }

    /// <summary>
    /// Writes the FIT file header.
    /// </summary>
    protected void WriteHeader(int dataSize = 0)
    {
        _buffer.Seek(0, SeekOrigin.Begin);

        // Header size (1 byte)
        _buffer.WriteByte(HeaderSize);

        // Protocol version (1 byte) - 1.0
        _buffer.WriteByte(16);

        // Profile version (2 bytes) - little endian
        WriteUInt16(108);

        // Data size (4 bytes) - little endian
        WriteUInt32((uint)dataSize);

        // Data type signature ".FIT" (4 bytes)
        _buffer.Write(".FIT"u8);
    }

    /// <summary>
    /// Writes the file info record.
    /// </summary>
    public void WriteFileInfo(
        uint? serialNumber = null,
        DateTime? timeCreated = null,
        ushort? manufacturer = null,
        ushort? product = null,
        ushort? number = null)
    {
        var timestamp = timeCreated ?? DateTime.Now;

        var content = new (int Num, FitType Type, object? Value, int? Scale)[]
        {
            (3, FitBaseType.UInt32z, serialNumber, null),
            (4, FitBaseType.UInt32, ToFitTimestamp(timestamp), null),
            (1, FitBaseType.UInt16, manufacturer, null),
            (2, FitBaseType.UInt16, product, null),
            (5, FitBaseType.UInt16, number, null),
            (0, FitBaseType.Enum, (byte)FileTypeWeight, null)
        };

        var (fields, values) = BuildContentBlock(content);

        // Definition message
        WriteRecordHeader(definition: true, lmsgType: LMsgTypeFileInfo);
        _buffer.WriteByte(0); // Reserved
        _buffer.WriteByte(0); // Architecture: 0 = little endian
        WriteUInt16(GlobalMessageNumber.FileId);
        _buffer.WriteByte((byte)content.Length);
        _buffer.Write(fields);

        // Data record
        WriteRecordHeader(definition: false, lmsgType: LMsgTypeFileInfo);
        _buffer.Write(values);
    }

    /// <summary>
    /// Writes the file creator record.
    /// </summary>
    public void WriteFileCreator(ushort? softwareVersion = null, byte? hardwareVersion = null)
    {
        var content = new (int Num, FitType Type, object? Value, int? Scale)[]
        {
            (0, FitBaseType.UInt16, softwareVersion, null),
            (1, FitBaseType.UInt8, hardwareVersion, null)
        };

        var (fields, values) = BuildContentBlock(content);

        // Definition message
        WriteRecordHeader(definition: true, lmsgType: LMsgTypeFileCreator);
        _buffer.WriteByte(0); // Reserved
        _buffer.WriteByte(0); // Architecture
        WriteUInt16(GlobalMessageNumber.FileCreator);
        _buffer.WriteByte((byte)content.Length);
        _buffer.Write(fields);

        // Data record
        WriteRecordHeader(definition: false, lmsgType: LMsgTypeFileCreator);
        _buffer.Write(values);
    }

    /// <summary>
    /// Writes device info record.
    /// </summary>
    public void WriteDeviceInfo(
        DateTime timestamp,
        uint? serialNumber = null,
        uint? cumulativeOperatingTime = null,
        ushort? manufacturer = null,
        ushort? product = null,
        ushort? softwareVersion = null,
        ushort? batteryVoltage = null,
        byte? deviceIndex = null,
        byte? deviceType = null,
        byte? hardwareVersion = null,
        byte? batteryStatus = null)
    {
        var content = new (int Num, FitType Type, object? Value, int? Scale)[]
        {
            (253, FitBaseType.UInt32, ToFitTimestamp(timestamp), 1),
            (3, FitBaseType.UInt32z, serialNumber, 1),
            (7, FitBaseType.UInt32, cumulativeOperatingTime, 1),
            (8, FitBaseType.UInt32, null, null), // Unknown field (undocumented)
            (2, FitBaseType.UInt16, manufacturer, 1),
            (4, FitBaseType.UInt16, product, 1),
            (5, FitBaseType.UInt16, softwareVersion, 100),
            (10, FitBaseType.UInt16, batteryVoltage, 256),
            (0, FitBaseType.UInt8, deviceIndex, 1),
            (1, FitBaseType.UInt8, deviceType, 1),
            (6, FitBaseType.UInt8, hardwareVersion, 1),
            (11, FitBaseType.UInt8, batteryStatus, null)
        };

        var (fields, values) = BuildContentBlock(content);

        if (!_deviceInfoDefined)
        {
            WriteRecordHeader(definition: true, lmsgType: LMsgTypeDeviceInfo);
            _buffer.WriteByte(0); // Reserved
            _buffer.WriteByte(0); // Architecture
            WriteUInt16(GlobalMessageNumber.DeviceInfo);
            _buffer.WriteByte((byte)content.Length);
            _buffer.Write(fields);
            _deviceInfoDefined = true;
        }

        WriteRecordHeader(definition: false, lmsgType: LMsgTypeDeviceInfo);
        _buffer.Write(values);
    }

    /// <summary>
    /// Builds field definitions and values from content specification.
    /// </summary>
    private protected static (byte[] Fields, byte[] Values) BuildContentBlock(
        (int Num, FitType Type, object? Value, int? Scale)[] content)
    {
        using var fieldsStream = new MemoryStream();
        using var valuesStream = new MemoryStream();

        foreach (var (num, type, value, scale) in content)
        {
            // Field definition: field number, size, base type field
            fieldsStream.WriteByte((byte)num);
            fieldsStream.WriteByte((byte)type.Size);
            fieldsStream.WriteByte(type.Field);

            // Value
            object? finalValue = value;
            if (finalValue is null)
            {
                finalValue = type.Invalid;
            }
            else if (scale.HasValue && scale.Value != 1)
            {
                finalValue = Convert.ToDouble(finalValue) * scale.Value;
            }

            var packed = type.Pack(finalValue);
            valuesStream.Write(packed);
        }

        return (fieldsStream.ToArray(), valuesStream.ToArray());
    }

    /// <summary>
    /// Writes a record header byte.
    /// </summary>
    protected void WriteRecordHeader(bool definition, int lmsgType)
    {
        byte header = (byte)lmsgType;
        if (definition)
        {
            header |= 0x40; // Bit 6 set for definition message
        }
        _buffer.WriteByte(header);
    }

    /// <summary>
    /// Finishes the FIT file by rewriting the header with correct data size and appending CRC.
    /// </summary>
    public void Finish()
    {
        var dataSize = (int)_buffer.Length - HeaderSize;
        WriteHeader(dataSize);

        // Append CRC
        var crc = CalculateCrc();
        _buffer.Seek(0, SeekOrigin.End);
        WriteUInt16(crc);
    }

    /// <summary>
    /// Gets the encoded FIT file data.
    /// </summary>
    public byte[] GetBytes()
    {
        return _buffer.ToArray();
    }

    /// <summary>
    /// Converts a DateTime to FIT timestamp (seconds since FIT epoch).
    /// </summary>
    protected static uint ToFitTimestamp(DateTime dateTime)
    {
        var utc = dateTime.Kind == DateTimeKind.Utc ? dateTime : dateTime.ToUniversalTime();
        return (uint)(utc - FitEpoch).TotalSeconds;
    }

    /// <summary>
    /// Calculates the CRC checksum for the current buffer content.
    /// </summary>
    private ushort CalculateCrc()
    {
        _buffer.Seek(0, SeekOrigin.Begin);
        ushort crc = 0;

        int b;
        while ((b = _buffer.ReadByte()) != -1)
        {
            crc = CalcCrc(crc, (byte)b);
        }

        return crc;
    }

    /// <summary>
    /// CRC calculation for a single byte.
    /// </summary>
    private static ushort CalcCrc(ushort crc, byte b)
    {
        // Compute checksum of lower four bits
        var tmp = CrcTable[crc & 0xF];
        crc = (ushort)((crc >> 4) & 0x0FFF);
        crc = (ushort)(crc ^ tmp ^ CrcTable[b & 0xF]);

        // Compute checksum of upper four bits
        tmp = CrcTable[crc & 0xF];
        crc = (ushort)((crc >> 4) & 0x0FFF);
        crc = (ushort)(crc ^ tmp ^ CrcTable[(b >> 4) & 0xF]);

        return crc;
    }

    /// <summary>
    /// Writes a 16-bit unsigned integer in little-endian format.
    /// </summary>
    protected void WriteUInt16(ushort value)
    {
        Span<byte> buffer = stackalloc byte[2];
        BinaryPrimitives.WriteUInt16LittleEndian(buffer, value);
        _buffer.Write(buffer);
    }

    /// <summary>
    /// Writes a 32-bit unsigned integer in little-endian format.
    /// </summary>
    protected void WriteUInt32(uint value)
    {
        Span<byte> buffer = stackalloc byte[4];
        BinaryPrimitives.WriteUInt32LittleEndian(buffer, value);
        _buffer.Write(buffer);
    }

    /// <summary>
    /// Gets the underlying buffer for derived classes.
    /// </summary>
    protected MemoryStream Buffer => _buffer;

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed) return;
        _buffer.Dispose();
        _disposed = true;
        GC.SuppressFinalize(this);
    }
}
