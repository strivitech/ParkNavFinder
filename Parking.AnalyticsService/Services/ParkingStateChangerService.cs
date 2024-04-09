using Confluent.Kafka;
using Kafka.Events.Contracts.Parking.State;
using Kafka.Settings;
using KafkaFlow;
using KafkaFlow.Producers;
using Parking.AnalyticsService.Configurations;
using Parking.AnalyticsService.Contracts;

namespace Parking.AnalyticsService.Services;

public class ParkingStateChangerService(
    IParkingStatesCalculator parkingStatesCalculator,
    IProducerAccessor producerAccessor,
    ILogger<ParkingStateChangerService> logger)
{
    private readonly IParkingStatesCalculator _parkingStatesCalculator = parkingStatesCalculator;
    private readonly IMessageProducer _messageProducer = producerAccessor.GetProducer(KafkaConstants.ProducerName);
    private readonly ILogger<ParkingStateChangerService> _logger = logger;

    public async Task Complete()
    {
        while (true)
        {
            _logger.LogDebug("Publishing parking analytics data");

            var parkingAnalyticsData = await _parkingStatesCalculator.CalculateNextBatchAsync();

            if (parkingAnalyticsData.Count == 0)
            {
                return;
            }

            _logger.LogDebug("Calculated parking analytics data: {@ParkingAnalyticsData}", parkingAnalyticsData);

            await PublishAsync(parkingAnalyticsData);
        }
    }

    private async Task PublishAsync(List<ParkingAnalyticsData> parkingAnalyticsData)
    {
        var parkingAnalyticsChangedEvent = new ParkingAnalyticsChangedEvent(parkingAnalyticsData.Select(data =>
                new ParkingStateInfo(ParkingId: data.ParkingId, TotalObservers: data.TotalObservers,
                    Probability: data.Probability, LastCalculatedUtc: data.CalculatedAtUtc))
            .ToList());

        var response =
            await _messageProducer.ProduceAsync(TopicConfig.ParkingAnalyticsData.TopicName, Guid.NewGuid().ToString(),
                parkingAnalyticsChangedEvent);

        if (response.Status != PersistenceStatus.Persisted)
        {
            _logger.LogError("Failed to publish parking analytics data");
            return;
        }
        
        _logger.LogDebug("Parking analytics data published");
    }
}