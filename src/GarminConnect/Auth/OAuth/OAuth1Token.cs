using System.Text.Json.Serialization;

namespace GarminConnect.Auth.OAuth;

/// <summary>
/// OAuth 1.0a token for Garmin Connect (long-lived, ~1 year validity).
/// </summary>
public sealed record OAuth1Token
{
    /// <summary>
    /// OAuth token value.
    /// </summary>
    [JsonPropertyName("oauth_token")]
    public required string Token { get; init; }

    /// <summary>
    /// OAuth token secret.
    /// </summary>
    [JsonPropertyName("oauth_token_secret")]
    public required string TokenSecret { get; init; }

    /// <summary>
    /// MFA token (if MFA was used during authentication).
    /// </summary>
    [JsonPropertyName("mfa_token")]
    public string? MfaToken { get; init; }

    /// <summary>
    /// MFA expiration timestamp.
    /// </summary>
    [JsonPropertyName("mfa_expiration_timestamp")]
    public DateTimeOffset? MfaExpirationTimestamp { get; init; }

    /// <summary>
    /// Domain used for authentication (garmin.com or garmin.cn).
    /// </summary>
    [JsonPropertyName("domain")]
    public string Domain { get; init; } = "garmin.com";
}
