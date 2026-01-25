namespace GarminConnect.Exceptions;

/// <summary>
/// Exception thrown when rate limit is exceeded (HTTP 429).
/// </summary>
public class GarminConnectTooManyRequestsException : GarminConnectException
{
    /// <summary>
    /// Time to wait before retrying, if provided by the server.
    /// </summary>
    public TimeSpan? RetryAfter { get; }

    public GarminConnectTooManyRequestsException(string message, TimeSpan? retryAfter = null)
        : base(message, 429)
    {
        RetryAfter = retryAfter;
    }

    public GarminConnectTooManyRequestsException(string message, TimeSpan? retryAfter, Exception innerException)
        : base(message, 429, innerException)
    {
        RetryAfter = retryAfter;
    }
}
