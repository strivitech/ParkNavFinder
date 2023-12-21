using Auth.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using UserWsHandler.Hubs;
using UserWsHandler.Hubs.Clients;
using UserWsHandler.Models;
using UserWsHandler.Services;

namespace UserWsHandler.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class UserController(ILocationService locationService, IHubContext<UsersHub, IUsersClient> hubContext) : ControllerBase
{
    private readonly ILocationService _locationService = locationService;
    private readonly IHubContext<UsersHub, IUsersClient> _hubContext = hubContext;
    
    [Authorize(Roles = Roles.User)]
    [HttpPost]
    public async Task<IActionResult> SendLocation(Coordinate coordinate)
    {
        await _locationService.SendLocation(coordinate);
        return Ok();
    }
    
    [ApiKey(ApiKeyConstants.ParkingOfferingService)]
    [HttpPost]
    public async Task<IActionResult> UpdateParkingInfo(string groupId, List<Parking> parkingInfo)
    {
        await _hubContext.Clients.Group(groupId).ParkingInfoUpdated(parkingInfo);
        return Ok();
    }
}