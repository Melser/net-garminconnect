using FluentAssertions;
using GarminConnect.Auth.OAuth;

namespace GarminConnect.Tests.Auth;

public class GarminConnectTokensTests
{
    private static OAuth1Token CreateOAuth1Token() => new()
    {
        Token = "oauth1-token",
        TokenSecret = "oauth1-secret"
    };

    private static OAuth2Token CreateOAuth2Token(DateTimeOffset? expiresAt = null) => new()
    {
        AccessToken = "access-token",
        RefreshToken = "refresh-token",
        ExpiresAt = expiresAt ?? DateTimeOffset.UtcNow.AddHours(1)
    };

    [Fact]
    public void IsValid_ReturnsTrue_WhenBothTokensAreValidAndNotExpired()
    {
        // Arrange
        var tokens = new GarminConnectTokens
        {
            OAuth1 = CreateOAuth1Token(),
            OAuth2 = CreateOAuth2Token()
        };

        // Act & Assert
        tokens.IsValid.Should().BeTrue();
    }

    [Fact]
    public void IsValid_ReturnsFalse_WhenOAuth1IsNull()
    {
        // Arrange
        var tokens = new GarminConnectTokens
        {
            OAuth1 = null,
            OAuth2 = CreateOAuth2Token()
        };

        // Act & Assert
        tokens.IsValid.Should().BeFalse();
    }

    [Fact]
    public void IsValid_ReturnsFalse_WhenOAuth2IsNull()
    {
        // Arrange
        var tokens = new GarminConnectTokens
        {
            OAuth1 = CreateOAuth1Token(),
            OAuth2 = null
        };

        // Act & Assert
        tokens.IsValid.Should().BeFalse();
    }

    [Fact]
    public void IsValid_ReturnsFalse_WhenOAuth2IsExpired()
    {
        // Arrange
        var tokens = new GarminConnectTokens
        {
            OAuth1 = CreateOAuth1Token(),
            OAuth2 = CreateOAuth2Token(DateTimeOffset.UtcNow.AddMinutes(-5))
        };

        // Act & Assert
        tokens.IsValid.Should().BeFalse();
    }

    [Fact]
    public void CanRefresh_ReturnsTrue_WhenOAuth1ExistsAndRefreshTokenNotExpired()
    {
        // Arrange
        var tokens = new GarminConnectTokens
        {
            OAuth1 = CreateOAuth1Token(),
            OAuth2 = CreateOAuth2Token(DateTimeOffset.UtcNow.AddMinutes(-5)) // Expired but can refresh
        };

        // Act & Assert
        tokens.CanRefresh.Should().BeTrue();
    }

    [Fact]
    public void CanRefresh_ReturnsFalse_WhenOAuth1IsNull()
    {
        // Arrange
        var tokens = new GarminConnectTokens
        {
            OAuth1 = null,
            OAuth2 = CreateOAuth2Token()
        };

        // Act & Assert
        tokens.CanRefresh.Should().BeFalse();
    }

    [Fact]
    public void AccessToken_ReturnsOAuth2AccessToken()
    {
        // Arrange
        var tokens = new GarminConnectTokens
        {
            OAuth1 = CreateOAuth1Token(),
            OAuth2 = CreateOAuth2Token()
        };

        // Act & Assert
        tokens.AccessToken.Should().Be("access-token");
    }

    [Fact]
    public void AccessToken_ReturnsNull_WhenOAuth2IsNull()
    {
        // Arrange
        var tokens = new GarminConnectTokens
        {
            OAuth1 = CreateOAuth1Token(),
            OAuth2 = null
        };

        // Act & Assert
        tokens.AccessToken.Should().BeNull();
    }
}
