using StackExchange.Redis;

namespace User.LocationService.Services;

public class IndicesService(IConnectionMultiplexer connectionMultiplexer, ILogger<IndicesService> logger)
    : IIndicesService
{
    private readonly IConnectionMultiplexer _connectionMultiplexer = connectionMultiplexer;
    private readonly ILogger<IndicesService> _logger = logger;

    public async Task<List<string>> GetUsersAttachedToIndexAsync(string index)
    {
        _logger.LogDebug("Getting users attached to index {Index}", index);

        var db = _connectionMultiplexer.GetDatabase();
        var cachedData = await db.SetMembersAsync(index);
        
        return cachedData.Length <= 0
            ? []
            : cachedData
                .Select(item => item.ToString())
                .ToList();
    }
}