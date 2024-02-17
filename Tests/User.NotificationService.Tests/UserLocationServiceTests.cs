using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using NSubstitute;
using Shared;
using User.NotificationService.Services;

namespace User.NotificationService.Tests;

public class UserLocationServiceTests
{
    private readonly MockHttpMessageHandler _mockHttpMessageHandler;
    private readonly UserLocationService _service;

    public UserLocationServiceTests()
    {
        _mockHttpMessageHandler = Substitute.ForPartsOf<MockHttpMessageHandler>();
        var httpClient = new HttpClient(_mockHttpMessageHandler)
        {
            BaseAddress = new Uri("http://example.com/")
        };

        _service = new UserLocationService(httpClient);
    }

    [Fact]
    public async Task GetUsersAttachedToIndex_WhenSuccessful_ReturnsIndexUsersResponse()
    {
        // Arrange
        const string index = "testIndex";
        var users = new List<string> { "user1", "user2" };
        var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(users), Encoding.UTF8, "application/json")
        };
        _mockHttpMessageHandler.MockSend(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(mockResponse);

        // Act
        var result = await _service.GetUsersAttachedToIndex(index);

        // Assert
        result.UserIds.Should().BeEquivalentTo(users);
    }

    [Fact]
    public async Task GetUsersAttachedToIndex_WhenUnsuccessful_ThrowsHttpRequestException()
    {
        // Arrange
        const string index = "testIndex";
        var mockResponse = new HttpResponseMessage(HttpStatusCode.BadRequest);
        _mockHttpMessageHandler.MockSend(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(mockResponse);

        // Act & Assert
        Func<Task> act = async () => await _service.GetUsersAttachedToIndex(index);
        await act.Should().ThrowAsync<HttpRequestException>();
    }
}