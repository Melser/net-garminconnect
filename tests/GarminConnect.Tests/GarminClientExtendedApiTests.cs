using System.Text.Json;
using FluentAssertions;
using GarminConnect.Api;
using GarminConnect.Auth;
using GarminConnect.Exceptions;
using Moq;

namespace GarminConnect.Tests;

/// <summary>
/// Unit tests for Phase 5 Extended APIs: Devices, Gear, Workouts, Badges, Goals, TrainingPlans
/// </summary>
public class GarminClientExtendedApiTests : IDisposable
{
    private readonly Mock<IGarminApiClient> _mockApiClient;
    private readonly Mock<IGarminAuthenticator> _mockAuthenticator;
    private readonly GarminClient _client;

    public GarminClientExtendedApiTests()
    {
        _mockApiClient = new Mock<IGarminApiClient>();
        _mockAuthenticator = new Mock<IGarminAuthenticator>();
        _mockAuthenticator.Setup(a => a.IsAuthenticated).Returns(true);
        _client = new GarminClient(_mockApiClient.Object, _mockAuthenticator.Object);
    }

    public void Dispose()
    {
        _client.Dispose();
    }

    #region Devices Tests

    [Fact]
    public async Task GetDevicesAsync_ReturnsDevices()
    {
        // Arrange
        var expectedJson = JsonDocument.Parse("[{\"deviceId\":\"123\",\"productDisplayName\":\"Fenix 7\"}]").RootElement;
        _mockApiClient
            .Setup(c => c.GetAsync<JsonElement>(Endpoints.Devices, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedJson);

        // Act
        var result = await _client.GetDevicesAsync();

        // Assert
        result.GetArrayLength().Should().Be(1);
    }

    [Fact]
    public async Task GetDevicesAsync_ThrowsWhenNotAuthenticated()
    {
        // Arrange
        _mockAuthenticator.Setup(a => a.IsAuthenticated).Returns(false);

        // Act & Assert
        await Assert.ThrowsAsync<GarminConnectAuthenticationException>(() => _client.GetDevicesAsync());
    }

    [Fact]
    public async Task GetDeviceSettingsAsync_ReturnsSettings()
    {
        // Arrange
        var expectedJson = JsonDocument.Parse("{\"alarms\":[]}").RootElement;
        _mockApiClient
            .Setup(c => c.GetAsync<JsonElement>(It.Is<string>(s => s.Contains("/device-service/deviceservice/device-info/settings/")), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedJson);

        // Act
        var result = await _client.GetDeviceSettingsAsync("device123");

        // Assert
        result.TryGetProperty("alarms", out _).Should().BeTrue();
    }

    [Fact]
    public async Task GetDeviceSettingsAsync_ThrowsOnNullDeviceId()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _client.GetDeviceSettingsAsync(null!));
    }

    [Fact]
    public async Task GetDeviceSettingsAsync_ThrowsOnEmptyDeviceId()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _client.GetDeviceSettingsAsync(""));
    }

    [Fact]
    public async Task GetPrimaryTrainingDeviceAsync_ReturnsDevice()
    {
        // Arrange
        var expectedJson = JsonDocument.Parse("{\"deviceId\":\"primary123\"}").RootElement;
        _mockApiClient
            .Setup(c => c.GetAsync<JsonElement>(Endpoints.PrimaryTrainingDevice, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedJson);

        // Act
        var result = await _client.GetPrimaryTrainingDeviceAsync();

        // Assert
        result.GetProperty("deviceId").GetString().Should().Be("primary123");
    }

    [Fact]
    public async Task GetDeviceSolarDataAsync_ReturnsData()
    {
        // Arrange
        var expectedJson = JsonDocument.Parse("{\"solarIntensity\":75}").RootElement;
        _mockApiClient
            .Setup(c => c.GetAsync<JsonElement>(It.Is<string>(s => s.Contains("/web-gateway/solar/")), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedJson);

        // Act
        var result = await _client.GetDeviceSolarDataAsync("device123", new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 31));

        // Assert
        result.GetProperty("solarIntensity").GetInt32().Should().Be(75);
    }

    [Fact]
    public async Task GetDeviceSolarDataAsync_ThrowsOnInvalidDateRange()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _client.GetDeviceSolarDataAsync("device123", new DateOnly(2024, 2, 1), new DateOnly(2024, 1, 1)));
    }

    [Fact]
    public async Task GetDeviceLastUsedAsync_ReturnsDevice()
    {
        // Arrange
        var expectedJson = JsonDocument.Parse("{\"deviceId\":\"last123\"}").RootElement;
        _mockApiClient
            .Setup(c => c.GetAsync<JsonElement>(Endpoints.DeviceLastUsed, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedJson);

        // Act
        var result = await _client.GetDeviceLastUsedAsync();

        // Assert
        result.GetProperty("deviceId").GetString().Should().Be("last123");
    }

    [Fact]
    public async Task GetDeviceAlarmsAsync_AggregatesAlarmsFromAllDevices()
    {
        // Arrange
        var devicesJson = JsonDocument.Parse("[{\"deviceId\":\"dev1\"},{\"deviceId\":\"dev2\"}]").RootElement;
        var settings1 = JsonDocument.Parse("{\"alarms\":[{\"time\":\"07:00\"}]}").RootElement;
        var settings2 = JsonDocument.Parse("{\"alarms\":[{\"time\":\"08:00\"}]}").RootElement;

        _mockApiClient
            .Setup(c => c.GetAsync<JsonElement>(Endpoints.Devices, It.IsAny<CancellationToken>()))
            .ReturnsAsync(devicesJson);
        _mockApiClient
            .SetupSequence(c => c.GetAsync<JsonElement>(It.Is<string>(s => s.Contains("settings")), It.IsAny<CancellationToken>()))
            .ReturnsAsync(settings1)
            .ReturnsAsync(settings2);

        // Act
        var result = await _client.GetDeviceAlarmsAsync();

        // Assert
        result.Should().HaveCount(2);
    }

    #endregion

    #region Gear Tests

    [Fact]
    public async Task GetGearAsync_ReturnsGear()
    {
        // Arrange
        var expectedJson = JsonDocument.Parse("[{\"gearUuid\":\"gear123\",\"gearName\":\"Running Shoes\"}]").RootElement;
        _mockApiClient
            .Setup(c => c.GetAsync<JsonElement>(It.Is<string>(s => s.Contains("/gear-service/gear/filterGear")), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedJson);

        // Act
        var result = await _client.GetGearAsync("user123");

        // Assert
        result.GetArrayLength().Should().Be(1);
    }

    [Fact]
    public async Task GetGearAsync_ThrowsOnNullUserProfileNumber()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _client.GetGearAsync(null!));
    }

    [Fact]
    public async Task GetGearStatsAsync_ReturnsStats()
    {
        // Arrange
        var expectedJson = JsonDocument.Parse("{\"totalDistance\":1000.5}").RootElement;
        _mockApiClient
            .Setup(c => c.GetAsync<JsonElement>(It.Is<string>(s => s.Contains("/gear-service/gear/stats/")), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedJson);

        // Act
        var result = await _client.GetGearStatsAsync("gear123");

        // Assert
        result.GetProperty("totalDistance").GetDouble().Should().Be(1000.5);
    }

    [Fact]
    public async Task GetGearDefaultsAsync_ReturnsDefaults()
    {
        // Arrange
        var expectedJson = JsonDocument.Parse("{\"running\":{\"gearUuid\":\"gear123\"}}").RootElement;
        _mockApiClient
            .Setup(c => c.GetAsync<JsonElement>(It.Is<string>(s => s.Contains("/gear-service/gear/user/")), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedJson);

        // Act
        var result = await _client.GetGearDefaultsAsync("user123");

        // Assert
        result.TryGetProperty("running", out _).Should().BeTrue();
    }

    [Fact]
    public async Task SetGearDefaultAsync_WhenIsDefaultTrue_CallsPut()
    {
        // Arrange
        _mockApiClient
            .Setup(c => c.PutAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _client.SetGearDefaultAsync("running", "gear123", true);

        // Assert
        _mockApiClient.Verify(c => c.PutAsync(It.Is<string>(s => s.Contains("/default/true")), It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SetGearDefaultAsync_WhenIsDefaultFalse_CallsDelete()
    {
        // Arrange
        _mockApiClient
            .Setup(c => c.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _client.SetGearDefaultAsync("running", "gear123", false);

        // Assert
        _mockApiClient.Verify(c => c.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetGearActivitiesAsync_ReturnsActivities()
    {
        // Arrange
        var expectedJson = JsonDocument.Parse("[{\"activityId\":123}]").RootElement;
        _mockApiClient
            .Setup(c => c.GetAsync<JsonElement>(It.Is<string>(s => s.Contains("/activitylist-service/activities/")), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedJson);

        // Act
        var result = await _client.GetGearActivitiesAsync("gear123", 100);

        // Assert
        result.GetArrayLength().Should().Be(1);
    }

    [Fact]
    public async Task GetGearActivitiesAsync_ThrowsOnInvalidLimit()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _client.GetGearActivitiesAsync("gear123", 0));
    }

    [Fact]
    public async Task AddGearToActivityAsync_ReturnsResult()
    {
        // Arrange
        var expectedJson = JsonDocument.Parse("{\"success\":true}").RootElement;
        _mockApiClient
            .Setup(c => c.PutAsync<JsonElement>(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedJson);

        // Act
        var result = await _client.AddGearToActivityAsync("gear123", 456);

        // Assert
        result.GetProperty("success").GetBoolean().Should().BeTrue();
    }

    [Fact]
    public async Task AddGearToActivityAsync_ThrowsOnInvalidActivityId()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _client.AddGearToActivityAsync("gear123", 0));
    }

    #endregion

    #region Workouts Tests

    [Fact]
    public async Task GetWorkoutsAsync_ReturnsWorkouts()
    {
        // Arrange
        var expectedJson = JsonDocument.Parse("[{\"workoutId\":123,\"workoutName\":\"Morning Run\"}]").RootElement;
        _mockApiClient
            .Setup(c => c.GetAsync<JsonElement>(It.Is<string>(s => s.Contains("/workout-service/workouts")), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedJson);

        // Act
        var result = await _client.GetWorkoutsAsync(0, 100);

        // Assert
        result.GetArrayLength().Should().Be(1);
    }

    [Fact]
    public async Task GetWorkoutsAsync_ThrowsOnNegativeStart()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _client.GetWorkoutsAsync(-1, 100));
    }

    [Fact]
    public async Task GetWorkoutsAsync_ThrowsOnInvalidLimit()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _client.GetWorkoutsAsync(0, 0));
    }

    [Fact]
    public async Task GetWorkoutByIdAsync_ReturnsWorkout()
    {
        // Arrange
        var expectedJson = JsonDocument.Parse("{\"workoutId\":123,\"workoutName\":\"Test Workout\"}").RootElement;
        _mockApiClient
            .Setup(c => c.GetAsync<JsonElement>(It.Is<string>(s => s.Contains("/workout-service/workout/")), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedJson);

        // Act
        var result = await _client.GetWorkoutByIdAsync(123);

        // Assert
        result.GetProperty("workoutId").GetInt64().Should().Be(123);
    }

    [Fact]
    public async Task GetWorkoutByIdAsync_ThrowsOnInvalidId()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _client.GetWorkoutByIdAsync(0));
    }

    [Fact]
    public async Task DownloadWorkoutAsync_ReturnsBytes()
    {
        // Arrange
        var expectedBytes = new byte[] { 0x0E, 0x10, 0x00 };
        _mockApiClient
            .Setup(c => c.GetBytesAsync(It.Is<string>(s => s.Contains("/FIT")), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedBytes);

        // Act
        var result = await _client.DownloadWorkoutAsync(123);

        // Assert
        result.Should().BeEquivalentTo(expectedBytes);
    }

    [Fact]
    public async Task DownloadWorkoutAsync_ThrowsOnInvalidId()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _client.DownloadWorkoutAsync(-1));
    }

    [Fact]
    public async Task UploadWorkoutAsync_WithJsonElement_ReturnsResult()
    {
        // Arrange
        var workout = JsonDocument.Parse("{\"workoutName\":\"New Workout\"}").RootElement;
        var expectedJson = JsonDocument.Parse("{\"workoutId\":456}").RootElement;
        _mockApiClient
            .Setup(c => c.PostAsync<JsonElement>(Endpoints.WorkoutUpload, It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedJson);

        // Act
        var result = await _client.UploadWorkoutAsync(workout);

        // Assert
        result.GetProperty("workoutId").GetInt64().Should().Be(456);
    }

    [Fact]
    public async Task UploadWorkoutAsync_WithJsonString_ReturnsResult()
    {
        // Arrange
        var workoutJson = "{\"workoutName\":\"New Workout\"}";
        var expectedJson = JsonDocument.Parse("{\"workoutId\":456}").RootElement;
        _mockApiClient
            .Setup(c => c.PostAsync<JsonElement>(Endpoints.WorkoutUpload, It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedJson);

        // Act
        var result = await _client.UploadWorkoutAsync(workoutJson);

        // Assert
        result.GetProperty("workoutId").GetInt64().Should().Be(456);
    }

    [Fact]
    public async Task UploadWorkoutAsync_ThrowsOnNullJsonString()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _client.UploadWorkoutAsync((string)null!));
    }

    [Fact]
    public async Task GetScheduledWorkoutByIdAsync_ReturnsWorkout()
    {
        // Arrange
        var expectedJson = JsonDocument.Parse("{\"scheduledWorkoutId\":789}").RootElement;
        _mockApiClient
            .Setup(c => c.GetAsync<JsonElement>(It.Is<string>(s => s.Contains("/workout-service/schedule/")), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedJson);

        // Act
        var result = await _client.GetScheduledWorkoutByIdAsync(789);

        // Assert
        result.GetProperty("scheduledWorkoutId").GetInt64().Should().Be(789);
    }

    #endregion

    #region Badges Tests

    [Fact]
    public async Task GetEarnedBadgesAsync_ReturnsBadges()
    {
        // Arrange
        var expectedJson = JsonDocument.Parse("[{\"badgeId\":1,\"badgeName\":\"First Run\"}]").RootElement;
        _mockApiClient
            .Setup(c => c.GetAsync<JsonElement>(Endpoints.EarnedBadges, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedJson);

        // Act
        var result = await _client.GetEarnedBadgesAsync();

        // Assert
        result.GetArrayLength().Should().Be(1);
    }

    [Fact]
    public async Task GetAvailableBadgesAsync_ReturnsBadges()
    {
        // Arrange
        var expectedJson = JsonDocument.Parse("[{\"badgeId\":2}]").RootElement;
        _mockApiClient
            .Setup(c => c.GetAsync<JsonElement>(Endpoints.AvailableBadges, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedJson);

        // Act
        var result = await _client.GetAvailableBadgesAsync();

        // Assert
        result.GetArrayLength().Should().Be(1);
    }

    [Fact]
    public async Task GetInProgressBadgesAsync_ReturnsBadges()
    {
        // Arrange
        var expectedJson = JsonDocument.Parse("[{\"badgeId\":3,\"progress\":50}]").RootElement;
        _mockApiClient
            .Setup(c => c.GetAsync<JsonElement>(Endpoints.InProgressBadges, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedJson);

        // Act
        var result = await _client.GetInProgressBadgesAsync();

        // Assert
        result.GetArrayLength().Should().Be(1);
    }

    [Fact]
    public async Task GetBadgeChallengesAsync_ReturnsChallenges()
    {
        // Arrange
        var expectedJson = JsonDocument.Parse("[{\"challengeId\":1}]").RootElement;
        _mockApiClient
            .Setup(c => c.GetAsync<JsonElement>(It.Is<string>(s => s.Contains("/badgechallenge-service/badgeChallenge/available")), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedJson);

        // Act
        var result = await _client.GetBadgeChallengesAsync(0, 100);

        // Assert
        result.GetArrayLength().Should().Be(1);
    }

    [Fact]
    public async Task GetBadgeChallengesAsync_ThrowsOnNegativeStart()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _client.GetBadgeChallengesAsync(-1, 100));
    }

    [Fact]
    public async Task GetAvailableBadgeChallengesAsync_ReturnsChallenges()
    {
        // Arrange
        var expectedJson = JsonDocument.Parse("[{\"challengeId\":2}]").RootElement;
        _mockApiClient
            .Setup(c => c.GetAsync<JsonElement>(It.Is<string>(s => s.Contains("/available")), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedJson);

        // Act
        var result = await _client.GetAvailableBadgeChallengesAsync(0, 100);

        // Assert
        result.GetArrayLength().Should().Be(1);
    }

    [Fact]
    public async Task GetNonCompletedBadgeChallengesAsync_ReturnsChallenges()
    {
        // Arrange
        var expectedJson = JsonDocument.Parse("[{\"challengeId\":3}]").RootElement;
        _mockApiClient
            .Setup(c => c.GetAsync<JsonElement>(It.Is<string>(s => s.Contains("/nonCompleted")), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedJson);

        // Act
        var result = await _client.GetNonCompletedBadgeChallengesAsync(0, 100);

        // Assert
        result.GetArrayLength().Should().Be(1);
    }

    #endregion

    #region Goals Tests

    [Fact]
    public async Task GetGoalsAsync_ReturnsGoals()
    {
        // Arrange
        var expectedJson = JsonDocument.Parse("[{\"goalId\":1,\"goalType\":\"steps\"}]").RootElement;
        _mockApiClient
            .Setup(c => c.GetAsync<JsonElement>(It.Is<string>(s => s.Contains("/goal-service/goal/goals")), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedJson);

        // Act
        var result = await _client.GetGoalsAsync();

        // Assert
        result.GetArrayLength().Should().Be(1);
    }

    [Fact]
    public async Task GetGoalsAsync_WithGoalType_FiltersGoals()
    {
        // Arrange
        var expectedJson = JsonDocument.Parse("[{\"goalId\":1,\"goalType\":\"steps\"}]").RootElement;
        _mockApiClient
            .Setup(c => c.GetAsync<JsonElement>(It.Is<string>(s => s.Contains("goalType=steps")), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedJson);

        // Act
        var result = await _client.GetGoalsAsync("steps");

        // Assert
        result.GetArrayLength().Should().Be(1);
        _mockApiClient.Verify(c => c.GetAsync<JsonElement>(It.Is<string>(s => s.Contains("goalType=steps")), It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region Training Plans Tests

    [Fact]
    public async Task GetTrainingPlansAsync_ReturnsPlans()
    {
        // Arrange
        var expectedJson = JsonDocument.Parse("[{\"planId\":1,\"planName\":\"5K Training\"}]").RootElement;
        _mockApiClient
            .Setup(c => c.GetAsync<JsonElement>(Endpoints.TrainingPlans, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedJson);

        // Act
        var result = await _client.GetTrainingPlansAsync();

        // Assert
        result.GetArrayLength().Should().Be(1);
    }

    [Fact]
    public async Task GetTrainingPlanByIdAsync_ReturnsPlan()
    {
        // Arrange
        var expectedJson = JsonDocument.Parse("{\"planId\":123,\"planName\":\"Marathon Training\"}").RootElement;
        _mockApiClient
            .Setup(c => c.GetAsync<JsonElement>(It.Is<string>(s => s.Contains("/trainingplan-service/trainingplan/")), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedJson);

        // Act
        var result = await _client.GetTrainingPlanByIdAsync(123);

        // Assert
        result.GetProperty("planId").GetInt64().Should().Be(123);
    }

    [Fact]
    public async Task GetAdaptiveTrainingPlanByIdAsync_ReturnsPlan()
    {
        // Arrange
        var expectedJson = JsonDocument.Parse("{\"planId\":456,\"adaptive\":true}").RootElement;
        _mockApiClient
            .Setup(c => c.GetAsync<JsonElement>(It.Is<string>(s => s.Contains("/adaptive")), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedJson);

        // Act
        var result = await _client.GetAdaptiveTrainingPlanByIdAsync(456);

        // Assert
        result.GetProperty("adaptive").GetBoolean().Should().BeTrue();
    }

    #endregion

    #region Body Composition Tests

    [Fact]
    public async Task GetBodyCompositionAsync_ReturnsData()
    {
        // Arrange
        var expectedJson = JsonDocument.Parse("{\"totalWeighIns\":5}").RootElement;
        _mockApiClient
            .Setup(c => c.GetAsync<JsonElement>(It.Is<string>(s => s.Contains("/weight-service/weight/dateRange")), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedJson);

        // Act
        var result = await _client.GetBodyCompositionAsync(new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 31));

        // Assert
        result.GetProperty("totalWeighIns").GetInt32().Should().Be(5);
    }

    [Fact]
    public async Task GetDailyWeighInsAsync_ReturnsData()
    {
        // Arrange
        var expectedJson = JsonDocument.Parse("[{\"weight\":70.5}]").RootElement;
        _mockApiClient
            .Setup(c => c.GetAsync<JsonElement>(It.Is<string>(s => s.Contains("/weight-service/weight/dayview/")), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedJson);

        // Act
        var result = await _client.GetDailyWeighInsAsync(new DateOnly(2024, 1, 15));

        // Assert
        result.GetArrayLength().Should().Be(1);
    }

    [Fact]
    public async Task DeleteWeighInAsync_CallsDeleteEndpoint()
    {
        // Arrange
        _mockApiClient
            .Setup(c => c.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _client.DeleteWeighInAsync("weighin123", new DateOnly(2024, 1, 15));

        // Assert
        _mockApiClient.Verify(c => c.DeleteAsync(It.Is<string>(s => s.Contains("/weight-service/weight/")), It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion
}
