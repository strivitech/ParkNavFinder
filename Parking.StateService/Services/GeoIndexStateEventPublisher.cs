using Confluent.Kafka;
using ErrorOr;
using Kafka.Events.Contracts.Parking.State;
using Kafka.Settings;
using KafkaFlow;
using KafkaFlow.Producers;
using Parking.StateService.Common;
using Parking.StateService.Configurations;

namespace Parking.StateService.Services;

internal class GeoIndexStateEventPublisher(
    IProducerAccessor producerAccessor,
    ILogger<GeoIndexStateEventPublisher> logger) : IGeoIndexStateEventPublisher
{
    private readonly IMessageProducer _messageProducer = producerAccessor.GetProducer(KafkaConstants.ProducerName);
    private readonly ILogger<GeoIndexStateEventPublisher> _logger = logger;

    public async Task<ErrorOr<Success>> PublishStateChangedAsync(IndexStateChangedEvent indexStateChangedEvent)
    {
        _logger.LogDebug("Publishing index state changed event");

        var response = await _messageProducer.ProduceAsync(TopicConfig.ParkingStateEvents.TopicName,
            indexStateChangedEvent.Index, indexStateChangedEvent);
        
        if (response.Status != PersistenceStatus.Persisted)
        {
            _logger.LogError("Failed to publish index state changed event");
            return Errors.EventPublisher.PublishFailed(nameof(IndexStateChangedEvent));
        }

        _logger.LogDebug("Parking state changed event published successfully");

        return new Success();
    }
}