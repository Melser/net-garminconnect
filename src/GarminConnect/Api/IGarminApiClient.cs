namespace GarminConnect.Api;

/// <summary>
/// Low-level HTTP client interface for Garmin Connect API.
/// </summary>
public interface IGarminApiClient : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Performs a GET request and deserializes the response.
    /// </summary>
    Task<T> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs a POST request with optional content and deserializes the response.
    /// </summary>
    Task<T> PostAsync<T>(string endpoint, object? content = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs a POST request with optional content.
    /// </summary>
    Task PostAsync(string endpoint, object? content = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs a PUT request with content and deserializes the response.
    /// </summary>
    Task<T> PutAsync<T>(string endpoint, object content, CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs a PUT request with content.
    /// </summary>
    Task PutAsync(string endpoint, object content, CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs a DELETE request.
    /// </summary>
    Task DeleteAsync(string endpoint, CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs a GET request and returns raw bytes.
    /// </summary>
    Task<byte[]> GetBytesAsync(string endpoint, CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs a GET request and returns a stream.
    /// </summary>
    Task<Stream> GetStreamAsync(string endpoint, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uploads a file using POST request.
    /// </summary>
    /// <param name="endpoint">API endpoint.</param>
    /// <param name="file">File stream.</param>
    /// <param name="fileName">File name.</param>
    /// <param name="contentType">Content type (default: application/octet-stream).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<T> PostFileAsync<T>(string endpoint, Stream file, string fileName, string? contentType = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the access token for authenticated requests.
    /// </summary>
    void SetAccessToken(string? token);
}
