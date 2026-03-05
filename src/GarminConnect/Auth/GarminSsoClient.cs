using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Web;
using GarminConnect.Auth.OAuth;
using GarminConnect.Exceptions;
using Microsoft.Extensions.Logging;

namespace GarminConnect.Auth;

/// <summary>
/// Client for Garmin SSO (Single Sign-On) authentication.
/// Handles OAuth flow with Garmin's authentication servers.
/// </summary>
internal sealed partial class GarminSsoClient : IDisposable
{
    private const string UserAgent = "GCM-iOS-5.7.2.1";

    // SSO Endpoints
    private const string SsoEmbedUrl = "https://sso.garmin.com/sso/embed";
    private const string SsoSigninUrl = "https://sso.garmin.com/sso/signin";
    private const string SsoMfaUrl = "https://sso.garmin.com/sso/verifyMFA/loginEnterMfaCode";

    // OAuth Endpoints
    private const string OAuth1RequestTokenUrl = "https://connectapi.garmin.com/oauth-service/oauth/request_token";
    private const string OAuth1AccessTokenUrl = "https://connectapi.garmin.com/oauth-service/oauth/access_token";
    private const string OAuth2ExchangeUrl = "https://connectapi.garmin.com/oauth-service/oauth/exchange/user/2.0";

    // OAuth Consumer (public, used by official Garmin apps)
    private const string ConsumerKey = "fc3e99d2-118c-44b8-8ae3-03370dde24c0";
    private const string ConsumerSecret = "E08WAR897WEy2knn7aFBrvegVAf0AFdWBBF";

    private readonly HttpClient _httpClient;
    private readonly ILogger? _logger;
    private readonly CookieContainer _cookies;

    private string? _mfaClientState;

    public GarminSsoClient(ILogger? logger = null)
    {
        _logger = logger;
        _cookies = new CookieContainer();

        var handler = new HttpClientHandler
        {
            CookieContainer = _cookies,
            AllowAutoRedirect = true,
            UseCookies = true
        };

        _httpClient = new HttpClient(handler)
        {
            Timeout = TimeSpan.FromSeconds(30)
        };

        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    /// <summary>
    /// Authenticates with Garmin SSO using email and password.
    /// </summary>
    /// <returns>Tokens if successful, null if MFA is required (check MfaClientState).</returns>
    public async Task<GarminConnectTokens?> LoginAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Starting Garmin SSO login for {Email}", email);

        // Step 1: Get SSO login page to establish session and get CSRF token
        var (csrfToken, loginParams) = await GetLoginPageAsync(cancellationToken).ConfigureAwait(false);

        // Step 2: Submit credentials
        var loginResult = await SubmitCredentialsAsync(email, password, csrfToken, loginParams, cancellationToken).ConfigureAwait(false);

        if (loginResult.RequiresMfa)
        {
            _mfaClientState = loginResult.MfaClientState;
            _logger?.LogInformation("MFA required for login");
            return null;
        }

        if (loginResult.Ticket is null)
        {
            throw new GarminConnectAuthenticationException("Login failed: no ticket received");
        }

        // Step 3: Exchange ticket for OAuth tokens
        return await ExchangeTicketForTokensAsync(loginResult.Ticket, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Completes MFA authentication.
    /// </summary>
    public async Task<GarminConnectTokens> CompleteMfaAsync(
        string mfaCode,
        string? clientState = null,
        CancellationToken cancellationToken = default)
    {
        var state = clientState ?? _mfaClientState
            ?? throw new InvalidOperationException("No MFA client state available. Call LoginAsync first.");

        _logger?.LogDebug("Completing MFA authentication");

        var ticket = await SubmitMfaCodeAsync(mfaCode, state, cancellationToken).ConfigureAwait(false);

        return await ExchangeTicketForTokensAsync(ticket, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Refreshes OAuth2 token using OAuth1 credentials.
    /// </summary>
    public async Task<OAuth2Token> RefreshOAuth2TokenAsync(
        OAuth1Token oauth1,
        CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Refreshing OAuth2 token");

        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        var nonce = Guid.NewGuid().ToString("N");

        var oauthParams = new Dictionary<string, string>
        {
            ["oauth_consumer_key"] = ConsumerKey,
            ["oauth_token"] = oauth1.Token,
            ["oauth_signature_method"] = "HMAC-SHA1",
            ["oauth_timestamp"] = timestamp,
            ["oauth_nonce"] = nonce,
            ["oauth_version"] = "1.0"
        };

        var signature = GenerateOAuthSignature("POST", OAuth2ExchangeUrl, oauthParams, oauth1.TokenSecret);
        oauthParams["oauth_signature"] = signature;

        using var request = new HttpRequestMessage(HttpMethod.Post, OAuth2ExchangeUrl);
        request.Headers.Authorization = new AuthenticationHeaderValue("OAuth", BuildOAuthHeader(oauthParams));

        using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            throw new GarminConnectAuthenticationException($"Failed to refresh OAuth2 token: {error}");
        }

        var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        var token = JsonSerializer.Deserialize<OAuth2Token>(json)
            ?? throw new GarminConnectAuthenticationException("Failed to parse OAuth2 token response");

        return token;
    }

    /// <summary>
    /// Gets the MFA client state if MFA is pending.
    /// </summary>
    public string? MfaClientState => _mfaClientState;

    private async Task<(string CsrfToken, Dictionary<string, string> Params)> GetLoginPageAsync(
        CancellationToken cancellationToken)
    {
        var embedParams = new Dictionary<string, string>
        {
            ["id"] = "gauth-widget",
            ["embedWidget"] = "true",
            ["gauthHost"] = "https://sso.garmin.com/sso/embed"
        };

        var url = $"{SsoEmbedUrl}?{BuildQueryString(embedParams)}";
        var html = await _httpClient.GetStringAsync(url, cancellationToken).ConfigureAwait(false);

        // Extract CSRF token
        var csrfMatch = CsrfTokenRegex().Match(html);
        var csrfToken = csrfMatch.Success ? csrfMatch.Groups[1].Value : "";

        // Extract form parameters
        var serviceUrl = ServiceUrlRegex().Match(html);
        var formParams = new Dictionary<string, string>();

        if (serviceUrl.Success)
        {
            formParams["service"] = HttpUtility.HtmlDecode(serviceUrl.Groups[1].Value);
        }

        return (csrfToken, formParams);
    }

    private async Task<LoginResult> SubmitCredentialsAsync(
        string email,
        string password,
        string csrfToken,
        Dictionary<string, string> loginParams,
        CancellationToken cancellationToken)
    {
        var formData = new Dictionary<string, string>
        {
            ["username"] = email,
            ["password"] = password,
            ["embed"] = "true",
            ["_csrf"] = csrfToken
        };

        foreach (var (key, value) in loginParams)
        {
            formData[key] = value;
        }

        using var content = new FormUrlEncodedContent(formData);
        using var response = await _httpClient.PostAsync(SsoSigninUrl, content, cancellationToken).ConfigureAwait(false);

        var html = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        _logger?.LogDebug("Garmin SSO response status: {StatusCode}, body length: {Length}",
            response.StatusCode, html.Length);

        // Check for Forbidden first — clear auth failure
        if (response.StatusCode == HttpStatusCode.Forbidden)
        {
            _logger?.LogWarning("Garmin SSO returned 403 Forbidden");
            throw new GarminConnectAuthenticationException("Invalid email or password");
        }

        // Check for MFA requirement
        if (html.Contains("MFA") || html.Contains("verifyMFA") || html.Contains("mfa-code"))
        {
            var stateMatch = MfaStateRegex().Match(html);
            var state = stateMatch.Success ? stateMatch.Groups[1].Value : Guid.NewGuid().ToString();

            return new LoginResult { RequiresMfa = true, MfaClientState = state };
        }

        // Check for ticket in response body
        var ticketMatch = TicketRegex().Match(html);
        if (ticketMatch.Success)
        {
            return new LoginResult { Ticket = ticketMatch.Groups[1].Value };
        }

        // Try to extract ticket from redirect
        var location = response.Headers.Location?.ToString();
        if (location is not null)
        {
            _logger?.LogDebug("Garmin SSO redirect location: {Location}", location);

            var ticketFromUrl = TicketRegex().Match(location);
            if (ticketFromUrl.Success)
            {
                return new LoginResult { Ticket = ticketFromUrl.Groups[1].Value };
            }
        }

        // Check for specific authentication error indicators in the HTML.
        // Garmin SSO shows errors in elements with class "status-error" or specific error message divs.
        // We check for concrete error patterns rather than generic "error"/"invalid" substrings
        // that would false-positive on CSS classes and JavaScript code.
        var authErrorMatch = AuthErrorRegex().Match(html);
        if (authErrorMatch.Success)
        {
            var errorText = (authErrorMatch.Groups[1].Value + authErrorMatch.Groups[2].Value + authErrorMatch.Groups[3].Value).Trim();
            _logger?.LogWarning("Garmin SSO authentication error: {Error}", errorText);
            throw new GarminConnectAuthenticationException(
                string.IsNullOrWhiteSpace(errorText) ? "Invalid email or password" : errorText);
        }

        // Fallback: log the response for debugging and throw a descriptive error
        _logger?.LogWarning(
            "Garmin SSO login: no ticket, no redirect, no recognized error. Status={StatusCode}, BodyPreview={Preview}",
            response.StatusCode,
            html.Length > 500 ? html[..500] : html);
        throw new GarminConnectAuthenticationException("Login failed: unable to extract authentication ticket");
    }

    private async Task<string> SubmitMfaCodeAsync(
        string mfaCode,
        string clientState,
        CancellationToken cancellationToken)
    {
        var formData = new Dictionary<string, string>
        {
            ["mfa-code"] = mfaCode,
            ["embed"] = "true",
            ["fromPage"] = "setupEnterMfaCode"
        };

        using var content = new FormUrlEncodedContent(formData);
        using var response = await _httpClient.PostAsync(SsoMfaUrl, content, cancellationToken).ConfigureAwait(false);

        if (response.StatusCode == HttpStatusCode.TooManyRequests)
        {
            throw new GarminConnectTooManyRequestsException("Too many MFA attempts. Please wait before trying again.");
        }

        var html = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        // Check for ticket
        var ticketMatch = TicketRegex().Match(html);
        if (ticketMatch.Success)
        {
            _mfaClientState = null;
            return ticketMatch.Groups[1].Value;
        }

        // Check redirect for ticket
        var location = response.Headers.Location?.ToString();
        if (location is not null)
        {
            var ticketFromUrl = TicketRegex().Match(location);
            if (ticketFromUrl.Success)
            {
                _mfaClientState = null;
                return ticketFromUrl.Groups[1].Value;
            }
        }

        var mfaErrorMatch = AuthErrorRegex().Match(html);
        if (mfaErrorMatch.Success)
        {
            var errorText = (mfaErrorMatch.Groups[1].Value + mfaErrorMatch.Groups[2].Value + mfaErrorMatch.Groups[3].Value).Trim();
            throw new GarminConnectAuthenticationException(
                string.IsNullOrWhiteSpace(errorText) ? "Invalid MFA code" : errorText);
        }

        throw new GarminConnectAuthenticationException("MFA verification failed: unable to extract ticket");
    }

    private async Task<GarminConnectTokens> ExchangeTicketForTokensAsync(
        string ticket,
        CancellationToken cancellationToken)
    {
        _logger?.LogDebug("Exchanging ticket for OAuth tokens");

        // Step 1: Get OAuth1 request token
        var oauth1RequestToken = await GetOAuth1RequestTokenAsync(cancellationToken).ConfigureAwait(false);

        // Step 2: Authorize with ticket
        await AuthorizeOAuth1TokenAsync(oauth1RequestToken, ticket, cancellationToken).ConfigureAwait(false);

        // Step 3: Get OAuth1 access token
        var oauth1 = await GetOAuth1AccessTokenAsync(oauth1RequestToken, cancellationToken).ConfigureAwait(false);

        // Step 4: Exchange for OAuth2 token
        var oauth2 = await RefreshOAuth2TokenAsync(oauth1, cancellationToken).ConfigureAwait(false);

        return new GarminConnectTokens
        {
            OAuth1 = oauth1,
            OAuth2 = oauth2,
            UpdatedAt = DateTimeOffset.UtcNow
        };
    }

    private async Task<OAuth1RequestToken> GetOAuth1RequestTokenAsync(CancellationToken cancellationToken)
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        var nonce = Guid.NewGuid().ToString("N");

        var oauthParams = new Dictionary<string, string>
        {
            ["oauth_consumer_key"] = ConsumerKey,
            ["oauth_signature_method"] = "HMAC-SHA1",
            ["oauth_timestamp"] = timestamp,
            ["oauth_nonce"] = nonce,
            ["oauth_version"] = "1.0",
            ["oauth_callback"] = "https://connect.garmin.com/modern/"
        };

        var signature = GenerateOAuthSignature("POST", OAuth1RequestTokenUrl, oauthParams, null);
        oauthParams["oauth_signature"] = signature;

        using var request = new HttpRequestMessage(HttpMethod.Post, OAuth1RequestTokenUrl);
        request.Headers.Authorization = new AuthenticationHeaderValue("OAuth", BuildOAuthHeader(oauthParams));

        using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var body = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            throw new GarminConnectAuthenticationException($"Failed to get OAuth1 request token: {body}");
        }

        var parsed = HttpUtility.ParseQueryString(body);
        return new OAuth1RequestToken
        {
            Token = parsed["oauth_token"] ?? throw new GarminConnectAuthenticationException("Missing oauth_token"),
            TokenSecret = parsed["oauth_token_secret"] ?? throw new GarminConnectAuthenticationException("Missing oauth_token_secret")
        };
    }

    private async Task AuthorizeOAuth1TokenAsync(
        OAuth1RequestToken requestToken,
        string ticket,
        CancellationToken cancellationToken)
    {
        var authorizeUrl = $"https://sso.garmin.com/sso/embed/oauth1Authorize?oauth_token={requestToken.Token}&ticket={ticket}";

        using var response = await _httpClient.GetAsync(authorizeUrl, cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            throw new GarminConnectAuthenticationException($"Failed to authorize OAuth1 token: {error}");
        }
    }

    private async Task<OAuth1Token> GetOAuth1AccessTokenAsync(
        OAuth1RequestToken requestToken,
        CancellationToken cancellationToken)
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        var nonce = Guid.NewGuid().ToString("N");

        var oauthParams = new Dictionary<string, string>
        {
            ["oauth_consumer_key"] = ConsumerKey,
            ["oauth_token"] = requestToken.Token,
            ["oauth_signature_method"] = "HMAC-SHA1",
            ["oauth_timestamp"] = timestamp,
            ["oauth_nonce"] = nonce,
            ["oauth_version"] = "1.0",
            ["oauth_verifier"] = ""
        };

        var signature = GenerateOAuthSignature("POST", OAuth1AccessTokenUrl, oauthParams, requestToken.TokenSecret);
        oauthParams["oauth_signature"] = signature;

        using var request = new HttpRequestMessage(HttpMethod.Post, OAuth1AccessTokenUrl);
        request.Headers.Authorization = new AuthenticationHeaderValue("OAuth", BuildOAuthHeader(oauthParams));

        using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var body = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            throw new GarminConnectAuthenticationException($"Failed to get OAuth1 access token: {body}");
        }

        var parsed = HttpUtility.ParseQueryString(body);
        return new OAuth1Token
        {
            Token = parsed["oauth_token"] ?? throw new GarminConnectAuthenticationException("Missing oauth_token"),
            TokenSecret = parsed["oauth_token_secret"] ?? throw new GarminConnectAuthenticationException("Missing oauth_token_secret")
        };
    }

    private string GenerateOAuthSignature(
        string method,
        string url,
        Dictionary<string, string> oauthParams,
        string? tokenSecret)
    {
        var sortedParams = oauthParams
            .OrderBy(p => p.Key)
            .Select(p => $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value)}");

        var paramString = string.Join("&", sortedParams);
        var signatureBase = $"{method}&{Uri.EscapeDataString(url)}&{Uri.EscapeDataString(paramString)}";
        var signingKey = $"{Uri.EscapeDataString(ConsumerSecret)}&{Uri.EscapeDataString(tokenSecret ?? "")}";

        using var hmac = new System.Security.Cryptography.HMACSHA1(Encoding.ASCII.GetBytes(signingKey));
        var hash = hmac.ComputeHash(Encoding.ASCII.GetBytes(signatureBase));

        return Convert.ToBase64String(hash);
    }

    private static string BuildOAuthHeader(Dictionary<string, string> oauthParams)
    {
        var parts = oauthParams.Select(p => $"{Uri.EscapeDataString(p.Key)}=\"{Uri.EscapeDataString(p.Value)}\"");
        return string.Join(", ", parts);
    }

    private static string BuildQueryString(Dictionary<string, string> parameters)
    {
        var parts = parameters.Select(p => $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value)}");
        return string.Join("&", parts);
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }

    // Regex patterns
    [GeneratedRegex(@"name=""_csrf""\s+value=""([^""]+)""", RegexOptions.IgnoreCase)]
    private static partial Regex CsrfTokenRegex();

    [GeneratedRegex(@"name=""service""\s+value=""([^""]+)""", RegexOptions.IgnoreCase)]
    private static partial Regex ServiceUrlRegex();

    [GeneratedRegex(@"ticket=([A-Za-z0-9\-_]+)", RegexOptions.IgnoreCase)]
    private static partial Regex TicketRegex();

    [GeneratedRegex(@"MFA_TOKEN[""']?\s*[:=]\s*[""']?([^""']+)", RegexOptions.IgnoreCase)]
    private static partial Regex MfaStateRegex();

    // Matches Garmin SSO error messages shown in status-error elements or data-error attributes.
    // Garmin renders auth errors inside: <div class="status-error">...</div>
    // or as title/data attributes on error containers.
    [GeneratedRegex(@"class=""[^""]*status-error[^""]*""[^>]*>([^<]+)<|""error-message""\s*>([^<]+)<|data-error=""([^""]+)""|<title>\s*Error\b", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex AuthErrorRegex();

    private record LoginResult
    {
        public string? Ticket { get; init; }
        public bool RequiresMfa { get; init; }
        public string? MfaClientState { get; init; }
    }

    private record OAuth1RequestToken
    {
        public required string Token { get; init; }
        public required string TokenSecret { get; init; }
    }
}
