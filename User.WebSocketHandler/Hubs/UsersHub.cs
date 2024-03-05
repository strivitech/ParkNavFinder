using System.Security.Authentication;
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
    ILogger<UsersHub> logger)
    : Hub<IUsersClient>
{
    private readonly IWsManagerService _wsManagerService = wsManagerService;
    private readonly IUserLocationService _userLocationService = userLocationService;
    private readonly ILogger<UsersHub> _logger = logger;

    public override async Task OnConnectedAsync()
    {
        var userId = GetUserId();

        try
        {
            await _wsManagerService.SendUserConnectedMessage(userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to connect to Hub API");
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = GetUserId();
        
        try
        {
            await _wsManagerService.SendUserDisconnectedMessage(userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to disconnect from Hub API");
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendLocation(Coordinate coordinate)
    {
        var userId = GetUserId();
        
        await _userLocationService.PostLocationAsync(new PostUserLocationRequest(
            UserId: userId,
            Latitude: coordinate.Latitude,
            Longitude: coordinate.Longitude));
    }

    private string GetUserId()
    {
        var userId = Context.User!.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            throw new AuthenticationException("User is not authenticated");
        }
        
        return userId;
    }
}