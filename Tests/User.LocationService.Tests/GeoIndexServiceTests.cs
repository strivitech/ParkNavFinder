using System.Net;
using FluentAssertions;
using NSubstitute;
using Shared;
using User.LocationService.Services;

namespace User.LocationService.Tests;

public class GeoIndexServiceTests
{
    private readonly GeoIndexService _service;
    private readonly MockHttpMessageHandler _mockHttpMessageHandler;

    public GeoIndexServiceTests()
    {
        _mockHttpMessageHandler = Substitute.ForPartsOf<MockHttpMessageHandler>();
        var httpClient = new HttpClient(_mockHttpMessageHandler)
        {
            BaseAddress = new Uri("http://example.com/")
        };

        _service = new GeoIndexService(httpClient);
    }

    [Fact]
    public async Task GetGeoIndexAsync_WhenSuccessful_ReturnsGeoIndex()
    {
        // Arrange
        const double lat = 40.7128;
        const double lon = 28.0060;
        const int resolution = 10;
        const string expectedResponse = "\"testGeoIndex\"";
        var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(expectedResponse)
        };

        _mockHttpMessageHandler.MockSend(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(mockResponse);

        // Act
        var result = await _service.GetGeoIndexAsync(lat, lon, resolution);

        // Assert
        result.Should().Be("testGeoIndex");
    }

    [Fact]
    public async Task GetGeoIndexAsync_WhenUnsuccessfulHttpResponse_ThrowsHttpRequestException()
    {
        // Arrange
        const double lat = 40.7128;
        const double lon = 28.0060;
        const int resolution = 10;
        var mockResponse = new HttpResponseMessage(HttpStatusCode.BadRequest);
        _mockHttpMessageHandler.MockSend(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(mockResponse);

        // Act & Assert
        Func<Task> act = async () => await _service.GetGeoIndexAsync(lat, lon, resolution);
        await act.Should().ThrowAsync<HttpRequestException>();
    }
    
    [Fact]
    public async Task GetGeoIndexAsync_WhenResponseContentIsEmpty_ThrowsJsonException()
    {
        // Arrange
        const double lat = 40.7128;
        const double lon = 28.0060;
        const int resolution = 10;
        var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(string.Empty)
        };

        _mockHttpMessageHandler.MockSend(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(mockResponse);

        // Act & Assert
        Func<Task> act = async () => await _service.GetGeoIndexAsync(lat, lon, resolution);
        await act.Should().ThrowAsync<System.Text.Json.JsonException>();
    }
}