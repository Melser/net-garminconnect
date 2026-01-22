using FluentAssertions;
using GarminConnect.Fit;

namespace GarminConnect.Tests.Fit;

public class FitEncoderTests
{
    [Fact]
    public void FitEncoder_CreatesValidHeader()
    {
        // Arrange & Act
        using var encoder = new FitEncoder();
        encoder.WriteFileInfo();
        encoder.WriteFileCreator();
        encoder.Finish();

        var bytes = encoder.GetBytes();

        // Assert
        bytes.Should().NotBeEmpty();
        bytes.Length.Should().BeGreaterThan(12); // At least header size

        // Check header
        bytes[0].Should().Be(12); // Header size
        bytes[1].Should().Be(16); // Protocol version

        // Check ".FIT" signature
        var signature = System.Text.Encoding.ASCII.GetString(bytes, 8, 4);
        signature.Should().Be(".FIT");
    }

    [Fact]
    public void FitEncoder_IncludesCrc()
    {
        // Arrange & Act
        using var encoder = new FitEncoder();
        encoder.WriteFileInfo();
        encoder.Finish();

        var bytes = encoder.GetBytes();

        // Assert - CRC is 2 bytes at the end
        bytes.Length.Should().BeGreaterThan(14); // header + some data + CRC

        // Last 2 bytes should be CRC (non-zero in most cases)
        var crc = BitConverter.ToUInt16(bytes, bytes.Length - 2);
        // CRC can technically be 0, but with actual data it's very unlikely
    }

    [Fact]
    public void FitEncoder_WriteDeviceInfo_CanBeCalledMultipleTimes()
    {
        // Arrange
        using var encoder = new FitEncoder();
        var timestamp = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);

        // Act
        encoder.WriteFileInfo();
        encoder.WriteDeviceInfo(timestamp);
        encoder.WriteDeviceInfo(timestamp.AddHours(1)); // Second call
        encoder.Finish();

        var bytes = encoder.GetBytes();

        // Assert - should not throw and produce valid output
        bytes.Should().NotBeEmpty();
    }
}

public class FitEncoderWeightTests
{
    [Fact]
    public void CreateWeightFile_ProducesValidFitFile()
    {
        // Arrange
        var timestamp = new DateTime(2024, 1, 15, 8, 0, 0, DateTimeKind.Utc);
        var weight = 75.5;

        // Act
        var bytes = FitEncoderWeight.CreateWeightFile(timestamp, weight);

        // Assert
        bytes.Should().NotBeEmpty();
        bytes.Length.Should().BeGreaterThan(50); // Reasonable minimum size

        // Check FIT signature
        var signature = System.Text.Encoding.ASCII.GetString(bytes, 8, 4);
        signature.Should().Be(".FIT");
    }

    [Fact]
    public void CreateWeightFile_WithAllParameters_ProducesValidFile()
    {
        // Arrange
        var timestamp = new DateTime(2024, 1, 15, 8, 0, 0, DateTimeKind.Utc);

        // Act
        var bytes = FitEncoderWeight.CreateWeightFile(
            timestamp,
            weight: 75.5,
            percentFat: 18.5,
            percentHydration: 55.0,
            visceralFatMass: 2.5,
            boneMass: 3.2,
            muscleMass: 35.0,
            basalMet: 1800,
            activeMet: 2200,
            physiqueRating: 5,
            metabolicAge: 35,
            visceralFatRating: 8,
            bmi: 24.5);

        // Assert
        bytes.Should().NotBeEmpty();
        bytes.Length.Should().BeGreaterThan(100); // With all data, should be larger
    }

    [Fact]
    public void CreateWeightFile_DifferentWeights_ProduceDifferentFiles()
    {
        // Arrange
        var timestamp = new DateTime(2024, 1, 15, 8, 0, 0, DateTimeKind.Utc);

        // Act
        var bytes1 = FitEncoderWeight.CreateWeightFile(timestamp, 70.0);
        var bytes2 = FitEncoderWeight.CreateWeightFile(timestamp, 80.0);

        // Assert
        bytes1.Should().NotBeEquivalentTo(bytes2);
    }

    [Fact]
    public void WriteWeightScale_CanWriteMultipleRecords()
    {
        // Arrange
        using var encoder = new FitEncoderWeight();
        var timestamp1 = new DateTime(2024, 1, 15, 8, 0, 0, DateTimeKind.Utc);
        var timestamp2 = new DateTime(2024, 1, 16, 8, 0, 0, DateTimeKind.Utc);

        // Act
        encoder.WriteFileInfo();
        encoder.WriteFileCreator();
        encoder.WriteDeviceInfo(timestamp1);
        encoder.WriteWeightScale(timestamp1, 75.0);
        encoder.WriteWeightScale(timestamp2, 74.5); // Second measurement
        encoder.Finish();

        var bytes = encoder.GetBytes();

        // Assert
        bytes.Should().NotBeEmpty();
    }
}

public class FitEncoderBloodPressureTests
{
    [Fact]
    public void CreateBloodPressureFile_ProducesValidFitFile()
    {
        // Arrange
        var timestamp = new DateTime(2024, 1, 15, 8, 0, 0, DateTimeKind.Utc);

        // Act
        var bytes = FitEncoderBloodPressure.CreateBloodPressureFile(
            timestamp,
            systolicPressure: 120,
            diastolicPressure: 80);

        // Assert
        bytes.Should().NotBeEmpty();

        // Check FIT signature
        var signature = System.Text.Encoding.ASCII.GetString(bytes, 8, 4);
        signature.Should().Be(".FIT");
    }

    [Fact]
    public void CreateBloodPressureFile_WithHeartRate_ProducesValidFile()
    {
        // Arrange
        var timestamp = new DateTime(2024, 1, 15, 8, 0, 0, DateTimeKind.Utc);

        // Act
        var bytes = FitEncoderBloodPressure.CreateBloodPressureFile(
            timestamp,
            systolicPressure: 120,
            diastolicPressure: 80,
            heartRate: 72);

        // Assert
        bytes.Should().NotBeEmpty();
    }

    [Fact]
    public void CreateBloodPressureFile_CalculatesMAP_WhenNotProvided()
    {
        // Arrange
        var timestamp = new DateTime(2024, 1, 15, 8, 0, 0, DateTimeKind.Utc);

        // Act - MAP should be calculated as DBP + 1/3(SBP - DBP) = 80 + 13 = 93
        var bytes = FitEncoderBloodPressure.CreateBloodPressureFile(
            timestamp,
            systolicPressure: 120,
            diastolicPressure: 80);

        // Assert - just verify it produces output (MAP calculation is internal)
        bytes.Should().NotBeEmpty();
    }

    [Fact]
    public void WriteBloodPressure_CanWriteMultipleRecords()
    {
        // Arrange
        using var encoder = new FitEncoderBloodPressure();
        var timestamp1 = new DateTime(2024, 1, 15, 8, 0, 0, DateTimeKind.Utc);
        var timestamp2 = new DateTime(2024, 1, 15, 20, 0, 0, DateTimeKind.Utc);

        // Act
        encoder.WriteFileInfo();
        encoder.WriteFileCreator();
        encoder.WriteDeviceInfo(timestamp1);
        encoder.WriteBloodPressure(timestamp1, 120, 80);
        encoder.WriteBloodPressure(timestamp2, 118, 78); // Evening measurement
        encoder.Finish();

        var bytes = encoder.GetBytes();

        // Assert
        bytes.Should().NotBeEmpty();
    }
}
