using System.Globalization;
using System.Net.Http.Headers;
using System.Text.Json;
using GarminConnect.Api;
using GarminConnect.Exceptions;
using GarminConnect.Models;
using Microsoft.Extensions.Logging;

namespace GarminConnect;

public sealed partial class GarminClient
{
    private const int DefaultActivityLimit = 20;
    private const int MaxActivityLimit = 1000;
    private const int MaxPaginationIterations = 100; // Safety limit: 100 * 20 = 2000 activities max
    private const int UploadPollIntervalMs = 1000;
    private const int UploadPollMaxAttempts = 30; // 30 seconds max wait

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

        var (contentType, formatSuffix) = format switch
        {
            ActivityFileFormat.Fit => ("application/vnd.ant.fit", "/.fit"),
            ActivityFileFormat.Tcx => ("application/vnd.garmin.tcx+xml", "/.tcx"),
            ActivityFileFormat.Gpx => ("application/gpx+xml", "/.gpx"),
            _ => throw new GarminConnectInvalidFileFormatException($"Upload not supported for format: {format}", fileName)
        };

        // Garmin expects format suffix in URL (e.g., /upload-service/upload/.tcx)
        var uploadUrl = Endpoints.ActivityUpload + formatSuffix;

        var response = await _apiClient.PostFileAsync<UploadResponse>(
            uploadUrl,
            fileStream,
            fileName,
            contentType,
            cancellationToken).ConfigureAwait(false);

        var result = response?.DetailedImportResult;

        if (result is null)
            throw new GarminConnectException("Activity upload failed: No response from server");

        if (result.Failures?.Count > 0)
        {
            var failure = result.Failures[0];
            throw new GarminConnectException($"Activity upload failed: {failure.Messages?[0]?.Content ?? "Unknown error"}");
        }

        if (result.Successes?.Count > 0)
            return result.Successes[0].InternalId;

        if (result.CreatedId is > 0)
            return result.CreatedId.Value;

        // Garmin upload is async (HTTP 202) - poll status endpoint for activity ID
        if (result.UploadUuid?.Uuid is not null && result.CreationDate is not null)
        {
            return await PollUploadStatusAsync(result, cancellationToken).ConfigureAwait(false);
        }

        _logger?.LogWarning(
            "Upload returned no activity ID and no UUID for polling. UploadId={UploadId}, CreatedId={CreatedId}",
            result.UploadId, result.CreatedId);

        throw new GarminConnectException(
            $"Activity upload completed but no activity ID returned (uploadId={result.UploadId}, createdId={result.CreatedId})");
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

    #region Upload Status Polling

    private async Task<long> PollUploadStatusAsync(DetailedImportResult result, CancellationToken cancellationToken)
    {
        var uuid = result.UploadUuid!.Uuid!;
        var uuidNoDashes = uuid.Replace("-", "");
        var epochMs = ParseCreationDateToEpochMs(result.CreationDate!);
        var statusUrl = string.Format(Endpoints.ActivityUploadStatus, epochMs, uuidNoDashes);

        _logger?.LogInformation(
            "Upload is processing async (uploadId={UploadId}), polling status...",
            result.UploadId);

        for (var attempt = 1; attempt <= UploadPollMaxAttempts; attempt++)
        {
            await Task.Delay(UploadPollIntervalMs, cancellationToken).ConfigureAwait(false);

            try
            {
                var statusJson = await _apiClient.GetAsync<JsonElement>(statusUrl, cancellationToken).ConfigureAwait(false);

                if (TryExtractActivityResult(statusJson, out var activityId, out var errorMessage))
                {
                    if (activityId > 0)
                    {
                        _logger?.LogInformation(
                            "Upload processing complete, activityId={ActivityId} (poll attempt {Attempt})",
                            activityId, attempt);
                        return activityId;
                    }

                    throw new GarminConnectException($"Activity upload failed during processing: {errorMessage}");
                }

                _logger?.LogDebug("Upload still processing (attempt {Attempt}/{Max})", attempt, UploadPollMaxAttempts);
            }
            catch (GarminConnectException ex) when (ex.Message.Contains("server error", StringComparison.OrdinalIgnoreCase))
            {
                // Status endpoint may return HTTP 500 with failure details in the body
                // Try to extract failure info from the error message (contains the JSON body)
                if (TryParseFailureFromErrorBody(ex.Message, out var failureMsg))
                {
                    throw new GarminConnectException($"Activity upload failed during processing: {failureMsg}");
                }

                throw;
            }
            catch (GarminConnectException) { throw; }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Error polling upload status (attempt {Attempt})", attempt);
            }
        }

        throw new GarminConnectException(
            $"Activity upload timed out after {UploadPollMaxAttempts}s waiting for processing (uploadId={result.UploadId})");
    }

    private static bool TryExtractActivityResult(JsonElement json, out long activityId, out string errorMessage)
    {
        activityId = 0;
        errorMessage = "Unknown error";

        // Response might wrap in detailedImportResult or be the result directly
        var target = json.TryGetProperty("detailedImportResult", out var importResult)
            ? importResult
            : json;

        if (TryGetActivityIdFromJson(target, out activityId))
            return true;

        if (HasFailuresInJson(target, out errorMessage))
        {
            activityId = 0;
            return true; // Found a result (failure)
        }

        return false; // Still processing
    }

    private static bool TryParseFailureFromErrorBody(string errorMessage, out string failureMessage)
    {
        failureMessage = "Unknown error";

        // Error body might contain JSON like: {"detailedImportResult":{...,"failures":[...]}}
        var jsonStart = errorMessage.IndexOf('{');
        if (jsonStart < 0) return false;

        try
        {
            var json = JsonDocument.Parse(errorMessage[jsonStart..]);
            var root = json.RootElement;
            var target = root.TryGetProperty("detailedImportResult", out var importResult)
                ? importResult
                : root;

            return HasFailuresInJson(target, out failureMessage);
        }
        catch
        {
            return false;
        }
    }

    private static bool TryGetActivityIdFromJson(JsonElement json, out long activityId)
    {
        activityId = 0;

        if (json.TryGetProperty("successes", out var successes) &&
            successes.ValueKind == JsonValueKind.Array &&
            successes.GetArrayLength() > 0)
        {
            var first = successes[0];
            if (first.TryGetProperty("internalId", out var internalId) &&
                internalId.ValueKind == JsonValueKind.Number)
            {
                activityId = internalId.GetInt64();
                return activityId > 0;
            }
        }

        if (json.TryGetProperty("createdId", out var createdId) &&
            createdId.ValueKind == JsonValueKind.Number)
        {
            activityId = createdId.GetInt64();
            return activityId > 0;
        }

        return false;
    }

    private static bool HasFailuresInJson(JsonElement json, out string errorMessage)
    {
        errorMessage = "Unknown error";
        if (json.TryGetProperty("failures", out var failures) &&
            failures.ValueKind == JsonValueKind.Array &&
            failures.GetArrayLength() > 0)
        {
            var first = failures[0];
            if (first.TryGetProperty("messages", out var messages) &&
                messages.ValueKind == JsonValueKind.Array &&
                messages.GetArrayLength() > 0 &&
                messages[0].TryGetProperty("content", out var content))
            {
                errorMessage = content.GetString() ?? "Unknown error";
            }

            return true;
        }

        return false;
    }

    private static long ParseCreationDateToEpochMs(string creationDate)
    {
        // Format varies: "2023-09-29 01:58:19.113 GMT", "2023-09-29 01:58:19.82 GMT", etc.
        var cleaned = creationDate.Replace(" GMT", "").Trim();
        var dt = DateTimeOffset.Parse(cleaned, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
        return dt.ToUnixTimeMilliseconds();
    }

    #endregion

    #region Upload Response DTOs

    private record UploadResponse
    {
        public DetailedImportResult? DetailedImportResult { get; init; }
    }

    private record DetailedImportResult
    {
        public long? CreatedId { get; init; }
        public long? UploadId { get; init; }
        public UploadUuidWrapper? UploadUuid { get; init; }
        public string? CreationDate { get; init; }
        public List<UploadSuccess>? Successes { get; init; }
        public List<UploadFailure>? Failures { get; init; }
    }

    private record UploadUuidWrapper
    {
        public string? Uuid { get; init; }
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
