namespace GarminConnect.Exceptions;

/// <summary>
/// Exception thrown when authentication with Garmin Connect fails.
/// </summary>
public class GarminConnectAuthenticationException : GarminConnectException
{
    public GarminConnectAuthenticationException(string message)
        : base(message, 401)
    {
    }

    public GarminConnectAuthenticationException(string message, Exception innerException)
        : base(message, 401, innerException)
    {
    }
}
