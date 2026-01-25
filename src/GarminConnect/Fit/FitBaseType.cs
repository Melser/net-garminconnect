namespace GarminConnect.Fit;

/// <summary>
/// FIT Protocol base type definitions.
/// See FIT Protocol Document (Page 20).
/// </summary>
internal static class FitBaseType
{
    public static readonly FitType Enum = new(0, false, 0x00, 0xFF, 1, "B");
    public static readonly FitType SInt8 = new(1, false, 0x01, 0x7F, 1, "b");
    public static readonly FitType UInt8 = new(2, false, 0x02, 0xFF, 1, "B");
    public static readonly FitType SInt16 = new(3, true, 0x83, 0x7FFF, 2, "h");
    public static readonly FitType UInt16 = new(4, true, 0x84, 0xFFFF, 2, "H");
    public static readonly FitType SInt32 = new(5, true, 0x85, 0x7FFFFFFF, 4, "i");
    public static readonly FitType UInt32 = new(6, true, 0x86, 0xFFFFFFFF, 4, "I");
    public static readonly FitType String = new(7, false, 0x07, 0x00, 1, "s");
    public static readonly FitType Float32 = new(8, true, 0x88, 0xFFFFFFFF, 4, "f");
    public static readonly FitType Float64 = new(9, true, 0x89, 0xFFFFFFFFFFFFFFFF, 8, "d");
    public static readonly FitType UInt8z = new(10, false, 0x0A, 0x00, 1, "B");
    public static readonly FitType UInt16z = new(11, true, 0x8B, 0x0000, 2, "H");
    public static readonly FitType UInt32z = new(12, true, 0x8C, 0x00000000, 4, "I");
    public static readonly FitType Byte = new(13, false, 0x0D, 0xFF, 1, "c");
}

/// <summary>
/// Represents a FIT base type with its properties.
/// </summary>
internal readonly record struct FitType(
    int Number,
    bool HasEndian,
    byte Field,
    ulong Invalid,
    int Size,
    string Format)
{
    /// <summary>
    /// Packs a value according to this base type.
    /// </summary>
    public byte[] Pack(object? value)
    {
        if (value is null)
        {
            value = Invalid;
        }

        return Number switch
        {
            0 or 2 or 10 or 13 => [(byte)Convert.ToUInt64(value)],
            1 => [(byte)(sbyte)Convert.ToInt64(value)],
            3 => BitConverter.GetBytes((short)Convert.ToInt64(value)),
            4 or 11 => BitConverter.GetBytes((ushort)Convert.ToUInt64(value)),
            5 => BitConverter.GetBytes((int)Convert.ToInt64(value)),
            6 or 12 => BitConverter.GetBytes((uint)Convert.ToUInt64(value)),
            7 => [(byte)Convert.ToUInt64(value)],
            8 => BitConverter.GetBytes((float)Convert.ToDouble(value)),
            9 => BitConverter.GetBytes(Convert.ToDouble(value)),
            _ => throw new ArgumentOutOfRangeException(nameof(Number))
        };
    }
}
