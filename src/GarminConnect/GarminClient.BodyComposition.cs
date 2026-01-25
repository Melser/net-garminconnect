using System.Text.Json;
using GarminConnect.Api;
using GarminConnect.Fit;

namespace GarminConnect;

public sealed partial class GarminClient
{
    #region Body Composition - Read

    /// <summary>
    /// Gets body composition data for a date range.
    /// </summary>
    public async Task<JsonElement> GetBodyCompositionAsync(
        DateOnly startDate,
        DateOnly? endDate = null,
        CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        EnsureAuthenticated();

        var end = endDate ?? startDate;
        if (startDate > end)
        {
            throw new ArgumentException("startDate cannot be after endDate", nameof(startDate));
        }

        var url = $"{Endpoints.WeightDateRange}?startDate={startDate:yyyy-MM-dd}&endDate={end:yyyy-MM-dd}";

        return await _apiClient.GetAsync<JsonElement>(url, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets daily weigh-ins for a specific date.
    /// </summary>
    public async Task<JsonElement> GetDailyWeighInsAsync(
        DateOnly date,
        CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        EnsureAuthenticated();

        var url = $"{string.Format(Endpoints.DailyWeighIns, date.ToString("yyyy-MM-dd"))}?includeAll=true";

        return await _apiClient.GetAsync<JsonElement>(url, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes a specific weigh-in entry.
    /// </summary>
    public async Task DeleteWeighInAsync(
        string weightPk,
        DateOnly date,
        CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        EnsureAuthenticated();

        ArgumentException.ThrowIfNullOrWhiteSpace(weightPk, nameof(weightPk));

        var url = string.Format(
            Endpoints.DeleteWeighIn,
            date.ToString("yyyy-MM-dd"),
            weightPk);

        await _apiClient.DeleteAsync(url, cancellationToken).ConfigureAwait(false);
    }

    #endregion

    #region Body Composition - Write

    /// <summary>
    /// Adds a body composition (weight) measurement to Garmin Connect.
    /// </summary>
    /// <param name="weight">Weight in kilograms.</param>
    /// <param name="timestamp">Measurement timestamp (defaults to now).</param>
    /// <param name="percentFat">Body fat percentage (0-100).</param>
    /// <param name="percentHydration">Body hydration percentage (0-100).</param>
    /// <param name="visceralFatMass">Visceral fat mass in kg.</param>
    /// <param name="boneMass">Bone mass in kg.</param>
    /// <param name="muscleMass">Muscle mass in kg.</param>
    /// <param name="basalMet">Basal metabolic rate.</param>
    /// <param name="activeMet">Active metabolic rate.</param>
    /// <param name="physiqueRating">Physique rating (1-9).</param>
    /// <param name="metabolicAge">Metabolic age in years.</param>
    /// <param name="visceralFatRating">Visceral fat rating.</param>
    /// <param name="bmi">Body mass index.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Upload response from Garmin Connect.</returns>
    public async Task<WeightUploadResult> AddBodyCompositionAsync(
        double weight,
        DateTime? timestamp = null,
        double? percentFat = null,
        double? percentHydration = null,
        double? visceralFatMass = null,
        double? boneMass = null,
        double? muscleMass = null,
        double? basalMet = null,
        double? activeMet = null,
        double? physiqueRating = null,
        double? metabolicAge = null,
        double? visceralFatRating = null,
        double? bmi = null,
        CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        EnsureAuthenticated();

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(weight);

        var measurementTime = timestamp ?? DateTime.Now;

        var fitData = FitEncoderWeight.CreateWeightFile(
            measurementTime,
            weight,
            percentFat,
            percentHydration,
            visceralFatMass,
            boneMass,
            muscleMass,
            basalMet,
            activeMet,
            physiqueRating,
            metabolicAge,
            visceralFatRating,
            bmi);

        using var stream = new MemoryStream(fitData);

        var response = await _apiClient.PostFileAsync<WeightUploadResponse>(
            Endpoints.ActivityUpload,
            stream,
            "body_composition.fit",
            "application/vnd.ant.fit",
            cancellationToken).ConfigureAwait(false);

        return new WeightUploadResult
        {
            Success = response?.DetailedImportResult?.Successes?.Count > 0,
            CreatedId = response?.DetailedImportResult?.Successes?.FirstOrDefault()?.InternalId
        };
    }

    #endregion

    #region Blood Pressure

    /// <summary>
    /// Adds a blood pressure measurement to Garmin Connect.
    /// </summary>
    /// <param name="systolicPressure">Systolic blood pressure in mmHg.</param>
    /// <param name="diastolicPressure">Diastolic blood pressure in mmHg.</param>
    /// <param name="timestamp">Measurement timestamp (defaults to now).</param>
    /// <param name="heartRate">Heart rate in bpm (optional).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Upload response from Garmin Connect.</returns>
    public async Task<BloodPressureUploadResult> AddBloodPressureAsync(
        int systolicPressure,
        int diastolicPressure,
        DateTime? timestamp = null,
        int? heartRate = null,
        CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        EnsureAuthenticated();

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(systolicPressure);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(diastolicPressure);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(systolicPressure, 300);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(diastolicPressure, 200);

        if (heartRate.HasValue)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(heartRate.Value);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(heartRate.Value, 250);
        }

        var measurementTime = timestamp ?? DateTime.Now;

        var fitData = FitEncoderBloodPressure.CreateBloodPressureFile(
            measurementTime,
            (ushort)systolicPressure,
            (ushort)diastolicPressure,
            heartRate: heartRate.HasValue ? (byte)heartRate.Value : null);

        using var stream = new MemoryStream(fitData);

        var response = await _apiClient.PostFileAsync<BloodPressureUploadResponse>(
            Endpoints.ActivityUpload,
            stream,
            "blood_pressure.fit",
            "application/vnd.ant.fit",
            cancellationToken).ConfigureAwait(false);

        return new BloodPressureUploadResult
        {
            Success = response?.DetailedImportResult?.Successes?.Count > 0,
            CreatedId = response?.DetailedImportResult?.Successes?.FirstOrDefault()?.InternalId
        };
    }

    #endregion

    #region Upload Result DTOs

    private record WeightUploadResponse
    {
        public DetailedImportResult? DetailedImportResult { get; init; }
    }

    private record BloodPressureUploadResponse
    {
        public DetailedImportResult? DetailedImportResult { get; init; }
    }

    #endregion
}

/// <summary>
/// Result of a body composition upload.
/// </summary>
public record WeightUploadResult
{
    /// <summary>
    /// Whether the upload was successful.
    /// </summary>
    public bool Success { get; init; }

    /// <summary>
    /// The ID of the created weight entry, if successful.
    /// </summary>
    public long? CreatedId { get; init; }
}

/// <summary>
/// Result of a blood pressure upload.
/// </summary>
public record BloodPressureUploadResult
{
    /// <summary>
    /// Whether the upload was successful.
    /// </summary>
    public bool Success { get; init; }

    /// <summary>
    /// The ID of the created blood pressure entry, if successful.
    /// </summary>
    public long? CreatedId { get; init; }
}
