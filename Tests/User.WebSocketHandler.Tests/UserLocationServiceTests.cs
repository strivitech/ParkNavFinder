using FluentAssertions;
using Kafka.Events.Contracts.User.Location;
using Kafka.Settings;
using KafkaFlow;
using KafkaFlow.Producers;
using Microsoft.Extensions.Logging;
using NSubstitute;
using User.WebSocketHandler.Configurations;
using User.WebSocketHandler.Contracts;
using User.WebSocketHandler.Services;
using User.WebSocketHandler.Validation;

namespace User.WebSocketHandler.Tests;

public class UserLocationServiceTests
{
    private readonly IMessageProducer _mockMessageProducer;
    private readonly IRequestValidator _mockRequestValidator;
    private readonly UserLocationService _service;

    public UserLocationServiceTests()
    {
        var mockProducerAccessor = Substitute.For<IProducerAccessor>();
        _mockMessageProducer = Substitute.For<IMessageProducer>();
        ILogger<UserLocationService> mockLogger = Substitute.For<ILogger<UserLocationService>>();
        _mockRequestValidator = Substitute.For<IRequestValidator>();

        mockProducerAccessor.GetProducer(KafkaConstants.ProducerName).Returns(_mockMessageProducer);
        
        _service = new UserLocationService(mockProducerAccessor, mockLogger, _mockRequestValidator);
    }

    [Fact]
    public async Task PostLocationAsync_ValidRequest_ProducesMessage()
    {
        // Arrange
        var request = new PostUserLocationRequest("UserId", 10.0, 20.0, DateTime.UtcNow);

        // Act
        await _service.PostLocationAsync(request);

        // Assert
        await _mockMessageProducer.Received(1).ProduceAsync(
            TopicConfig.UserLocations.TopicName, 
            request.UserId,
            Arg.Any<UserLocationChangedEvent>());
    }

    [Fact]
    public async Task PostLocationAsync_InvalidRequest_ThrowsValidationException()
    {
        // Arrange
        var invalidRequest = new PostUserLocationRequest("", 0, 0, DateTime.MinValue);

        _mockRequestValidator.WhenForAnyArgs(x => x.ThrowIfNotValid(Arg.Any<PostUserLocationRequest>()))
            .Do(_ => throw new RequestValidationException());

        // Act
        Func<Task> act = async () => await _service.PostLocationAsync(invalidRequest);

        // Assert
        await act.Should().ThrowAsync<RequestValidationException>();
    }
}