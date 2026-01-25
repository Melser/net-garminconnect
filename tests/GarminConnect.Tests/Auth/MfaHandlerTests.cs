using FluentAssertions;
using GarminConnect.Auth.Mfa;

namespace GarminConnect.Tests.Auth;

public class CallbackMfaHandlerTests
{
    [Fact]
    public async Task GetMfaCodeAsync_ReturnsCodeFromAsyncCallback()
    {
        // Arrange
        var handler = new CallbackMfaHandler(ct => Task.FromResult<string?>("123456"));

        // Act
        var result = await handler.GetMfaCodeAsync();

        // Assert
        result.Should().Be("123456");
    }

    [Fact]
    public async Task GetMfaCodeAsync_ReturnsCodeFromSyncCallback()
    {
        // Arrange
        var handler = new CallbackMfaHandler(() => "654321");

        // Act
        var result = await handler.GetMfaCodeAsync();

        // Assert
        result.Should().Be("654321");
    }

    [Fact]
    public async Task GetMfaCodeAsync_ReturnsNull_WhenCallbackReturnsNull()
    {
        // Arrange
        var handler = new CallbackMfaHandler(() => null);

        // Act
        var result = await handler.GetMfaCodeAsync();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetMfaCodeAsync_PassesCancellationToken()
    {
        // Arrange
        CancellationToken receivedToken = default;
        var handler = new CallbackMfaHandler(ct =>
        {
            receivedToken = ct;
            return Task.FromResult<string?>("123456");
        });

        using var cts = new CancellationTokenSource();

        // Act
        await handler.GetMfaCodeAsync(cts.Token);

        // Assert
        receivedToken.Should().Be(cts.Token);
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenAsyncCallbackIsNull()
    {
        // Act
        var act = () => new CallbackMfaHandler((Func<CancellationToken, Task<string?>>)null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenSyncCallbackIsNull()
    {
        // Act
        var act = () => new CallbackMfaHandler((Func<string?>)null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }
}

public class ConsoleMfaHandlerTests
{
    [Fact]
    public void Constructor_WithDefaultPrompt_DoesNotThrow()
    {
        // Act
        var act = () => new ConsoleMfaHandler();

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Constructor_WithCustomPrompt_DoesNotThrow()
    {
        // Act
        var act = () => new ConsoleMfaHandler("Custom prompt: ");

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenPromptIsNull()
    {
        // Act
        var act = () => new ConsoleMfaHandler(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }
}
