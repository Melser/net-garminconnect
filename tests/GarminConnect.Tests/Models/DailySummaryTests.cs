using System.Text.Json;
using FluentAssertions;
using GarminConnect.Models;

namespace GarminConnect.Tests.Models;

public class DailySummaryTests
{
    [Fact]
    public void Deserialize_FullResponse_SetsAllProperties()
    {
        // Arrange
        var json = """
        {
            "calendarDate": "2024-01-15",
            "totalKilocalories": 2500,
            "activeKilocalories": 800,
            "bmrKilocalories": 1700,
            "totalSteps": 12345,
            "dailyStepGoal": 10000,
            "totalDistanceMeters": 9200.5,
            "floorsAscended": 15,
            "floorsDescended": 12,
            "minHeartRate": 45,
            "maxHeartRate": 165,
            "restingHeartRate": 62,
            "averageStressLevel": 35,
            "maxStressLevel": 65,
            "bodyBatteryChargedValue": 85,
            "bodyBatteryDrainedValue": 45,
            "bodyBatteryHighestValue": 100,
            "bodyBatteryLowestValue": 25,
            "moderateIntensityMinutes": 30,
            "vigorousIntensityMinutes": 15,
            "privacyProtected": false
        }
        """;

        // Act
        var summary = JsonSerializer.Deserialize<DailySummary>(json);

        // Assert
        summary.Should().NotBeNull();
        summary!.CalendarDate.Should().Be("2024-01-15");
        summary.TotalKilocalories.Should().Be(2500);
        summary.ActiveKilocalories.Should().Be(800);
        summary.TotalSteps.Should().Be(12345);
        summary.DailyStepGoal.Should().Be(10000);
        summary.TotalDistanceMeters.Should().Be(9200.5);
        summary.FloorsAscended.Should().Be(15);
        summary.MinHeartRate.Should().Be(45);
        summary.MaxHeartRate.Should().Be(165);
        summary.RestingHeartRate.Should().Be(62);
        summary.AverageStressLevel.Should().Be(35);
        summary.BodyBatteryChargedValue.Should().Be(85);
        summary.ModerateIntensityMinutes.Should().Be(30);
        summary.VigorousIntensityMinutes.Should().Be(15);
        summary.PrivacyProtected.Should().BeFalse();
    }

    [Fact]
    public void Deserialize_PartialResponse_HandlesNulls()
    {
        // Arrange
        var json = """
        {
            "calendarDate": "2024-01-15",
            "totalSteps": 5000
        }
        """;

        // Act
        var summary = JsonSerializer.Deserialize<DailySummary>(json);

        // Assert
        summary.Should().NotBeNull();
        summary!.CalendarDate.Should().Be("2024-01-15");
        summary.TotalSteps.Should().Be(5000);
        summary.TotalKilocalories.Should().BeNull();
        summary.RestingHeartRate.Should().BeNull();
        summary.AverageStressLevel.Should().BeNull();
    }

    [Fact]
    public void Deserialize_EmptyObject_ReturnsEmptyRecord()
    {
        // Arrange
        var json = "{}";

        // Act
        var summary = JsonSerializer.Deserialize<DailySummary>(json);

        // Assert
        summary.Should().NotBeNull();
        summary!.CalendarDate.Should().BeNull();
        summary.TotalSteps.Should().BeNull();
    }
}
