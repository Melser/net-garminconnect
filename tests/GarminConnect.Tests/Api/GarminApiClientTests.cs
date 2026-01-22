using System.Net;
using FluentAssertions;
using GarminConnect.Api;
using GarminConnect.Exceptions;
using RichardSzalay.MockHttp;

namespace GarminConnect.Tests.Api;

public class GarminApiClientTests : IDisposable
{
    private readonly MockHttpMessageHandler _mockHttp;
    private readonly GarminApiClient _client;

    public GarminApiClientTests()
    {
        _mockHttp = new MockHttpMessageHandler();
        var httpClient = _mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri("https://connectapi.garmin.com");
        _client = new GarminApiClient(httpClient);
    }

    public void Dispose()
    {
        _client.Dispose();
        _mockHttp.Dispose();
    }

    #region GetAsync Tests

    [Fact]
    public async Task GetAsync_ReturnsDeserializedResponse()
    {
        // Arrange
        _mockHttp.When("/test")
            .Respond("application/json", """{"name": "Test", "value": 42}""");

        // Act
        var result = await _client.GetAsync<TestResponse>("/test");

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Test");
        result.Value.Should().Be(42);
    }

    [Fact]
    public async Task GetAsync_IncludesAuthorizationHeader_WhenTokenSet()
    {
        // Arrange
        _client.SetAccessToken("test-token");
        _mockHttp.When("/test")
            .WithHeaders("Authorization", "Bearer test-token")
            .Respond("application/json", """{"name": "Test"}""");

        // Act
        var result = await _client.GetAsync<TestResponse>("/test");

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAsync_ThrowsAuthenticationException_On401()
    {
        // Arrange
        _mockHttp.When("/test")
            .Respond(HttpStatusCode.Unauthorized, "text/plain", "Unauthorized");

        // Act
        var act = () => _client.GetAsync<TestResponse>("/test");

        // Assert
        await act.Should().ThrowAsync<GarminConnectAuthenticationException>()
            .WithMessage("*Unauthorized*");
    }

    [Fact]
    public async Task GetAsync_ThrowsTooManyRequestsException_On429()
    {
        // Arrange
        _mockHttp.When("/test")
            .Respond(HttpStatusCode.TooManyRequests, "text/plain", "Rate limit exceeded");

        // Act
        var act = () => _client.GetAsync<TestResponse>("/test");

        // Assert
        await act.Should().ThrowAsync<GarminConnectTooManyRequestsException>();
    }

    [Fact]
    public async Task GetAsync_ThrowsConnectionException_On500()
    {
        // Arrange
        _mockHttp.When("/test")
            .Respond(HttpStatusCode.InternalServerError, "text/plain", "Server error");

        // Act
        var act = () => _client.GetAsync<TestResponse>("/test");

        // Assert
        await act.Should().ThrowAsync<GarminConnectException>()
            .WithMessage("*server error*");
    }

    #endregion

    #region PostAsync Tests

    [Fact]
    public async Task PostAsync_SendsJsonContent()
    {
        // Arrange
        _mockHttp.When(HttpMethod.Post, "/test")
            .WithContent("""{"name":"Test"}""")
            .Respond("application/json", """{"success": true}""");

        // Act
        var result = await _client.PostAsync<SuccessResponse>("/test", new { name = "Test" });

        // Assert
        result.Success.Should().BeTrue();
    }

    [Fact]
    public async Task PostAsync_WithoutContent_SendsEmptyBody()
    {
        // Arrange
        _mockHttp.When(HttpMethod.Post, "/test")
            .Respond("application/json", """{"success": true}""");

        // Act
        var result = await _client.PostAsync<SuccessResponse>("/test");

        // Assert
        result.Success.Should().BeTrue();
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_SendsDeleteRequest()
    {
        // Arrange
        _mockHttp.When(HttpMethod.Delete, "/test/123")
            .Respond(HttpStatusCode.NoContent);

        // Act
        var act = () => _client.DeleteAsync("/test/123");

        // Assert
        await act.Should().NotThrowAsync();
    }

    #endregion

    #region GetBytesAsync Tests

    [Fact]
    public async Task GetBytesAsync_ReturnsByteArray()
    {
        // Arrange
        var expectedBytes = new byte[] { 1, 2, 3, 4, 5 };
        _mockHttp.When("/download")
            .Respond("application/octet-stream", new MemoryStream(expectedBytes));

        // Act
        var result = await _client.GetBytesAsync("/download");

        // Assert
        result.Should().BeEquivalentTo(expectedBytes);
    }

    #endregion

    #region GetStreamAsync Tests

    [Fact]
    public async Task GetStreamAsync_ReturnsReadableStream()
    {
        // Arrange
        var expectedContent = "test content"u8.ToArray();
        _mockHttp.When("/stream")
            .Respond("application/octet-stream", new MemoryStream(expectedContent));

        // Act
        await using var stream = await _client.GetStreamAsync("/stream");

        // Assert
        using var reader = new StreamReader(stream);
        var content = await reader.ReadToEndAsync();
        content.Should().Be("test content");
    }

    #endregion

    #region SetAccessToken Tests

    [Fact]
    public async Task SetAccessToken_ClearsToken_WhenSetToNull()
    {
        // Arrange
        _client.SetAccessToken("test-token");
        _client.SetAccessToken(null);

        _mockHttp.When("/test")
            .Respond(req =>
            {
                req.Headers.Authorization.Should().BeNull();
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""{"name":"Test"}""", System.Text.Encoding.UTF8, "application/json")
                };
            });

        // Act & Assert
        await _client.GetAsync<TestResponse>("/test");
    }

    #endregion

    #region Dispose Tests

    [Fact]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        // Act
        var act = () =>
        {
            _client.Dispose();
            _client.Dispose();
        };

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public async Task DisposeAsync_CanBeCalledMultipleTimes()
    {
        // Act
        var act = async () =>
        {
            await _client.DisposeAsync();
            await _client.DisposeAsync();
        };

        // Assert
        await act.Should().NotThrowAsync();
    }

    #endregion

    #region Test DTOs

    private record TestResponse
    {
        public string? Name { get; init; }
        public int Value { get; init; }
    }

    private record SuccessResponse
    {
        public bool Success { get; init; }
    }

    #endregion
}
