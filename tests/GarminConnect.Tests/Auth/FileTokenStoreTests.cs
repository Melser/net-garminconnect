using FluentAssertions;
using GarminConnect.Auth.OAuth;
using Microsoft.Extensions.Logging;
using Moq;

namespace GarminConnect.Tests.Auth;

public class FileTokenStoreTests : IDisposable
{
    private readonly string _testDirectory;
    private readonly FileTokenStore _store;

    public FileTokenStoreTests()
    {
        _testDirectory = Path.Combine(Path.GetTempPath(), $"garmin-test-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_testDirectory);
        _store = new FileTokenStore(_testDirectory);
    }

    public void Dispose()
    {
        _store.Dispose();
        if (Directory.Exists(_testDirectory))
        {
            Directory.Delete(_testDirectory, recursive: true);
        }
    }

    private static GarminConnectTokens CreateTestTokens() => new()
    {
        OAuth1 = new OAuth1Token
        {
            Token = "oauth1-token",
            TokenSecret = "oauth1-secret"
        },
        OAuth2 = new OAuth2Token
        {
            AccessToken = "access-token",
            RefreshToken = "refresh-token",
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(1)
        }
    };

    [Fact]
    public async Task SaveAsync_CreatesTokenFile()
    {
        // Arrange
        var tokens = CreateTestTokens();

        // Act
        await _store.SaveAsync(tokens);

        // Assert
        File.Exists(_store.TokenFilePath).Should().BeTrue();
    }

    [Fact]
    public async Task LoadAsync_ReturnsNull_WhenNoTokensStored()
    {
        // Act
        var result = await _store.LoadAsync();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task LoadAsync_ReturnsTokens_AfterSave()
    {
        // Arrange
        var tokens = CreateTestTokens();
        await _store.SaveAsync(tokens);

        // Act
        var result = await _store.LoadAsync();

        // Assert
        result.Should().NotBeNull();
        result!.OAuth1.Should().NotBeNull();
        result.OAuth1!.Token.Should().Be("oauth1-token");
        result.OAuth1.TokenSecret.Should().Be("oauth1-secret");
        result.OAuth2.Should().NotBeNull();
        result.OAuth2!.AccessToken.Should().Be("access-token");
        result.OAuth2.RefreshToken.Should().Be("refresh-token");
    }

    [Fact]
    public async Task ClearAsync_RemovesTokenFile()
    {
        // Arrange
        var tokens = CreateTestTokens();
        await _store.SaveAsync(tokens);
        File.Exists(_store.TokenFilePath).Should().BeTrue();

        // Act
        await _store.ClearAsync();

        // Assert
        File.Exists(_store.TokenFilePath).Should().BeFalse();
    }

    [Fact]
    public async Task ExistsAsync_ReturnsFalse_WhenNoTokens()
    {
        // Act
        var exists = await _store.ExistsAsync();

        // Assert
        exists.Should().BeFalse();
    }

    [Fact]
    public async Task ExistsAsync_ReturnsTrue_AfterSave()
    {
        // Arrange
        await _store.SaveAsync(CreateTestTokens());

        // Act
        var exists = await _store.ExistsAsync();

        // Assert
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task SaveAsync_UpdatesTimestamp()
    {
        // Arrange
        var tokens = CreateTestTokens();
        var beforeSave = DateTimeOffset.UtcNow;

        // Act
        await _store.SaveAsync(tokens);
        var result = await _store.LoadAsync();

        // Assert
        result!.UpdatedAt.Should().BeOnOrAfter(beforeSave);
    }

    [Fact]
    public async Task LoadAsync_ReturnsNull_ForInvalidJson()
    {
        // Arrange
        await File.WriteAllTextAsync(_store.TokenFilePath, "not valid json");

        // Act
        var result = await _store.LoadAsync();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task LoadAsync_ReturnsNull_ForEmptyFile()
    {
        // Arrange
        await File.WriteAllTextAsync(_store.TokenFilePath, "");

        // Act
        var result = await _store.LoadAsync();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void TokenFilePath_EndsWithTokensJson()
    {
        // Assert
        _store.TokenFilePath.Should().EndWith("tokens.json");
    }

    [Fact]
    public void Constructor_WithNullPath_UsesDefaultLocation()
    {
        // Arrange
        using var store = new FileTokenStore(null);

        // Assert
        store.TokenFilePath.Should().Contain(".garminconnect");
        store.TokenFilePath.Should().EndWith("tokens.json");
    }

    [Fact]
    public async Task LoadAsync_ThrowsObjectDisposedException_AfterDispose()
    {
        // Arrange
        var store = new FileTokenStore(_testDirectory);
        store.Dispose();

        // Act & Assert
        await Assert.ThrowsAsync<ObjectDisposedException>(() => store.LoadAsync());
    }

    [Fact]
    public async Task SaveAsync_ThrowsObjectDisposedException_AfterDispose()
    {
        // Arrange
        var store = new FileTokenStore(_testDirectory);
        store.Dispose();

        // Act & Assert
        await Assert.ThrowsAsync<ObjectDisposedException>(() => store.SaveAsync(CreateTestTokens()));
    }

    [Fact]
    public async Task ClearAsync_ThrowsObjectDisposedException_AfterDispose()
    {
        // Arrange
        var store = new FileTokenStore(_testDirectory);
        store.Dispose();

        // Act & Assert
        await Assert.ThrowsAsync<ObjectDisposedException>(() => store.ClearAsync());
    }

    [Fact]
    public async Task ExistsAsync_ThrowsObjectDisposedException_AfterDispose()
    {
        // Arrange
        var store = new FileTokenStore(_testDirectory);
        store.Dispose();

        // Act & Assert
        await Assert.ThrowsAsync<ObjectDisposedException>(async () => await store.ExistsAsync());
    }

    [Fact]
    public async Task LoadAsync_LogsWarning_ForInvalidJson()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<FileTokenStore>>();
        using var store = new FileTokenStore(_testDirectory, mockLogger.Object);
        await File.WriteAllTextAsync(store.TokenFilePath, "not valid json");

        // Act
        var result = await store.LoadAsync();

        // Assert
        result.Should().BeNull();
        mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("deserialize")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task ConcurrentSaveAndLoad_IsThreadSafe()
    {
        // Arrange
        var tasks = new List<Task>();

        // Act - run multiple save/load operations concurrently
        for (int i = 0; i < 10; i++)
        {
            tasks.Add(Task.Run(async () =>
            {
                await _store.SaveAsync(CreateTestTokens());
                await _store.LoadAsync();
            }));
        }

        // Assert - should complete without exceptions
        await Task.WhenAll(tasks);
    }

    [Fact]
    public async Task CancellationToken_IsPropagated()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert - TaskCanceledException inherits from OperationCanceledException
        await Assert.ThrowsAsync<TaskCanceledException>(() => _store.SaveAsync(CreateTestTokens(), cts.Token));
    }
}
