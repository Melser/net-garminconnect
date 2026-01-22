using System.Text.Json;
using FluentAssertions;
using GarminConnect.Models;

namespace GarminConnect.Tests.Models;

public class SleepTests
{
    [Fact]
    public void Deserialize_SleepData_SetsDailySleepDto()
    {
        // Arrange
        var json = """
        {
            "dailySleepDTO": {
                "id": 123456789,
                "calendarDate": "2024-01-15",
                "sleepTimeSeconds": 28800,
                "deepSleepSeconds": 7200,
                "lightSleepSeconds": 14400,
                "remSleepSeconds": 5400,
                "awakeSleepSeconds": 1800,
                "sleepStartTimestampGMT": 1705276800000,
                "sleepEndTimestampGMT": 1705305600000,
                "averageSpO2Value": 96.5,
                "lowestSpO2Value": 92,
                "averageRespirationValue": 15.5,
                "awakeCount": 3,
                "avgSleepStress": 25.5
            },
            "remSleepData": true,
            "restlessMomentsCount": 5
        }
        """;

        // Act
        var data = JsonSerializer.Deserialize<SleepData>(json);

        // Assert
        data.Should().NotBeNull();
        data!.DailySleep.Should().NotBeNull();
        data.DailySleep!.Id.Should().Be(123456789);
        data.DailySleep.CalendarDate.Should().Be("2024-01-15");
        data.DailySleep.SleepTimeSeconds.Should().Be(28800);
        data.DailySleep.DeepSleepSeconds.Should().Be(7200);
        data.DailySleep.LightSleepSeconds.Should().Be(14400);
        data.DailySleep.RemSleepSeconds.Should().Be(5400);
        data.DailySleep.AwakeSleepSeconds.Should().Be(1800);
        data.DailySleep.AverageSpO2Value.Should().Be(96.5);
        data.DailySleep.LowestSpO2Value.Should().Be(92);
        data.DailySleep.AwakeCount.Should().Be(3);
        data.RemSleepData.Should().BeTrue();
        data.RestlessMomentsCount.Should().Be(5);
    }

    [Fact]
    public void Deserialize_SleepScores_SetsComponents()
    {
        // Arrange
        var json = """
        {
            "sleepScores": {
                "overall": {
                    "value": 85,
                    "qualifierKey": "EXCELLENT"
                },
                "totalDuration": {
                    "value": 480,
                    "qualifierKey": "GOOD",
                    "optimalStart": 420,
                    "optimalEnd": 540
                },
                "deepPercentage": {
                    "value": 25,
                    "qualifierKey": "FAIR"
                }
            }
        }
        """;

        // Act
        var data = JsonSerializer.Deserialize<SleepData>(json);

        // Assert
        data.Should().NotBeNull();
        data!.SleepScores.Should().NotBeNull();
        data.SleepScores!.Overall.Should().NotBeNull();
        data.SleepScores.Overall!.Value.Should().Be(85);
        data.SleepScores.Overall.QualifierKey.Should().Be("EXCELLENT");
        data.SleepScores.TotalDuration.Should().NotBeNull();
        data.SleepScores.TotalDuration!.OptimalStart.Should().Be(420);
        data.SleepScores.TotalDuration.OptimalEnd.Should().Be(540);
    }

    [Fact]
    public void Deserialize_SleepLevels_SetsArray()
    {
        // Arrange
        var json = """
        {
            "sleepLevels": [
                {"startGMT": 1705276800000, "endGMT": 1705280400000, "activityLevel": 0.0},
                {"startGMT": 1705280400000, "endGMT": 1705284000000, "activityLevel": 1.0},
                {"startGMT": 1705284000000, "endGMT": 1705287600000, "activityLevel": 2.0}
            ]
        }
        """;

        // Act
        var data = JsonSerializer.Deserialize<SleepData>(json);

        // Assert
        data.Should().NotBeNull();
        data!.SleepLevels.Should().HaveCount(3);
        data.SleepLevels![0].ActivityLevel.Should().Be(0.0);
        data.SleepLevels[1].ActivityLevel.Should().Be(1.0);
        data.SleepLevels[2].ActivityLevel.Should().Be(2.0);
    }
}
