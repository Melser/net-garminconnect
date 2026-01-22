using FluentAssertions;
using GarminConnect.Api;
using GarminConnect.Auth;
using GarminConnect.Auth.OAuth;
using GarminConnect.Exceptions;
using GarminConnect.Models;
using Moq;

namespace GarminConnect.Tests;

public class GarminClientTests : IDisposable
{
    private readonly Mock<IGarminApiClient> _mockApiClient;
    private readonly Mock<IGarminAuthenticator> _mockAuthenticator;
    private readonly GarminClient _client;

    public GarminClientTests()
    {
        _mockApiClient = new Mock<IGarminApiClient>();
        _mockAuthenticator = new Mock<IGarminAuthenticator>();
        _client = new GarminClient(_mockApiClient.Object, _mockAuthenticator.Object);
    }

    public void Dispose()
    {
        _client.Dispose();
    }

    #region Authentication Tests

    [Fact]
    public void IsAuthenticated_ReturnsFalse_WhenNotAuthenticated()
    {
        // Arrange
        _mockAuthenticator.Setup(a => a.IsAuthenticated).Returns(false);

        // Assert
        _client.IsAuthenticated.Should().BeFalse();
    }

    [Fact]
    public void IsAuthenticated_ReturnsTrue_WhenAuthenticated()
    {
        // Arrange
        _mockAuthenticator.Setup(a => a.IsAuthenticated).Returns(true);

        // Assert
        _client.IsAuthenticated.Should().BeTrue();
    }

    [Fact]
    public async Task LoginAsync_ReturnsSuccess_WhenCredentialsValid()
    {
        // Arrange
        _mockAuthenticator
            .Setup(a => a.LoginAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(AuthResult.Succeeded());
        _mockAuthenticator.Setup(a => a.AccessToken).Returns("test-token");

        // Act
        var result = await _client.LoginAsync("test@example.com", "password");

        // Assert
        result.Success.Should().BeTrue();
        _mockApiClient.Verify(c => c.SetAccessToken("test-token"), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_ReturnsFailed_WhenCredentialsInvalid()
    {
        // Arrange
        _mockAuthenticator
            .Setup(a => a.LoginAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(AuthResult.Failed("Invalid credentials"));

        // Act
        var result = await _client.LoginAsync("test@example.com", "wrong-password");

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Invalid credentials");
        _mockApiClient.Verify(c => c.SetAccessToken(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_ReturnsMfaRequired_WhenMfaNeeded()
    {
        // Arrange
        _mockAuthenticator
            .Setup(a => a.LoginAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(AuthResult.MfaRequired("mfa-state-123"));

        // Act
        var result = await _client.LoginAsync("test@example.com", "password");

        // Assert
        result.Success.Should().BeFalse();
        result.RequiresMfa.Should().BeTrue();
        result.MfaClientState.Should().Be("mfa-state-123");
    }

    [Fact]
    public async Task ResumeSessionAsync_ReturnsTrue_WhenTokensValid()
    {
        // Arrange
        _mockAuthenticator.Setup(a => a.ResumeSessionAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _mockAuthenticator.Setup(a => a.AccessToken).Returns("resumed-token");

        // Act
        var result = await _client.ResumeSessionAsync();

        // Assert
        result.Should().BeTrue();
        _mockApiClient.Verify(c => c.SetAccessToken("resumed-token"), Times.Once);
    }

    [Fact]
    public async Task LogoutAsync_ClearsToken()
    {
        // Arrange
        _mockAuthenticator.Setup(a => a.LogoutAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        await _client.LogoutAsync();

        // Assert
        _mockApiClient.Verify(c => c.SetAccessToken(null), Times.Once);
        _mockAuthenticator.Verify(a => a.LogoutAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region User Tests

    [Fact]
    public async Task GetUserProfileAsync_ThrowsException_WhenNotAuthenticated()
    {
        // Arrange
        _mockAuthenticator.Setup(a => a.IsAuthenticated).Returns(false);

        // Act
        var act = () => _client.GetUserProfileAsync();

        // Assert
        await act.Should().ThrowAsync<GarminConnectAuthenticationException>();
    }

    [Fact]
    public async Task GetUserProfileAsync_ReturnsProfile_WhenAuthenticated()
    {
        // Arrange
        _mockAuthenticator.Setup(a => a.IsAuthenticated).Returns(true);
        var expectedProfile = new UserProfile { DisplayName = "TestUser", FirstName = "Test", LastName = "User" };
        _mockApiClient
            .Setup(c => c.GetAsync<UserProfile>(Endpoints.UserProfile, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedProfile);

        // Act
        var result = await _client.GetUserProfileAsync();

        // Assert
        result.Should().NotBeNull();
        result!.DisplayName.Should().Be("TestUser");
        result.FirstName.Should().Be("Test");
        result.LastName.Should().Be("User");
    }

    [Fact]
    public async Task GetUserSettingsAsync_CachesResult()
    {
        // Arrange
        _mockAuthenticator.Setup(a => a.IsAuthenticated).Returns(true);
        var expectedSettings = new UserSettings { DisplayName = "TestUser", MeasurementSystem = "METRIC" };
        _mockApiClient
            .Setup(c => c.GetAsync<UserSettings>(Endpoints.UserSettings, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedSettings);

        // Act
        var result1 = await _client.GetUserSettingsAsync();
        var result2 = await _client.GetUserSettingsAsync();

        // Assert
        result1.Should().Be(result2);
        _mockApiClient.Verify(c => c.GetAsync<UserSettings>(Endpoints.UserSettings, It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region Health Tests

    [Fact]
    public async Task GetDailySummaryAsync_ReturnsData()
    {
        // Arrange
        _mockAuthenticator.Setup(a => a.IsAuthenticated).Returns(true);
        var date = new DateOnly(2024, 1, 15);
        var expectedSummary = new DailySummary { CalendarDate = "2024-01-15", TotalSteps = 10000 };
        _mockApiClient
            .Setup(c => c.GetAsync<DailySummary>("/usersummary-service/usersummary/daily/2024-01-15", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedSummary);

        // Act
        var result = await _client.GetDailySummaryAsync(date);

        // Assert
        result.Should().NotBeNull();
        result!.CalendarDate.Should().Be("2024-01-15");
        result.TotalSteps.Should().Be(10000);
    }

    [Fact]
    public async Task GetHeartRatesAsync_ReturnsData()
    {
        // Arrange
        _mockAuthenticator.Setup(a => a.IsAuthenticated).Returns(true);
        var date = new DateOnly(2024, 1, 15);
        var expectedData = new HeartRateData { CalendarDate = "2024-01-15", RestingHeartRate = 62 };
        _mockApiClient
            .Setup(c => c.GetAsync<HeartRateData>("/wellness-service/wellness/dailyHeartRate/2024-01-15", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedData);

        // Act
        var result = await _client.GetHeartRatesAsync(date);

        // Assert
        result.Should().NotBeNull();
        result!.RestingHeartRate.Should().Be(62);
    }

    [Fact]
    public async Task GetRestingHeartRateAsync_ReturnsValue()
    {
        // Arrange
        _mockAuthenticator.Setup(a => a.IsAuthenticated).Returns(true);
        var date = new DateOnly(2024, 1, 15);
        var expectedData = new HeartRateData { CalendarDate = "2024-01-15", RestingHeartRate = 58 };
        _mockApiClient
            .Setup(c => c.GetAsync<HeartRateData>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedData);

        // Act
        var result = await _client.GetRestingHeartRateAsync(date);

        // Assert
        result.Should().Be(58);
    }

    [Fact]
    public async Task GetSleepDataAsync_ReturnsData()
    {
        // Arrange
        _mockAuthenticator.Setup(a => a.IsAuthenticated).Returns(true);
        var date = new DateOnly(2024, 1, 15);
        var expectedData = new SleepData { DailySleep = new DailySleepDto { SleepTimeSeconds = 28800 } };
        _mockApiClient
            .Setup(c => c.GetAsync<SleepData>(It.Is<string>(s => s.Contains("dailySleepData")), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedData);

        // Act
        var result = await _client.GetSleepDataAsync(date);

        // Assert
        result.Should().NotBeNull();
        result!.DailySleep!.SleepTimeSeconds.Should().Be(28800);
    }

    [Fact]
    public async Task GetStressDataAsync_ReturnsData()
    {
        // Arrange
        _mockAuthenticator.Setup(a => a.IsAuthenticated).Returns(true);
        var date = new DateOnly(2024, 1, 15);
        var expectedData = new StressData { CalendarDate = "2024-01-15", AvgStressLevel = 35 };
        _mockApiClient
            .Setup(c => c.GetAsync<StressData>("/wellness-service/wellness/dailyStress/2024-01-15", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedData);

        // Act
        var result = await _client.GetStressDataAsync(date);

        // Assert
        result.Should().NotBeNull();
        result!.AvgStressLevel.Should().Be(35);
    }

    [Fact]
    public async Task GetBodyBatteryAsync_ReturnsList()
    {
        // Arrange
        _mockAuthenticator.Setup(a => a.IsAuthenticated).Returns(true);
        var date = new DateOnly(2024, 1, 15);
        var expectedData = new List<BodyBatteryReport> { new() { Date = "2024-01-15", Charged = 85 } };
        _mockApiClient
            .Setup(c => c.GetAsync<List<BodyBatteryReport>>(It.Is<string>(s => s.Contains("bodyBattery")), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedData);

        // Act
        var result = await _client.GetBodyBatteryAsync(date);

        // Assert
        result.Should().HaveCount(1);
        result[0].Charged.Should().Be(85);
    }

    #endregion

    #region Activity Tests

    [Fact]
    public async Task GetActivitiesAsync_ReturnsList()
    {
        // Arrange
        _mockAuthenticator.Setup(a => a.IsAuthenticated).Returns(true);
        var expectedActivities = new List<Activity>
        {
            new() { ActivityId = 1, ActivityName = "Morning Run" },
            new() { ActivityId = 2, ActivityName = "Evening Walk" }
        };
        _mockApiClient
            .Setup(c => c.GetAsync<List<Activity>>(It.Is<string>(s => s.Contains("activities/search")), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedActivities);

        // Act
        var result = await _client.GetActivitiesAsync(0, 20);

        // Assert
        result.Should().HaveCount(2);
        result[0].ActivityName.Should().Be("Morning Run");
        result[1].ActivityName.Should().Be("Evening Walk");
    }

    [Fact]
    public async Task GetActivitiesAsync_ThrowsException_WhenStartNegative()
    {
        // Arrange
        _mockAuthenticator.Setup(a => a.IsAuthenticated).Returns(true);

        // Act
        var act = () => _client.GetActivitiesAsync(-1, 20);

        // Assert
        await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }

    [Fact]
    public async Task GetActivitiesAsync_ThrowsException_WhenLimitExceedsMax()
    {
        // Arrange
        _mockAuthenticator.Setup(a => a.IsAuthenticated).Returns(true);

        // Act
        var act = () => _client.GetActivitiesAsync(0, 1001);

        // Assert
        await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }

    [Fact]
    public async Task GetLastActivityAsync_ReturnsFirstActivity()
    {
        // Arrange
        _mockAuthenticator.Setup(a => a.IsAuthenticated).Returns(true);
        var expectedActivities = new List<Activity> { new() { ActivityId = 123, ActivityName = "Latest Run" } };
        _mockApiClient
            .Setup(c => c.GetAsync<List<Activity>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedActivities);

        // Act
        var result = await _client.GetLastActivityAsync();

        // Assert
        result.Should().NotBeNull();
        result!.ActivityId.Should().Be(123);
        result.ActivityName.Should().Be("Latest Run");
    }

    [Fact]
    public async Task GetLastActivityAsync_ReturnsNull_WhenNoActivities()
    {
        // Arrange
        _mockAuthenticator.Setup(a => a.IsAuthenticated).Returns(true);
        _mockApiClient
            .Setup(c => c.GetAsync<List<Activity>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Activity>());

        // Act
        var result = await _client.GetLastActivityAsync();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetActivityAsync_ReturnsActivity()
    {
        // Arrange
        _mockAuthenticator.Setup(a => a.IsAuthenticated).Returns(true);
        var expectedActivity = new Activity { ActivityId = 123, ActivityName = "Test Run" };
        _mockApiClient
            .Setup(c => c.GetAsync<Activity>("/activity-service/activity/123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedActivity);

        // Act
        var result = await _client.GetActivityAsync(123);

        // Assert
        result.Should().NotBeNull();
        result!.ActivityId.Should().Be(123);
    }

    [Fact]
    public async Task GetActivityDetailsAsync_ReturnsDetails()
    {
        // Arrange
        _mockAuthenticator.Setup(a => a.IsAuthenticated).Returns(true);
        var expectedDetails = new ActivityDetails { ActivityId = 123, MeasurementCount = 1000 };
        _mockApiClient
            .Setup(c => c.GetAsync<ActivityDetails>(It.Is<string>(s => s.Contains("/details")), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedDetails);

        // Act
        var result = await _client.GetActivityDetailsAsync(123);

        // Assert
        result.Should().NotBeNull();
        result!.MeasurementCount.Should().Be(1000);
    }

    [Fact]
    public async Task DownloadActivityAsync_ReturnsBytes()
    {
        // Arrange
        _mockAuthenticator.Setup(a => a.IsAuthenticated).Returns(true);
        var expectedBytes = new byte[] { 1, 2, 3, 4, 5 };
        _mockApiClient
            .Setup(c => c.GetBytesAsync(It.Is<string>(s => s.Contains("/fit/")), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedBytes);

        // Act
        var result = await _client.DownloadActivityAsync(123, ActivityFileFormat.Fit);

        // Assert
        result.Should().BeEquivalentTo(expectedBytes);
    }

    [Fact]
    public async Task DeleteActivityAsync_CallsDeleteEndpoint()
    {
        // Arrange
        _mockAuthenticator.Setup(a => a.IsAuthenticated).Returns(true);
        _mockApiClient
            .Setup(c => c.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _client.DeleteActivityAsync(123);

        // Assert
        _mockApiClient.Verify(c => c.DeleteAsync("/activity-service/activity/123", It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region Dispose Tests

    [Fact]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        // Act
        var act = () =>
        {
            _client.Dispose();
            _client.Dispose();
        };

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public async Task DisposeAsync_CanBeCalledMultipleTimes()
    {
        // Act
        var act = async () =>
        {
            await _client.DisposeAsync();
            await _client.DisposeAsync();
        };

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Methods_ThrowObjectDisposedException_AfterDispose()
    {
        // Arrange
        _client.Dispose();

        // Act & Assert
        var act = () => _client.LoginAsync("test", "test");
        await act.Should().ThrowAsync<ObjectDisposedException>();
    }

    #endregion
}
