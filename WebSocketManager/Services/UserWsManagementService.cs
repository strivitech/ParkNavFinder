using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using WebSocketManager.Common;

namespace WebSocketManager.Services;

internal class UserWsManagementService(IDistributedCache distributedCache, ILogger<UserWsManagementService> logger) : IUserWsManagementService
{
    private readonly IDistributedCache _distributedCache = distributedCache;
    private readonly ILogger<UserWsManagementService> _logger = logger;

    public async Task<string?> GetWebSocketHandlerHostByUserIdAsync(string userId)
    {
        ArgumentException.ThrowIfNullOrEmpty(userId);
        
        _logger.LogInformation("Getting WebSocket handler host for user {UserId}", userId);
        
        var cacheKey = CacheKeys.UserKey(userId);
        var cachedData = await _distributedCache.GetStringAsync(cacheKey);
        
        _logger.LogInformation("WebSocket handler host for user {UserId} is {WebSocketHandlerHost}", userId, cachedData);

        return string.IsNullOrEmpty(cachedData) ? null : JsonSerializer.Deserialize<string>(cachedData);
    }

    public async Task SetWebSocketHandlerHostForUserIdAsync(string userId, string wsHandlerUri)
    {
        ArgumentException.ThrowIfNullOrEmpty(userId);
        ArgumentException.ThrowIfNullOrEmpty(wsHandlerUri);
        
        _logger.LogInformation("Setting WebSocket handler host {WebSocketHandlerHost} for user {UserId}", wsHandlerUri, userId);
        
        var cacheKey = CacheKeys.UserKey(userId);
        var serializedData = JsonSerializer.Serialize(wsHandlerUri);
        
        await _distributedCache.SetStringAsync(cacheKey, serializedData);
        
        _logger.LogInformation("WebSocket handler host {WebSocketHandlerHost} for user {UserId} is set", wsHandlerUri, userId);
    }

    public async Task RemoveWebSocketHandlerHostForUserIdAsync(string userId)
    {
        ArgumentException.ThrowIfNullOrEmpty(userId);
        
        _logger.LogInformation("Removing WebSocket handler host for user {UserId}", userId);
        
        var cacheKey = CacheKeys.UserKey(userId);
        await _distributedCache.RemoveAsync(cacheKey);
        
        _logger.LogInformation("WebSocket handler host for user {UserId} is removed", userId);
    }
}