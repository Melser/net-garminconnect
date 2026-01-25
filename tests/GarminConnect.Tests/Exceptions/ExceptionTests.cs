using FluentAssertions;
using GarminConnect.Exceptions;

namespace GarminConnect.Tests.Exceptions;

public class GarminConnectExceptionTests
{
    [Fact]
    public void Constructor_WithMessage_SetsMessage()
    {
        // Arrange & Act
        var exception = new GarminConnectException("Test error");

        // Assert
        exception.Message.Should().Be("Test error");
        exception.StatusCode.Should().BeNull();
        exception.RequestId.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithMessageAndStatusCode_SetsBothProperties()
    {
        // Arrange & Act
        var exception = new GarminConnectException("Test error", 404);

        // Assert
        exception.Message.Should().Be("Test error");
        exception.StatusCode.Should().Be(404);
    }

    [Fact]
    public void Constructor_WithMessageStatusCodeAndRequestId_SetsAllProperties()
    {
        // Arrange & Act
        var exception = new GarminConnectException("Test error", 500, "req-123");

        // Assert
        exception.Message.Should().Be("Test error");
        exception.StatusCode.Should().Be(500);
        exception.RequestId.Should().Be("req-123");
    }

    [Fact]
    public void Constructor_WithInnerException_SetsInnerException()
    {
        // Arrange
        var inner = new InvalidOperationException("Inner error");

        // Act
        var exception = new GarminConnectException("Outer error", inner);

        // Assert
        exception.Message.Should().Be("Outer error");
        exception.InnerException.Should().Be(inner);
    }
}

public class GarminConnectAuthenticationExceptionTests
{
    [Fact]
    public void Constructor_SetsStatusCodeTo401()
    {
        // Arrange & Act
        var exception = new GarminConnectAuthenticationException("Auth failed");

        // Assert
        exception.StatusCode.Should().Be(401);
    }

    [Fact]
    public void IsGarminConnectException()
    {
        // Arrange & Act
        var exception = new GarminConnectAuthenticationException("Auth failed");

        // Assert
        exception.Should().BeAssignableTo<GarminConnectException>();
    }
}

public class GarminConnectTooManyRequestsExceptionTests
{
    [Fact]
    public void Constructor_SetsStatusCodeTo429()
    {
        // Arrange & Act
        var exception = new GarminConnectTooManyRequestsException("Rate limited");

        // Assert
        exception.StatusCode.Should().Be(429);
    }

    [Fact]
    public void Constructor_WithRetryAfter_SetsRetryAfterProperty()
    {
        // Arrange
        var retryAfter = TimeSpan.FromMinutes(5);

        // Act
        var exception = new GarminConnectTooManyRequestsException("Rate limited", retryAfter);

        // Assert
        exception.RetryAfter.Should().Be(retryAfter);
    }

    [Fact]
    public void RetryAfter_IsNull_WhenNotProvided()
    {
        // Arrange & Act
        var exception = new GarminConnectTooManyRequestsException("Rate limited");

        // Assert
        exception.RetryAfter.Should().BeNull();
    }
}

public class GarminConnectConnectionExceptionTests
{
    [Fact]
    public void Constructor_WithMessage_SetsMessage()
    {
        // Arrange & Act
        var exception = new GarminConnectConnectionException("Connection failed");

        // Assert
        exception.Message.Should().Be("Connection failed");
    }

    [Fact]
    public void Constructor_WithStatusCode_SetsStatusCode()
    {
        // Arrange & Act
        var exception = new GarminConnectConnectionException("Server error", 503);

        // Assert
        exception.StatusCode.Should().Be(503);
    }
}

public class GarminConnectInvalidFileFormatExceptionTests
{
    [Fact]
    public void Constructor_SetsStatusCodeTo400()
    {
        // Arrange & Act
        var exception = new GarminConnectInvalidFileFormatException("Invalid file");

        // Assert
        exception.StatusCode.Should().Be(400);
    }

    [Fact]
    public void Constructor_WithFileName_SetsFileNameProperty()
    {
        // Arrange & Act
        var exception = new GarminConnectInvalidFileFormatException("Invalid file", "activity.xyz");

        // Assert
        exception.FileName.Should().Be("activity.xyz");
    }

    [Fact]
    public void ExpectedFormats_ContainsSupportedFormats()
    {
        // Arrange & Act
        var exception = new GarminConnectInvalidFileFormatException("Invalid file");

        // Assert
        exception.ExpectedFormats.Should().Contain("FIT");
        exception.ExpectedFormats.Should().Contain("GPX");
        exception.ExpectedFormats.Should().Contain("TCX");
    }
}
