using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using NSubstitute;
using Shared;
using User.NotificationService.Services;

namespace User.NotificationService.Tests;

public class WebsocketManagerTests
{
    private readonly MockHttpMessageHandler _mockHttpMessageHandler;
    private readonly WebsocketManager _service;

    public WebsocketManagerTests()
    {
        _mockHttpMessageHandler = Substitute.ForPartsOf<MockHttpMessageHandler>();
        var httpClient = new HttpClient(_mockHttpMessageHandler)
        {
            BaseAddress = new Uri("http://example.com/")
        };

        _service = new WebsocketManager(httpClient);
    }

    [Fact]
    public async Task GetHandlersAsync_WhenSuccessful_ReturnsListOfUserHandlerDescriptions()
    {
        // Arrange
        var userIds = new List<string> { "user1", "user2" };
        var idsHandlers = new Dictionary<string, string?>
        {
            {"user1", "handler1"},
            {"user2", "handler2"}
        };
        var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(idsHandlers), Encoding.UTF8, "application/json")
        };
        _mockHttpMessageHandler.MockSend(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(mockResponse);

        // Act
        var result = await _service.GetHandlersAsync(userIds);

        // Assert
        result.Should().HaveCount(idsHandlers.Count);
        result.Should().Contain(items => items.UserId == "user1" && items.Handler == "handler1");
        result.Should().Contain(items => items.UserId == "user2" && items.Handler == "handler2");
    }

    [Fact]
    public async Task GetHandlersAsync_WhenUnsuccessfulHttpResponse_ThrowsHttpRequestException()
    {   
        // Arrange  
        var userIds = new List<string> { "user1", "user2" };
        var mockResponse = new HttpResponseMessage(HttpStatusCode.BadRequest);
        _mockHttpMessageHandler.MockSend(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(mockResponse);

        // Act & Assert
        Func<Task> act = async () => await _service.GetHandlersAsync(userIds);
        await act.Should().ThrowAsync<HttpRequestException>();
    }
}