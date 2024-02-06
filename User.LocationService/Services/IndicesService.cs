using System.Runtime.Serialization;
using System.Text.Json;
using StackExchange.Redis;

namespace User.LocationService.Services;

internal class IndicesService(IConnectionMultiplexer connectionMultiplexer, ILogger<IndicesService> logger)
    : IIndicesService
{
    private readonly IConnectionMultiplexer _connectionMultiplexer = connectionMultiplexer;
    private readonly ILogger<IndicesService> _logger = logger;

    public async Task<List<string>> GetUsersAttachedToIndexAsync(string index)
    {
        _logger.LogDebug("Getting users attached to index {Index}", index);

        var db = _connectionMultiplexer.GetDatabase();
        var cachedData = await db.SetMembersAsync(index);
        if (cachedData.Length <= 0)
        {
            return [];
        }
        
        return cachedData.Length <= 0
            ? []
            : cachedData
                .Select(item => item.ToString())
                .ToList();
    }
}