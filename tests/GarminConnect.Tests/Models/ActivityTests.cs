using System.Text.Json;
using FluentAssertions;
using GarminConnect.Models;

namespace GarminConnect.Tests.Models;

public class ActivityTests
{
    [Fact]
    public void Deserialize_Activity_SetsBasicProperties()
    {
        // Arrange
        var json = """
        {
            "activityId": 12345678901,
            "activityName": "Morning Run",
            "description": "Easy morning jog",
            "startTimeLocal": "2024-01-15T07:00:00",
            "startTimeGMT": "2024-01-15T06:00:00",
            "distance": 5000.5,
            "duration": 1800000,
            "elapsedDuration": 1850000,
            "movingDuration": 1750000,
            "elevationGain": 50.5,
            "elevationLoss": 48.2,
            "averageSpeed": 2.78,
            "maxSpeed": 3.5,
            "calories": 450,
            "averageHR": 145,
            "maxHR": 175
        }
        """;

        // Act
        var activity = JsonSerializer.Deserialize<Activity>(json);

        // Assert
        activity.Should().NotBeNull();
        activity!.ActivityId.Should().Be(12345678901);
        activity.ActivityName.Should().Be("Morning Run");
        activity.Description.Should().Be("Easy morning jog");
        activity.StartTimeLocal.Should().Be("2024-01-15T07:00:00");
        activity.Distance.Should().Be(5000.5);
        activity.Duration.Should().Be(1800000);
        activity.ElevationGain.Should().Be(50.5);
        activity.AverageSpeed.Should().Be(2.78);
        activity.Calories.Should().Be(450);
        activity.AverageHeartRate.Should().Be(145);
        activity.MaxHeartRate.Should().Be(175);
    }

    [Fact]
    public void Deserialize_Activity_SetsActivityType()
    {
        // Arrange
        var json = """
        {
            "activityId": 123,
            "activityType": {
                "typeId": 1,
                "typeKey": "running",
                "parentTypeId": 17,
                "isHidden": false
            }
        }
        """;

        // Act
        var activity = JsonSerializer.Deserialize<Activity>(json);

        // Assert
        activity.Should().NotBeNull();
        activity!.ActivityType.Should().NotBeNull();
        activity.ActivityType!.TypeId.Should().Be(1);
        activity.ActivityType.TypeKey.Should().Be("running");
        activity.ActivityType.ParentTypeId.Should().Be(17);
        activity.ActivityType.IsHidden.Should().BeFalse();
    }

    [Fact]
    public void Deserialize_Activity_SetsCyclingMetrics()
    {
        // Arrange
        var json = """
        {
            "activityId": 123,
            "averageBikingCadenceInRevPerMinute": 85.5,
            "maxBikingCadenceInRevPerMinute": 110.0,
            "avgPower": 200.5,
            "maxPower": 450.0,
            "normPower": 220.0,
            "trainingStressScore": 75.5,
            "intensityFactor": 0.85
        }
        """;

        // Act
        var activity = JsonSerializer.Deserialize<Activity>(json);

        // Assert
        activity.Should().NotBeNull();
        activity!.AverageBikingCadence.Should().Be(85.5);
        activity.MaxBikingCadence.Should().Be(110.0);
        activity.AveragePower.Should().Be(200.5);
        activity.MaxPower.Should().Be(450.0);
        activity.NormalizedPower.Should().Be(220.0);
        activity.TrainingStressScore.Should().Be(75.5);
        activity.IntensityFactor.Should().Be(0.85);
    }

    [Fact]
    public void Deserialize_Activity_SetsRunningMetrics()
    {
        // Arrange
        var json = """
        {
            "activityId": 123,
            "averageRunningCadenceInStepsPerMinute": 180.0,
            "maxRunningCadenceInStepsPerMinute": 195.0,
            "avgVerticalOscillation": 8.5,
            "avgGroundContactTime": 245.0,
            "avgStrideLength": 1.15,
            "steps": 8500
        }
        """;

        // Act
        var activity = JsonSerializer.Deserialize<Activity>(json);

        // Assert
        activity.Should().NotBeNull();
        activity!.AverageRunningCadence.Should().Be(180.0);
        activity.MaxRunningCadence.Should().Be(195.0);
        activity.AvgVerticalOscillation.Should().Be(8.5);
        activity.AvgGroundContactTime.Should().Be(245.0);
        activity.AvgStrideLength.Should().Be(1.15);
        activity.Steps.Should().Be(8500);
    }

    [Fact]
    public void Deserialize_Activity_SetsSwimmingMetrics()
    {
        // Arrange
        var json = """
        {
            "activityId": 123,
            "averageSwimCadenceInStrokesPerMinute": 28.5,
            "strokes": 1500,
            "avgSwolf": 45.0,
            "activeLengths": 50,
            "poolLength": 25.0,
            "unitOfPoolLength": "meter"
        }
        """;

        // Act
        var activity = JsonSerializer.Deserialize<Activity>(json);

        // Assert
        activity.Should().NotBeNull();
        activity!.AverageSwimCadence.Should().Be(28.5);
        activity.Strokes.Should().Be(1500);
        activity.AvgSwolf.Should().Be(45.0);
        activity.ActiveLengths.Should().Be(50);
        activity.PoolLength.Should().Be(25.0);
        activity.UnitOfPoolLength.Should().Be("meter");
    }

    [Fact]
    public void Deserialize_Activity_SetsTrainingEffect()
    {
        // Arrange
        var json = """
        {
            "activityId": 123,
            "aerobicTrainingEffect": 3.5,
            "anaerobicTrainingEffect": 2.1,
            "trainingEffectLabel": "IMPROVING",
            "activityTrainingLoad": 125.5,
            "vO2MaxValue": 52.0
        }
        """;

        // Act
        var activity = JsonSerializer.Deserialize<Activity>(json);

        // Assert
        activity.Should().NotBeNull();
        activity!.AerobicTrainingEffect.Should().Be(3.5);
        activity.AnaerobicTrainingEffect.Should().Be(2.1);
        activity.TrainingEffectLabel.Should().Be("IMPROVING");
        activity.ActivityTrainingLoad.Should().Be(125.5);
        activity.VO2MaxValue.Should().Be(52.0);
    }

    [Fact]
    public void Deserialize_Activity_SetsSplitSummaries()
    {
        // Arrange
        var json = """
        {
            "activityId": 123,
            "splitSummaries": [
                {
                    "noOfSplits": 5,
                    "totalDistance": 5000,
                    "duration": 1800000,
                    "averageSpeed": 2.78,
                    "averageHR": 145,
                    "splitType": "KILOMETER"
                }
            ]
        }
        """;

        // Act
        var activity = JsonSerializer.Deserialize<Activity>(json);

        // Assert
        activity.Should().NotBeNull();
        activity!.SplitSummaries.Should().HaveCount(1);
        activity.SplitSummaries![0].NoOfSplits.Should().Be(5);
        activity.SplitSummaries[0].TotalDistance.Should().Be(5000);
        activity.SplitSummaries[0].SplitType.Should().Be("KILOMETER");
    }
}
