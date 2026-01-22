using FluentAssertions;
using GarminConnect.Auth;

namespace GarminConnect.Tests.Auth;

public class AuthResultTests
{
    [Fact]
    public void Succeeded_ReturnsSuccessfulResult()
    {
        // Act
        var result = AuthResult.Succeeded();

        // Assert
        result.Success.Should().BeTrue();
        result.RequiresMfa.Should().BeFalse();
        result.ErrorMessage.Should().BeNull();
        result.MfaClientState.Should().BeNull();
    }

    [Fact]
    public void Failed_ReturnsFailedResult_WithErrorMessage()
    {
        // Arrange
        var errorMessage = "Invalid credentials";

        // Act
        var result = AuthResult.Failed(errorMessage);

        // Assert
        result.Success.Should().BeFalse();
        result.RequiresMfa.Should().BeFalse();
        result.ErrorMessage.Should().Be(errorMessage);
        result.MfaClientState.Should().BeNull();
    }

    [Fact]
    public void MfaRequired_ReturnsResultWithMfaFlag()
    {
        // Arrange
        var clientState = "test-state-123";

        // Act
        var result = AuthResult.MfaRequired(clientState);

        // Assert
        result.Success.Should().BeFalse();
        result.RequiresMfa.Should().BeTrue();
        result.MfaClientState.Should().Be(clientState);
        result.ErrorMessage.Should().BeNull();
    }
}
