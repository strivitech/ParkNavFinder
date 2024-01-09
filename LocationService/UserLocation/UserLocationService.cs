using Kafka.Settings;
using KafkaFlow;
using KafkaFlow.Producers;
using LocationService.Kafka;
using LocationService.Validation;

namespace LocationService.UserLocation;

public class UserLocationService(
    IProducerAccessor producerAccessor,
    ILogger<UserLocationService> logger,
    IRequestValidator requestValidator)
    : IUserLocationService
{
    private readonly IMessageProducer _messageProducer = producerAccessor.GetProducer(KafkaConstants.ProducerName);
    private readonly ILogger<UserLocationService> _logger = logger;
    private readonly IRequestValidator _requestValidator = requestValidator;
    
    public async Task PostNewLocation(PostUserLocationRequest postUserLocationRequest)
    {
        _requestValidator.ThrowIfNotValid(postUserLocationRequest);

        _logger.LogDebug(
            "Posting new location for user {UserId}, Latitude: {Latitude}, Longitude: {Longitude}, DateTime: {Timestamp}",
            postUserLocationRequest.UserId, postUserLocationRequest.Latitude, postUserLocationRequest.Longitude,
            postUserLocationRequest.Timestamp);

        var message = postUserLocationRequest.ToUserLocationChangedEvent();

        _ = await _messageProducer.ProduceAsync(TopicConfig.UserLocations.TopicName, message.UserId, message);

        _logger.LogDebug("New location posted for user {UserId}", postUserLocationRequest.UserId);
    }
}