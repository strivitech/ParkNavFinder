using ErrorOr;
using Kafka.Events.Contracts.Parking.State;
using ParkingStateService.Common;
using ParkingStateService.Models;
using Polly;
using Polly.Retry;

namespace ParkingStateService.Services;

internal class IndexStateNotificationService(
    IParkingIndicesRetrieverService parkingIndicesRetrieverService,
    ILogger<IndexStateNotificationService> logger,
    IIndexStateEventPublisher indexStateEventPublisher,
    IParkingStateProvider parkingStateProvider)
    : IIndexStateNotificationService
{
    private readonly IParkingIndicesRetrieverService _parkingIndicesRetrieverService = parkingIndicesRetrieverService;
    private readonly ILogger<IndexStateNotificationService> _logger = logger;
    private readonly IIndexStateEventPublisher _indexStateEventPublisher = indexStateEventPublisher;
    private readonly IParkingStateProvider _parkingStateProvider = parkingStateProvider;

    private readonly AsyncRetryPolicy _retryPolicy =
        Policy.Handle<Exception>().WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(1));

    public async Task NotifyWithParkingStatesAsync()
    {
        _logger.LogDebug("Starting to process indices");
        
        var indices = await RetrieveNextIndices();
        var tasks = indices.Select(ProcessIndexAsync).ToList();

        try
        {
            await Task.WhenAll(tasks);
        }
        catch (AggregateException ae)
        {
            _logger.LogError(ae, "Error while processing indices");
        }
        
        _logger.LogDebug("Finished processing indices");
    }

    private async Task ProcessIndexAsync(string index)
    {
        try
        {
            var parkingStates = await GetParkingStates(index);
            var @event = new IndexStateChangedEvent(
                Guid.NewGuid().ToString(), index,
                parkingStates.Select(ps => ps.ToParkingState()), DateTime.UtcNow);

            var retryPolicy = Policy
                .HandleResult<ErrorOr<Success>>(result => result.IsError)
                .WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(1));

            var result = await retryPolicy.ExecuteAsync(async () =>
                await _indexStateEventPublisher.PublishStateChangedAsync(@event));

            if (result.IsError)
            {
                _logger.LogError("Failed to publish state change event after retries for index {Index}", index);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while processing index {Index}", index);
        }
    }

    private async Task<IList<string>> RetrieveNextIndices()
    {
        return await _retryPolicy.ExecuteAsync(
            async () => await _parkingIndicesRetrieverService.GetNextParkingIndices());
    }

    private async Task<List<ActiveParkingState>> GetParkingStates(string index)
    {
        return await _retryPolicy.ExecuteAsync(
            async () => await _parkingStateProvider.GetParkingStatesAsync(index));
    }
}