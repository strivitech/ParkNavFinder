using Kafka.Events.Contracts.User.Location;
using Kafka.Settings;
using KafkaFlow;
using KafkaFlow.Producers;
using StackExchange.Redis;
using User.LocationService.Configurations;
using User.LocationService.Services;

namespace User.LocationService.EventHandlers;

public class UserLocationChangedEventHandler : IMessageHandler<UserLocationChangedEvent>
{
    public async Task Handle(IMessageContext context, UserLocationChangedEvent changedEvent)
    {
        var logger = context.DependencyResolver.Resolve<ILogger<UserLocationChangedEventHandler>>();
        var geoIndexService = context.DependencyResolver.Resolve<IGeoIndexService>();
        var connectionMultiplexer = context.DependencyResolver.Resolve<IConnectionMultiplexer>();
        var producerAccessor = context.DependencyResolver.Resolve<IProducerAccessor>();
        var messageProducer = producerAccessor.GetProducer(KafkaConstants.ProducerName);
        var db = connectionMultiplexer.GetDatabase();

        logger.LogDebug("User location changed event received: {UserId}, {Latitude}, {Longitude}",
            changedEvent.UserId, changedEvent.Latitude, changedEvent.Longitude);

        const int resolution = 8;
        const int distance = 1;
        var ringIndices = await geoIndexService.GetRingIndicesAsync(changedEvent.Latitude, changedEvent.Longitude,
            resolution, distance);

        var tran = db.CreateTransaction();
        var currentIndicesRedisValue = await db.StringGetAsync(changedEvent.UserId);
        ProcessUserIndices(changedEvent, currentIndicesRedisValue, tran, logger, ringIndices);

        await CommitTransactionAsync(changedEvent, tran, logger);
        await PublishUserLocationAreasAnalytics(changedEvent, logger, messageProducer, ringIndices);

        logger.LogDebug("User location changed event processed successfully");
    }

    private static async Task PublishUserLocationAreasAnalytics(UserLocationChangedEvent changedEvent,
        ILogger<UserLocationChangedEventHandler> logger, IMessageProducer messageProducer, IList<string> ringIndices)
    {   
        logger.LogDebug("Publishing user location analytics event");
        await messageProducer.ProduceAsync(TopicConfig.UserLocationAreasAnalytics.TopicName, changedEvent.UserId,
            new UserLocationArea(changedEvent.UserId, string.Join(",", ringIndices)));
        logger.LogDebug("User location analytics event published successfully");
    }

    private static async Task CommitTransactionAsync(UserLocationChangedEvent changedEvent, ITransaction tran,
        ILogger<UserLocationChangedEventHandler> logger)
    {
        var committed = await tran.ExecuteAsync();
        if (!committed)
        {
            logger.LogError("Redis transaction failed to commit");
        }

        logger.LogDebug("Redis transaction committed to user with id: {UserId}", changedEvent.UserId);
    }

    private static void ProcessUserIndices(UserLocationChangedEvent changedEvent, RedisValue currentIndicesRedisValue,
        ITransaction tran, ILogger<UserLocationChangedEventHandler> logger, IList<string> ringIndices)
    {
        if (currentIndicesRedisValue.HasValue)
        {
            ProcessExistingIndices(tran, logger, currentIndicesRedisValue, ringIndices, changedEvent.UserId);
        }
        else
        {
            AddIndexData(tran, ringIndices, changedEvent.UserId);
        }
    }

    private static void ProcessExistingIndices(ITransaction tran, ILogger logger, RedisValue currentIndicesRedisValue,
        IList<string> ringIndices, string userId)
    {
        var currentIndices = currentIndicesRedisValue.ToString();
        logger.LogInformation("Current H3Index: {CurrentH3Index}", currentIndices);

        var concatenatedRingIndices = string.Join(",", ringIndices);
        if (currentIndicesRedisValue == concatenatedRingIndices)
        {
            return;
        }

        UpdateIndexData(tran, ringIndices, concatenatedRingIndices, currentIndices, userId);
    }

    private static void UpdateIndexData(ITransaction tran, IList<string> ringIndices,
        string concatenatedRingIndices, string currentIndices, string userId)
    {
        var listOfCurrentIndices = currentIndices.Split(',');
        var indicesToAdd = ringIndices.Except(listOfCurrentIndices).ToList();
        var indicesToRemove = listOfCurrentIndices.Except(ringIndices).ToList();
        
        if (indicesToAdd.Any() || indicesToRemove.Any())
        {
            _ = tran.StringSetAsync(userId, concatenatedRingIndices);

            foreach (var index in indicesToRemove)
            {
                _ = tran.SetRemoveAsync(index, userId);
            }

            foreach (var index in indicesToAdd)
            {
                _ = tran.SetAddAsync(index, userId);
            }
        }   
    }

    private static void AddIndexData(ITransaction tran, IList<string> ringIndices, string userId)
    {
        var concatenatedRingIndices = string.Join(",", ringIndices);
        _ = tran.StringSetAsync(userId, concatenatedRingIndices);
        foreach (var index in ringIndices)
        {
            _ = tran.SetAddAsync(index, userId);
        }
    }
}