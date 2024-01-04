using Confluent.Kafka;
using ErrorOr;
using KafkaFlow;
using KafkaFlow.Producers;
using ParkingManagementService.Broker;
using ParkingManagementService.Common;
using ParkingManagementService.Events;

namespace ParkingManagementService.Services;

internal class ParkingServiceEventPublisher(
    IProducerAccessor producerAccessor,
    ILogger<ParkingServiceEventPublisher> logger) : IParkingServiceEventPublisher
{
    private readonly IMessageProducer _messageProducer = producerAccessor.GetProducer(KafkaConstants.ProducerName);
    private readonly ILogger<ParkingServiceEventPublisher> _logger = logger;

    public async Task<ErrorOr<Success>> PublishParkingAddedAsync(ParkingAddedEvent parkingAddedEvent)
    {
        _logger.LogDebug(
            "Publishing parking added event for parking {ParkingId}, Latitude: {Latitude}, Longitude: {Longitude}",
            parkingAddedEvent.ParkingId, parkingAddedEvent.Latitude, parkingAddedEvent.Longitude);

        var response = await _messageProducer.ProduceAsync(KafkaConstants.ParkingManagementEvents,
            parkingAddedEvent.ParkingId.ToString(), parkingAddedEvent);

        if (response.Status != PersistenceStatus.Persisted)
        {
            _logger.LogError("Failed to publish parking added event for parking {ParkingId}",
                parkingAddedEvent.ParkingId);
            return Errors.EventPublisher.PublishFailed(nameof(ParkingAddedEvent));
        }

        _logger.LogDebug("Parking added event published for parking {ParkingId}", parkingAddedEvent.ParkingId);

        return new Success();
    }

    public async Task<ErrorOr<Success>> PublishParkingUpdatedAsync(ParkingUpdatedEvent parkingUpdatedEvent)
    {
        _logger.LogDebug("Publishing parking updated event for parking {ParkingId}", parkingUpdatedEvent.ParkingId);

        var response = await _messageProducer.ProduceAsync(KafkaConstants.ParkingManagementEvents,
            parkingUpdatedEvent.ParkingId.ToString(), parkingUpdatedEvent);

        if (response.Status != PersistenceStatus.Persisted)
        {
            _logger.LogError("Failed to publish parking updated event for parking {ParkingId}",
                parkingUpdatedEvent.ParkingId);
            return Errors.EventPublisher.PublishFailed(nameof(ParkingUpdatedEvent));
        }

        _logger.LogDebug("Parking updated event published for parking {ParkingId}", parkingUpdatedEvent.ParkingId);

        return new Success();
    }

    public async Task<ErrorOr<Success>> PublishParkingDeletedAsync(ParkingDeletedEvent parkingDeletedEvent)
    {
        _logger.LogDebug("Publishing parking deleted event for parking {ParkingId}", parkingDeletedEvent.ParkingId);

        var response = await _messageProducer.ProduceAsync(KafkaConstants.ParkingManagementEvents,
            parkingDeletedEvent.ParkingId.ToString(), parkingDeletedEvent);

        if (response.Status != PersistenceStatus.Persisted)
        {
            _logger.LogError("Failed to publish parking deleted event for parking {ParkingId}",
                parkingDeletedEvent.ParkingId);
            return Errors.EventPublisher.PublishFailed(nameof(ParkingDeletedEvent));
        }

        _logger.LogDebug("Parking deleted event published for parking {ParkingId}", parkingDeletedEvent.ParkingId);

        return new Success();
    }
}