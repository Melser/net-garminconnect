using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using GarminConnect.Exceptions;
using Microsoft.Extensions.Logging;

namespace GarminConnect.Api;

/// <summary>
/// Low-level HTTP client for Garmin Connect API.
/// </summary>
public sealed class GarminApiClient : IGarminApiClient
{
    private const string DefaultUserAgent = "GCM-iOS-5.7.2.1";

    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ILogger? _logger;
    private readonly bool _ownsHttpClient;

    private volatile string? _accessToken;
    private bool _disposed;

    /// <summary>
    /// Creates a new instance of GarminApiClient.
    /// </summary>
    /// <param name="httpClient">The HttpClient instance to use.</param>
    /// <param name="ownsHttpClient">
    /// If true, the HttpClient will be disposed when this client is disposed.
    /// Set to false when HttpClient is managed by IHttpClientFactory or DI container.
    /// </param>
    /// <param name="logger">Optional logger instance.</param>
    public GarminApiClient(HttpClient httpClient, bool ownsHttpClient = true, ILogger? logger = null)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _ownsHttpClient = ownsHttpClient;
        _logger = logger;

        // Set default User-Agent if not already set
        if (_httpClient.DefaultRequestHeaders.UserAgent.Count == 0)
        {
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(DefaultUserAgent);
        }

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
        };
    }

    /// <inheritdoc />
    public void SetAccessToken(string? token)
    {
        _accessToken = token;
    }

    /// <inheritdoc />
    public async Task<T> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default)
    {
        using var request = CreateRequest(HttpMethod.Get, endpoint);
        using var response = await SendRequestAsync(request, cancellationToken).ConfigureAwait(false);
        return await DeserializeResponseAsync<T>(response, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<T> PostAsync<T>(string endpoint, object? content = null, CancellationToken cancellationToken = default)
    {
        using var request = CreateRequest(HttpMethod.Post, endpoint, content);
        using var response = await SendRequestAsync(request, cancellationToken).ConfigureAwait(false);
        return await DeserializeResponseAsync<T>(response, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task PostAsync(string endpoint, object? content = null, CancellationToken cancellationToken = default)
    {
        using var request = CreateRequest(HttpMethod.Post, endpoint, content);
        using var response = await SendRequestAsync(request, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<T> PutAsync<T>(string endpoint, object content, CancellationToken cancellationToken = default)
    {
        using var request = CreateRequest(HttpMethod.Put, endpoint, content);
        using var response = await SendRequestAsync(request, cancellationToken).ConfigureAwait(false);
        return await DeserializeResponseAsync<T>(response, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task PutAsync(string endpoint, object content, CancellationToken cancellationToken = default)
    {
        using var request = CreateRequest(HttpMethod.Put, endpoint, content);
        using var response = await SendRequestAsync(request, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(string endpoint, CancellationToken cancellationToken = default)
    {
        using var request = CreateRequest(HttpMethod.Delete, endpoint);
        using var response = await SendRequestAsync(request, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<byte[]> GetBytesAsync(string endpoint, CancellationToken cancellationToken = default)
    {
        using var request = CreateRequest(HttpMethod.Get, endpoint);
        using var response = await SendRequestAsync(request, cancellationToken).ConfigureAwait(false);
        return await response.Content.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<Stream> GetStreamAsync(string endpoint, CancellationToken cancellationToken = default)
    {
        var request = CreateRequest(HttpMethod.Get, endpoint);
        HttpResponseMessage? response = null;
        Stream? innerStream = null;

        try
        {
            response = await SendRequestAsync(request, cancellationToken).ConfigureAwait(false);
            innerStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);

            // HttpResponseStream will dispose both response and innerStream
            return new HttpResponseStream(response, innerStream);
        }
        catch
        {
            // Clean up on failure
            innerStream?.Dispose();
            response?.Dispose();
            request.Dispose();
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<T> PostFileAsync<T>(string endpoint, Stream file, string fileName, string? contentType = null, CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
        AddAuthorizationHeader(request);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        // Use a simple alphanumeric boundary to avoid .NET's default quoted boundary issue.
        // Some servers (including Garmin) don't parse quoted boundaries in Content-Type correctly.
        var boundary = $"----WebKitFormBoundary{Guid.NewGuid():N}";
        var content = new MultipartFormDataContent(boundary);
        // Remove quotes that .NET adds around the boundary parameter
        content.Headers.Remove("Content-Type");
        content.Headers.TryAddWithoutValidation("Content-Type", $"multipart/form-data; boundary={boundary}");

        var streamContent = new StreamContent(file);
        streamContent.Headers.ContentType = new MediaTypeHeaderValue(contentType ?? "application/octet-stream");
        content.Add(streamContent, "file", fileName);
        request.Content = content;

        // Log request details for debugging
        _logger?.LogWarning("Upload request: ContentType={ContentType}, ContentLength={Length}, Boundary={Boundary}",
            content.Headers.ContentType?.ToString(),
            content.Headers.ContentLength,
            boundary);
        _logger?.LogWarning("Upload part headers: ContentDisposition={Disposition}, PartContentType={PartType}, StreamLength={StreamLen}",
            streamContent.Headers.ContentDisposition?.ToString(),
            streamContent.Headers.ContentType?.ToString(),
            file.CanSeek ? file.Length : -1);

        try
        {
            using var response = await SendRequestAsync(request, cancellationToken).ConfigureAwait(false);

            // Log raw response for debugging upload issues
            var rawBody = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            _logger?.LogDebug("Upload response ({StatusCode}): {Body}", (int)response.StatusCode, rawBody);

            // Re-create stream for deserialization since we consumed it
            using var bodyStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(rawBody));
            var result = await JsonSerializer.DeserializeAsync<T>(bodyStream, _jsonOptions, cancellationToken).ConfigureAwait(false);

            if (result is null)
            {
                throw new GarminConnectException("Failed to deserialize response: result is null");
            }

            return result;
        }
        catch (JsonException ex)
        {
            _logger?.LogError(ex, "Failed to deserialize upload response");
            throw new GarminConnectException($"Failed to deserialize upload response: {ex.Message}", ex);
        }
        finally
        {
            request.Dispose();
        }
    }

    private HttpRequestMessage CreateRequest(HttpMethod method, string endpoint, object? content = null)
    {
        var request = new HttpRequestMessage(method, endpoint);

        AddAuthorizationHeader(request);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        if (content is not null)
        {
            request.Content = JsonContent.Create(content, options: _jsonOptions);
        }

        return request;
    }

    private void AddAuthorizationHeader(HttpRequestMessage request)
    {
        // Read volatile field once for thread-safety
        var token = _accessToken;
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    private async Task<HttpResponseMessage> SendRequestAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        _logger?.LogDebug("Sending {Method} request to {Url}", request.Method, request.RequestUri);

        HttpResponseMessage response;
        try
        {
            response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
        catch (HttpRequestException ex)
        {
            _logger?.LogError(ex, "HTTP request failed for {Url}", request.RequestUri);
            throw new GarminConnectConnectionException($"Failed to connect to Garmin API: {ex.Message}", ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger?.LogError(ex, "Request timeout for {Url}", request.RequestUri);
            throw new GarminConnectConnectionException("Request to Garmin API timed out", ex);
        }

        if (!response.IsSuccessStatusCode)
        {
            await HandleErrorResponseAsync(response, cancellationToken).ConfigureAwait(false);
        }

        return response;
    }

    private async Task HandleErrorResponseAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        var statusCode = (int)response.StatusCode;

        _logger?.LogWarning(
            "API request failed with status {StatusCode}: {Content}",
            statusCode,
            content);

        throw response.StatusCode switch
        {
            HttpStatusCode.Unauthorized =>
                new GarminConnectAuthenticationException($"Authentication failed: {content}"),

            HttpStatusCode.TooManyRequests =>
                new GarminConnectTooManyRequestsException(
                    "Rate limit exceeded. Please wait before making more requests.",
                    response.Headers.RetryAfter?.Delta),

            HttpStatusCode.BadRequest =>
                new GarminConnectException($"Bad request: {content}", statusCode),

            HttpStatusCode.NotFound =>
                new GarminConnectException($"Resource not found: {content}", statusCode),

            HttpStatusCode.Forbidden =>
                new GarminConnectException($"Access forbidden: {content}", statusCode),

            HttpStatusCode.InternalServerError =>
                new GarminConnectException($"Garmin server error: {content}", statusCode),

            _ => new GarminConnectConnectionException(
                $"API request failed with status {statusCode}: {content}",
                statusCode)
        };
    }

    private async Task<T> DeserializeResponseAsync<T>(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        // Handle 204 No Content
        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            return default!;
        }

        try
        {
            var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            var result = await JsonSerializer.DeserializeAsync<T>(stream, _jsonOptions, cancellationToken).ConfigureAwait(false);

            if (result is null)
            {
                throw new GarminConnectException("Failed to deserialize response: result is null");
            }

            return result;
        }
        catch (JsonException ex)
        {
            _logger?.LogError(ex, "Failed to deserialize response");
            throw new GarminConnectException($"Failed to deserialize response: {ex.Message}", ex);
        }
    }

    public void Dispose()
    {
        if (_disposed) return;

        if (_ownsHttpClient)
        {
            _httpClient.Dispose();
        }

        _disposed = true;
    }

    public ValueTask DisposeAsync()
    {
        if (_disposed) return ValueTask.CompletedTask;

        if (_ownsHttpClient)
        {
            _httpClient.Dispose();
        }

        _disposed = true;
        return ValueTask.CompletedTask;
    }
}
