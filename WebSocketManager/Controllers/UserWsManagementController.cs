using Auth.Shared;
using Microsoft.AspNetCore.Mvc;
using WebSocketManager.Services;

namespace WebSocketManager.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class UserWsManagementController(IUserWsManagementService userWsService) : ControllerBase
{
    private readonly IUserWsManagementService _userWsManagementService = userWsService;

    [ApiKey(ApiKeyConstants.UserNotificationService)]
    [HttpGet("{userId}")]
    public async Task<ActionResult<string>> GetHandlerHost(string userId)
    {
        var wsHandlerUri = await _userWsManagementService.GetHandlerHostAsync(userId);

        return wsHandlerUri is null ? NotFound() : Ok(wsHandlerUri);
    }

    [ApiKey(ApiKeyConstants.UserNotificationService)]
    [HttpPost]
    public async Task<ActionResult<Dictionary<string, string?>>> GetHandlerHosts(List<string> userIds)
    {
        var wsHandlerUris = await _userWsManagementService.GetHandlerHostsAsync(userIds);
        return wsHandlerUris.Count == 0 ? NoContent() : Ok(wsHandlerUris);
    }

    [ApiKey(ApiKeyConstants.UserWebSocketHandler)]
    [HttpPost]
    public async Task<IActionResult> SetHandler([FromBody] string userId)
    {
        // TODO: remove hardcoded sender endpoint, use service discovery or pass server info with id instead
        const string senderIp = "localhost";
        const int senderPort = 5002;    
        var senderEndpoint = $"{senderIp}:{senderPort}";
        
        await _userWsManagementService.SetHandlerAsync(userId, senderEndpoint);
        return Ok();
    }

    [ApiKey(ApiKeyConstants.UserWebSocketHandler)]
    [HttpDelete("{userId}")]
    public async Task<IActionResult> RemoveHandler(string userId)
    {
        await _userWsManagementService.RemoveHandlerAsync(userId);
        return Ok();
    }
}