using System.Collections.Concurrent;
using System.Security.Claims;
using Auth.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using User.WebSocketHandler.Contracts;
using User.WebSocketHandler.Services;

namespace User.WebSocketHandler.Hubs;

[Authorize(Roles = Roles.User)]
public class UsersHub(
    IWsManagerService wsManagerService,
    IUserLocationService userLocationService,
    ILogger<UsersHub> logger) : Hub<IUsersClient>
{
    private readonly ConcurrentDictionary<string, string> _userIdToConnectionIdMap = new();
    private readonly IWsManagerService _wsManagerService = wsManagerService;
    private readonly ILogger<UsersHub> _logger = logger;
    private readonly IUserLocationService _userLocationService = userLocationService;

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User!.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var isMapped = _userIdToConnectionIdMap.TryAdd(userId, Context.ConnectionId);
        if (isMapped)
        {
            try
            {
                await _wsManagerService.SendUserConnectedMessage(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to connect to Hub API");
                _userIdToConnectionIdMap.TryRemove(userId, out _);
            }
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        try
        {
            var userId = Context.User!.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _wsManagerService.SendUserDisconnectedMessage(userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to disconnect from Hub API");
        }

        _userIdToConnectionIdMap.TryRemove(Context.ConnectionId, out _);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendLocation(Coordinate coordinate)
    {
        var userId = Context.User!.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _userLocationService.PostLocationAsync(new PostUserLocationRequest(
            UserId: userId,
            Latitude: coordinate.Latitude,
            Longitude: coordinate.Longitude,
            Timestamp: DateTime.UtcNow));
    }
}