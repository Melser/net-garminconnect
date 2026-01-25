namespace GarminConnect.Auth;

/// <summary>
/// Result of an authentication attempt.
/// </summary>
public sealed record AuthResult
{
    /// <summary>
    /// Whether authentication was successful.
    /// </summary>
    public required bool Success { get; init; }

    /// <summary>
    /// Whether MFA is required to complete authentication.
    /// </summary>
    public bool RequiresMfa { get; init; }

    /// <summary>
    /// Error message if authentication failed.
    /// </summary>
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// Client state for resuming MFA authentication.
    /// Only set when RequiresMfa is true.
    /// </summary>
    public string? MfaClientState { get; init; }

    /// <summary>
    /// Creates a successful authentication result.
    /// </summary>
    public static AuthResult Succeeded() => new() { Success = true };

    /// <summary>
    /// Creates a failed authentication result.
    /// </summary>
    /// <param name="errorMessage">The error message.</param>
    public static AuthResult Failed(string errorMessage) => new()
    {
        Success = false,
        ErrorMessage = errorMessage
    };

    /// <summary>
    /// Creates a result indicating MFA is required.
    /// </summary>
    /// <param name="clientState">The client state for resuming authentication.</param>
    public static AuthResult MfaRequired(string clientState) => new()
    {
        Success = false,
        RequiresMfa = true,
        MfaClientState = clientState
    };
}
