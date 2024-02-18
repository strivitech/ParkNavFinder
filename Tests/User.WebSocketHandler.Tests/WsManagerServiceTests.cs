using System.Net;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shared;
using User.WebSocketHandler.Services;

namespace User.WebSocketHandler.Tests;

public class WsManagerServiceTests
{
    private readonly MockHttpMessageHandler _mockHttpMessageHandler;
    private readonly WsManagerService _service;

    public WsManagerServiceTests()
    {
        _mockHttpMessageHandler = Substitute.ForPartsOf<MockHttpMessageHandler>();
        var httpClient = new HttpClient(_mockHttpMessageHandler)
        {
            BaseAddress = new Uri("http://example.com/")
        };

        ILogger<WsManagerService> mockLogger = Substitute.For<ILogger<WsManagerService>>();
        _service = new WsManagerService(httpClient, mockLogger);
    }

    [Fact]
    public async Task SendUserConnectedMessage_WhenCalled_SendsPostRequestAndEnsureSuccessStatusCode()  
    {
        // Arrange
        var userId = "testUser";
        var mockResponse = new HttpResponseMessage(HttpStatusCode.OK);
        _mockHttpMessageHandler.MockSend(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(mockResponse);

        // Act
        Func<Task> act = async () => await _service.SendUserConnectedMessage(userId);

        // Assert
        await act.Should().NotThrowAsync();
    }
    
    [Fact]
    public async Task SendUserDisconnectedMessage_WhenCalled_SendsDeleteRequestAndEnsureSuccessStatusCode()
    {
        // Arrange
        var userId = "testUser";
        var mockResponse = new HttpResponseMessage(HttpStatusCode.OK);
        _mockHttpMessageHandler.MockSend(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(mockResponse);

        // Act
        Func<Task> act = async () => await _service.SendUserDisconnectedMessage(userId);

        // Assert
        await act.Should().NotThrowAsync();
    }
}