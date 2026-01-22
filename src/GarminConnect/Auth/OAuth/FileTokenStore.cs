using System.Text.Json;

namespace GarminConnect.Auth.OAuth;

/// <summary>
/// File-based token storage implementation.
/// Stores tokens in JSON files in the specified directory.
/// </summary>
public sealed class FileTokenStore : IOAuthTokenStore
{
    private const string TokenFileName = "tokens.json";
    private const string DefaultDirectoryName = ".garminconnect";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly string _tokenFilePath;
    private readonly object _fileLock = new();

    /// <summary>
    /// Creates a new FileTokenStore with default location (~/.garminconnect/tokens.json).
    /// </summary>
    public FileTokenStore() : this(null)
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
    public FileTokenStore(string? path)
    {
        _tokenFilePath = ResolvePath(path);
    }

    /// <inheritdoc />
    public async Task<GarminConnectTokens?> LoadAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(_tokenFilePath))
        {
            return null;
        }

        try
        {
            var json = await File.ReadAllTextAsync(_tokenFilePath, cancellationToken).ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            return JsonSerializer.Deserialize<GarminConnectTokens>(json, JsonOptions);
        }
        catch (JsonException)
        {
            // Invalid JSON - treat as no tokens
            return null;
        }
        catch (IOException)
        {
            // File access error - treat as no tokens
            return null;
        }
    }

    /// <inheritdoc />
    public async Task SaveAsync(GarminConnectTokens tokens, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(tokens);

        var directory = Path.GetDirectoryName(_tokenFilePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var tokensWithTimestamp = tokens with { UpdatedAt = DateTimeOffset.UtcNow };
        var json = JsonSerializer.Serialize(tokensWithTimestamp, JsonOptions);

        // Use lock to prevent concurrent writes
        lock (_fileLock)
        {
            File.WriteAllText(_tokenFilePath, json);
        }

        await Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task ClearAsync(CancellationToken cancellationToken = default)
    {
        if (File.Exists(_tokenFilePath))
        {
            File.Delete(_tokenFilePath);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<bool> ExistsAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(File.Exists(_tokenFilePath));
    }

    /// <summary>
    /// Gets the resolved token file path.
    /// </summary>
    public string TokenFilePath => _tokenFilePath;

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
