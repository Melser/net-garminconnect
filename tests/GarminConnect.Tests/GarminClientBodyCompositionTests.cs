using FluentAssertions;
using GarminConnect.Api;
using GarminConnect.Auth;
using Moq;

namespace GarminConnect.Tests;

public class GarminClientBodyCompositionTests
{
    private readonly Mock<IGarminApiClient> _apiClientMock;
    private readonly Mock<IGarminAuthenticator> _authenticatorMock;
    private readonly GarminClient _client;

    public GarminClientBodyCompositionTests()
    {
        _apiClientMock = new Mock<IGarminApiClient>();
        _authenticatorMock = new Mock<IGarminAuthenticator>();
        _authenticatorMock.Setup(a => a.IsAuthenticated).Returns(true);

        _client = new GarminClient(_apiClientMock.Object, _authenticatorMock.Object);
    }

    #region AddBodyCompositionAsync Tests

    [Fact]
    public async Task AddBodyCompositionAsync_WhenNotAuthenticated_ThrowsException()
    {
        // Arrange
        _authenticatorMock.Setup(a => a.IsAuthenticated).Returns(false);

        // Act
        var act = () => _client.AddBodyCompositionAsync(75.0);

        // Assert
        await act.Should().ThrowAsync<GarminConnect.Exceptions.GarminConnectAuthenticationException>();
    }

    [Fact]
    public async Task AddBodyCompositionAsync_WithZeroWeight_ThrowsException()
    {
        // Act
        var act = () => _client.AddBodyCompositionAsync(0);

        // Assert
        await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }

    [Fact]
    public async Task AddBodyCompositionAsync_WithNegativeWeight_ThrowsException()
    {
        // Act
        var act = () => _client.AddBodyCompositionAsync(-5.0);

        // Assert
        await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }

    #endregion

    #region AddBloodPressureAsync Tests

    [Fact]
    public async Task AddBloodPressureAsync_WhenNotAuthenticated_ThrowsException()
    {
        // Arrange
        _authenticatorMock.Setup(a => a.IsAuthenticated).Returns(false);

        // Act
        var act = () => _client.AddBloodPressureAsync(120, 80);

        // Assert
        await act.Should().ThrowAsync<GarminConnect.Exceptions.GarminConnectAuthenticationException>();
    }

    [Fact]
    public async Task AddBloodPressureAsync_WithZeroSystolic_ThrowsException()
    {
        // Act
        var act = () => _client.AddBloodPressureAsync(0, 80);

        // Assert
        await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }

    [Fact]
    public async Task AddBloodPressureAsync_WithZeroDiastolic_ThrowsException()
    {
        // Act
        var act = () => _client.AddBloodPressureAsync(120, 0);

        // Assert
        await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }

    [Fact]
    public async Task AddBloodPressureAsync_WithTooHighSystolic_ThrowsException()
    {
        // Act
        var act = () => _client.AddBloodPressureAsync(350, 80);

        // Assert
        await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }

    [Fact]
    public async Task AddBloodPressureAsync_WithTooHighDiastolic_ThrowsException()
    {
        // Act
        var act = () => _client.AddBloodPressureAsync(120, 250);

        // Assert
        await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }

    [Fact]
    public async Task AddBloodPressureAsync_WithInvalidHeartRate_ThrowsException()
    {
        // Act
        var act = () => _client.AddBloodPressureAsync(120, 80, heartRate: 300);

        // Assert
        await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }

    [Fact]
    public async Task AddBloodPressureAsync_WithZeroHeartRate_ThrowsException()
    {
        // Act
        var act = () => _client.AddBloodPressureAsync(120, 80, heartRate: 0);

        // Assert
        await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }

    #endregion
}
