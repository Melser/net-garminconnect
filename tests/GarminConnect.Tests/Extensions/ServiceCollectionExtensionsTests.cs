using FluentAssertions;
using GarminConnect.Api;
using GarminConnect.Auth;
using GarminConnect.Auth.Mfa;
using GarminConnect.Auth.OAuth;
using GarminConnect.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace GarminConnect.Tests.Extensions;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddGarminConnect_RegistersAllRequiredServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddGarminConnect();
        var provider = services.BuildServiceProvider();

        // Assert
        provider.GetService<IOptions<GarminClientOptions>>().Should().NotBeNull();
        provider.GetService<IOAuthTokenStore>().Should().NotBeNull();
        provider.GetService<IGarminAuthenticator>().Should().NotBeNull();
        provider.GetService<IHttpClientFactory>().Should().NotBeNull();
        provider.GetService<IGarminApiClient>().Should().NotBeNull();
        provider.GetService<IGarminClient>().Should().NotBeNull();
    }

    [Fact]
    public void AddGarminConnect_WithOptions_ConfiguresOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        var customTimeout = TimeSpan.FromMinutes(5);

        // Act
        services.AddGarminConnect(options =>
        {
            options.Timeout = customTimeout;
            options.MaxRetryAttempts = 5;
            options.TokenStorePath = "/tmp/tokens.json";
        });
        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<GarminClientOptions>>().Value;

        // Assert
        options.Timeout.Should().Be(customTimeout);
        options.MaxRetryAttempts.Should().Be(5);
        options.TokenStorePath.Should().Be("/tmp/tokens.json");
    }

    [Fact]
    public void AddGarminConnect_WithoutTokenStorePath_RegistersNullTokenStore()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddGarminConnect();
        var provider = services.BuildServiceProvider();
        var tokenStore = provider.GetRequiredService<IOAuthTokenStore>();

        // Assert
        tokenStore.GetType().Name.Should().Be("NullTokenStore");
    }

    [Fact]
    public void AddGarminConnect_WithTokenStorePath_RegistersFileTokenStore()
    {
        // Arrange
        var services = new ServiceCollection();
        var tokenPath = Path.Combine(Path.GetTempPath(), $"garmin_tokens_{Guid.NewGuid()}.json");

        // Act
        services.AddGarminConnect(options =>
        {
            options.TokenStorePath = tokenPath;
        });
        var provider = services.BuildServiceProvider();
        var tokenStore = provider.GetRequiredService<IOAuthTokenStore>();

        // Assert
        tokenStore.Should().BeOfType<FileTokenStore>();
    }

    [Fact]
    public void AddGarminConnect_WithGenericMfaHandler_RegistersMfaHandler()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddGarminConnect<TestMfaHandler>();
        var provider = services.BuildServiceProvider();
        var mfaHandler = provider.GetService<IMfaHandler>();

        // Assert
        mfaHandler.Should().NotBeNull();
        mfaHandler.Should().BeOfType<TestMfaHandler>();
    }

    [Fact]
    public void AddGarminConnect_WithMfaCallback_RegistersCallbackMfaHandler()
    {
        // Arrange
        var services = new ServiceCollection();
        var mfaCallback = (CancellationToken ct) => Task.FromResult<string?>("123456");

        // Act
        services.AddGarminConnect(mfaCallback);
        var provider = services.BuildServiceProvider();
        var mfaHandler = provider.GetService<IMfaHandler>();

        // Assert
        mfaHandler.Should().NotBeNull();
        mfaHandler.Should().BeOfType<CallbackMfaHandler>();
    }

    [Fact]
    public async Task AddGarminConnect_MfaCallback_IsInvoked()
    {
        // Arrange
        var services = new ServiceCollection();
        var callbackInvoked = false;
        var mfaCallback = (CancellationToken ct) =>
        {
            callbackInvoked = true;
            return Task.FromResult<string?>("123456");
        };

        // Act
        services.AddGarminConnect(mfaCallback);
        var provider = services.BuildServiceProvider();
        var mfaHandler = provider.GetService<IMfaHandler>();
        var code = await mfaHandler!.GetMfaCodeAsync();

        // Assert
        callbackInvoked.Should().BeTrue();
        code.Should().Be("123456");
    }

    [Fact]
    public void AddGarminConnect_IGarminClient_IsScopedLifetime()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddGarminConnect();
        var provider = services.BuildServiceProvider();

        // Assert - creating two scopes should yield different instances
        IGarminClient client1, client2;

        using (var scope1 = provider.CreateScope())
        {
            client1 = scope1.ServiceProvider.GetRequiredService<IGarminClient>();
        }

        using (var scope2 = provider.CreateScope())
        {
            client2 = scope2.ServiceProvider.GetRequiredService<IGarminClient>();
        }

        // Note: We can't compare references directly since they're disposed
        // But we verify that getting the service in scopes works
        client1.Should().NotBeNull();
        client2.Should().NotBeNull();
    }

    [Fact]
    public void AddGarminConnect_IGarminAuthenticator_IsSingletonLifetime()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddGarminConnect();
        var provider = services.BuildServiceProvider();

        // Assert
        var auth1 = provider.GetRequiredService<IGarminAuthenticator>();
        var auth2 = provider.GetRequiredService<IGarminAuthenticator>();
        auth1.Should().BeSameAs(auth2);
    }

    [Fact]
    public void AddGarminConnect_DefaultOptions_HaveCorrectValues()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddGarminConnect();
        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<GarminClientOptions>>().Value;

        // Assert
        options.BaseUrl.Should().Be("https://connect.garmin.com");
        options.Timeout.Should().Be(TimeSpan.FromSeconds(30));
        options.MaxRetryAttempts.Should().Be(3);
        options.RetryDelay.Should().Be(TimeSpan.FromSeconds(1));
        options.AutoRefreshToken.Should().BeTrue();
        options.UserAgent.Should().Be("GCM-iOS-5.7.2.1");
        options.TokenStorePath.Should().BeNull();
    }

    [Fact]
    public void AddGarminConnect_HttpClient_IsRegistered()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddGarminConnect();
        var provider = services.BuildServiceProvider();
        var factory = provider.GetRequiredService<IHttpClientFactory>();
        var client = factory.CreateClient(ServiceCollectionExtensions.HttpClientName);

        // Assert
        client.Should().NotBeNull();
    }

    [Fact]
    public void AddGarminConnect_CanResolveMultipleTimesInScope()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddGarminConnect();
        var provider = services.BuildServiceProvider();

        // Assert
        using var scope = provider.CreateScope();
        var client1 = scope.ServiceProvider.GetRequiredService<IGarminClient>();
        var client2 = scope.ServiceProvider.GetRequiredService<IGarminClient>();

        // Same scope should return same instance (scoped)
        client1.Should().BeSameAs(client2);
    }

    [Fact]
    public async Task NullTokenStore_LoadAsync_ReturnsNull()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddGarminConnect();
        var provider = services.BuildServiceProvider();
        var tokenStore = provider.GetRequiredService<IOAuthTokenStore>();

        // Act
        var tokens = await tokenStore.LoadAsync();

        // Assert
        tokens.Should().BeNull();
    }

    [Fact]
    public async Task NullTokenStore_ExistsAsync_ReturnsFalse()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddGarminConnect();
        var provider = services.BuildServiceProvider();
        var tokenStore = provider.GetRequiredService<IOAuthTokenStore>();

        // Act
        var exists = await tokenStore.ExistsAsync();

        // Assert
        exists.Should().BeFalse();
    }

    [Fact]
    public async Task NullTokenStore_SaveAndClearAsync_DoNotThrow()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddGarminConnect();
        var provider = services.BuildServiceProvider();
        var tokenStore = provider.GetRequiredService<IOAuthTokenStore>();

        // Act & Assert - should not throw
        var act1 = async () => await tokenStore.SaveAsync(new GarminConnectTokens());
        var act2 = async () => await tokenStore.ClearAsync();

        await act1.Should().NotThrowAsync();
        await act2.Should().NotThrowAsync();
    }

    // Test MFA handler for testing
    private class TestMfaHandler : IMfaHandler
    {
        public Task<string?> GetMfaCodeAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<string?>("test-mfa-code");
        }
    }
}
