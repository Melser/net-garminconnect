using FluentAssertions;
using GarminConnect.Auth.OAuth;

namespace GarminConnect.Tests.Auth;

public class OAuth2TokenTests
{
    [Fact]
    public void IsExpired_ReturnsFalse_WhenTokenIsValid()
    {
        // Arrange
        var token = new OAuth2Token
        {
            AccessToken = "test-access-token",
            RefreshToken = "test-refresh-token",
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(1)
        };

        // Act & Assert
        token.IsExpired.Should().BeFalse();
    }

    [Fact]
    public void IsExpired_ReturnsTrue_WhenTokenIsExpired()
    {
        // Arrange
        var token = new OAuth2Token
        {
            AccessToken = "test-access-token",
            RefreshToken = "test-refresh-token",
            ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(-5)
        };

        // Act & Assert
        token.IsExpired.Should().BeTrue();
    }

    [Fact]
    public void IsExpired_ReturnsTrue_WhenTokenExpiresWithinOneMinute()
    {
        // Arrange
        var token = new OAuth2Token
        {
            AccessToken = "test-access-token",
            RefreshToken = "test-refresh-token",
            ExpiresAt = DateTimeOffset.UtcNow.AddSeconds(30)
        };

        // Act & Assert
        token.IsExpired.Should().BeTrue();
    }

    [Fact]
    public void IsRefreshTokenExpired_ReturnsFalse_WhenNoExpirationSet()
    {
        // Arrange
        var token = new OAuth2Token
        {
            AccessToken = "test-access-token",
            RefreshToken = "test-refresh-token",
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(1),
            RefreshTokenExpiresAt = null
        };

        // Act & Assert
        token.IsRefreshTokenExpired.Should().BeFalse();
    }

    [Fact]
    public void IsRefreshTokenExpired_ReturnsTrue_WhenRefreshTokenExpired()
    {
        // Arrange
        var token = new OAuth2Token
        {
            AccessToken = "test-access-token",
            RefreshToken = "test-refresh-token",
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(1),
            RefreshTokenExpiresAt = DateTimeOffset.UtcNow.AddDays(-1)
        };

        // Act & Assert
        token.IsRefreshTokenExpired.Should().BeTrue();
    }

    [Fact]
    public void TokenType_DefaultsToBearer()
    {
        // Arrange
        var token = new OAuth2Token
        {
            AccessToken = "test-access-token",
            RefreshToken = "test-refresh-token",
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(1)
        };

        // Act & Assert
        token.TokenType.Should().Be("Bearer");
    }
}
