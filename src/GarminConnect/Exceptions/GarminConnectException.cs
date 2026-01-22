namespace GarminConnect.Exceptions;

/// <summary>
/// Base exception for all Garmin Connect errors.
/// </summary>
public class GarminConnectException : Exception
{
    /// <summary>
    /// HTTP status code if applicable.
    /// </summary>
    public int? StatusCode { get; }

    /// <summary>
    /// Request ID for debugging purposes.
    /// </summary>
    public string? RequestId { get; }

    public GarminConnectException(string message)
        : base(message)
    {
    }

    public GarminConnectException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public GarminConnectException(string message, int statusCode, string? requestId = null)
        : base(message)
    {
        StatusCode = statusCode;
        RequestId = requestId;
    }

    public GarminConnectException(string message, int statusCode, Exception innerException)
        : base(message, innerException)
    {
        StatusCode = statusCode;
    }
}
