using System.Text.Json;
using FluentAssertions;
using GarminConnect.Models;

namespace GarminConnect.Tests.Models;

public class HeartRateTests
{
    [Fact]
    public void Deserialize_HeartRateData_SetsAllProperties()
    {
        // Arrange
        var json = """
        {
            "calendarDate": "2024-01-15",
            "startTimestampGMT": 1705276800000,
            "endTimestampGMT": 1705363199000,
            "maxHeartRate": 165,
            "minHeartRate": 45,
            "restingHeartRate": 62,
            "lastSevenDaysAvgRestingHeartRate": 60,
            "heartRateValues": [
                {"timestamp": 1705276800000, "heartRate": 65},
                {"timestamp": 1705280400000, "heartRate": 72}
            ]
        }
        """;

        // Act
        var data = JsonSerializer.Deserialize<HeartRateData>(json);

        // Assert
        data.Should().NotBeNull();
        data!.CalendarDate.Should().Be("2024-01-15");
        data.MaxHeartRate.Should().Be(165);
        data.MinHeartRate.Should().Be(45);
        data.RestingHeartRate.Should().Be(62);
        data.LastSevenDaysAvgRestingHeartRate.Should().Be(60);
        data.HeartRateValues.Should().HaveCount(2);
        data.HeartRateValues![0].HeartRate.Should().Be(65);
        data.HeartRateValues[1].HeartRate.Should().Be(72);
    }

    [Fact]
    public void Deserialize_HeartRateValue_SetsProperties()
    {
        // Arrange
        var json = """
        {
            "timestamp": 1705276800000,
            "heartRate": 75
        }
        """;

        // Act
        var value = JsonSerializer.Deserialize<HeartRateValue>(json);

        // Assert
        value.Should().NotBeNull();
        value!.Timestamp.Should().Be(1705276800000);
        value.HeartRate.Should().Be(75);
    }

    [Fact]
    public void Deserialize_EmptyHeartRateValues_ReturnsNull()
    {
        // Arrange
        var json = """
        {
            "calendarDate": "2024-01-15",
            "restingHeartRate": 62
        }
        """;

        // Act
        var data = JsonSerializer.Deserialize<HeartRateData>(json);

        // Assert
        data.Should().NotBeNull();
        data!.HeartRateValues.Should().BeNull();
    }
}
