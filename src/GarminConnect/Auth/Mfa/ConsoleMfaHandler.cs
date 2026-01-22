namespace GarminConnect.Auth.Mfa;

/// <summary>
/// MFA handler that prompts for code via console input.
/// </summary>
public sealed class ConsoleMfaHandler : IMfaHandler
{
    private readonly string _prompt;

    /// <summary>
    /// Creates a new ConsoleMfaHandler with default prompt.
    /// </summary>
    public ConsoleMfaHandler() : this("Enter MFA code: ")
    {
    }

    /// <summary>
    /// Creates a new ConsoleMfaHandler with custom prompt.
    /// </summary>
    /// <param name="prompt">The prompt message to display.</param>
    public ConsoleMfaHandler(string prompt)
    {
        _prompt = prompt ?? throw new ArgumentNullException(nameof(prompt));
    }

    /// <inheritdoc />
    public Task<string?> GetMfaCodeAsync(CancellationToken cancellationToken = default)
    {
        Console.Write(_prompt);
        var code = Console.ReadLine();

        return Task.FromResult(string.IsNullOrWhiteSpace(code) ? null : code.Trim());
    }
}
