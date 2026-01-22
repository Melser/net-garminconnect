using GarminConnect.Api;
using GarminConnect.Models;

namespace GarminConnect;

public sealed partial class GarminClient
{
    #region Daily Summary

    /// <inheritdoc />
    public async Task<DailySummary?> GetDailySummaryAsync(DateOnly date, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        EnsureAuthenticated();

        var url = string.Format(Endpoints.DailySummary, FormatDate(date));
        return await _apiClient.GetAsync<DailySummary>(url, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public Task<DailySummary?> GetTodaySummaryAsync(CancellationToken cancellationToken = default)
    {
        return GetDailySummaryAsync(DateOnly.FromDateTime(DateTime.Today), cancellationToken);
    }

    #endregion

    #region Heart Rate

    /// <inheritdoc />
    public async Task<HeartRateData?> GetHeartRatesAsync(DateOnly date, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        EnsureAuthenticated();

        var url = string.Format(Endpoints.DailyHeartRate, FormatDate(date));
        return await _apiClient.GetAsync<HeartRateData>(url, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<int?> GetRestingHeartRateAsync(DateOnly date, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        EnsureAuthenticated();

        var heartRates = await GetHeartRatesAsync(date, cancellationToken).ConfigureAwait(false);
        return heartRates?.RestingHeartRate;
    }

    #endregion

    #region Sleep

    /// <inheritdoc />
    public async Task<SleepData?> GetSleepDataAsync(DateOnly date, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        EnsureAuthenticated();

        var url = $"{Endpoints.DailySleep}?date={FormatDate(date)}&nonSleepBufferMinutes=60";
        return await _apiClient.GetAsync<SleepData>(url, cancellationToken).ConfigureAwait(false);
    }

    #endregion

    #region Stress

    /// <inheritdoc />
    public async Task<StressData?> GetStressDataAsync(DateOnly date, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        EnsureAuthenticated();

        var url = string.Format(Endpoints.DailyStress, FormatDate(date));
        return await _apiClient.GetAsync<StressData>(url, cancellationToken).ConfigureAwait(false);
    }

    #endregion

    #region Body Battery

    /// <inheritdoc />
    public async Task<List<BodyBatteryReport>> GetBodyBatteryAsync(DateOnly startDate, DateOnly? endDate = null, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        EnsureAuthenticated();

        var end = endDate ?? startDate;
        var url = $"{Endpoints.BodyBatteryDaily}?startDate={FormatDate(startDate)}&endDate={FormatDate(end)}";

        var result = await _apiClient.GetAsync<List<BodyBatteryReport>>(url, cancellationToken).ConfigureAwait(false);
        return result ?? [];
    }

    /// <inheritdoc />
    public async Task<List<BodyBatteryEvent>> GetBodyBatteryEventsAsync(DateOnly date, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        EnsureAuthenticated();

        var url = $"{Endpoints.BodyBatteryEvents}/{FormatDate(date)}";
        var result = await _apiClient.GetAsync<List<BodyBatteryEvent>>(url, cancellationToken).ConfigureAwait(false);
        return result ?? [];
    }

    #endregion
}
