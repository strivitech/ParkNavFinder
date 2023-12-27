using Auth.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using UserWsHandler.Hubs;
using UserWsHandler.Hubs.Clients;
using UserWsHandler.Models;

namespace UserWsHandler.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class UserController(IHubContext<UsersHub, IUsersClient> hubContext) : ControllerBase
{
    private readonly IHubContext<UsersHub, IUsersClient> _hubContext = hubContext;
    
    [ApiKey(ApiKeyConstants.ParkingOfferingService)]
    [HttpPost]
    public async Task<IActionResult> UpdateParkingInfo(string groupId, List<Parking> parkingInfo)
    {
        await _hubContext.Clients.Group(groupId).ParkingInfoUpdated(parkingInfo);
        return Ok();
    }
}