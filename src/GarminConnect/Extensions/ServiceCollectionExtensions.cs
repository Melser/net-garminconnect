using GarminConnect.Api;
using GarminConnect.Auth;
using GarminConnect.Auth.Mfa;
using GarminConnect.Auth.OAuth;
using GarminConnect.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;

namespace GarminConnect.Extensions;

/// <summary>
/// Extension methods for registering Garmin Connect services with dependency injection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// The name used for the HttpClient registration.
    /// </summary>
    public const string HttpClientName = "GarminConnect";

    /// <summary>
    /// Adds Garmin Connect client services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureOptions">Optional action to configure options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddGarminConnect(
        this IServiceCollection services,
        Action<GarminClientOptions>? configureOptions = null)
    {
        // Register options
        if (configureOptions is not null)
        {
            services.Configure(configureOptions);
        }
        else
        {
            services.Configure<GarminClientOptions>(_ => { });
        }

        // Register HttpClient with IHttpClientFactory
        services.AddHttpClient(HttpClientName, (serviceProvider, client) =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<GarminClientOptions>>().Value;
                client.BaseAddress = new Uri(options.BaseUrl);
                client.Timeout = options.Timeout;
                client.DefaultRequestHeaders.UserAgent.ParseAdd(options.UserAgent);
            })
            .AddStandardResilienceHandler()
            .Configure((options, serviceProvider) =>
            {
                var clientOptions = serviceProvider.GetRequiredService<IOptions<GarminClientOptions>>().Value;
                ConfigureResilience(options, clientOptions);
            });

        // Register token store (optional, based on configuration)
        services.AddSingleton<IOAuthTokenStore>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<GarminClientOptions>>().Value;
            if (!string.IsNullOrEmpty(options.TokenStorePath))
            {
                return new FileTokenStore(options.TokenStorePath);
            }

            // Return a no-op token store if no path is configured
            return new NullTokenStore();
        });

        // Register authenticator
        services.AddSingleton<IGarminAuthenticator>(sp =>
        {
            var tokenStore = sp.GetRequiredService<IOAuthTokenStore>();
            var mfaHandler = sp.GetService<IMfaHandler>();
            var logger = sp.GetService<ILogger<GarminAuthenticator>>();

            return new GarminAuthenticator(tokenStore, mfaHandler, logger);
        });

        // Register API client
        services.AddScoped<IGarminApiClient>(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient(HttpClientName);
            var logger = sp.GetService<ILogger<GarminApiClient>>();

            // Don't own HttpClient when using IHttpClientFactory
            return new GarminApiClient(httpClient, ownsHttpClient: false, logger);
        });

        // Register main client
        services.AddScoped<IGarminClient>(sp =>
        {
            var apiClient = sp.GetRequiredService<IGarminApiClient>();
            var authenticator = sp.GetRequiredService<IGarminAuthenticator>();
            var logger = sp.GetService<ILogger<GarminClient>>();

            return new GarminClient(apiClient, authenticator, logger);
        });

        return services;
    }

    /// <summary>
    /// Adds Garmin Connect client with a custom MFA handler.
    /// </summary>
    /// <typeparam name="TMfaHandler">The MFA handler implementation type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="configureOptions">Optional action to configure options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddGarminConnect<TMfaHandler>(
        this IServiceCollection services,
        Action<GarminClientOptions>? configureOptions = null)
        where TMfaHandler : class, IMfaHandler
    {
        services.AddSingleton<IMfaHandler, TMfaHandler>();
        return services.AddGarminConnect(configureOptions);
    }

    /// <summary>
    /// Adds Garmin Connect client with a callback-based MFA handler.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="mfaCallback">Callback function to get MFA code.</param>
    /// <param name="configureOptions">Optional action to configure options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddGarminConnect(
        this IServiceCollection services,
        Func<CancellationToken, Task<string?>> mfaCallback,
        Action<GarminClientOptions>? configureOptions = null)
    {
        services.AddSingleton<IMfaHandler>(new CallbackMfaHandler(mfaCallback));
        return services.AddGarminConnect(configureOptions);
    }

    private static void ConfigureResilience(HttpStandardResilienceOptions options, GarminClientOptions clientOptions)
    {
        // Configure retry policy using client options
        options.Retry.MaxRetryAttempts = clientOptions.MaxRetryAttempts;
        options.Retry.Delay = clientOptions.RetryDelay;
        options.Retry.BackoffType = DelayBackoffType.Exponential;
        options.Retry.UseJitter = true;
        options.Retry.ShouldHandle = args => ValueTask.FromResult(ShouldRetry(args.Outcome));

        // Configure timeout for individual attempts using client options
        options.AttemptTimeout.Timeout = clientOptions.Timeout;

        // Configure circuit breaker
        // SamplingDuration must be at least 2x AttemptTimeout (Polly validation requirement)
        var samplingDuration = TimeSpan.FromTicks(Math.Max(
            clientOptions.Timeout.Ticks * 2,
            TimeSpan.FromSeconds(60).Ticks));
        options.CircuitBreaker.SamplingDuration = samplingDuration;
        options.CircuitBreaker.FailureRatio = 0.5;
        options.CircuitBreaker.MinimumThroughput = 5;
        options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(30);
        options.CircuitBreaker.ShouldHandle = args => ValueTask.FromResult(ShouldBreak(args.Outcome));

        // Configure total request timeout (should be enough for all retries)
        var totalTimeout = TimeSpan.FromTicks(clientOptions.Timeout.Ticks * (clientOptions.MaxRetryAttempts + 1) * 2);
        options.TotalRequestTimeout.Timeout = totalTimeout;
    }

    private static bool ShouldRetry(Outcome<HttpResponseMessage> outcome)
    {
        // Retry on transient HTTP errors
        if (outcome.Exception is not null)
        {
            return outcome.Exception is HttpRequestException or TaskCanceledException;
        }

        if (outcome.Result is not null)
        {
            var statusCode = (int)outcome.Result.StatusCode;

            // Retry on 429 (Too Many Requests) and 5xx errors
            return statusCode == 429 || (statusCode >= 500 && statusCode < 600);
        }

        return false;
    }

    private static bool ShouldBreak(Outcome<HttpResponseMessage> outcome)
    {
        // Open circuit on server errors and too many requests
        if (outcome.Exception is not null)
        {
            return true;
        }

        if (outcome.Result is not null)
        {
            var statusCode = (int)outcome.Result.StatusCode;
            return statusCode == 429 || (statusCode >= 500 && statusCode < 600);
        }

        return false;
    }
}

/// <summary>
/// A token store that does not persist tokens (no-op implementation).
/// </summary>
internal sealed class NullTokenStore : IOAuthTokenStore
{
    public Task<GarminConnectTokens?> LoadAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<GarminConnectTokens?>(null);

    public Task SaveAsync(GarminConnectTokens tokens, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public Task ClearAsync(CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public Task<bool> ExistsAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(false);
}
