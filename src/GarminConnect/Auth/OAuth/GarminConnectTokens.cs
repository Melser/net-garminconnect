using System.Text.Json.Serialization;

namespace GarminConnect.Auth.OAuth;

/// <summary>
/// Combined OAuth tokens for Garmin Connect authentication.
/// </summary>
public sealed record GarminConnectTokens
{
    /// <summary>
    /// OAuth 1.0a token (long-lived, ~1 year).
    /// Required for refreshing OAuth2 tokens.
    /// </summary>
    [JsonPropertyName("oauth1")]
    public OAuth1Token? OAuth1 { get; init; }

    /// <summary>
    /// OAuth 2.0 token (short-lived, typically 1 hour).
    /// Used for API requests.
    /// </summary>
    [JsonPropertyName("oauth2")]
    public OAuth2Token? OAuth2 { get; init; }

    /// <summary>
    /// Timestamp when tokens were last updated.
    /// </summary>
    [JsonPropertyName("updated_at")]
    public DateTimeOffset UpdatedAt { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Checks if both tokens are present and valid.
    /// </summary>
    [JsonIgnore]
    public bool IsValid => OAuth1 is not null && OAuth2 is not null && !OAuth2.IsExpired;

    /// <summary>
    /// Checks if tokens can be refreshed (OAuth1 must be present).
    /// </summary>
    [JsonIgnore]
    public bool CanRefresh => OAuth1 is not null && OAuth2 is not null && !OAuth2.IsRefreshTokenExpired;

    /// <summary>
    /// Gets the access token for API requests.
    /// </summary>
    [JsonIgnore]
    public string? AccessToken => OAuth2?.AccessToken;
}
