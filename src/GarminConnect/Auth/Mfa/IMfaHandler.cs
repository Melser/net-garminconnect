namespace GarminConnect.Auth.Mfa;

/// <summary>
/// Interface for handling Multi-Factor Authentication (MFA) prompts.
/// </summary>
public interface IMfaHandler
{
    /// <summary>
    /// Called when an MFA code is required to complete authentication.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The MFA code entered by the user, or null to cancel authentication.</returns>
    Task<string?> GetMfaCodeAsync(CancellationToken cancellationToken = default);
}
