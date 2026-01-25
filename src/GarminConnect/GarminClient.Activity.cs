using System.Net.Http.Headers;
using GarminConnect.Api;
using GarminConnect.Exceptions;
using GarminConnect.Models;

namespace GarminConnect;

public sealed partial class GarminClient
{
    private const int DefaultActivityLimit = 20;
    private const int MaxActivityLimit = 1000;
    private const int MaxPaginationIterations = 100; // Safety limit: 100 * 20 = 2000 activities max

    #region Activities - List

    /// <inheritdoc />
    public async Task<List<Activity>> GetActivitiesAsync(int start = 0, int limit = DefaultActivityLimit, string? activityType = null, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        EnsureAuthenticated();

        ArgumentOutOfRangeException.ThrowIfNegative(start);
        ArgumentOutOfRangeException.ThrowIfLessThan(limit, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(limit, MaxActivityLimit);

        var url = $"{Endpoints.Activities}?start={start}&limit={limit}";

        if (!string.IsNullOrEmpty(activityType))
        {
            url += $"&activityType={Uri.EscapeDataString(activityType)}";
        }

        var result = await _apiClient.GetAsync<List<Activity>>(url, cancellationToken).ConfigureAwait(false);
        return result ?? [];
    }

    /// <inheritdoc />
    public async Task<List<Activity>> GetActivitiesByDateAsync(DateOnly startDate, DateOnly? endDate = null, string? activityType = null, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        EnsureAuthenticated();

        var end = endDate ?? DateOnly.FromDateTime(DateTime.Today);
        var activities = new List<Activity>();
        var seenIds = new HashSet<long>();
        var start = 0;
        var iterations = 0;

        while (iterations < MaxPaginationIterations)
        {
            iterations++;

            var url = $"{Endpoints.Activities}?startDate={FormatDate(startDate)}&endDate={FormatDate(end)}&start={start}&limit={DefaultActivityLimit}";

            if (!string.IsNullOrEmpty(activityType))
            {
                url += $"&activityType={Uri.EscapeDataString(activityType)}";
            }

            var batch = await _apiClient.GetAsync<List<Activity>>(url, cancellationToken).ConfigureAwait(false);

            if (batch is null || batch.Count == 0)
            {
                break;
            }

            // Check for duplicates to prevent infinite loops from API issues
            var newActivities = batch.Where(a => seenIds.Add(a.ActivityId)).ToList();
            if (newActivities.Count == 0)
            {
                // All activities in this batch were duplicates - stop to prevent infinite loop
                break;
            }

            activities.AddRange(newActivities);
            start += batch.Count;

            // If we got fewer than the limit, we've reached the end
            if (batch.Count < DefaultActivityLimit)
            {
                break;
            }
        }

        return activities;
    }

    /// <inheritdoc />
    public async Task<Activity?> GetLastActivityAsync(CancellationToken cancellationToken = default)
    {
        var activities = await GetActivitiesAsync(0, 1, null, cancellationToken).ConfigureAwait(false);
        return activities.Count > 0 ? activities[0] : null;
    }

    #endregion

    #region Activities - Details

    /// <inheritdoc />
    public async Task<Activity?> GetActivityAsync(long activityId, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        EnsureAuthenticated();

        var url = string.Format(Endpoints.Activity, activityId);
        return await _apiClient.GetAsync<Activity>(url, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<ActivityDetails?> GetActivityDetailsAsync(long activityId, int maxChartSize = 2000, int maxPolySize = 4000, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        EnsureAuthenticated();

        var url = $"{string.Format(Endpoints.ActivityDetails, activityId)}?maxChartSize={maxChartSize}&maxPolylineSize={maxPolySize}";
        return await _apiClient.GetAsync<ActivityDetails>(url, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<ActivitySplits?> GetActivitySplitsAsync(long activityId, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        EnsureAuthenticated();

        var url = string.Format(Endpoints.ActivitySplits, activityId);
        return await _apiClient.GetAsync<ActivitySplits>(url, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<ActivityWeather?> GetActivityWeatherAsync(long activityId, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        EnsureAuthenticated();

        var url = string.Format(Endpoints.ActivityWeather, activityId);
        return await _apiClient.GetAsync<ActivityWeather>(url, cancellationToken).ConfigureAwait(false);
    }

    #endregion

    #region Activities - Download/Upload

    /// <inheritdoc />
    public async Task<byte[]> DownloadActivityAsync(long activityId, ActivityFileFormat format = ActivityFileFormat.Fit, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        EnsureAuthenticated();

        var url = format switch
        {
            ActivityFileFormat.Fit => string.Format(Endpoints.ActivityDownloadFit, activityId),
            ActivityFileFormat.Tcx => string.Format(Endpoints.ActivityDownloadTcx, activityId),
            ActivityFileFormat.Gpx => string.Format(Endpoints.ActivityDownloadGpx, activityId),
            ActivityFileFormat.Kml => string.Format(Endpoints.ActivityDownloadKml, activityId),
            ActivityFileFormat.Csv => string.Format(Endpoints.ActivityDownloadCsv, activityId),
            _ => throw new ArgumentOutOfRangeException(nameof(format))
        };

        return await _apiClient.GetBytesAsync(url, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<long> UploadActivityAsync(byte[] fileData, string fileName, ActivityFileFormat format = ActivityFileFormat.Fit, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(fileData);
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);

        using var stream = new MemoryStream(fileData);
        return await UploadActivityAsync(stream, fileName, format, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<long> UploadActivityAsync(Stream fileStream, string fileName, ActivityFileFormat format = ActivityFileFormat.Fit, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        EnsureAuthenticated();
        ArgumentNullException.ThrowIfNull(fileStream);
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);

        var contentType = format switch
        {
            ActivityFileFormat.Fit => "application/vnd.ant.fit",
            ActivityFileFormat.Tcx => "application/vnd.garmin.tcx+xml",
            ActivityFileFormat.Gpx => "application/gpx+xml",
            _ => throw new GarminConnectInvalidFileFormatException($"Upload not supported for format: {format}", fileName)
        };

        var response = await _apiClient.PostFileAsync<UploadResponse>(
            Endpoints.ActivityUpload,
            fileStream,
            fileName,
            contentType,
            cancellationToken).ConfigureAwait(false);

        if (response?.DetailedImportResult?.Successes?.Count > 0)
        {
            return response.DetailedImportResult.Successes[0].InternalId;
        }

        if (response?.DetailedImportResult?.Failures?.Count > 0)
        {
            var failure = response.DetailedImportResult.Failures[0];
            throw new GarminConnectException($"Activity upload failed: {failure.Messages?[0]?.Content ?? "Unknown error"}");
        }

        throw new GarminConnectException("Activity upload failed: No response from server");
    }

    /// <inheritdoc />
    public async Task DeleteActivityAsync(long activityId, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        EnsureAuthenticated();

        var url = string.Format(Endpoints.Activity, activityId);
        await _apiClient.DeleteAsync(url, cancellationToken).ConfigureAwait(false);
    }

    #endregion

    #region Upload Response DTOs

    private record UploadResponse
    {
        public DetailedImportResult? DetailedImportResult { get; init; }
    }

    private record DetailedImportResult
    {
        public List<UploadSuccess>? Successes { get; init; }
        public List<UploadFailure>? Failures { get; init; }
    }

    private record UploadSuccess
    {
        public long InternalId { get; init; }
        public long ExternalId { get; init; }
    }

    private record UploadFailure
    {
        public long InternalId { get; init; }
        public long ExternalId { get; init; }
        public List<UploadMessage>? Messages { get; init; }
    }

    private record UploadMessage
    {
        public int Code { get; init; }
        public string? Content { get; init; }
    }

    #endregion
}
