using Confluent.Kafka;
using ErrorOr;
using KafkaFlow;
using KafkaFlow.Producers;
using ParkingStateService.Broker;
using ParkingStateService.Common;
using ParkingStateService.Events;

namespace ParkingStateService.Services;

internal class IndexStateEventPublisher(
    IProducerAccessor producerAccessor,
    ILogger<IndexStateEventPublisher> logger) : IIndexStateEventPublisher
{
    private readonly IMessageProducer _messageProducer = producerAccessor.GetProducer(KafkaConstants.ProducerName);
    private readonly ILogger<IndexStateEventPublisher> _logger = logger;

    public async Task<ErrorOr<Success>> PublishStateChangedAsync(IndexStateChangedEvent indexStateChangedEvent)
    {
        _logger.LogDebug("Publishing index state changed event");

        var response = await _messageProducer.ProduceAsync(KafkaConstants.ParkingStateEvents,
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