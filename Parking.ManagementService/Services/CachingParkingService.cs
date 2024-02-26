using System.Text.Json;
using ErrorOr;
using Parking.ManagementService.Common;
using Parking.ManagementService.Contracts;
using StackExchange.Redis;

namespace Parking.ManagementService.Services;

public class CachingParkingService(
    IParkingService parkingService,
    IConnectionMultiplexer redisConnection,
    ILogger<CachingParkingService> logger,
    ICurrentUserService currentUserService)
    : IParkingService   
{
    private readonly IParkingService _parkingService = parkingService;
    private readonly IConnectionMultiplexer _redisConnection = redisConnection;
    private readonly ILogger<CachingParkingService> _logger = logger;
    private readonly ICurrentUserService _currentUserService = currentUserService;

    public async Task<ErrorOr<Created>> AddAsync(AddParkingRequest request)
    { 
        var result = await _parkingService.AddAsync(request);
        return await result.MatchAsync<ErrorOr<Created>>(
            async created =>
            {
                await InvalidateProviderParkingCacheAsync(_currentUserService.SessionData.UserId);
                return created;
            },
            errors => Task.FromResult<ErrorOr<Created>>(errors));
    }
        

    public async Task<ErrorOr<Updated>> UpdateAsync(UpdateParkingRequest request)
    {
        var result = await _parkingService.UpdateAsync(request);
        return await result.MatchAsync<ErrorOr<Updated>>(
            async updated =>
            {
                await InvalidateParkingCacheAsync(request.Id);
                await InvalidateProviderParkingCacheAsync(_currentUserService.SessionData.UserId);
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
                await InvalidateParkingCacheAsync(request.Id);
                await InvalidateProviderParkingCacheAsync(_currentUserService.SessionData.UserId);
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

    public async Task<ErrorOr<List<GetParkingResponse>>> GetAllByProviderAsync()
    {
        var cacheKey = CacheKeys.ProviderParkingKey(_currentUserService.SessionData.UserId);
        var db = _redisConnection.GetDatabase();
        var cachedData = await db.StringGetAsync(cacheKey);

        if (cachedData.HasValue)
        {
            try
            {
                return (ErrorOr<List<GetParkingResponse>>)JsonSerializer.Deserialize<List<GetParkingResponse>>(cachedData!)!;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize cached parking data");
            }
        }

        var response = await _parkingService.GetAllByProviderAsync();

        if (!response.IsError)
        {
            var serializedData = JsonSerializer.Serialize(response.Value);
            await db.StringSetAsync(cacheKey, serializedData);
        }

        return response;
    }

    private async Task InvalidateParkingCacheAsync(Guid parkingId)
    {
        var cacheKey = CacheKeys.ParkingKey(parkingId.ToString());
        var db = _redisConnection.GetDatabase();
        await db.KeyDeleteAsync(cacheKey);
    }

    private async Task InvalidateProviderParkingCacheAsync(string providerId)
    {   
        var cacheKey = CacheKeys.ProviderParkingKey(providerId);
        var db = _redisConnection.GetDatabase();
        await db.KeyDeleteAsync(cacheKey);
    }
}