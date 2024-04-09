using Auth.Shared;
using Kafka.Events.Contracts.Parking.State;
using KafkaFlow;
using User.NotificationService.Contracts;
using User.NotificationService.Services;

namespace User.NotificationService.EventHandlers;

public class IndexStateChangedEventHandler : IMessageHandler<IndexStateChangedEvent>
{
    public async Task Handle(IMessageContext context, IndexStateChangedEvent message)
    {
        var logger = context.DependencyResolver.Resolve<ILogger<IndexStateChangedEventHandler>>();

        logger.LogDebug("Received message of type {MessageType}", nameof(IndexStateChangedEvent));

        var userLocationService = context.DependencyResolver.Resolve<IUserLocationService>();

        var usersToNotify = await userLocationService.GetUsersAttachedToIndexAsync(message.GeoIndex);
        logger.LogDebug("Users to notify: {UsersToNotify}", usersToNotify.UserIds);

        var userHandler = context.DependencyResolver.Resolve<IWebsocketManager>();
        var config = context.DependencyResolver.Resolve<IConfiguration>();
        
        var httpClient = context.DependencyResolver.Resolve<IHttpClientFactory>().CreateClient();
        httpClient.DefaultRequestHeaders.Add(ApiKeyConstants.HeaderName,
            config[ApiKeyConstants.OwnApiKeyName]);

        var userHandlerDescriptions = await GetUserHandlerDescriptions(usersToNotify, userHandler, logger);

        if (userHandlerDescriptions.Length == 0)
        {
            logger.LogWarning("No user handlers found");
            return;
        }

        Dictionary<string, List<string>> groupedUserIds = userHandlerDescriptions
            .SelectMany(x => x)
            .Where(uh => !string.IsNullOrEmpty(uh.Handler))
            .GroupBy(uh => uh.Handler!)
            .ToDictionary(x => x.Key,
                x => x.Select(uh => uh.UserId).ToList());

        await SendUserIdsInChunks(httpClient, groupedUserIds, message, logger);
    }

    private static async Task<List<UserHandlerDescription>[]> GetUserHandlerDescriptions(
        IndexUsersResponse usersToNotify, IWebsocketManager userHandler,
        ILogger<IndexStateChangedEventHandler> logger)
    {
        const int userChunkSize = 100;
        var userChunks = usersToNotify.UserIds.Chunk(userChunkSize);

        var userHandlerTasks = userChunks.Select(userHandler.GetHandlersAsync);

        List<UserHandlerDescription>[] userHandlerDescriptions = null!;

        try
        {
            userHandlerDescriptions = await Task.WhenAll(userHandlerTasks);
        }
        catch (AggregateException ae)
        {
            logger.LogError(ae, "Error while processing user handlers");
        }

        return userHandlerDescriptions;
    }

    private static async Task SendUserIdsInChunks(HttpClient httpClient,
        Dictionary<string, List<string>> groupedUserIds,
        IndexStateChangedEvent message, ILogger<IndexStateChangedEventHandler> logger)
    {
        const int handlerChunkSize = 100;
        var allSendTasks = new List<Task>();

        foreach (var (handler, ids) in groupedUserIds)
        {
            var idsChunks = ids.Chunk(handlerChunkSize);

            var sendToHandlerTasks = idsChunks
                .Select(idsChunk => SendToHandlers(httpClient, handler, idsChunk, message));

            allSendTasks.AddRange(sendToHandlerTasks);
        }

        try
        {
            await Task.WhenAll(allSendTasks);
        }
        catch (AggregateException ae)
        {
            logger.LogError(ae, "Error while processing user handlers");
        }
    }

    private static async Task SendToHandlers(HttpClient httpClient, string handler, IList<string> ids,
        IndexStateChangedEvent message)
    {
        string requestUri = $"http://{handler}/api/IndexState/Notify";
        var content = JsonContent.Create(new IndexStateNotification
        {
            ReceiverIds = ids,
            State = message.ParkingStates
        });
        var response = await httpClient.PostAsync(requestUri, content);
        response.EnsureSuccessStatusCode();
    }
}