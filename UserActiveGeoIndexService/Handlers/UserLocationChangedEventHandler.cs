using KafkaFlow;
using StackExchange.Redis;
using UserActiveGeoIndexService.Events;
using UserActiveGeoIndexService.Services;

namespace UserActiveGeoIndexService.Handlers;

public class UserLocationChangedEventHandler : IMessageHandler<UserLocationChangedEvent>
{
    public async Task Handle(IMessageContext context, UserLocationChangedEvent changedEvent)
    {
        var logger = context.DependencyResolver.Resolve<ILogger<UserLocationChangedEventHandler>>();
        var geoIndexService = context.DependencyResolver.Resolve<IGeoIndexService>();
        var connectionMultiplexer = context.DependencyResolver.Resolve<IConnectionMultiplexer>();
        var db = connectionMultiplexer.GetDatabase();
        
        logger.LogDebug("User location changed event received: {UserId}, {Latitude}, {Longitude}",
            changedEvent.UserId, changedEvent.Latitude, changedEvent.Longitude);
        
        var newGeoIndex = await geoIndexService.GetGeoIndexAsync(changedEvent.Latitude, changedEvent.Longitude, 8);
        logger.LogDebug("User new GeoIndex: {GeoIndex}", newGeoIndex);
        
        var tran = db.CreateTransaction();
        var currentGeoIndexRedisValue = await db.StringGetAsync(changedEvent.UserId);
        ProcessGeoIndex(changedEvent, currentGeoIndexRedisValue, tran, logger, newGeoIndex);
        
        await CommitTransactionAsync(changedEvent, tran, logger);
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

    private static void ProcessGeoIndex(UserLocationChangedEvent changedEvent, RedisValue currentGeoIndexRedisValue,
        ITransaction tran, ILogger<UserLocationChangedEventHandler> logger, string newGeoIndex)
    {
        if (currentGeoIndexRedisValue.HasValue)
        {
            ProcessExistingGeoIndex(tran, logger, currentGeoIndexRedisValue, newGeoIndex, changedEvent.UserId);
        }
        else
        {
            AddNewGeoIndex(tran, newGeoIndex, changedEvent.UserId);
        }
    }

    private static void ProcessExistingGeoIndex(ITransaction tran, ILogger logger, RedisValue currentGeoIndexRedisValue,
        string newGeoIndex, string userId)
    {
        var currentH3Index = currentGeoIndexRedisValue.ToString();
        logger.LogInformation("Current H3Index: {CurrentH3Index}", currentH3Index);

        if (currentGeoIndexRedisValue == newGeoIndex)
        {
            return;
        }

        UpdateGeoIndex(tran, newGeoIndex, currentH3Index, userId);
    }

    private static void UpdateGeoIndex(ITransaction tran, string newGeoIndex, string currentH3Index, string userId)
    {
        _ = tran.StringSetAsync(userId, newGeoIndex);
        _ = tran.SetRemoveAsync(currentH3Index, userId);
        _ = tran.SetAddAsync(newGeoIndex, userId);
    }

    private static void AddNewGeoIndex(ITransaction tran, string newGeoIndex, string userId)
    {
        _ = tran.StringSetAsync(userId, newGeoIndex);
        _ = tran.SetAddAsync(newGeoIndex, userId);
    }
}