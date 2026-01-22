namespace GarminConnect.Exceptions;

/// <summary>
/// Exception thrown when there is a connection error with Garmin Connect API.
/// </summary>
public class GarminConnectConnectionException : GarminConnectException
{
    public GarminConnectConnectionException(string message)
        : base(message)
    {
    }

    public GarminConnectConnectionException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public GarminConnectConnectionException(string message, int statusCode)
        : base(message, statusCode)
    {
    }
}
