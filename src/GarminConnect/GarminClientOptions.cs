namespace GarminConnect;

/// <summary>
/// Configuration options for <see cref="GarminClient"/>.
/// </summary>
public sealed class GarminClientOptions
{
    /// <summary>
    /// Base URL for Garmin Connect API.
    /// Default: https://connect.garmin.com
    /// </summary>
    public string BaseUrl { get; set; } = "https://connect.garmin.com";

    /// <summary>
    /// Path to the file where authentication tokens will be stored.
    /// If null, tokens will not be persisted.
    /// Default: null
    /// </summary>
    public string? TokenStorePath { get; set; }

    /// <summary>
    /// HTTP request timeout.
    /// Default: 30 seconds
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Maximum number of retry attempts for transient failures.
    /// Default: 3
    /// </summary>
    public int MaxRetryAttempts { get; set; } = 3;

    /// <summary>
    /// Delay between retry attempts. Will be multiplied by attempt number for exponential backoff.
    /// Default: 1 second
    /// </summary>
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(1);

    /// <summary>
    /// Whether to enable automatic token refresh when token is expired.
    /// Default: true
    /// </summary>
    public bool AutoRefreshToken { get; set; } = true;

    /// <summary>
    /// User-Agent string to use for HTTP requests.
    /// Default: GCM-iOS-5.7.2.1 (matches official Garmin app)
    /// </summary>
    public string UserAgent { get; set; } = "GCM-iOS-5.7.2.1";
}
