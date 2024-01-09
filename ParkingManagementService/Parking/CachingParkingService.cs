using System.Text.Json;
using ErrorOr;
using ParkingManagementService.Common;
using StackExchange.Redis;

namespace ParkingManagementService.Parking;

public class CachingParkingService(
    IParkingService parkingService,
    IConnectionMultiplexer redisConnection,
    ILogger<CachingParkingService> logger)
    : IParkingService   
{
    private readonly IParkingService _parkingService = parkingService;
    private readonly IConnectionMultiplexer _redisConnection = redisConnection;
    private readonly ILogger<CachingParkingService> _logger = logger;

    public async Task<ErrorOr<Created>> AddAsync(AddParkingRequest request) =>
        await _parkingService.AddAsync(request);

    public async Task<ErrorOr<Updated>> UpdateAsync(UpdateParkingRequest request)
    {
        var result = await _parkingService.UpdateAsync(request);
        return await result.MatchAsync<ErrorOr<Updated>>(
            async updated =>
            {
                await InvalidateCacheAsync(request.Id);
                return updated;
            },
            errors => Task.FromResult<ErrorOr<Updated>>(errors));
    }

    public async Task<ErrorOr<Deleted>> DeleteAsync(DeleteParkingRequest request)
    {
        var result = await _parkingService.DeleteAsync(request);
        return await result.MatchAsync<ErrorOr<Deleted>>(
            async deleted =>
            {
                await InvalidateCacheAsync(request.Id);
                return deleted;
            },
            errors => Task.FromResult<ErrorOr<Deleted>>(errors));
    }

    public async Task<ErrorOr<GetParkingResponse>> GetAsync(GetParkingRequest request)
    {
        var cacheKey = CacheKeys.ParkingKey(request.Id.ToString());
        var db = _redisConnection.GetDatabase();
        var cachedData = await db.StringGetAsync(cacheKey);

        if (cachedData.HasValue)
        {
            try
            {
                return (ErrorOr<GetParkingResponse>)JsonSerializer.Deserialize<GetParkingResponse>(cachedData!)!;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize cached parking data");
            }
        }

        var response = await _parkingService.GetAsync(request);

        if (!response.IsError)
        {
            var serializedData = JsonSerializer.Serialize(response.Value);
            await db.StringSetAsync(cacheKey, serializedData);
        }

        return response;
    }

    private async Task InvalidateCacheAsync(Guid parkingId)
    {
        var cacheKey = CacheKeys.ParkingKey(parkingId.ToString());
        var db = _redisConnection.GetDatabase();
        await db.KeyDeleteAsync(cacheKey);
    }
}