﻿using StackExchange.Redis;
using WebSocketManager.Common;

namespace WebSocketManager.Services;

internal class UserWsManagementService(
    IConnectionMultiplexer connectionMultiplexer,
    ILogger<UserWsManagementService> logger) : IUserWsManagementService
{
    private readonly IConnectionMultiplexer _connectionMultiplexer = connectionMultiplexer;
    private readonly ILogger<UserWsManagementService> _logger = logger;

    public async Task<string?> GetWebSocketHandlerHostByUserIdAsync(string userId)
    {
        ArgumentException.ThrowIfNullOrEmpty(userId);
        _logger.LogInformation("Getting WebSocket handler host for user {UserId}", userId);

        var db = _connectionMultiplexer.GetDatabase();
        var cacheKey = CacheKeys.UserKey(userId);
        var cachedData = await db.StringGetAsync(cacheKey);

        return LogAndReturnCacheValue(userId, cachedData);
    }

    private string? LogAndReturnCacheValue(string userId, RedisValue cachedData)
    {
        if (!cachedData.HasValue)
        {
            _logger.LogDebug("No WebSocket handler host found for user {UserId}", userId);
            return null;
        }

        _logger.LogInformation("WebSocket handler host for user {UserId} is {WebSocketHandlerHost}", userId,
            cachedData);
        return cachedData.ToString();
    }

    public async Task SetWebSocketHandlerHostForUserIdAsync(string userId, string wsHandlerUri)
    {
        ArgumentException.ThrowIfNullOrEmpty(userId);
        ArgumentException.ThrowIfNullOrEmpty(wsHandlerUri);
        _logger.LogInformation("Setting WebSocket handler host {WebSocketHandlerHost} for user {UserId}", wsHandlerUri,
            userId);

        var db = _connectionMultiplexer.GetDatabase();
        var cacheKey = CacheKeys.UserKey(userId);

        var setResult = await db.StringSetAsync(cacheKey, wsHandlerUri);
        LogSetOperationResult(userId, wsHandlerUri, setResult);
    }

    private void LogSetOperationResult(string userId, string wsHandlerUri, bool setResult)
    {
        if (!setResult)
        {
            _logger.LogError("Failed to set WebSocket handler host for user {UserId}", userId);
            return;
        }

        _logger.LogInformation("WebSocket handler host {WebSocketHandlerHost} for user {UserId} is set", wsHandlerUri,
            userId);
    }

    public async Task RemoveWebSocketHandlerHostForUserIdAsync(string userId)
    {
        ArgumentException.ThrowIfNullOrEmpty(userId);
        _logger.LogInformation("Removing WebSocket handler host for user {UserId}", userId);

        var db = _connectionMultiplexer.GetDatabase();
        var cacheKey = CacheKeys.UserKey(userId);
        var deleteResult = await db.KeyDeleteAsync(cacheKey);
        LogDeleteOperationResult(userId, deleteResult);
    }

    private void LogDeleteOperationResult(string userId, bool deleteResult)
    {
        if (!deleteResult)
        {
            _logger.LogError("Failed to remove WebSocket handler host for user {UserId}", userId);
            return;
        }

        _logger.LogInformation("WebSocket handler host for user {UserId} is removed", userId);
    }
}