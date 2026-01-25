using System.Text.Json.Serialization;

namespace GarminConnect.Auth.OAuth;

/// <summary>
/// OAuth 2.0 token for Garmin Connect API (short-lived, typically 1 hour).
/// </summary>
public sealed record OAuth2Token
{
    /// <summary>
    /// Access token for API requests.
    /// </summary>
    [JsonPropertyName("access_token")]
    public required string AccessToken { get; init; }

    /// <summary>
    /// Refresh token for obtaining new access tokens.
    /// </summary>
    [JsonPropertyName("refresh_token")]
    public required string RefreshToken { get; init; }

    /// <summary>
    /// Token type (usually "Bearer").
    /// </summary>
    [JsonPropertyName("token_type")]
    public string TokenType { get; init; } = "Bearer";

    /// <summary>
    /// Token scope.
    /// </summary>
    [JsonPropertyName("scope")]
    public string? Scope { get; init; }

    /// <summary>
    /// Number of seconds until the access token expires.
    /// </summary>
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; init; }

    /// <summary>
    /// Timestamp when the token expires.
    /// </summary>
    [JsonPropertyName("expires_at")]
    public DateTimeOffset ExpiresAt { get; init; }

    /// <summary>
    /// Number of seconds until the refresh token expires.
    /// </summary>
    [JsonPropertyName("refresh_token_expires_in")]
    public int? RefreshTokenExpiresIn { get; init; }

    /// <summary>
    /// Timestamp when the refresh token expires.
    /// </summary>
    [JsonPropertyName("refresh_token_expires_at")]
    public DateTimeOffset? RefreshTokenExpiresAt { get; init; }

    /// <summary>
    /// Checks if the access token is expired or about to expire (within 1 minute).
    /// </summary>
    public bool IsExpired => DateTimeOffset.UtcNow >= ExpiresAt.AddMinutes(-1);

    /// <summary>
    /// Checks if the refresh token is expired.
    /// </summary>
    public bool IsRefreshTokenExpired =>
        RefreshTokenExpiresAt.HasValue && DateTimeOffset.UtcNow >= RefreshTokenExpiresAt.Value;
}
