using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace GarminConnect.Auth.OAuth;

/// <summary>
/// File-based token storage implementation.
/// Stores tokens in JSON files in the specified directory.
/// </summary>
public sealed class FileTokenStore : IOAuthTokenStore, IDisposable
{
    private const string TokenFileName = "tokens.json";
    private const string DefaultDirectoryName = ".garminconnect";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly string _tokenFilePath;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private readonly ILogger<FileTokenStore>? _logger;
    private bool _disposed;

    /// <summary>
    /// Creates a new FileTokenStore with default location (~/.garminconnect/tokens.json).
    /// </summary>
    public FileTokenStore() : this(null, null)
    {
    }

    /// <summary>
    /// Creates a new FileTokenStore with the specified directory or file path.
    /// </summary>
    /// <param name="path">
    /// Path to the token storage. Can be:
    /// - null: uses default location (~/.garminconnect/tokens.json)
    /// - directory path: stores tokens.json in that directory
    /// - file path: stores tokens in that file
    /// </param>
    /// <param name="logger">Optional logger for error reporting.</param>
    public FileTokenStore(string? path, ILogger<FileTokenStore>? logger = null)
    {
        _tokenFilePath = ResolvePath(path);
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<GarminConnectTokens?> LoadAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (!File.Exists(_tokenFilePath))
        {
            return null;
        }

        await _lock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            var json = await File.ReadAllTextAsync(_tokenFilePath, cancellationToken).ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            return JsonSerializer.Deserialize<GarminConnectTokens>(json, JsonOptions);
        }
        catch (JsonException ex)
        {
            _logger?.LogWarning(ex, "Failed to deserialize stored tokens from {FilePath}. Token file may be corrupted", _tokenFilePath);
            return null;
        }
        catch (IOException ex)
        {
            _logger?.LogWarning(ex, "Failed to read token file from {FilePath}. Check file permissions and disk access", _tokenFilePath);
            return null;
        }
        finally
        {
            _lock.Release();
        }
    }

    /// <inheritdoc />
    /// <exception cref="GarminConnect.Exceptions.GarminConnectException">Thrown when token saving fails due to file access or serialization errors.</exception>
    public async Task SaveAsync(GarminConnectTokens tokens, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(tokens);

        try
        {
            var directory = Path.GetDirectoryName(_tokenFilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var tokensWithTimestamp = tokens with { UpdatedAt = DateTimeOffset.UtcNow };
            var json = JsonSerializer.Serialize(tokensWithTimestamp, JsonOptions);

            await _lock.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                await File.WriteAllTextAsync(_tokenFilePath, json, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                _lock.Release();
            }
        }
        catch (IOException ex)
        {
            _logger?.LogError(ex, "Failed to save tokens to {FilePath}. Check disk space and file permissions", _tokenFilePath);
            throw new Exceptions.GarminConnectException($"Failed to save tokens to {_tokenFilePath}. Check disk space and file permissions.", ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger?.LogError(ex, "Access denied when saving tokens to {FilePath}. Check file and directory permissions", _tokenFilePath);
            throw new Exceptions.GarminConnectException($"Access denied when saving tokens to {_tokenFilePath}. Check file and directory permissions.", ex);
        }
        catch (JsonException ex)
        {
            _logger?.LogError(ex, "Failed to serialize tokens for storage at {FilePath}", _tokenFilePath);
            throw new Exceptions.GarminConnectException($"Failed to serialize tokens for storage.", ex);
        }
    }

    /// <inheritdoc />
    public async Task ClearAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        await _lock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (File.Exists(_tokenFilePath))
            {
                File.Delete(_tokenFilePath);
            }
        }
        finally
        {
            _lock.Release();
        }
    }

    /// <inheritdoc />
    public Task<bool> ExistsAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return Task.FromResult(File.Exists(_tokenFilePath));
    }

    /// <summary>
    /// Gets the resolved token file path.
    /// </summary>
    public string TokenFilePath => _tokenFilePath;

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed) return;
        _lock.Dispose();
        _disposed = true;
    }

    private static string ResolvePath(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            // Default: ~/.garminconnect/tokens.json
            var homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            return Path.Combine(homeDir, DefaultDirectoryName, TokenFileName);
        }

        // Expand ~ to home directory
        if (path.StartsWith('~'))
        {
            var homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            path = Path.Combine(homeDir, path[1..].TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        }

        // If path is a directory, append tokens.json
        if (Directory.Exists(path) || !Path.HasExtension(path))
        {
            return Path.Combine(path, TokenFileName);
        }

        return Path.GetFullPath(path);
    }
}
