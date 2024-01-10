using Kafka.Settings;
using KafkaFlow;
using KafkaFlow.Producers;
using User.WebSocketHandler.Configurations;
using User.WebSocketHandler.Contracts;
using User.WebSocketHandler.Mapping;
using User.WebSocketHandler.Validation;

namespace User.WebSocketHandler.Services;

public class UserLocationService(
    IProducerAccessor producerAccessor,
    ILogger<UserLocationService> logger,
    IRequestValidator requestValidator)
    : IUserLocationService
{
    private readonly IMessageProducer _messageProducer = producerAccessor.GetProducer(KafkaConstants.ProducerName);
    private readonly ILogger<UserLocationService> _logger = logger;
    private readonly IRequestValidator _requestValidator = requestValidator;

    public async Task PostLocationAsync(PostUserLocationRequest postUserLocationRequest)
    {
        _requestValidator.ThrowIfNotValid(postUserLocationRequest);

        _logger.LogDebug("Posting new location for user {UserId}", postUserLocationRequest.UserId);

        var userLocationChangedEvent = postUserLocationRequest.ToUserLocationChangedEvent();
        _ = await _messageProducer.ProduceAsync(TopicConfig.UserLocations.TopicName, userLocationChangedEvent.UserId,
            userLocationChangedEvent);

        _logger.LogDebug("New location posted for user {UserId}", postUserLocationRequest.UserId);
    }
}