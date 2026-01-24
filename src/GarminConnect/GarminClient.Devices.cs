using System.Text.Json;
using GarminConnect.Api;

namespace GarminConnect;

public sealed partial class GarminClient
{
    #region Devices

    /// <summary>
    /// Gets the list of devices registered to the user's Garmin Connect account.
    /// </summary>
    public async Task<JsonElement> GetDevicesAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        EnsureAuthenticated();

        return await _apiClient.GetAsync<JsonElement>(Endpoints.Devices, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets device settings for a specific device.
    /// </summary>
    /// <param name="deviceId">Device ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<JsonElement> GetDeviceSettingsAsync(string deviceId, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        EnsureAuthenticated();

        ArgumentException.ThrowIfNullOrWhiteSpace(deviceId, nameof(deviceId));

        var url = string.Format(Endpoints.DeviceSettings, deviceId);
        return await _apiClient.GetAsync<JsonElement>(url, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the primary training device.
    /// </summary>
    public async Task<JsonElement> GetPrimaryTrainingDeviceAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        EnsureAuthenticated();

        return await _apiClient.GetAsync<JsonElement>(Endpoints.PrimaryTrainingDevice, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets solar data for a specific device and date range.
    /// </summary>
    /// <param name="deviceId">Device ID.</param>
    /// <param name="startDate">Start date.</param>
    /// <param name="endDate">End date.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<JsonElement> GetDeviceSolarDataAsync(
        string deviceId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        EnsureAuthenticated();

        ArgumentException.ThrowIfNullOrWhiteSpace(deviceId, nameof(deviceId));

        if (startDate > endDate)
        {
            throw new ArgumentException("startDate cannot be after endDate", nameof(startDate));
        }

        var url = string.Format(
            Endpoints.DeviceSolarData,
            deviceId,
            startDate.ToString("yyyy-MM-dd"),
            endDate.ToString("yyyy-MM-dd"));

        return await _apiClient.GetAsync<JsonElement>(url, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the last used device information.
    /// </summary>
    public async Task<JsonElement> GetDeviceLastUsedAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        EnsureAuthenticated();

        return await _apiClient.GetAsync<JsonElement>(Endpoints.DeviceLastUsed, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets list of active alarms from all devices.
    /// This is a convenience method that aggregates alarms from all devices.
    /// </summary>
    public async Task<List<JsonElement>> GetDeviceAlarmsAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        EnsureAuthenticated();

        var alarms = new List<JsonElement>();
        var devicesJson = await GetDevicesAsync(cancellationToken).ConfigureAwait(false);

        if (devicesJson.ValueKind == JsonValueKind.Array)
        {
            foreach (var device in devicesJson.EnumerateArray())
            {
                if (device.TryGetProperty("deviceId", out var deviceIdProp))
                {
                    var deviceId = deviceIdProp.GetString();
                    if (!string.IsNullOrEmpty(deviceId))
                    {
                        var settings = await GetDeviceSettingsAsync(deviceId, cancellationToken).ConfigureAwait(false);
                        if (settings.TryGetProperty("alarms", out var deviceAlarms) &&
                            deviceAlarms.ValueKind == JsonValueKind.Array)
                        {
                            alarms.AddRange(deviceAlarms.EnumerateArray());
                        }
                    }
                }
            }
        }

        return alarms;
    }

    #endregion
}
