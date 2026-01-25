namespace GarminConnect.Exceptions;

/// <summary>
/// Exception thrown when an invalid file format is provided for upload.
/// </summary>
public class GarminConnectInvalidFileFormatException : GarminConnectException
{
    /// <summary>
    /// Name of the file that caused the error.
    /// </summary>
    public string? FileName { get; }

    /// <summary>
    /// List of expected/supported formats.
    /// </summary>
    public string ExpectedFormats { get; } = "FIT, GPX, TCX";

    public GarminConnectInvalidFileFormatException(string message, string? fileName = null)
        : base(message, 400)
    {
        FileName = fileName;
    }

    public GarminConnectInvalidFileFormatException(string message, string? fileName, Exception innerException)
        : base(message, 400, innerException)
    {
        FileName = fileName;
    }
}
