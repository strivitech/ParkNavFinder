using KafkaFlow;
using KafkaFlow.Producers;
using LocationService.Common.Configuration;
using LocationService.Models;
using LocationService.Requests;

namespace LocationService.Services;

public class UserLocationService(
    IProducerAccessor producerAccessor,
    ILogger<UserLocationService> logger,
    IModelValidationService modelValidationService)
    : IUserLocationService
{
    private readonly IMessageProducer _messageProducer = producerAccessor.GetProducer(KafkaConstants.ProducerName);
    private readonly ILogger<UserLocationService> _logger = logger;
    private readonly IModelValidationService _modelValidationService = modelValidationService;
    
    public async Task PostNewLocation(PostUserLocationRequest postUserLocationRequest)
    {
        _modelValidationService.ThrowIfNotValid(postUserLocationRequest);

        _logger.LogDebug(
            "Posting new location for user {UserId}, Latitude: {Latitude}, Longitude: {Longitude}, DateTime: {Timestamp}",
            postUserLocationRequest.UserId, postUserLocationRequest.Latitude, postUserLocationRequest.Longitude,
            postUserLocationRequest.Timestamp);

        var message = UserLocationMessage.FromPostUserLocationRequest(postUserLocationRequest);

        _ = await _messageProducer.ProduceAsync(KafkaConstants.UserLocationTopic, message.UserId, message);

        _logger.LogDebug("New location posted for user {UserId}", postUserLocationRequest.UserId);
    }
}